using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace DocCare_Clinic.Config
{
    /// <summary>
    /// Appointment status rules, in one place for every page.
    /// Status only moves forward: Pending -> Confirmed -> Completed, and Pending/Confirmed -> Cancelled or No Show.
    /// Completed, Cancelled and No Show are final for everyone.
    /// Time rules: No Show only after the appointment time; patients cannot cancel (or reschedule)
    /// online within PatientCutoffHours of the appointment.
    /// </summary>
    public static class AppointmentStatus
    {
        public const string Pending = "Pending", Confirmed = "Confirmed", Completed = "Completed", Cancelled = "Cancelled", NoShow = "No Show";

        // Patients must cancel or reschedule online at least this long before the appointment
        public const int PatientCutoffHours = 2;

        // Roles are Session["UserRole"] values set at login
        const string Receptionist = "Receptionist", Doctor = "Doctor", Patient = "User";

        // from status -> (to status -> roles allowed to make that change)
        static readonly Dictionary<string, Dictionary<string, string[]>> Rules = new Dictionary<string, Dictionary<string, string[]>>
        {
            { Pending, new Dictionary<string, string[]> {
                { Confirmed, new[] { Receptionist } },
                { Completed, new[] { Receptionist, Doctor } },
                { Cancelled, new[] { Receptionist, Patient } },
                { NoShow, new[] { Receptionist, Doctor } } } },
            { Confirmed, new Dictionary<string, string[]> {
                { Completed, new[] { Receptionist, Doctor } },
                { Cancelled, new[] { Receptionist, Patient } },
                { NoShow, new[] { Receptionist, Doctor } } } },
            { Completed, new Dictionary<string, string[]>() },
            { Cancelled, new Dictionary<string, string[]>() },
            { NoShow, new Dictionary<string, string[]>() },
        };

        /// <summary>True for statuses that can never change again.</summary>
        public static bool IsFinal(string status)
        {
            return status == Completed || status == Cancelled || status == NoShow;
        }

        /// <summary>True while a patient may still cancel or reschedule online (more than PatientCutoffHours ahead).</summary>
        public static bool PatientCanChange(DateTime appointmentStart)
        {
            return appointmentStart - DateTime.Now >= TimeSpan.FromHours(PatientCutoffHours);
        }

        /// <summary>The appointment's start, or null when it has no time yet.</summary>
        public static DateTime? StartOf(object appointDate, object appointTime)
        {
            if (appointDate == null || appointDate == DBNull.Value || !(appointTime is TimeSpan)) return null;
            return Convert.ToDateTime(appointDate).Date.Add((TimeSpan)appointTime);
        }

        /// <summary>
        /// Next statuses for this role at this moment: like NextFor, but No Show is only offered once the
        /// appointment time has passed.
        /// </summary>
        public static List<string> NextFor(string from, string role, DateTime? appointmentStart)
        {
            List<string> next = NextFor(from, role);
            if (!appointmentStart.HasValue || appointmentStart.Value > DateTime.Now) next.Remove(NoShow);
            return next;
        }

        /// <summary>True when this role may move an appointment from one status to another.</summary>
        public static bool CanChange(string from, string to, string role)
        {
            Dictionary<string, string[]> next;
            string[] roles;
            return from != null && to != null && role != null
                && Rules.TryGetValue(from, out next) && next.TryGetValue(to, out roles)
                && Array.IndexOf(roles, role) >= 0;
        }

        /// <summary>Statuses this role can choose next (empty when the status is final for them).</summary>
        public static List<string> NextFor(string from, string role)
        {
            List<string> result = new List<string>();
            Dictionary<string, string[]> next;
            if (from != null && Rules.TryGetValue(from, out next))
                foreach (KeyValuePair<string, string[]> kv in next)
                    if (Array.IndexOf(kv.Value, role) >= 0) result.Add(kv.Key);
            return result;
        }

        /// <summary>
        /// Changes the status if the rules allow it, checked against the status currently in the
        /// database, and emails the patient when it becomes Cancelled or Completed.
        /// ownerUserId (patients) limits the change to that user's own appointments.
        /// Returns false with a reason the user can read.
        /// </summary>
        public static bool TryChange(int appointmentId, string newStatus, string role, int? ownerUserId, out string message)
        {
            string cs = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                string current;
                DateTime? start;
                using (SqlCommand cmd = new SqlCommand("SELECT Status, UserID, AppointDate, AppointTime FROM Appointments WHERE ID = @ID", con))
                {
                    cmd.Parameters.AddWithValue("@ID", appointmentId);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                        {
                            message = "Appointment not found.";
                            return false;
                        }
                        current = r["Status"].ToString();
                        start = StartOf(r["AppointDate"], r["AppointTime"]);
                        if (ownerUserId.HasValue && (r["UserID"] == DBNull.Value || Convert.ToInt32(r["UserID"]) != ownerUserId.Value))
                        {
                            message = "You can only change your own appointments.";
                            return false;
                        }
                    }
                }

                if (current == newStatus)
                {
                    message = "The appointment is already " + current + ".";
                    return false;
                }
                if (!CanChange(current, newStatus, role))
                {
                    message = IsFinal(current)
                        ? "This appointment is " + current + ". Its status can no longer be changed."
                        : "A " + current + " appointment cannot be changed to " + newStatus + ".";
                    return false;
                }
                if (newStatus == NoShow && (!start.HasValue || start.Value > DateTime.Now))
                {
                    message = "No Show can only be set after the appointment time has passed.";
                    return false;
                }
                if (role == Patient && newStatus == Cancelled && start.HasValue && !PatientCanChange(start.Value))
                {
                    message = "Appointments can be cancelled online up to " + PatientCutoffHours + " hours before the time. Please call the clinic.";
                    return false;
                }

                // "AND Status = @Current": if someone else changed it a moment ago, nothing is overwritten
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Appointments SET Status = @New WHERE ID = @ID AND Status = @Current", con))
                {
                    cmd.Parameters.AddWithValue("@New", newStatus);
                    cmd.Parameters.AddWithValue("@ID", appointmentId);
                    cmd.Parameters.AddWithValue("@Current", current);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        message = "The status was just changed by someone else. Please refresh and try again.";
                        return false;
                    }
                }

                // Let the patient know (in the background; walk-ins without an email are skipped)
                if (newStatus == Cancelled)
                    AppointmentNotifier.SendInBackground(appointmentId, AppointmentNotifier.EmailKind.Cancelled, role == Patient);
                else if (newStatus == Completed)
                    AppointmentNotifier.SendInBackground(appointmentId, AppointmentNotifier.EmailKind.Completed);

                message = "Appointment marked as " + newStatus + ".";
                return true;
            }
        }

        /// <summary>
        /// Active (Pending/Confirmed) appointments of a doctor on a date, for the leave warning.
        /// </summary>
        public static List<string> ActiveOnDate(int doctorId, DateTime date)
        {
            List<string> result = new List<string>();
            string cs = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT p.PatientName, p.PatientCode, a.AppointTime, a.TokenNo FROM Appointments a
                JOIN Patients p ON p.PatientID = a.PatientID
                WHERE a.DoctorID = @DoctorID AND a.AppointDate = @Date AND a.Status IN ('Pending', 'Confirmed')
                ORDER BY a.AppointTime", con))
            {
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                cmd.Parameters.AddWithValue("@Date", date.Date);
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read())
                        result.Add(r["PatientName"] + " [" + r["PatientCode"] + "] (" + (r["AppointTime"] == DBNull.Value ? "no time" : SlotService.Format((TimeSpan)r["AppointTime"])) + ", " + r["TokenNo"] + ")");
            }
            return result;
        }

        /// <summary>
        /// The doctor marked this date as leave: cancel every active appointment that day and email
        /// each patient ("the doctor is not available"). This is a clinic decision, not a user status
        /// change, so it does not go through the role table above. Returns how many were cancelled.
        /// </summary>
        public static int CancelForDoctorLeave(int doctorId, DateTime date)
        {
            List<int> ids = new List<int>();
            string cs = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE Appointments SET Status = 'Cancelled'
                    OUTPUT inserted.ID
                    WHERE DoctorID = @DoctorID AND AppointDate = @Date AND Status IN ('Pending', 'Confirmed')", con))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    cmd.Parameters.AddWithValue("@Date", date.Date);
                    using (SqlDataReader r = cmd.ExecuteReader())
                        while (r.Read()) ids.Add(r.GetInt32(0));
                }
            }
            foreach (int id in ids)
                AppointmentNotifier.SendInBackground(id, AppointmentNotifier.EmailKind.DoctorUnavailable);
            return ids.Count;
        }

        /// <summary>
        /// The doctor's hours changed and these bookings no longer fit (Working Hours page, after the
        /// staff chose to cancel them): cancel the ones still active and email each patient to book again.
        /// </summary>
        public static int CancelForDoctorUnavailable(IEnumerable<int> appointmentIds)
        {
            List<int> cancelled = new List<int>();
            string cs = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                foreach (int id in appointmentIds)
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE Appointments SET Status = 'Cancelled' WHERE ID = @ID AND Status IN ('Pending', 'Confirmed')", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        if (cmd.ExecuteNonQuery() == 1) cancelled.Add(id);
                    }
            }
            foreach (int id in cancelled)
                AppointmentNotifier.SendInBackground(id, AppointmentNotifier.EmailKind.DoctorUnavailable);
            return cancelled.Count;
        }
    }
}
