using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DocCare_Clinic.Config
{
    /// <summary>
    /// Combines two records of the same person (old walk-ins got a new record on every visit). Everything
    /// moves to the record that is kept: appointments (with their visit notes and prescriptions), contact
    /// details it lacks, the online account it lacks, and the health summary. The duplicate is not deleted:
    /// it is hidden and remembers which record it was merged into (add_patient_merge.sql).
    /// </summary>
    public static class PatientMerge
    {
        static string Cs { get { return ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString; } }

        const string Card = @"
            SELECT p.PatientID, p.PatientCode, p.PatientName, p.DateOfBirth, p.Gender, p.UserID,
                   COALESCE(NULLIF(p.Mobile, ''), u.Mobile, '') AS Mobile, COALESCE(NULLIF(p.Email, ''), u.Email, '') AS Email,
                   COALESCE(NULLIF(p.Address, ''), u.Address, '') AS Address, ISNULL(u.Name, '') AS AccountName,
                   (SELECT COUNT(*) FROM Appointments a WHERE a.PatientID = p.PatientID) AS Appointments,
                   (SELECT MAX(a.AppointDate) FROM Appointments a WHERE a.PatientID = p.PatientID) AS LastVisit,
                   (SELECT COUNT(*) FROM VisitNotes v JOIN Appointments a ON a.ID = v.AppointmentID WHERE a.PatientID = p.PatientID) AS Notes,
                   CASE WHEN EXISTS (SELECT 1 FROM PatientHealth h WHERE h.PatientID = p.PatientID) THEN 1 ELSE 0 END AS HasHealth
            FROM Patients p LEFT JOIN Users u ON u.UserID = p.UserID";

        /// <summary>One active patient as shown on the merge page, or null.</summary>
        public static DataRow Load(int patientId)
        {
            var dt = new DataTable();
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand(Card + " WHERE p.PatientID = @ID AND ISNULL(p.IsDeleted, 0) = 0", con))
            {
                cmd.Parameters.AddWithValue("@ID", patientId);
                new SqlDataAdapter(cmd).Fill(dt);
            }
            return dt.Rows.Count == 1 ? dt.Rows[0] : null;
        }

        /// <summary>
        /// Pairs that look like the same person: the same name and the same mobile number or date of birth.
        /// The suggested record to keep is the one with an online account, then the one with more appointments.
        /// </summary>
        public static DataTable Suggestions()
        {
            var dt = new DataTable();
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand(@"
                WITH P AS (
                    SELECT p.PatientID, p.PatientCode, p.PatientName, p.DateOfBirth, p.UserID,
                           LOWER(LTRIM(RTRIM(p.PatientName))) AS NameKey,
                           COALESCE(NULLIF(p.Mobile, ''), u.Mobile, '') AS Mobile,
                           (SELECT COUNT(*) FROM Appointments a WHERE a.PatientID = p.PatientID) AS Visits
                    FROM Patients p LEFT JOIN Users u ON u.UserID = p.UserID
                    WHERE ISNULL(p.IsDeleted, 0) = 0)
                SELECT a.PatientID AS AID, a.PatientCode AS ACode, a.PatientName AS AName, a.Mobile AS AMobile, a.Visits AS AVisits, a.UserID AS AUser,
                       b.PatientID AS BID, b.PatientCode AS BCode, b.PatientName AS BName, b.Mobile AS BMobile, b.Visits AS BVisits, b.UserID AS BUser,
                       CASE WHEN a.Mobile <> '' AND a.Mobile = b.Mobile THEN 'same mobile' ELSE 'same date of birth' END AS Reason
                FROM P a JOIN P b ON a.PatientID < b.PatientID AND a.NameKey = b.NameKey
                WHERE (a.Mobile <> '' AND a.Mobile = b.Mobile) OR a.DateOfBirth = b.DateOfBirth
                ORDER BY a.PatientName, a.PatientID", con))
                new SqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        /// <summary>Why these two cannot be merged, or null when they can.</summary>
        public static string Problem(DataRow keep, DataRow dup)
        {
            if (keep == null || dup == null) return "Choose two active patients.";
            if (Convert.ToInt32(keep["PatientID"]) == Convert.ToInt32(dup["PatientID"])) return "Choose two different patients.";
            if (keep["UserID"] != DBNull.Value && dup["UserID"] != DBNull.Value && Convert.ToInt32(keep["UserID"]) != Convert.ToInt32(dup["UserID"]))
                return "These patients belong to two different online accounts, so they are kept apart.";
            return null;
        }

        /// <summary>Moves everything from dupId to keepId in one transaction; returns how many appointments moved.</summary>
        public static int Merge(int keepId, int dupId, string by)
        {
            using (var con = new SqlConnection(Cs))
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                using (var cmd = new SqlCommand(@"
                    -- appointments (visit notes and prescriptions belong to the appointment, so they move with it)
                    UPDATE Appointments SET PatientID = @Keep WHERE PatientID = @Dup;
                    DECLARE @moved INT = @@ROWCOUNT;

                    -- contact details and the online account the kept record does not have
                    UPDATE k SET k.Mobile = COALESCE(NULLIF(k.Mobile, ''), d.Mobile),
                                 k.Email = COALESCE(NULLIF(k.Email, ''), d.Email),
                                 k.Address = COALESCE(NULLIF(k.Address, ''), d.Address),
                                 k.UserID = COALESCE(k.UserID, d.UserID)
                    FROM Patients k CROSS JOIN Patients d WHERE k.PatientID = @Keep AND d.PatientID = @Dup;

                    -- health summary: take the duplicate's, or fill the kept one's empty facts from it
                    IF NOT EXISTS (SELECT 1 FROM PatientHealth WHERE PatientID = @Keep)
                        UPDATE PatientHealth SET PatientID = @Keep WHERE PatientID = @Dup;
                    ELSE
                    BEGIN
                        UPDATE k SET k.BloodGroup = COALESCE(NULLIF(k.BloodGroup, ''), d.BloodGroup),
                                     k.Allergies = COALESCE(NULLIF(k.Allergies, ''), d.Allergies),
                                     k.Conditions = COALESCE(NULLIF(k.Conditions, ''), d.Conditions),
                                     k.CurrentMedicines = COALESCE(NULLIF(k.CurrentMedicines, ''), d.CurrentMedicines),
                                     k.PastSurgeries = COALESCE(NULLIF(k.PastSurgeries, ''), d.PastSurgeries)
                        FROM PatientHealth k CROSS JOIN PatientHealth d WHERE k.PatientID = @Keep AND d.PatientID = @Dup;
                        DELETE FROM PatientHealth WHERE PatientID = @Dup;
                    END

                    -- the duplicate stays for the record, hidden, pointing at the kept one
                    UPDATE Patients SET IsDeleted = 1, MergedIntoPatientID = @Keep, MergedAt = GETDATE(), MergedBy = @By WHERE PatientID = @Dup;
                    SELECT @moved;", con, tx))
                {
                    cmd.Parameters.AddWithValue("@Keep", keepId);
                    cmd.Parameters.AddWithValue("@Dup", dupId);
                    cmd.Parameters.AddWithValue("@By", (object)by ?? DBNull.Value);
                    int moved = Convert.ToInt32(cmd.ExecuteScalar());
                    tx.Commit();
                    return moved;
                }
            }
        }
    }
}
