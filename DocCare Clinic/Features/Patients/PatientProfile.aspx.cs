using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    public partial class PatientProfile : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        string userRole = "";
        const int VisitsPerPage = 5;

        int PatientId
        {
            get { int id; return int.TryParse(Request.QueryString["Id"], out id) ? id : 0; }
        }

        int VisitPage
        {
            get { return ViewState["VisitPage"] == null ? 0 : (int)ViewState["VisitPage"]; }
            set { ViewState["VisitPage"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            userRole = Session["UserRole"] as string;
            if (PatientId == 0)
            {
                Response.Redirect(ResolveUrl("~/Features/Patients/Patients.aspx"));
                return;
            }
            if (userRole == "Doctor") HideAdminMenus();
            lnkEditHealth.Visible = MedicalRecords.EditsHealth(userRole) && !pnlHealthEdit.Visible;
            if (!IsPostBack)
            {
                LoadPatientDetails(PatientId);
                LoadHealth();
                LoadMedicalHistory(PatientId);
            }
        }

        private void HideAdminMenus()
        {
            // Find menu controls inside MasterPage
            Control menuReceptionist = Master.FindControl("menuReceptionist");
            Control menuDoctor = Master.FindControl("menuDoctor");
            Control menuDepartment = Master.FindControl("menuDepartment");

            if (menuReceptionist != null) menuReceptionist.Visible = false;
            if (menuDoctor != null) menuDoctor.Visible = false;
            if (menuDepartment != null) menuDepartment.Visible = false;
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            userRole = Session["UserRole"] as string;

            if (string.IsNullOrEmpty(userRole))
            {
                Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx")); // Redirect if no role is found
            }

            if (userRole == "Doctor")
            {
                this.MasterPageFile = ResolveUrl("~/Features/Doctors/DoctorAdmin.Master"); // Use Doctor's MasterPage
            }
            else if (userRole == "Receptionist")
            {
                this.MasterPageFile = ResolveUrl("~/Features/Receptionists/ReceptionistAdmin.Master");
            }
            else
            {
                this.MasterPageFile = ResolveUrl("~/Features/Admin/SuperAdmin.Master");
            }
        }

        // ── Health summary ────────────────────────────────────────────────────

        void LoadHealth()
        {
            MedicalRecords.Health h = MedicalRecords.LoadHealth(PatientId);
            Fact(litBloodGroup, h.BloodGroup, "Not known");
            Fact(litAllergiesFact, h.Allergies, "None known");
            Fact(litConditions, h.Conditions, "None recorded");
            Fact(litMedicines, h.CurrentMedicines, "None recorded");
            Fact(litSurgeries, h.PastSurgeries, "None recorded");
            litHealthUpdated.Text = h.UpdatedAt.HasValue
                ? "Updated by " + HttpUtility.HtmlEncode(h.UpdatedBy) + " on " + h.UpdatedAt.Value.ToString("dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture)
                : (MedicalRecords.EditsHealth(userRole) ? "Nothing recorded yet. Use Edit to add allergies, blood group and long-term conditions." : "Nothing recorded yet.");
            pnlAllergyBanner.Visible = !string.IsNullOrWhiteSpace(h.Allergies);
            litAllergyBanner.Text = HttpUtility.HtmlEncode(h.Allergies);

            ddlBloodGroup.SelectedValue = ddlBloodGroup.Items.FindByValue(h.BloodGroup ?? "") != null ? (h.BloodGroup ?? "") : "";
            txtAllergies.Text = h.Allergies;
            txtConditions.Text = h.Conditions;
            txtMedicines.Text = h.CurrentMedicines;
            txtSurgeries.Text = h.PastSurgeries;
        }

        // Literals wrap the value in <strong> already; an empty fact is shown in grey
        static void Fact(Literal lit, string value, string none)
        {
            lit.Text = string.IsNullOrWhiteSpace(value) ? "<span style=\"color:#9ca3af; font-weight:500;\">" + none + "</span>" : HttpUtility.HtmlEncode(value);
        }

        protected void lnkEditHealth_Click(object sender, EventArgs e)
        {
            if (!MedicalRecords.EditsHealth(userRole)) return;
            pnlHealthView.Visible = false;
            pnlHealthEdit.Visible = true;
            lnkEditHealth.Visible = false;
        }

        protected void btnCancelHealth_Click(object sender, EventArgs e)
        {
            pnlHealthEdit.Visible = false;
            pnlHealthView.Visible = true;
            lnkEditHealth.Visible = MedicalRecords.EditsHealth(userRole);
            LoadHealth();
        }

        protected void btnSaveHealth_Click(object sender, EventArgs e)
        {
            if (!MedicalRecords.EditsHealth(userRole)) return;
            Func<TextBox, string> v = t => { string s = t.Text.Trim(); return s.Length > 500 ? s.Substring(0, 500) : s; };
            MedicalRecords.SaveHealth(PatientId, new MedicalRecords.Health
            {
                BloodGroup = ddlBloodGroup.SelectedValue, Allergies = v(txtAllergies), Conditions = v(txtConditions),
                CurrentMedicines = v(txtMedicines), PastSurgeries = v(txtSurgeries)
            }, MedicalRecords.StaffName(Session));
            pnlHealthEdit.Visible = false;
            pnlHealthView.Visible = true;
            lnkEditHealth.Visible = true;
            LoadHealth();
            ClientScript.RegisterStartupScript(GetType(), "healthSaved", "swalAlert('success', 'Saved', 'The health summary is updated.');", true);
        }

        // ── Visits (medical history) ──────────────────────────────────────────

        private void LoadMedicalHistory(int patientId)
        {
            // A visit = an appointment that was not cancelled, from today back; newest first
            string query = @"
                SELECT a.ID, a.AppointDate, a.AppointTime, a.Status, a.TokenNo, a.DoctorID, d.Name AS Doctor, d.Specialization,
                       COALESCE(a.TotalFee, d.Charges, 0) AS Fee,
                       vn.BloodPressure, vn.Pulse, vn.TemperatureF, vn.WeightKg, vn.SpO2, vn.Complaint, vn.Diagnosis, vn.Advice, vn.FollowUpDate,
                       (SELECT COUNT(*) FROM PrescriptionItems pi WHERE pi.AppointmentID = a.ID) AS RxCount
                FROM Appointments a
                INNER JOIN Doctors d ON a.DoctorID = d.DoctorID
                LEFT JOIN VisitNotes vn ON vn.AppointmentID = a.ID
                WHERE a.PatientID = @PatientId AND a.Status <> 'Cancelled' AND a.AppointDate <= CAST(GETDATE() AS DATE)
                ORDER BY a.AppointDate DESC, a.AppointTime DESC;

                SELECT TOP 1 a.AppointDate, a.AppointTime, d.Name AS Doctor
                FROM Appointments a INNER JOIN Doctors d ON a.DoctorID = d.DoctorID
                WHERE a.PatientID = @PatientId AND a.Status IN ('Pending', 'Confirmed') AND a.AppointDate > CAST(GETDATE() AS DATE)
                ORDER BY a.AppointDate, a.AppointTime;";

            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PatientId", patientId);
                new SqlDataAdapter(cmd).Fill(ds);
            }
            DataTable visits = ds.Tables[0];

            int pages = Math.Max(1, (int)Math.Ceiling(visits.Rows.Count / (double)VisitsPerPage));
            VisitPage = Math.Min(Math.Max(VisitPage, 0), pages - 1);
            DataTable page = visits.Clone();
            foreach (DataRow r in visits.Rows.Cast<DataRow>().Skip(VisitPage * VisitsPerPage).Take(VisitsPerPage)) page.ImportRow(r);
            rptVisits.DataSource = page;
            rptVisits.DataBind();

            pnlNoVisits.Visible = visits.Rows.Count == 0;
            pnlVisitPager.Visible = pages > 1;
            litVisitPager.Text = visits.Rows.Count == 0 ? "" : "Showing " + (VisitPage * VisitsPerPage + 1) + "&ndash;" +
                                 Math.Min((VisitPage + 1) * VisitsPerPage, visits.Rows.Count) + " of " + visits.Rows.Count + " visits";
            lnkVisitPrev.Visible = VisitPage > 0;
            lnkVisitNext.Visible = VisitPage < pages - 1;

            DataTable next = ds.Tables[1];
            litNextAppt.Text = next.Rows.Count == 0 ? "" :
                "Next appointment: " + SlotService.FormatDate(Convert.ToDateTime(next.Rows[0]["AppointDate"])) +
                (next.Rows[0]["AppointTime"] == DBNull.Value ? "" : " at " + SlotService.Format((TimeSpan)next.Rows[0]["AppointTime"])) +
                " with Dr. " + HttpUtility.HtmlEncode(DoctorName.WithoutTitle(next.Rows[0]["Doctor"].ToString()));
        }

        protected void lnkVisitPrev_Click(object sender, EventArgs e) { VisitPage--; LoadMedicalHistory(PatientId); }
        protected void lnkVisitNext_Click(object sender, EventArgs e) { VisitPage++; LoadMedicalHistory(PatientId); }

        protected string TimeText(object time)
        {
            return time == null || time == DBNull.Value ? "" : SlotService.Format((TimeSpan)time);
        }

        // One line in the closed row: the diagnosis for doctors / admin, otherwise whether vitals were taken
        protected string SummaryText(object item)
        {
            DataRowView r = (DataRowView)item;
            if (MedicalRecords.SeesClinical(userRole) && r["Diagnosis"] != DBNull.Value && r["Diagnosis"].ToString().Trim() != "")
                return HttpUtility.HtmlEncode(r["Diagnosis"].ToString());
            if (r["Status"].ToString() == AppointmentStatus.NoShow) return "<span style=\"color:#9ca3af;\">Did not attend</span>";
            bool notes = r["Complaint"] != DBNull.Value || r["Diagnosis"] != DBNull.Value || Convert.ToInt32(r["RxCount"]) > 0;
            bool vitals = r["BloodPressure"] != DBNull.Value || r["Pulse"] != DBNull.Value || r["TemperatureF"] != DBNull.Value || r["WeightKg"] != DBNull.Value || r["SpO2"] != DBNull.Value;
            string s = MedicalRecords.SeesClinical(userRole) ? (notes ? "Notes recorded" : vitals ? "Vitals only" : "No notes yet")
                                                             : (vitals ? "Vitals recorded" : "No vitals recorded");
            return "<span style=\"color:#9ca3af;\">" + s + "</span>";
        }

        // The opened row: vitals for all staff; complaint, diagnosis, prescription, advice for doctors / admin
        protected string VisitDetails(object item)
        {
            DataRowView r = (DataRowView)item;
            int id = Convert.ToInt32(r["ID"]);
            Func<object, string> enc = o => HttpUtility.HtmlEncode(Convert.ToString(o));
            var n = new MedicalRecords.Notes
            {
                BloodPressure = r["BloodPressure"] as string,
                Pulse = r["Pulse"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["Pulse"]),
                TemperatureF = r["TemperatureF"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["TemperatureF"]),
                WeightKg = r["WeightKg"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["WeightKg"]),
                SpO2 = r["SpO2"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["SpO2"])
            };
            var html = new System.Text.StringBuilder();
            html.Append("<h6>Vitals</h6><p>").Append(n.HasVitals ? enc(n.VitalsLine()) : "<span class=\"muted\">Not recorded</span>").Append("</p>");

            bool clinical = MedicalRecords.SeesClinical(userRole);
            bool hasNotes = false;
            if (clinical)
            {
                Action<string, object> section = (title, value) =>
                {
                    if (value == DBNull.Value || Convert.ToString(value).Trim() == "") return;
                    hasNotes = true;
                    html.Append("<h6>").Append(title).Append("</h6><p>").Append(enc(value)).Append("</p>");
                };
                section("Complaint", r["Complaint"]);
                section("Diagnosis", r["Diagnosis"]);
                if (Convert.ToInt32(r["RxCount"]) > 0)
                {
                    hasNotes = true;
                    html.Append("<h6>Prescription</h6><ol>");
                    foreach (MedicalRecords.RxItem it in MedicalRecords.LoadRx(id))
                        html.Append("<li><strong>").Append(enc(it.Medicine)).Append("</strong>")
                            .Append(string.Join("", new[] { it.Dose, it.Frequency, it.Days.HasValue ? it.Days + " days" : null, it.Instructions }
                                .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => " &middot; " + enc(x))))
                            .Append("</li>");
                    html.Append("</ol>");
                }
                section("Advice", r["Advice"]);
                if (r["FollowUpDate"] != DBNull.Value)
                {
                    hasNotes = true;
                    html.Append("<h6>Follow-up</h6><p>").Append(SlotService.FormatDate(Convert.ToDateTime(r["FollowUpDate"]))).Append("</p>");
                }
                if (!hasNotes) html.Append("<h6>Consultation</h6><p class=\"muted\">No notes recorded for this visit.</p>");
            }

            html.Append("<div class=\"mh-actions\">");
            if (r["Status"].ToString() != AppointmentStatus.NoShow)
                html.Append("<a class=\"btn btn-primary btn-sm\" href=\"").Append(ResolveUrl("~/Features/Patients/VisitNotes.aspx?AppointmentID=" + id)).Append("\">")
                    .Append(OpenNotesLabel(r)).Append("</a>");
            if (clinical && hasNotes)
                html.Append("<a class=\"btn btn-outline-primary btn-sm\" target=\"_blank\" href=\"").Append(ResolveUrl("~/Features/Patients/Prescription.aspx?AppointmentID=" + id))
                    .Append("\"><i class=\"fa-solid fa-print\"></i> Print prescription</a>");
            html.Append("</div>");
            return html.ToString();
        }

        string OpenNotesLabel(DataRowView r)
        {
            int doctorId;
            bool own = userRole == "Doctor" && Session["DoctorID"] != null && int.TryParse(Session["DoctorID"].ToString(), out doctorId) && doctorId == Convert.ToInt32(r["DoctorID"]);
            if (own) return "<i class=\"fa-regular fa-pen-to-square\"></i> Write visit notes";
            if (userRole == "Receptionist") return "<i class=\"fa-solid fa-heart-pulse\"></i> Vitals";
            return "<i class=\"fa-regular fa-file-lines\"></i> Open visit notes";
        }

        private void LoadPatientDetails(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        p.PatientName, 
                        p.DateOfBirth, 
                        p.Gender,                     
                        COALESCE(u.Mobile, p.Mobile, 'N/A') AS Mobile,
                        COALESCE(u.Email, p.Email, 'N/A') AS Email,
                        COALESCE(u.Address, p.Address, 'N/A') AS Address
                    FROM Patients p
                    LEFT JOIN Users u ON p.UserId = u.UserId
                    WHERE p.PatientID = @PatientId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PatientId", patientId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblPatientName.Text = HttpUtility.HtmlEncode(reader["PatientName"].ToString()) +
                            " <span class=\"patient-code\" style=\"display:inline; font-size:13px;\">" + PatientCode.Of(patientId) + "</span>";

                        // ✅ Set Gender
                        lblGender.Text = reader["Gender"].ToString();

                        // ✅ Set Date of Birth
                        DateTime dob = Convert.ToDateTime(reader["DateOfBirth"]);
                        lblDob.Text = dob.ToString("dd-MM-yyyy");

                        // ✅ Calculate Age Automatically
                        int age = CalculateAge(dob);
                        lblAge.Text = age.ToString() + " Years";

                        // ✅ Set Full Name
                        lblFullName.Text = HttpUtility.HtmlEncode(reader["PatientName"].ToString()) + " (Patient ID " + PatientCode.Of(patientId) + ")";

                        // ✅ Set Mobile (if null then N/A)
                        lblMobile.Text = !string.IsNullOrEmpty(reader["Mobile"].ToString())
                            ? reader["Mobile"].ToString()
                            : "N/A";

                        // ✅ Set Email (if null then N/A)
                        lblEmail.Text = !string.IsNullOrEmpty(reader["Email"].ToString())
                            ? reader["Email"].ToString()
                            : "N/A";

                        // ✅ Set Address (if null then N/A)
                        lblAddress.Text = !string.IsNullOrEmpty(reader["Address"].ToString())
                            ? reader["Address"].ToString()
                            : "N/A";
                    }
                    conn.Close();
                }
            }
        }

        private int CalculateAge(DateTime dob)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}