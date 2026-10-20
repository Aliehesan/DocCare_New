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
        protected void Page_Load(object sender, EventArgs e)
        {
            userRole = Session["UserRole"] as string;
            int PatientId = 0;
            if (!IsPostBack)
            {
                if (Request.QueryString["Id"] != null && int.TryParse(Request.QueryString["Id"], out PatientId))
                {
                    LoadPatientDetails(PatientId);
                    LoadMedicalHistory(PatientId);
                }
                else
                {
                    Response.Redirect(ResolveUrl("~/Features/Patients/Patients.aspx"));
                }
                if (userRole == "Doctor")
                {
                    LoadPatientDetails(PatientId);
                    LoadMedicalHistory(PatientId);
                    HideAdminMenus();
                }
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

        private void LoadMedicalHistory(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    a.AppointDate AS Date, 
                    d.Name AS Doctor, 
                    d.Charges AS Charges
                FROM Appointments a
                INNER JOIN Doctors d ON a.DoctorId = d.DoctorId
                WHERE a.PatientId = @PatientId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PatientId", patientId);
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvMedicalHistory.DataSource = dt;
                    gvMedicalHistory.DataBind();
                    conn.Close();
                }
            }
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

        protected void gvMedicalHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            int PatientId = Convert.ToInt32(Request.QueryString["PatientId"]);
            gvMedicalHistory.PageIndex = e.NewPageIndex;
            LoadMedicalHistory(PatientId); // Rebind the grid after changing the page
        }

        protected void gvMedicalHistory_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = (DataTable)ViewState["MedicalHistory"];
            if (dt != null)
            {
                DataView dv = new DataView(dt);
                dv.Sort = e.SortExpression;
                gvMedicalHistory.DataSource = dv;
                gvMedicalHistory.DataBind();
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