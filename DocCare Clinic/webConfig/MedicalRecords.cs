using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DocCare_Clinic.Config
{
    /// <summary>
    /// A patient's medical record: vitals and the doctor's notes for each visit (VisitNotes), the
    /// prescription lines (PrescriptionItems), and permanent health facts (PatientHealth).
    ///
    /// Who may do what:
    ///   Doctor       - vitals and notes for their own visits; reads every visit; edits health facts
    ///   Admin        - reads everything, writes nothing
    ///   Receptionist - vitals only; never sees diagnoses, prescriptions or notes; sees allergies
    /// </summary>
    public static class MedicalRecords
    {
        const string AdminRole = "SuperAdmin", DoctorRole = "Doctor", ReceptionRole = "Receptionist";

        public static bool SeesClinical(string role) { return role == DoctorRole || role == AdminRole; }
        public static bool WritesVitals(string role) { return role == DoctorRole || role == ReceptionRole; }
        public static bool EditsHealth(string role) { return role == DoctorRole; }

        /// <summary>Notes belong to a visit that happened or is happening: not cancelled, not missed, not in the future.</summary>
        public static bool IsVisit(string status, DateTime date)
        {
            return status != AppointmentStatus.Cancelled && status != AppointmentStatus.NoShow && date.Date <= DateTime.Today;
        }

        static string Cs { get { return ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString; } }

        // ── Data ──────────────────────────────────────────────────────────────

        public class Visit
        {
            public int AppointmentID, PatientID, DoctorID, Age;
            public string Patient, PatientCode, Gender, Doctor, Specialization, Status, Token, Location;
            public DateTime Date;
            public TimeSpan? Time;
            public decimal Fee;
        }

        public class Notes
        {
            public bool Exists;
            public string BloodPressure, Complaint, Diagnosis, Advice, VitalsBy;
            public int? Pulse, SpO2, NotesByDoctorID;
            public decimal? TemperatureF, WeightKg;
            public DateTime? FollowUpDate, VitalsAt, NotesAt;

            public bool HasVitals { get { return !string.IsNullOrEmpty(BloodPressure) || Pulse.HasValue || TemperatureF.HasValue || WeightKg.HasValue || SpO2.HasValue; } }
            public bool HasClinical { get { return !string.IsNullOrEmpty(Complaint) || !string.IsNullOrEmpty(Diagnosis) || !string.IsNullOrEmpty(Advice) || FollowUpDate.HasValue; } }

            /// <summary>"BP 120/80 · Pulse 78 · Temp 98.6 °F · Weight 64 kg · SpO2 98%"</summary>
            public string VitalsLine()
            {
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(BloodPressure)) parts.Add("BP " + BloodPressure);
                if (Pulse.HasValue) parts.Add("Pulse " + Pulse);
                if (TemperatureF.HasValue) parts.Add("Temp " + TemperatureF.Value.ToString("0.#", CultureInfo.InvariantCulture) + " °F");
                if (WeightKg.HasValue) parts.Add("Weight " + WeightKg.Value.ToString("0.#", CultureInfo.InvariantCulture) + " kg");
                if (SpO2.HasValue) parts.Add("SpO2 " + SpO2 + "%");
                return string.Join(" · ", parts);
            }
        }

        [Serializable]   // kept in ViewState while the prescription is being edited
        public class RxItem
        {
            // properties (not fields): the page binds them with Eval
            public string Medicine { get; set; }
            public string Dose { get; set; }
            public string Frequency { get; set; }
            public string Instructions { get; set; }
            public int? Days { get; set; }
        }

        public class Health
        {
            public string BloodGroup, Allergies, Conditions, CurrentMedicines, PastSurgeries, UpdatedBy;
            public DateTime? UpdatedAt;
            public bool IsEmpty { get { return string.IsNullOrEmpty(BloodGroup) && string.IsNullOrEmpty(Allergies) && string.IsNullOrEmpty(Conditions) && string.IsNullOrEmpty(CurrentMedicines) && string.IsNullOrEmpty(PastSurgeries); } }
        }

        static string S(object v) { return v == null || v == DBNull.Value ? null : v.ToString(); }
        static int? I(object v) { return v == null || v == DBNull.Value ? (int?)null : Convert.ToInt32(v); }
        static decimal? D(object v) { return v == null || v == DBNull.Value ? (decimal?)null : Convert.ToDecimal(v); }
        static DateTime? T(object v) { return v == null || v == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(v); }
        static object Db(object v) { return v ?? DBNull.Value; }
        static object Db(string v) { return string.IsNullOrWhiteSpace(v) ? (object)DBNull.Value : v.Trim(); }

        public static Visit LoadVisit(int appointmentId)
        {
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand(@"
                SELECT a.ID, a.PatientID, a.DoctorID, a.AppointDate, a.AppointTime, a.Status, a.TokenNo,
                       COALESCE(a.TotalFee, d.Charges, 0) AS Fee, p.PatientName, p.PatientCode, p.Gender, p.DateOfBirth,
                       d.Name AS Doctor, d.Specialization, dp.Location
                FROM Appointments a
                JOIN Patients p ON p.PatientID = a.PatientID
                JOIN Doctors d ON d.DoctorID = a.DoctorID
                LEFT JOIN DoctorProfile dp ON dp.DoctorID = d.DoctorID
                WHERE a.ID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", appointmentId);
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    DateTime? dob = T(r["DateOfBirth"]);
                    int age = 0;
                    if (dob.HasValue) { age = DateTime.Today.Year - dob.Value.Year; if (dob.Value.Date > DateTime.Today.AddYears(-age)) age--; }
                    return new Visit
                    {
                        AppointmentID = Convert.ToInt32(r["ID"]), PatientID = Convert.ToInt32(r["PatientID"]), DoctorID = Convert.ToInt32(r["DoctorID"]),
                        Date = Convert.ToDateTime(r["AppointDate"]), Time = r["AppointTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)r["AppointTime"],
                        Status = S(r["Status"]), Token = S(r["TokenNo"]), Fee = Convert.ToDecimal(r["Fee"]),
                        Patient = S(r["PatientName"]), PatientCode = S(r["PatientCode"]), Gender = S(r["Gender"]), Age = age,
                        Doctor = "Dr. " + DoctorName.WithoutTitle(S(r["Doctor"])), Specialization = S(r["Specialization"]), Location = S(r["Location"])
                    };
                }
            }
        }

        public static Notes LoadNotes(int appointmentId)
        {
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand("SELECT * FROM VisitNotes WHERE AppointmentID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", appointmentId);
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return new Notes();
                    return new Notes
                    {
                        Exists = true, BloodPressure = S(r["BloodPressure"]), Pulse = I(r["Pulse"]), TemperatureF = D(r["TemperatureF"]),
                        WeightKg = D(r["WeightKg"]), SpO2 = I(r["SpO2"]), Complaint = S(r["Complaint"]), Diagnosis = S(r["Diagnosis"]),
                        Advice = S(r["Advice"]), FollowUpDate = T(r["FollowUpDate"]), VitalsBy = S(r["VitalsBy"]), VitalsAt = T(r["VitalsAt"]),
                        NotesByDoctorID = I(r["NotesByDoctorID"]), NotesAt = T(r["NotesAt"])
                    };
                }
            }
        }

        public static List<RxItem> LoadRx(int appointmentId)
        {
            var items = new List<RxItem>();
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand("SELECT * FROM PrescriptionItems WHERE AppointmentID = @ID ORDER BY SortOrder, ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", appointmentId);
                con.Open();
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        items.Add(new RxItem { Medicine = S(r["Medicine"]), Dose = S(r["Dose"]), Frequency = S(r["Frequency"]), Days = I(r["Days"]), Instructions = S(r["Instructions"]) });
            }
            return items;
        }

        public static Health LoadHealth(int patientId)
        {
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand("SELECT * FROM PatientHealth WHERE PatientID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", patientId);
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return new Health();
                    return new Health
                    {
                        BloodGroup = S(r["BloodGroup"]), Allergies = S(r["Allergies"]), Conditions = S(r["Conditions"]),
                        CurrentMedicines = S(r["CurrentMedicines"]), PastSurgeries = S(r["PastSurgeries"]),
                        UpdatedBy = S(r["UpdatedBy"]), UpdatedAt = T(r["UpdatedAt"])
                    };
                }
            }
        }

        // ── Checks ────────────────────────────────────────────────────────────

        static readonly Regex Bp = new Regex(@"^\s*(\d{2,3})\s*/\s*(\d{2,3})\s*$");

        /// <summary>Reads the vitals as typed; problems lists anything out of a believable range.</summary>
        public static void ParseVitals(string bp, string pulse, string temp, string weight, string spo2, Notes into, List<string> problems)
        {
            bp = (bp ?? "").Trim();
            if (bp == "") into.BloodPressure = null;
            else
            {
                Match m = Bp.Match(bp);
                int sys = m.Success ? int.Parse(m.Groups[1].Value) : 0, dia = m.Success ? int.Parse(m.Groups[2].Value) : 0;
                if (!m.Success || sys < 50 || sys > 260 || dia < 30 || dia > 160 || dia >= sys) problems.Add("Blood pressure should look like 120/80");
                else into.BloodPressure = sys + "/" + dia;
            }
            into.Pulse = Number(pulse, 20, 250, "Pulse (20 to 250)", problems);
            into.SpO2 = Number(spo2, 50, 100, "SpO2 (50 to 100 %)", problems);
            into.TemperatureF = Decimal(temp, 90, 110, "Temperature (90 to 110 °F)", problems);
            into.WeightKg = Decimal(weight, 0.5m, 300, "Weight (0.5 to 300 kg)", problems);
        }

        static int? Number(string text, int min, int max, string label, List<string> problems)
        {
            text = (text ?? "").Trim();
            if (text == "") return null;
            int v;
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out v) && v >= min && v <= max) return v;
            problems.Add(label);
            return null;
        }

        static decimal? Decimal(string text, decimal min, decimal max, string label, List<string> problems)
        {
            text = (text ?? "").Trim().Replace(',', '.');
            if (text == "") return null;
            decimal v;
            if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out v) && v >= min && v <= max) return Math.Round(v, 1);
            problems.Add(label);
            return null;
        }

        // ── Saving ────────────────────────────────────────────────────────────

        const string EnsureRow = @"IF NOT EXISTS (SELECT 1 FROM VisitNotes WHERE AppointmentID = @ID)
                                       INSERT INTO VisitNotes (AppointmentID) VALUES (@ID);";

        public static void SaveVitals(int appointmentId, Notes n, string by)
        {
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand(EnsureRow + @"
                UPDATE VisitNotes SET BloodPressure = @BP, Pulse = @Pulse, TemperatureF = @Temp, WeightKg = @Weight, SpO2 = @SpO2,
                                      VitalsBy = @By, VitalsAt = GETDATE()
                WHERE AppointmentID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", appointmentId);
                cmd.Parameters.AddWithValue("@BP", Db(n.BloodPressure));
                cmd.Parameters.AddWithValue("@Pulse", Db(n.Pulse));
                cmd.Parameters.AddWithValue("@Temp", Db(n.TemperatureF));
                cmd.Parameters.AddWithValue("@Weight", Db(n.WeightKg));
                cmd.Parameters.AddWithValue("@SpO2", Db(n.SpO2));
                cmd.Parameters.AddWithValue("@By", Db(by));
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>The doctor's notes and the whole prescription (replaces the previous lines), in one transaction.</summary>
        public static void SaveClinical(int appointmentId, Notes n, List<RxItem> items, int doctorId)
        {
            using (var con = new SqlConnection(Cs))
            {
                con.Open();
                using (SqlTransaction tx = con.BeginTransaction())
                {
                    using (var cmd = new SqlCommand(EnsureRow + @"
                        -- a new follow-up date starts afresh: it may be reminded again and is open again
                        UPDATE VisitNotes SET Complaint = @Complaint, Diagnosis = @Diagnosis, Advice = @Advice,
                                              FollowUpReminderSentAt = CASE WHEN FollowUpDate = @FollowUp THEN FollowUpReminderSentAt END,
                                              FollowUpClosedAt = CASE WHEN FollowUpDate = @FollowUp THEN FollowUpClosedAt END,
                                              FollowUpClosedBy = CASE WHEN FollowUpDate = @FollowUp THEN FollowUpClosedBy END,
                                              FollowUpDate = @FollowUp,
                                              NotesByDoctorID = @DoctorID, NotesAt = GETDATE()
                        WHERE AppointmentID = @ID;
                        DELETE FROM PrescriptionItems WHERE AppointmentID = @ID;", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@ID", appointmentId);
                        cmd.Parameters.AddWithValue("@Complaint", Db(n.Complaint));
                        cmd.Parameters.AddWithValue("@Diagnosis", Db(n.Diagnosis));
                        cmd.Parameters.AddWithValue("@Advice", Db(n.Advice));
                        cmd.Parameters.AddWithValue("@FollowUp", Db(n.FollowUpDate));
                        cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                        cmd.ExecuteNonQuery();
                    }
                    int order = 0;
                    foreach (RxItem it in items)
                        using (var cmd = new SqlCommand(@"INSERT INTO PrescriptionItems (AppointmentID, SortOrder, Medicine, Dose, Frequency, Days, Instructions)
                                                          VALUES (@ID, @Order, @Medicine, @Dose, @Frequency, @Days, @Instructions)", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@ID", appointmentId);
                            cmd.Parameters.AddWithValue("@Order", order++);
                            cmd.Parameters.AddWithValue("@Medicine", it.Medicine.Trim());
                            cmd.Parameters.AddWithValue("@Dose", Db(it.Dose));
                            cmd.Parameters.AddWithValue("@Frequency", Db(it.Frequency));
                            cmd.Parameters.AddWithValue("@Days", Db(it.Days));
                            cmd.Parameters.AddWithValue("@Instructions", Db(it.Instructions));
                            cmd.ExecuteNonQuery();
                        }
                    tx.Commit();
                }
            }
        }

        public static void SaveHealth(int patientId, Health h, string by)
        {
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM PatientHealth WHERE PatientID = @ID) INSERT INTO PatientHealth (PatientID) VALUES (@ID);
                UPDATE PatientHealth SET BloodGroup = @BloodGroup, Allergies = @Allergies, Conditions = @Conditions,
                                         CurrentMedicines = @Medicines, PastSurgeries = @Surgeries, UpdatedBy = @By, UpdatedAt = GETDATE()
                WHERE PatientID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", patientId);
                cmd.Parameters.AddWithValue("@BloodGroup", Db(h.BloodGroup));
                cmd.Parameters.AddWithValue("@Allergies", Db(h.Allergies));
                cmd.Parameters.AddWithValue("@Conditions", Db(h.Conditions));
                cmd.Parameters.AddWithValue("@Medicines", Db(h.CurrentMedicines));
                cmd.Parameters.AddWithValue("@Surgeries", Db(h.PastSurgeries));
                cmd.Parameters.AddWithValue("@By", Db(by));
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ── Follow-ups ────────────────────────────────────────────────────────

        /// <summary>
        /// Open follow-ups: the doctor set a follow-up date, nobody closed it, and the patient has no later visit
        /// with that doctor yet (a cancelled or missed one does not count). Only the date, doctor and patient are
        /// returned, never the diagnosis, so reception can use the list. Filters are optional; soonest first.
        /// </summary>
        public static System.Data.DataTable OpenFollowUps(int? userId = null, int? doctorId = null, DateTime? dueBy = null, int? appointmentId = null)
        {
            var dt = new System.Data.DataTable();
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand(@"
                SELECT vn.AppointmentID, vn.FollowUpDate, vn.FollowUpReminderSentAt, a.AppointDate AS VisitDate, a.PatientID, a.DoctorID,
                       p.PatientName, p.PatientCode, p.UserID,
                       COALESCE(NULLIF(pu.Mobile, ''), NULLIF(p.Mobile, ''), '') AS Mobile,
                       COALESCE(NULLIF(u.Email, ''), NULLIF(pu.Email, ''), NULLIF(p.Email, ''), '') AS Email,
                       d.Name AS Doctor, d.Specialization
                FROM VisitNotes vn
                JOIN Appointments a ON a.ID = vn.AppointmentID
                JOIN Patients p ON p.PatientID = a.PatientID
                JOIN Doctors d ON d.DoctorID = a.DoctorID
                LEFT JOIN Users u ON u.UserID = a.UserID
                LEFT JOIN Users pu ON pu.UserID = p.UserID
                WHERE vn.FollowUpDate IS NOT NULL AND vn.FollowUpClosedAt IS NULL
                  AND a.Status <> 'Cancelled' AND ISNULL(p.IsDeleted, 0) = 0 AND ISNULL(d.IsDeleted, 0) = 0
                  AND NOT EXISTS (SELECT 1 FROM Appointments b
                                  WHERE b.PatientID = a.PatientID AND b.DoctorID = a.DoctorID AND b.AppointDate > a.AppointDate
                                    AND b.Status NOT IN ('Cancelled', 'No Show'))
                  AND (@UserID IS NULL OR p.UserID = @UserID)
                  AND (@DoctorID IS NULL OR a.DoctorID = @DoctorID)
                  AND (@DueBy IS NULL OR vn.FollowUpDate <= @DueBy)
                  AND (@AppointmentID IS NULL OR vn.AppointmentID = @AppointmentID)
                ORDER BY vn.FollowUpDate, p.PatientName", con))
            {
                cmd.Parameters.AddWithValue("@UserID", Db(userId));
                cmd.Parameters.AddWithValue("@DoctorID", Db(doctorId));
                cmd.Parameters.Add("@DueBy", System.Data.SqlDbType.Date).Value = Db(dueBy.HasValue ? (object)dueBy.Value.Date : null);
                cmd.Parameters.AddWithValue("@AppointmentID", Db(appointmentId));
                new SqlDataAdapter(cmd).Fill(dt);
            }
            return dt;
        }

        /// <summary>Staff decided a follow-up is not needed (patient declined, seen elsewhere ...).</summary>
        public static bool CloseFollowUp(int appointmentId, string by)
        {
            using (var con = new SqlConnection(Cs))
            using (var cmd = new SqlCommand(@"UPDATE VisitNotes SET FollowUpClosedAt = GETDATE(), FollowUpClosedBy = @By
                                              WHERE AppointmentID = @ID AND FollowUpDate IS NOT NULL AND FollowUpClosedAt IS NULL", con))
            {
                cmd.Parameters.AddWithValue("@ID", appointmentId);
                cmd.Parameters.AddWithValue("@By", Db(by));
                con.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        /// <summary>The online booking page for this follow-up: doctor, date and patient already chosen.</summary>
        public static string FollowUpBookingPath(int doctorId, DateTime followUp, int patientId)
        {
            DateTime date = followUp.Date < DateTime.Today ? DateTime.Today : followUp.Date;   // overdue: from today
            return "~/Features/Appointments/Confirm-Appointment.aspx?doctorId=" + doctorId + "&date=" + date.ToString("yyyy-MM-dd") + "&patientId=" + patientId;
        }

        /// <summary>The logged-in staff member's name, for "vitals by" / "updated by".</summary>
        public static string StaffName(System.Web.SessionState.HttpSessionState session)
        {
            string role = session["UserRole"] as string;
            string name = Convert.ToString(role == DoctorRole ? session["DoctorName"] : role == ReceptionRole ? session["ReceptionistName"] : session["UserName"]);
            if (role == DoctorRole && !string.IsNullOrWhiteSpace(name)) name = "Dr. " + DoctorName.WithoutTitle(name);
            string label = role == DoctorRole ? "Doctor" : role == ReceptionRole ? "Reception" : role == AdminRole ? "Admin" : role;
            return string.IsNullOrWhiteSpace(name) ? label : name + " (" + label + ")";
        }
    }
}
