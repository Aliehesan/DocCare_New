using System;
using System.Configuration;
using System.Data.SqlClient;
using TaskManagement.config;

namespace DocCare_Clinic.Config
{
    /// <summary>
    /// Emails a patient about their appointment, in the same layout for every case:
    ///   Details   - "Notify Patient" on Doctor's Appointments, and automatically after online payment
    ///   Cancelled - automatically when the status changes to Cancelled (AppointmentStatus.TryChange)
    ///   Completed - automatically when the status changes to Completed
    /// </summary>
    public static class AppointmentNotifier
    {
        // DoctorUnavailable: cancelled because the doctor marked that day as leave
        // Reminder: the day before (ReminderService)
        public enum EmailKind { Details, Cancelled, Completed, DoctorUnavailable, Reminder }

        /// <summary>
        /// Sends the booking details email and records Appointments.NotifiedAt. Returns false (with a reason
        /// for the receptionist) when there is no email address, the appointment is cancelled, or sending fails.
        /// </summary>
        public static bool NotifyPatient(int appointmentId, out string message)
        {
            bool noEmail;
            return Send(appointmentId, EmailKind.Details, false, out message, out noEmail);
        }

        /// <summary>
        /// Sends the email on a background thread so the page does not wait for Gmail.
        /// A missing email address (common for walk-ins) is skipped quietly; other failures go to Logs/errors.txt.
        /// </summary>
        public static void SendInBackground(int appointmentId, EmailKind kind, bool cancelledByPatient = false)
        {
            string logFile = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs/errors.txt");
            System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancel =>
            {
                string message;
                bool noEmail = false;
                try
                {
                    if (Send(appointmentId, kind, cancelledByPatient, out message, out noEmail) || noEmail) return;
                }
                catch (Exception ex)
                {
                    message = ex.ToString();
                }
                try
                {
                    System.IO.File.AppendAllText(logFile, DateTime.Now + " Appointment " + appointmentId + " " +
                        kind + " email not sent: " + message + Environment.NewLine);
                }
                catch { }
            });
        }

        internal static bool Send(int appointmentId, EmailKind kind, bool cancelledByPatient, out string message, out bool noEmail)
        {
            noEmail = false;
            string cs = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Email: the booking account first, then the account the patient belongs to, then the patient record
                string query = @"
                    SELECT p.PatientName, p.PatientCode, d.Name AS DoctorName, d.Specialization,
                           a.AppointDate, a.AppointTime, a.TokenNo, a.Status,
                           ISNULL(a.TotalFee, 0) AS TotalFee, ISNULL(a.PaidAmount, 0) AS PaidAmount, ISNULL(a.DueAmount, 0) AS DueAmount,
                           COALESCE(NULLIF(u.Email, ''), NULLIF(pu.Email, ''), NULLIF(p.Email, ''), '') AS Email,
                           ISNULL(dp.Location, '') AS Location,
                           wh.SlotDuration
                    FROM Appointments a
                    INNER JOIN Patients p ON a.PatientID = p.PatientID
                    INNER JOIN Doctors d ON a.DoctorID = d.DoctorID
                    LEFT JOIN Users u ON a.UserID = u.UserID
                    LEFT JOIN Users pu ON p.UserID = pu.UserID
                    LEFT JOIN DoctorProfile dp ON dp.DoctorID = d.DoctorID
                    -- weekday 0 = Sunday, independent of the server's DATEFIRST setting (1900-01-07 was a Sunday)
                    LEFT JOIN DoctorWorkingHours wh ON wh.DoctorID = a.DoctorID AND wh.DayOfWeek = DATEDIFF(DAY, '19000107', a.AppointDate) % 7
                    WHERE a.ID = @ID";

                string patientName, patientCode, doctorName, specialization, tokenNo, status, email, location, timeSlot;
                DateTime appointDate;
                decimal totalFee, paidAmount, dueAmount;

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ID", appointmentId);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                        {
                            message = "Appointment not found.";
                            return false;
                        }

                        patientName = r["PatientName"].ToString();
                        patientCode = r["PatientCode"].ToString();
                        doctorName = r["DoctorName"].ToString();
                        specialization = r["Specialization"].ToString();
                        appointDate = Convert.ToDateTime(r["AppointDate"]);
                        tokenNo = r["TokenNo"].ToString();
                        status = r["Status"].ToString();
                        totalFee = Convert.ToDecimal(r["TotalFee"]);
                        paidAmount = Convert.ToDecimal(r["PaidAmount"]);
                        dueAmount = Convert.ToDecimal(r["DueAmount"]);
                        email = r["Email"].ToString().Trim();
                        location = r["Location"].ToString();

                        timeSlot = "To be allotted";
                        if (r["AppointTime"] != DBNull.Value)
                        {
                            TimeSpan start = (TimeSpan)r["AppointTime"];
                            timeSlot = SlotService.Format(start);
                            if (r["SlotDuration"] != DBNull.Value)
                                timeSlot += " - " + SlotService.Format(start.Add(TimeSpan.FromMinutes(Convert.ToInt32(r["SlotDuration"]))));
                        }
                    }
                }

                if (AppointmentStatus.IsFinal(status) && (kind == EmailKind.Details || kind == EmailKind.Reminder))
                {
                    message = "This appointment is " + status.ToLowerInvariant() + ", so no details email was sent.";
                    return false;
                }
                if (email == "")
                {
                    noEmail = true;
                    message = "There is no email address for " + patientName + ", so the email could not be sent.";
                    return false;
                }

                string subject =
                    kind == EmailKind.Cancelled || kind == EmailKind.DoctorUnavailable ? "Appointment cancelled - " :
                    kind == EmailKind.Completed ? "Thank you for visiting DocCare - " :
                    kind == EmailKind.Reminder ? "Reminder: your DocCare appointment tomorrow - " :
                                                  "Your DocCare appointment - ";
                string body = EmailService.BuildAppointmentEmail(patientName, doctorName, specialization, appointDate,
                                                                 timeSlot, tokenNo, totalFee, paidAmount, dueAmount, location, status,
                                                                 kind.ToString().ToLowerInvariant(), cancelledByPatient, patientCode);
                string result = new EmailService().SendEmail(email,
                    subject + appointDate.ToString("dd MMM yyyy") + ", " + timeSlot, body);
                if (result != "1")
                {
                    message = "The email to " + email + " could not be sent. " + result;
                    return false;
                }

                // NotifiedAt tracks the booking details email (shown as "Notified" on Doctor's Appointments);
                // ReminderSentAt makes sure each appointment gets one day-before reminder
                string stamp = kind == EmailKind.Details ? "NotifiedAt" : kind == EmailKind.Reminder ? "ReminderSentAt" : null;
                if (stamp != null)
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Appointments SET " + stamp + " = GETDATE() WHERE ID = @ID", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", appointmentId);
                        cmd.ExecuteNonQuery();
                    }
                }

                message = kind == EmailKind.Details
                    ? "Appointment details were emailed to " + email + "."
                    : "Appointment email was sent to " + email + ".";
                return true;
            }
        }
    }
}
