using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;

namespace DocCare_Clinic.Config
{
    /// <summary>
    /// Emails patients a reminder the day before their appointment. Started from Global.asax and
    /// checks every 15 minutes; from ReminderHour (10:00 AM) it reminds tomorrow's Pending/Confirmed
    /// appointments that have an email address and have not been reminded yet (Appointments.ReminderSentAt).
    /// Patients who received their booking details in the last 12 hours are skipped (they just got them).
    /// Runs while the website is running; failures are written to Logs/errors.txt and retried on the next check.
    /// </summary>
    public static class ReminderService
    {
        public const int ReminderHour = 10;
        static readonly TimeSpan CheckEvery = TimeSpan.FromMinutes(15);

        static Timer timer;
        static int running;   // 1 while a check is in progress

        public static void Start()
        {
            if (timer != null) return;
            timer = new Timer(_ => SafeRun(), null, TimeSpan.FromMinutes(1), CheckEvery);
        }

        public static void Stop()
        {
            if (timer != null) { timer.Dispose(); timer = null; }
        }

        static void SafeRun()
        {
            if (Interlocked.Exchange(ref running, 1) == 1) return;   // previous check still busy
            try { RunOnce(DateTime.Now); }
            catch (Exception ex) { Log("Reminder check failed: " + ex.Message); }
            finally { Interlocked.Exchange(ref running, 0); }
        }

        /// <summary>Appointments that should get a reminder at this moment.</summary>
        public static List<int> Due(DateTime now)
        {
            List<int> ids = new List<int>();
            if (now.Hour < ReminderHour) return ids;

            string cs = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT a.ID
                FROM Appointments a
                JOIN Patients p ON p.PatientID = a.PatientID
                LEFT JOIN Users u ON u.UserID = a.UserID
                LEFT JOIN Users pu ON pu.UserID = p.UserID
                WHERE a.AppointDate = @Tomorrow
                  AND a.Status IN ('Pending', 'Confirmed')
                  AND a.ReminderSentAt IS NULL
                  AND (a.NotifiedAt IS NULL OR a.NotifiedAt < @RecentCutoff)
                  AND COALESCE(NULLIF(u.Email, ''), NULLIF(pu.Email, ''), NULLIF(p.Email, '')) IS NOT NULL", con))
            {
                cmd.Parameters.AddWithValue("@Tomorrow", now.Date.AddDays(1));
                cmd.Parameters.AddWithValue("@RecentCutoff", now.AddHours(-12));
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) ids.Add(r.GetInt32(0));
            }
            return ids;
        }

        // Follow-up reminders go out this many days before the follow-up date the doctor advised
        public const int FollowUpReminderDaysBefore = 2;

        /// <summary>
        /// Visits whose follow-up is due within FollowUpReminderDaysBefore days (not yet passed), still open
        /// (not booked, not closed), not reminded yet, and with an email address.
        /// </summary>
        public static List<int> FollowUpsDue(DateTime now)
        {
            List<int> ids = new List<int>();
            if (now.Hour < ReminderHour) return ids;
            foreach (System.Data.DataRow r in MedicalRecords.OpenFollowUps(dueBy: now.Date.AddDays(FollowUpReminderDaysBefore)).Rows)
                if (Convert.ToDateTime(r["FollowUpDate"]).Date >= now.Date && r["FollowUpReminderSentAt"] == DBNull.Value && r["Email"].ToString().Trim() != "")
                    ids.Add(Convert.ToInt32(r["AppointmentID"]));
            return ids;
        }

        /// <summary>Sends the reminders due now (appointments tomorrow, follow-ups due soon); returns how many were sent.</summary>
        public static int RunOnce(DateTime now)
        {
            int sent = 0;
            foreach (int id in Due(now))
            {
                string message;
                bool noEmail;
                if (AppointmentNotifier.Send(id, AppointmentNotifier.EmailKind.Reminder, false, out message, out noEmail)) sent++;
                else if (!noEmail) Log("Appointment " + id + " reminder email not sent: " + message);
            }
            foreach (int id in FollowUpsDue(now))
            {
                string message;
                bool noEmail;
                if (AppointmentNotifier.SendFollowUpReminder(id, out message, out noEmail)) sent++;
                else if (!noEmail) Log("Visit " + id + " follow-up reminder not sent: " + message);
            }
            return sent;
        }

        static void Log(string text)
        {
            try
            {
                string file = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs/errors.txt");
                if (file != null) System.IO.File.AppendAllText(file, DateTime.Now + " " + text + Environment.NewLine);
            }
            catch { }
        }
    }
}
