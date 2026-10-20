using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using TaskManagement.config;
using System.Security.Cryptography;
using System.Text;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    public partial class AddDoctor : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string userRole = Session["UserRole"] as string;
                if (string.IsNullOrEmpty(userRole))
                {
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }

                if (userRole == "Receptionist")
                {
                    HideAdminMenus();
                }
                BindDepartmentDropdown();
            }
        }

        private void HideAdminMenus()
        {
            Control menuReceptionist = Master.FindControl("menuReceptionist");
            Control menuDepartment   = Master.FindControl("menuDepartment");

            if (menuReceptionist != null) menuReceptionist.Visible = false;
            if (menuDepartment   != null) menuDepartment.Visible   = false;
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            string userRole = Session["UserRole"] as string;

            if (string.IsNullOrEmpty(userRole))
            {
                Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
            }

            if (userRole == "Doctor")
                this.MasterPageFile = ResolveUrl("~/Features/Doctors/DoctorAdmin.Master");
            else if (userRole == "Receptionist")
                this.MasterPageFile = ResolveUrl("~/Features/Receptionists/ReceptionistAdmin.Master");
            else
                this.MasterPageFile = ResolveUrl("~/Features/Admin/SuperAdmin.Master");
        }

        private void BindDepartmentDropdown()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query   = "SELECT DepartmentID, Department FROM Departments WHERE Status = 'Active'";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlDepartment.DataSource     = reader;
                ddlDepartment.DataTextField  = "Department";
                ddlDepartment.DataValueField = "DepartmentID";
                ddlDepartment.DataBind();
                conn.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                // ── Collect form values ────────────────────────────────────────────
                string name           = DoctorName.WithoutTitle(txtName.Text); // pages add "Dr. " themselves
                string designation    = ddlDesignation.SelectedValue;
                string department     = ddlDepartment.SelectedValue;
                string specialization = ddlSpecialization.SelectedValue;
                string degree         = txtDegree.Text.Trim();
                string mobile         = txtMobile.Text.Trim();
                string email          = txtEmail.Text.Trim();
                string password       = txtPassword.Text.Trim();
                string hashedPassword = HashPassword(password);
                string chargesText    = txtCharges.Text.Trim();
                string joinDateText   = txtJoinDate.Text.Trim();
                string gender         = rblGender.SelectedValue;

                decimal charges;
                DateTime joinDate;

                // ── Validate ───────────────────────────────────────────────────────
                if (string.IsNullOrEmpty(name)           ||
                    string.IsNullOrEmpty(department)     ||
                    string.IsNullOrEmpty(specialization) ||
                    string.IsNullOrEmpty(degree)         ||
                    string.IsNullOrEmpty(txtExperience.Text) ||
                    string.IsNullOrEmpty(txtHospital.Text)   ||
                    string.IsNullOrEmpty(txtInstitute.Text)  ||
                    string.IsNullOrEmpty(designation)    ||
                    string.IsNullOrEmpty(txtYearOfCompletion.Text) ||
                    string.IsNullOrEmpty(txtLocation.Text) ||
                    string.IsNullOrEmpty(mobile)         ||
                    string.IsNullOrEmpty(email)          ||
                    string.IsNullOrEmpty(password)       ||
                    string.IsNullOrEmpty(chargesText)    ||
                    !decimal.TryParse(chargesText, out charges) ||
                    string.IsNullOrEmpty(joinDateText)   ||
                    !DateTime.TryParse(joinDateText, out joinDate) ||
                    string.IsNullOrEmpty(gender))
                {
                    lblErrorMessage.Text    = "All fields are required. Please fill out all fields correctly.";
                    lblErrorMessage.Visible = true;
                    return;
                }

                joinDate = joinDate.Date;

                // ── Handle image upload ────────────────────────────────────────────
                string imagePath = null;
                if (fileUpload.HasFile)
                {
                    string fileExtension      = Path.GetExtension(fileUpload.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                    if (allowedExtensions.Contains(fileExtension))
                    {
                        string fileName = Guid.NewGuid() + fileExtension;
                        string savePath = Server.MapPath("~/assets/img/profiles/") + fileName;
                        fileUpload.SaveAs(savePath);
                        imagePath = "~/assets/img/profiles/" + fileName;
                    }
                    else
                    {
                        lblErrorMessage.Text    = "Invalid file format. Please upload an image (jpg, jpeg, png, gif).";
                        lblErrorMessage.Visible = true;
                        return;
                    }
                }

                // ── Insert Doctor + DoctorProfile ──────────────────────────────────
                string query = @"
                    INSERT INTO Doctors
                        (Name, DepartmentID, Specialization, Degree, Mobile, Email, Password,
                         Charges, JoinDate, Gender, ImageUrl)
                    VALUES
                        (@Name, @DepartmentID, @Specialization, @Degree, @Mobile, @Email, @Password,
                         @Charges, @JoinDate, @Gender, @ImageUrl);

                    DECLARE @DoctorID INT = SCOPE_IDENTITY();

                    INSERT INTO DoctorProfile
                        (DoctorID, Designation, Location, Year_of_Completion, Experience, Hospital, Institute)
                    VALUES
                        (@DoctorID, @Designation, @Location, @Year_of_Completion, @Experience, @Hospital, @Institute);";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name",             name);
                        cmd.Parameters.AddWithValue("@DepartmentID",     ddlDepartment.SelectedValue);
                        cmd.Parameters.AddWithValue("@Specialization",   specialization);
                        cmd.Parameters.AddWithValue("@Degree",           degree);
                        cmd.Parameters.AddWithValue("@Mobile",           mobile);
                        cmd.Parameters.AddWithValue("@Email",            email);
                        cmd.Parameters.AddWithValue("@Password",         hashedPassword);
                        cmd.Parameters.AddWithValue("@Charges",          charges);
                        cmd.Parameters.AddWithValue("@JoinDate",         joinDate);
                        cmd.Parameters.AddWithValue("@Gender",           gender);
                        cmd.Parameters.AddWithValue("@ImageUrl",         (object)imagePath ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Designation",      ddlDesignation.SelectedValue);
                        cmd.Parameters.AddWithValue("@Location",         txtLocation.Text);
                        cmd.Parameters.AddWithValue("@Year_of_Completion", txtYearOfCompletion.Text);
                        cmd.Parameters.AddWithValue("@Experience",       txtExperience.Text);
                        cmd.Parameters.AddWithValue("@Hospital",         txtHospital.Text);
                        cmd.Parameters.AddWithValue("@Institute",        txtInstitute.Text);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // ── Send beautiful welcome email to the new Doctor ─────────────────
                string loginUrl  = Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/Features/Accounts/Login.aspx");
                string emailBody = EmailService.BuildDoctorWelcomeEmail(
                                        name, email, password,
                                        specialization,
                                        ddlDepartment.SelectedItem.Text,
                                        loginUrl);

                EmailService emailSvc = new EmailService();
                emailSvc.SendEmail(email, "🏥 Welcome to DocCare Clinic – Your Login Credentials", emailBody);

                // ── Success ────────────────────────────────────────────────────────
                lblErrorMessage.Text      = "Doctor added successfully!";
                lblErrorMessage.ForeColor = System.Drawing.Color.Green;
                lblErrorMessage.Visible   = true;
                clear();

                string script = "setTimeout(function() { window.location.href = '" + ResolveUrl("~/Features/Doctors/Doctors.aspx") + "'; }, 2000);";
                ClientScript.RegisterStartupScript(this.GetType(), "Redirect", script, true);
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text    = "An error occurred: " + ex.Message;
                lblErrorMessage.Visible = true;
            }
        }

        public void clear()
        {
            txtCharges.Text              = "";
            txtDegree.Text               = "";
            txtEmail.Text                = "";
            txtJoinDate.Text             = "";
            txtMobile.Text               = "";
            txtPassword.Text             = "";
            ddlDepartment.SelectedValue  = "";
            ddlSpecialization.SelectedValue = "";
            txtName.Text                 = "";
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes     = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Features/Doctors/Doctors.aspx"));
        }
    }
}