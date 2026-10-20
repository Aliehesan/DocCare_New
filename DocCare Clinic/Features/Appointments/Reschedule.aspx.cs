using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    /// <summary>
    /// A patient moves their own appointment to another free time with the same doctor.
    /// Allowed for Pending/Confirmed appointments more than PatientCutoffHours ahead; the payment
    /// carries over, the token is renumbered for a new date, and the details email is sent again.
    /// </summary>
    public partial class Reschedule : System.Web.UI.Page
    {
        static string ConnectionString { get { return ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString; } }

        class Appt
        {
            public int Id, DoctorId, PatientId;
            public string Doctor, Patient, Status, Token;
            public DateTime Date;
            public TimeSpan? Time;
            public DateTime? Start { get { return Time.HasValue ? Date.Add(Time.Value) : (DateTime?)null; } }
        }

        // The signed-in patient's own appointment, or null
        static Appt Load(int appointmentId, object sessionUserId)
        {
            int userId;
            if (sessionUserId == null || !int.TryParse(sessionUserId.ToString(), out userId)) return null;
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT a.ID, a.DoctorID, a.PatientID, a.AppointDate, a.AppointTime, a.Status, a.TokenNo, d.Name AS Doctor, p.PatientName
                FROM Appointments a
                JOIN Doctors d ON d.DoctorID = a.DoctorID
                JOIN Patients p ON p.PatientID = a.PatientID
                WHERE a.ID = @ID AND a.UserID = @UserID", con))
            {
                cmd.Parameters.AddWithValue("@ID", appointmentId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new Appt
                    {
                        Id = (int)r["ID"], DoctorId = (int)r["DoctorID"], PatientId = (int)r["PatientID"],
                        Date = Convert.ToDateTime(r["AppointDate"]).Date,
                        Time = r["AppointTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)r["AppointTime"],
                        Status = r["Status"].ToString(), Token = r["TokenNo"].ToString(),
                        Doctor = r["Doctor"].ToString(), Patient = r["PatientName"].ToString()
                    };
                }
            }
        }

        // Why this appointment cannot be rescheduled online, or null when it can
        static string Blocked(Appt a)
        {
            if (a == null) return "Appointment not found.";
            if (AppointmentStatus.IsFinal(a.Status)) return "This appointment is " + a.Status + " and can no longer be rescheduled.";
            if (a.Start.HasValue && !AppointmentStatus.PatientCanChange(a.Start.Value))
                return "Appointments can be rescheduled online up to " + AppointmentStatus.PatientCutoffHours + " hours before the time. Please call the clinic.";
            return null;
        }

        int AppointmentId
        {
            get { int id; return int.TryParse(Request.QueryString["id"], out id) ? id : 0; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Appt a = Load(AppointmentId, Session["UserId"]);
            if (a == null)
            {
                Response.Redirect(ResolveUrl("~/Features/Appointments/MyAppointment.aspx"));
                return;
            }
            if (!IsPostBack)
            {
                litDoctor.Text = HttpUtility.HtmlEncode(a.Doctor);
                litPatient.Text = HttpUtility.HtmlEncode(a.Patient) + " (ID " + PatientCode.Of(a.PatientId) + ")";
                litCurrent.Text = SlotService.FormatDate(a.Date) + (a.Time.HasValue ? " at " + SlotService.Format(a.Time.Value) : "") +
                                  " &middot; Token " + HttpUtility.HtmlEncode(a.Token);
                txtNewDate.Text = (a.Date >= DateTime.Today ? a.Date : DateTime.Today).ToString("yyyy-MM-dd");
                txtNewDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
                txtNewDate.Attributes["max"] = DateTime.Today.AddDays(SlotService.BookingWindowDays).ToString("yyyy-MM-dd");

                string blocked = Blocked(a);
                if (blocked != null)
                {
                    btnConfirm.Attributes["data-locked"] = "1";
                    Alert("info", "Cannot Reschedule Online", blocked);
                }
            }
        }

        static bool TryParseDate(string value, out DateTime date)
        {
            return DateTime.TryParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
                                          System.Globalization.DateTimeStyles.None, out date);
        }

        // Free times for the chosen date with this appointment's doctor
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static object GetSlots(int appointmentId, string date)
        {
            Appt a = Load(appointmentId, HttpContext.Current.Session["UserId"]);
            string blocked = Blocked(a);
            DateTime day;
            if (blocked != null) return new { available = false, message = blocked, slots = new object[0], dateLabel = "" };
            if (!TryParseDate(date, out day)) return new { available = false, message = "Please choose a valid date.", slots = new object[0], dateLabel = "" };

            string reason;
            List<TimeSpan> free;
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                free = SlotService.GetFreeSlots(con, a.DoctorId, day, SlotService.OnlineBufferMinutes, out reason);
            }

            // Leave out times that overlap the patient's other appointments that day (or the whole day when
            // they already see this doctor then); say why when that leaves nothing
            string firstClash = null;
            free = free.Where(t =>
            {
                string clash = SlotService.PatientConflict(a.PatientId, a.Patient, a.DoctorId, day, t, a.Id);
                if (clash != null && firstClash == null) firstClash = clash;
                return clash == null;
            }).ToList();
            if (free.Count == 0 && firstClash != null) reason = firstClash;

            return new
            {
                available = free.Count > 0,
                message = reason,
                dateLabel = SlotService.FormatDate(day),
                slots = free.Select(t => new { value = t.ToString(@"hh\:mm"), label = SlotService.Format(t) }).ToArray()
            };
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            Appt a = Load(AppointmentId, Session["UserId"]);
            string blocked = Blocked(a);
            if (blocked != null) { Alert("warning", "Cannot Reschedule", blocked); return; }

            DateTime newDate;
            TimeSpan newTime;
            if (!TryParseDate(txtNewDate.Text, out newDate) || !TimeSpan.TryParse(hdnNewSlot.Value, out newTime))
            {
                Alert("info", "Choose a Time", "Please choose a new date and time.");
                return;
            }
            if (newDate == a.Date && a.Time.HasValue && newTime == a.Time.Value)
            {
                Alert("info", "Same Time", "That is already your appointment time. Please choose a different time.");
                return;
            }

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                // Not with this doctor again that day, and no overlap with the patient's other appointments
                // (this appointment itself does not count)
                string clash = SlotService.PatientConflict(a.PatientId, a.Patient, a.DoctorId, newDate, newTime, a.Id);
                if (clash != null)
                {
                    Alert("warning", "Already Booked", clash);
                    return;
                }

                string reason;
                if (!SlotService.IsSlotFree(con, a.DoctorId, newDate, newTime, SlotService.OnlineBufferMinutes, out reason))
                {
                    Alert("warning", "Time Not Available", reason);
                    return;
                }

                // Same day keeps its token; a new day gets that day's next token
                string token = newDate == a.Date ? a.Token : SlotService.NextToken(con, null, a.DoctorId, newDate);
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE Appointments SET AppointDate = @Date, AppointTime = @Time, TokenNo = @Token, NotifiedAt = NULL, ReminderSentAt = NULL
                    WHERE ID = @ID AND UserID = @UserID AND Status IN ('Pending', 'Confirmed')", con))
                {
                    cmd.Parameters.AddWithValue("@Date", newDate);
                    cmd.Parameters.AddWithValue("@Time", newTime);
                    cmd.Parameters.AddWithValue("@Token", token);
                    cmd.Parameters.AddWithValue("@ID", a.Id);
                    cmd.Parameters.AddWithValue("@UserID", Session["UserId"]);
                    try
                    {
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            Alert("warning", "Cannot Reschedule", "This appointment was changed a moment ago. Please check My Appointments.");
                            return;
                        }
                    }
                    catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
                    {
                        // UX_Appointments_ActiveSlot: someone took this exact time a moment earlier
                        Alert("warning", "Time Just Taken", "That time was booked by someone else a moment ago. Please choose another time.");
                        return;
                    }
                }
            }

            // New details email (payment carries over)
            AppointmentNotifier.SendInBackground(a.Id, AppointmentNotifier.EmailKind.Details);
            Session["SuccessMessage"] = "Rescheduled to " + SlotService.FormatDate(newDate) + " at " + SlotService.Format(newTime) +
                                        ". The new details are being emailed to you.";
            Response.Redirect(ResolveUrl("~/Features/Appointments/MyAppointment.aspx"));
        }

        void Alert(string icon, string title, string text)
        {
            ClientScript.RegisterStartupScript(GetType(), "rescheduleMsg",
                "swalAlert('" + icon + "', '" + HttpUtility.JavaScriptStringEncode(title) + "', '" + HttpUtility.JavaScriptStringEncode(text) + "');", true);
        }
    }
}
