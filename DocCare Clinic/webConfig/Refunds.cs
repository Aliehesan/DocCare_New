using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DocCare_Clinic.Config
{
    /// <summary>
    /// Refunds of the online advance on cancelled appointments (add_refunds.sql). A refund is due when an
    /// appointment is Cancelled, an advance was paid and it has not been marked refunded yet. The money is
    /// returned outside DocCare (UPI, cash ...); staff record it here so nobody is refunded twice or forgotten.
    /// </summary>
    public static class Refunds
    {
        static string Cs { get { return ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString; } }

        const string Base = @"
            SELECT a.ID, a.AppointDate, a.AppointTime, a.TokenNo, a.PaidAmount, ISNULL(a.PaymentId, '') AS PaymentId,
                   a.RefundedAt, ISNULL(a.RefundedBy, '') AS RefundedBy, ISNULL(a.RefundNote, '') AS RefundNote,
                   p.PatientID, p.PatientName, p.PatientCode, d.Name AS Doctor,
                   COALESCE(NULLIF(u.Mobile, ''), NULLIF(pu.Mobile, ''), NULLIF(p.Mobile, ''), '') AS Mobile
            FROM Appointments a
            JOIN Patients p ON p.PatientID = a.PatientID
            JOIN Doctors d ON d.DoctorID = a.DoctorID
            LEFT JOIN Users u ON u.UserID = a.UserID
            LEFT JOIN Users pu ON pu.UserID = p.UserID
            WHERE a.Status = 'Cancelled' AND ISNULL(a.PaidAmount, 0) > 0";

        /// <summary>Still to be refunded (oldest first), or already refunded (latest first).</summary>
        public static DataTable List(bool refunded)
        {
            var dt = new DataTable();
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand(Base + (refunded
                ? " AND a.RefundedAt IS NOT NULL ORDER BY a.RefundedAt DESC"
                : " AND a.RefundedAt IS NULL ORDER BY a.AppointDate, a.ID"), con))
                new SqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        /// <summary>How many refunds are due and their total, for dashboards.</summary>
        public static void Due(out int count, out decimal total)
        {
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand(@"SELECT COUNT(*), ISNULL(SUM(PaidAmount), 0) FROM Appointments
                                              WHERE Status = 'Cancelled' AND ISNULL(PaidAmount, 0) > 0 AND RefundedAt IS NULL", con))
            {
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    r.Read();
                    count = r.GetInt32(0);
                    total = r.GetDecimal(1);
                }
            }
        }

        /// <summary>Records the refund once (a second click, or another person at the same time, changes nothing).</summary>
        public static bool MarkRefunded(int appointmentId, string note, string by)
        {
            note = (note ?? "").Trim();
            if (note.Length > 200) note = note.Substring(0, 200);
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand(@"UPDATE Appointments SET RefundedAt = GETDATE(), RefundedBy = @By, RefundNote = @Note
                                              WHERE ID = @ID AND Status = 'Cancelled' AND ISNULL(PaidAmount, 0) > 0 AND RefundedAt IS NULL", con))
            {
                cmd.Parameters.AddWithValue("@ID", appointmentId);
                cmd.Parameters.AddWithValue("@By", (object)by ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Note", note == "" ? (object)DBNull.Value : note);
                con.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }
    }
}
