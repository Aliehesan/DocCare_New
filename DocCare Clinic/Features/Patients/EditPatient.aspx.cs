using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace DocCare_Clinic
{
    public partial class EditPatient : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        string userRole = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            userRole = Session["UserRole"] as string;
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(userRole))
                {
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }
                int patientId;
                if (Request.QueryString["Id"] != null && int.TryParse(Request.QueryString["Id"], out patientId))
                {
                    LoadPatientDetails(patientId);
                }
                if (userRole == "Doctor")
                {
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

        private void LoadPatientDetails(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                        SELECT 
                            p.PatientID, 
                            p.PatientName, 
                            p.DateOfBirth,
                            p.Age,
                            p.Gender, 
                            p.Status,                            
                            COALESCE(u.Mobile, p.Mobile, 'N/A') AS Mobile,
                            COALESCE(u.Address, p.Address, 'N/A') AS Address,                            
                            COALESCE(u.Email, p.Email, 'N/A') AS Email                             
                        FROM 
                            Patients p
                        LEFT JOIN 
                            Users u ON p.UserID = u.UserID
                        WHERE 
                            p.PatientId = @PatientId;
                        ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PatientId", patientId);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtName.Text = reader["PatientName"].ToString();
                        txtMobile.Text = reader["Mobile"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        txtAddress.Text = reader["Address"].ToString();
                        txtDOB.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("yyyy-MM-dd");
                        txtAge.Text = reader["Age"].ToString();

                        rblGender.SelectedValue = reader["Gender"].ToString();
                        rblStatus.SelectedValue = reader["Status"].ToString();
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string mobile = txtMobile.Text.Trim();
            string email = txtEmail.Text.Trim();
            string dateOfBirthText = txtDOB.Text.Trim();
            string ageText = txtAge.Text.Trim();
            string gender = rblGender.SelectedValue;
            string status = rblStatus.SelectedValue;

            DateTime dateOfBirth;
            int age;

            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(mobile) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(gender) ||
                string.IsNullOrEmpty(status) ||
                !DateTime.TryParse(dateOfBirthText, out dateOfBirth) ||
                !int.TryParse(ageText, out age))
            {
                // Show error message if validation fails
                lblErrorMessage.Text = "*Please fill out all fields correctly.";
                lblErrorMessage.Visible = true;
                return;
            }

            int patientId = Convert.ToInt32(Request.QueryString["Id"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string updateQuery = @"
                    UPDATE Patients 
                            SET 
                                PatientName = @Name, 
                                DateOfBirth = @DateOfBirth, 
                                Age = @Age, 
                                Gender = @Gender, 
                                Status = @Status,
                                Mobile = CASE WHEN (SELECT UserID FROM Patients WHERE PatientId = @PatientId) IS NULL THEN @Mobile ELSE Mobile END,
                                Address = CASE WHEN (SELECT UserID FROM Patients WHERE PatientId = @PatientId) IS NULL THEN @Address ELSE Address END,
                                Email = CASE WHEN (SELECT UserID FROM Patients WHERE PatientId = @PatientId) IS NULL THEN @Email ELSE Email END
                            WHERE 
                                PatientId = @PatientId;

                            UPDATE Users 
                            SET 
                                Mobile = @Mobile, 
                                Address = @Address, 
                                Email = @Email
                            WHERE 
                                UserID = (SELECT UserID FROM Patients WHERE PatientId = @PatientId) AND UserID IS NOT NULL;";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                    cmd.Parameters.AddWithValue("@Age", age);
                    cmd.Parameters.AddWithValue("@Gender", gender);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Mobile", txtMobile.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@PatientId", patientId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Redirect to the patient details page after successful update
            Response.Redirect(ResolveUrl("~/Features/Patients/Patients.aspx") + "?Id=" + patientId);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Features/Patients/Patients.aspx"));
        }
    }
}