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
    public partial class AddPatient : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            string userRole = Session["UserRole"] as string;
            if (string.IsNullOrEmpty(userRole))
            {
                Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
            }
            if (userRole == "Doctor")
            {
                HideAdminMenus();
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
            string userRole = Session["UserRole"] as string;

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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DateTime dob;
            int age;

            string patientName = txtPatientName.Text.Trim();
            string dobInput = txtDob.Text.Trim();
            string gender = rblGender.SelectedValue;
            string status = rblStatus.SelectedValue;
            string mobile = txtMobile.Text.Trim();
            string email = txtEmail.Text.Trim();
            string address = txtAddress.Text.Trim();

            lblErrorMessage.Visible = false;

            int doctorId;
            if (Session["DoctorID"] == null || !int.TryParse(Session["DoctorID"].ToString(), out doctorId))
            {
                lblErrorMessage.Text = "*Doctor ID is missing. Please log in again.";
                lblErrorMessage.Visible = true;
                return;
            }

            // Validate Date of Birth and dynamically calculate age
            if (string.IsNullOrEmpty(dobInput) || !DateTime.TryParse(dobInput, out dob))
            {
                lblErrorMessage.Text = "*Please enter a valid Date of Birth.";
                lblErrorMessage.Visible = true;
                return;
            }

            // Calculate age dynamically
            age = CalculateAge(dob);
            txtAge.Text = age.ToString(); // Set the calculated age in the textbox

            // Validate all fields
            if (string.IsNullOrEmpty(patientName) ||
                string.IsNullOrEmpty(gender) ||
                string.IsNullOrEmpty(status))
            {
                lblErrorMessage.Text = "*All fields are required. Please fill out all fields correctly.";
                lblErrorMessage.Visible = true;
                return;
            }

            if (age <= 0 || age > 120)
            {
                lblErrorMessage.Text = "*Age calculated is invalid. Please check the Date of Birth.";
                lblErrorMessage.Visible = true;
                return;
            }

            lblErrorMessage.Visible = false;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        INSERT INTO Patients 
                        (DoctorID, PatientName, DateOfBirth, Age, Gender, Status, Mobile, Email, Address) 
                        VALUES 
                        (@DoctorID, @PatientName, @DateOfBirth, @Age, @Gender, @Status, @Mobile, @Email, @Address)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                        cmd.Parameters.AddWithValue("@PatientName", patientName);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dob);
                        cmd.Parameters.AddWithValue("@Age", age);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@Mobile", mobile);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.ExecuteNonQuery();
                    }
                }

                lblErrorMessage.Text = "*Patient added successfully!";
                lblErrorMessage.ForeColor = System.Drawing.Color.Green;
                lblErrorMessage.Visible = true;
                clear();

                string script = "setTimeout(function() { window.location.href = '" + ResolveUrl("~/Features/Patients/Patients.aspx") + "'; }, 2000);";
                ClientScript.RegisterStartupScript(this.GetType(), "Redirect", script, true);
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
            }
        }

        // Function to calculate age dynamically
        int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
                age--;
            return age;
        }

        // Method to clear all fields
        private void clear()
        {
            txtPatientName.Text = string.Empty;
            txtDob.Text = string.Empty;
            txtAge.Text = string.Empty;

            rblGender.ClearSelection();

            rblStatus.ClearSelection();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            clear();
            Response.Redirect(ResolveUrl("~/Features/Patients/Patients.aspx"));
        }
    }
}