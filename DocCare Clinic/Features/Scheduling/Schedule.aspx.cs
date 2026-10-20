using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    /// <summary>
    /// A doctor's weekly working hours and days off. Doctors edit their own;
    /// reception and admin open it for a doctor from Manage Doctor (?DoctorID=).
    /// Booking availability is worked out from these in SlotService.
    /// </summary>
    public partial class Schedule : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

        // Monday first, the way the clinic reads a week
        static readonly int[] WeekOrder = { 1, 2, 3, 4, 5, 6, 0 };

        protected void Page_PreInit(object sender, EventArgs e)
        {
            string role = Session["UserRole"] as string;
            if (role == "Receptionist")
                this.MasterPageFile = ResolveUrl("~/Features/Receptionists/ReceptionistAdmin.Master");
            else if (role == "SuperAdmin")
                this.MasterPageFile = ResolveUrl("~/Features/Admin/SuperAdmin.Master");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DoctorId == 0)
            {
                // Staff arrived without choosing a doctor
                string role = Session["UserRole"] as string;
                Response.Redirect(ResolveUrl(role == "Receptionist" ? "~/Features/Doctors/Manage_Doctor.aspx" : "~/Features/Doctors/Doctors.aspx"));
                return;
            }

            if (!IsPostBack)
            {
                SetHeader();
                txtLeaveDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
                LoadHours();
                LoadLeave();
            }
        }

        // The doctor whose hours are shown: a doctor sees their own; staff pick one with ?DoctorID=
        int DoctorId
        {
            get
            {
                int id;
                if ((Session["UserRole"] as string) == "Doctor")
                    return Session["DoctorID"] != null && int.TryParse(Session["DoctorID"].ToString(), out id) ? id : 0;
                return int.TryParse(Request.QueryString["DoctorID"], out id) && DoctorExists(id) ? id : 0;
            }
        }

        bool DoctorExists(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Doctors WHERE DoctorID = @ID AND IsDeleted = 0", con))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        void SetHeader()
        {
            string role = Session["UserRole"] as string;
            if (role == "Doctor") return;

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT Name FROM Doctors WHERE DoctorID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", DoctorId);
                con.Open();
                lblForDoctor.Text = "for " + HttpUtility.HtmlEncode(Convert.ToString(cmd.ExecuteScalar()));
            }
            lnkBack.Text = role == "Receptionist" ? "Manage Doctor" : "Doctors";
            lnkBack.NavigateUrl = role == "Receptionist" ? "~/Features/Doctors/Manage_Doctor.aspx" : "~/Features/Doctors/Doctors.aspx";
        }

        // ── Working hours ─────────────────────────────────────────────────────

        void LoadHours()
        {
            DataTable saved = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM DoctorWorkingHours WHERE DoctorID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", DoctorId);
                new SqlDataAdapter(cmd).Fill(saved);
            }

            DataTable week = new DataTable();
            week.Columns.Add("DayOfWeek", typeof(int));
            week.Columns.Add("DayName");
            week.Columns.Add("IsWorking", typeof(bool));
            week.Columns.Add("Start");
            week.Columns.Add("End");
            week.Columns.Add("SlotDuration");
            week.Columns.Add("BreakFrom");
            week.Columns.Add("BreakTo");
            week.Columns.Add("SlotSummary");

            foreach (int day in WeekOrder)
            {
                DataRow s = saved.Select("DayOfWeek = " + day).FirstOrDefault();
                bool working = s != null && Convert.ToBoolean(s["IsWorking"]);
                // Suggested values for days never set up
                TimeSpan start = s != null && s["StartTime"] != DBNull.Value ? (TimeSpan)s["StartTime"] : new TimeSpan(9, 0, 0);
                TimeSpan end = s != null && s["EndTime"] != DBNull.Value ? (TimeSpan)s["EndTime"] : new TimeSpan(13, 0, 0);
                int slot = s != null && s["SlotDuration"] != DBNull.Value ? Convert.ToInt32(s["SlotDuration"]) : 30;
                TimeSpan? breakFrom = s != null && s["BreakStart"] != DBNull.Value ? (TimeSpan?)s["BreakStart"] : null;
                TimeSpan? breakTo = s != null && s["BreakEnd"] != DBNull.Value ? (TimeSpan?)s["BreakEnd"] : null;

                week.Rows.Add(day, CultureInfo.InvariantCulture.DateTimeFormat.GetDayName((DayOfWeek)day), working,
                    start.ToString(@"hh\:mm"), end.ToString(@"hh\:mm"), slot,
                    breakFrom.HasValue ? breakFrom.Value.ToString(@"hh\:mm") : "", breakTo.HasValue ? breakTo.Value.ToString(@"hh\:mm") : "",
                    working ? Summary(start, end, slot, breakFrom, breakTo) : "Day off");
            }

            rptHours.DataSource = week;
            rptHours.DataBind();
        }

        // e.g. "7 patients · 09:00 AM - 01:00 PM · break 11:00 AM - 11:30 AM" (same text the page script shows while typing)
        static string Summary(TimeSpan start, TimeSpan end, int slot, TimeSpan? breakFrom, TimeSpan? breakTo)
        {
            List<TimeSpan> slots = SlotService.SlotTimes(start, end, slot, breakFrom, breakTo);
            if (slots.Count < 1) return "<span class=\"slot-warn\">Slot is longer than the working time</span>";
            TimeSpan lastEnd = slots[slots.Count - 1] + TimeSpan.FromMinutes(slot);
            return "<strong>" + slots.Count + "</strong> patients &middot; " + SlotService.Format(start) + " - " + SlotService.Format(lastEnd) +
                (breakFrom.HasValue && breakTo.HasValue ? " &middot; break " + SlotService.Format(breakFrom.Value) + " - " + SlotService.Format(breakTo.Value) : "") +
                (lastEnd < end ? " <span class=\"slot-warn\">(" + (int)(end - lastEnd).TotalMinutes + " min unused)</span>" : "");
        }

        protected void rptHours_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            if (!(bool)DataBinder.Eval(e.Item.DataItem, "IsWorking"))
                ((System.Web.UI.HtmlControls.HtmlTableRow)e.Item.FindControl("rowDay")).Attributes["class"] = "day-off";
        }

        protected void btnSaveHours_Click(object sender, EventArgs e)
        {
            List<string> problems;
            List<object[]> rows = ReadHours(out problems);
            if (problems.Count > 0)
            {
                Alert("warning", "Please check the hours", string.Join(". ", problems) + ".");
                return;
            }

            // Patients already booked at times the new hours no longer have: ask first
            List<SlotService.Booking> off = OffSlotBookings(rows);
            if (off.Count > 0)
            {
                litHoursWarningTitle.Text = off.Count + (off.Count == 1 ? " patient is" : " patients are") + " booked outside the new hours";
                litHoursWarningList.Text = string.Join("", off.Select(b => "<li>" + HttpUtility.HtmlEncode(b.ToString()) + "</li>"));
                hdnHoursOffIds.Value = string.Join(",", off.Select(b => b.ID));
                btnHoursCancel.Text = "Save and Cancel " + (off.Count == 1 ? "This Appointment" : "These " + off.Count + " Appointments");
                pnlHoursWarning.Visible = true;
                return;
            }

            SaveHours(rows);
            Alert("success", "Working Hours Saved", "Patients can now book the new hours. Existing appointments keep their time.");
        }

        protected void btnHoursKeep_Click(object sender, EventArgs e)
        {
            List<string> problems;
            List<object[]> rows = ReadHours(out problems);
            pnlHoursWarning.Visible = false;
            if (problems.Count > 0) { Alert("warning", "Please check the hours", string.Join(". ", problems) + "."); return; }

            int kept = OffSlotBookings(rows).Count;
            SaveHours(rows);
            Alert("success", "Working Hours Saved", kept == 0 ? "Patients can now book the new hours."
                : kept + (kept == 1 ? " appointment was kept at its booked time. It shows" : " appointments were kept at their booked time. They show") + " on Doctor's Appointments marked Outside working hours.");
        }

        protected void btnHoursCancel_Click(object sender, EventArgs e)
        {
            List<string> problems;
            List<object[]> rows = ReadHours(out problems);
            pnlHoursWarning.Visible = false;
            if (problems.Count > 0) { Alert("warning", "Please check the hours", string.Join(". ", problems) + "."); return; }

            // Only the bookings that were listed and still do not fit (in case the hours were edited after the warning)
            var shown = new HashSet<int>(hdnHoursOffIds.Value.Split(',').Where(s => s != "").Select(int.Parse));
            List<int> ids = OffSlotBookings(rows).Select(b => b.ID).Where(shown.Contains).ToList();
            SaveHours(rows);
            int cancelled = AppointmentStatus.CancelForDoctorUnavailable(ids);
            hdnHoursOffIds.Value = "";
            Alert("success", "Working Hours Saved", cancelled == 0 ? "Patients can now book the new hours."
                : cancelled + (cancelled == 1 ? " appointment was cancelled and the patient is" : " appointments were cancelled and the patients are") + " being emailed to book again.");
        }

        protected void btnHoursBack_Click(object sender, EventArgs e)
        {
            // Nothing saved: show the saved hours again so the form never looks changed when it is not
            pnlHoursWarning.Visible = false;
            hdnHoursOffIds.Value = "";
            LoadHours();
            Alert("info", "Not Saved", "The working hours were not changed. The saved hours are shown again.");
        }

        // Upcoming bookings that are not on one of the slot times the given (unsaved) hours would have
        List<SlotService.Booking> OffSlotBookings(List<object[]> rows)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                return SlotService.BookingsOffSlots(con, DoctorId, weekday =>
                {
                    object[] r = rows.FirstOrDefault(x => (int)x[0] == weekday);
                    if (r == null || !(bool)r[1] || r[2] == DBNull.Value) return new List<TimeSpan>();
                    return SlotService.SlotTimes((TimeSpan)r[2], (TimeSpan)r[3], (int)r[4],
                        r[6] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)r[6], r[7] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)r[7]);
                });
            }
        }

        // The week as typed: { day, working, start, end, slot, max, breakStart, breakEnd } per day, plus any problems
        List<object[]> ReadHours(out List<string> problems)
        {
            var rows = new List<object[]>();
            problems = new List<string>();

            foreach (RepeaterItem item in rptHours.Items)
            {
                int day = int.Parse(((HiddenField)item.FindControl("hdnDay")).Value);
                string dayName = CultureInfo.InvariantCulture.DateTimeFormat.GetDayName((DayOfWeek)day);
                bool working = ((CheckBox)item.FindControl("chkWorking")).Checked;
                TimeSpan start, end;
                int slot;
                bool ok = TimeSpan.TryParse(((TextBox)item.FindControl("txtStart")).Text, out start)
                        & TimeSpan.TryParse(((TextBox)item.FindControl("txtEnd")).Text, out end)
                        & int.TryParse(((TextBox)item.FindControl("txtSlot")).Text, out slot);

                // Optional break: both times or neither, inside the working hours
                string bFromText = ((TextBox)item.FindControl("txtBreakFrom")).Text.Trim();
                string bToText = ((TextBox)item.FindControl("txtBreakTo")).Text.Trim();
                TimeSpan bFrom = TimeSpan.Zero, bTo = TimeSpan.Zero;
                bool hasBreak = bFromText != "" || bToText != "";
                bool breakOk = !hasBreak || (TimeSpan.TryParse(bFromText, out bFrom) & TimeSpan.TryParse(bToText, out bTo)
                                             && bFrom < bTo && (!ok || (bFrom >= start && bTo <= end)));
                TimeSpan? breakFrom = hasBreak && breakOk ? bFrom : (TimeSpan?)null;
                TimeSpan? breakTo = hasBreak && breakOk ? bTo : (TimeSpan?)null;

                // Patients per day comes from the hours, slot length and break; it is stored for reports
                int max = ok ? SlotService.Capacity(start, end, slot, breakFrom, breakTo) : 0;
                if (working)
                {
                    if (!ok) problems.Add(dayName + ": fill in the start time, end time and slot length");
                    else if (end <= start) problems.Add(dayName + ": end time must be after start time");
                    else if (slot < 5 || slot > 240) problems.Add(dayName + ": slot must be 5 to 240 minutes");
                    else if (!breakOk) problems.Add(dayName + ": the break needs both times, inside the working hours");
                    else if (max < 1) problems.Add(dayName + ": the slot is longer than the working time");
                }
                // Days off keep their times as suggestions for later, if they are valid
                bool keepTimes = ok && end > start && slot > 0 && max > 0;
                rows.Add(new object[] { day, working,
                    keepTimes ? (object)start : DBNull.Value, keepTimes ? (object)end : DBNull.Value,
                    keepTimes ? (object)slot : DBNull.Value, keepTimes ? (object)max : DBNull.Value,
                    keepTimes && breakFrom.HasValue ? (object)breakFrom.Value : DBNull.Value,
                    keepTimes && breakTo.HasValue ? (object)breakTo.Value : DBNull.Value });
            }
            return rows;
        }

        void SaveHours(List<object[]> rows)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                foreach (object[] r in rows)
                {
                    using (SqlCommand cmd = new SqlCommand(@"
                        MERGE DoctorWorkingHours AS t
                        USING (SELECT @DoctorID AS DoctorID, @Day AS DayOfWeek) AS s
                            ON t.DoctorID = s.DoctorID AND t.DayOfWeek = s.DayOfWeek
                        WHEN MATCHED THEN UPDATE SET IsWorking = @IsWorking, StartTime = @Start, EndTime = @End,
                                                     SlotDuration = @Slot, MaxPatients = @Max, BreakStart = @BreakStart, BreakEnd = @BreakEnd
                        WHEN NOT MATCHED THEN INSERT (DoctorID, DayOfWeek, IsWorking, StartTime, EndTime, SlotDuration, MaxPatients, BreakStart, BreakEnd)
                                              VALUES (@DoctorID, @Day, @IsWorking, @Start, @End, @Slot, @Max, @BreakStart, @BreakEnd);", con))
                    {
                        cmd.Parameters.AddWithValue("@DoctorID", DoctorId);
                        cmd.Parameters.AddWithValue("@Day", r[0]);
                        cmd.Parameters.AddWithValue("@IsWorking", r[1]);
                        cmd.Parameters.AddWithValue("@Start", r[2]);
                        cmd.Parameters.AddWithValue("@End", r[3]);
                        cmd.Parameters.AddWithValue("@Slot", r[4]);
                        cmd.Parameters.AddWithValue("@Max", r[5]);
                        cmd.Parameters.AddWithValue("@BreakStart", r[6]);
                        cmd.Parameters.AddWithValue("@BreakEnd", r[7]);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            LoadHours();
        }

        // ── Leave ─────────────────────────────────────────────────────────────

        void LoadLeave()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"SELECT LeaveID, LeaveDate, Reason FROM DoctorLeave
                                                    WHERE DoctorID = @ID AND LeaveDate >= @Today ORDER BY LeaveDate", con))
            {
                cmd.Parameters.AddWithValue("@ID", DoctorId);
                cmd.Parameters.AddWithValue("@Today", DateTime.Today);
                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                gvLeave.DataSource = dt;
                gvLeave.DataBind();
            }
        }

        const int MaxLeaveDays = 60;

        // The dates from From to To (or just From), not already marked; null with a message when invalid
        List<DateTime> LeaveDates(string fromText, string toText, out string problem, out DateTime from, out DateTime to)
        {
            problem = null;
            to = DateTime.MinValue;
            if (!DateTime.TryParse(fromText, out from)) { problem = "Please choose the date you are not available."; return null; }
            from = from.Date;
            to = from;
            if (toText.Trim() != "" && DateTime.TryParse(toText, out to)) to = to.Date;
            if (from < DateTime.Today) { problem = "Please choose today or a future date."; return null; }
            if (to < from) { problem = "The To date must be on or after the From date."; return null; }
            if ((to - from).TotalDays + 1 > MaxLeaveDays) { problem = "Please mark at most " + MaxLeaveDays + " days at a time."; return null; }

            List<DateTime> dates = new List<DateTime>();
            for (DateTime d = from; d <= to; d = d.AddDays(1))
                if (!LeaveExists(d)) dates.Add(d);
            return dates;
        }

        static string RangeLabel(DateTime from, DateTime to)
        {
            return from == to ? SlotService.FormatDate(from)
                : SlotService.FormatDate(from) + " to " + SlotService.FormatDate(to) + " (" + ((to - from).TotalDays + 1) + " days)";
        }

        protected void btnAddLeave_Click(object sender, EventArgs e)
        {
            string problem;
            DateTime from, to;
            List<DateTime> dates = LeaveDates(txtLeaveDate.Text, txtLeaveTo.Text, out problem, out from, out to);
            if (dates == null) { Alert("warning", "Check the Dates", problem); return; }
            if (dates.Count == 0)
            {
                Alert("info", "Already Marked", RangeLabel(from, to) + " is already marked as not available.");
                return;
            }

            // Patients already booked on those days: show them and ask before cancelling
            List<string> booked = new List<string>();
            foreach (DateTime d in dates)
                booked.AddRange(AppointmentStatus.ActiveOnDate(DoctorId, d).Select(b => (from == to ? "" : d.ToString("ddd dd MMM", CultureInfo.InvariantCulture) + ": ") + b));
            if (booked.Count > 0)
            {
                hdnPendingLeaveDate.Value = from.ToString("yyyy-MM-dd");
                hdnPendingLeaveTo.Value = to.ToString("yyyy-MM-dd");
                hdnPendingLeaveReason.Value = txtLeaveReason.Text.Trim();
                litWarningTitle.Text = booked.Count + (booked.Count == 1 ? " patient is" : " patients are") + " booked on " + HttpUtility.HtmlEncode(RangeLabel(from, to));
                litWarningList.Text = string.Join("", booked.Select(b => "<li>" + HttpUtility.HtmlEncode(b) + "</li>"));
                btnConfirmLeave.Text = "Mark Not Available and Cancel " + booked.Count + (booked.Count == 1 ? " Appointment" : " Appointments");
                pnlLeaveWarning.Visible = true;
                return;
            }

            SaveLeave(dates, from, to, txtLeaveReason.Text.Trim());
        }

        protected void btnConfirmLeave_Click(object sender, EventArgs e)
        {
            string problem;
            DateTime from, to;
            List<DateTime> dates = LeaveDates(hdnPendingLeaveDate.Value, hdnPendingLeaveTo.Value, out problem, out from, out to);
            pnlLeaveWarning.Visible = false;
            if (dates == null || dates.Count == 0) { LoadLeave(); return; }

            // Mark the days first so nobody can book them while the existing ones are being cancelled
            SaveLeave(dates, from, to, hdnPendingLeaveReason.Value);
        }

        protected void btnCancelLeave_Click(object sender, EventArgs e)
        {
            pnlLeaveWarning.Visible = false;
            hdnPendingLeaveDate.Value = hdnPendingLeaveTo.Value = hdnPendingLeaveReason.Value = "";
        }

        // Closes the dates for booking and cancels (and emails) anyone still booked on them
        void SaveLeave(List<DateTime> dates, DateTime from, DateTime to, string reason)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                foreach (DateTime date in dates)
                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO DoctorLeave (DoctorID, LeaveDate, Reason) VALUES (@ID, @Date, @Reason)", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", DoctorId);
                        cmd.Parameters.AddWithValue("@Date", date.Date);
                        cmd.Parameters.AddWithValue("@Reason", reason == "" ? (object)DBNull.Value : reason);
                        cmd.ExecuteNonQuery();
                    }
            }

            // Also catches anyone who booked between the warning and the confirmation
            int cancelled = dates.Sum(d => AppointmentStatus.CancelForDoctorLeave(DoctorId, d));

            txtLeaveDate.Text = txtLeaveTo.Text = txtLeaveReason.Text = "";
            hdnPendingLeaveDate.Value = hdnPendingLeaveTo.Value = hdnPendingLeaveReason.Value = "";
            LoadLeave();
            Alert("success", "Marked Not Available", RangeLabel(from, to) + " is closed for booking." +
                (cancelled > 0 ? " " + cancelled + (cancelled == 1 ? " appointment was" : " appointments were") + " cancelled and the patients are being emailed." : ""));
        }

        bool LeaveExists(DateTime date)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM DoctorLeave WHERE DoctorID = @ID AND LeaveDate = @Date", con))
            {
                cmd.Parameters.AddWithValue("@ID", DoctorId);
                cmd.Parameters.AddWithValue("@Date", date.Date);
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        protected void btnRemoveLeave_Click(object sender, EventArgs e)
        {
            int leaveId;
            if (!int.TryParse(((Button)sender).CommandArgument, out leaveId)) return;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM DoctorLeave WHERE LeaveID = @LeaveID AND DoctorID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@LeaveID", leaveId);
                cmd.Parameters.AddWithValue("@ID", DoctorId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            LoadLeave();
            Alert("success", "Available Again", "Patients can book this date again. Appointments cancelled earlier stay cancelled.");
        }

        void Alert(string icon, string title, string text)
        {
            ClientScript.RegisterStartupScript(GetType(), "scheduleMsg",
                "swalAlert('" + icon + "', '" + HttpUtility.JavaScriptStringEncode(title) + "', '" + HttpUtility.JavaScriptStringEncode(text) + "');", true);
        }
    }
}
