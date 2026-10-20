using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using TaskManagement.config;
using System.Security.Cryptography;
using System.Text;

namespace DocCare_Clinic
{
    public partial class AddReceptionist : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblErrorMessage.Visible = false;
            }
        }

        private bool AddNewReceptionist(string name, string mobile, string email, string password)
        {
            try
            {
                string hashedPassword = HashPassword(password);
                string query = "INSERT INTO Receptionists (Name, Mobile, Email, Password) VALUES (@Name, @Mobile, @Email, @Password)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Mobile", mobile);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword); // Store hashed password

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Send beautiful welcome email to the new Receptionist
                        string loginUrl  = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.ToAbsolute("~/Features/Accounts/Login.aspx");
                        string emailBody = EmailService.BuildReceptionistWelcomeEmail(name, email, password, loginUrl);

                        EmailService emailSvc = new EmailService();
                        emailSvc.SendEmail(email, "🏥 Welcome to DocCare Clinic – Your Login Credentials", emailBody);

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "An error occurred: " + ex.Message;
                lblErrorMessage.Visible = true;
                return false;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string mobile = txtMobile.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Validate inputs
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblErrorMessage.Text = "*All fields are required.";
                lblErrorMessage.Visible = true;

                // Add the script to hide the error message after 5 seconds
                string hideErrorMessageScript = @"
            setTimeout(function() {
                var errorLabel = document.getElementById('" + lblErrorMessage.ClientID + @"');
                if (errorLabel) {
                    errorLabel.style.display = 'none';
                }
            }, 3000);";
                ClientScript.RegisterStartupScript(this.GetType(), "HideErrorMessage", hideErrorMessageScript, true);

                return;
            }

            // Insert data into the database
            bool isAdded = AddNewReceptionist(name, mobile, email, password);

            if (isAdded)
            {
                // Redirect back to the receptionist list page
                Response.Redirect(ResolveUrl("~/Features/Receptionists/Receptionist.aspx"));
            }
            else
            {
                lblErrorMessage.Text = "*An error occurred while adding the receptionist. Please try again.";
                lblErrorMessage.Visible = true;

                // Add the script to hide the error message after 5 seconds
                string hideErrorMessageScript = @"
            setTimeout(function() {
                var errorLabel = document.getElementById('" + lblErrorMessage.ClientID + @"');
                if (errorLabel) {
                    errorLabel.style.display = 'none';
                }
            }, 3000);";
                ClientScript.RegisterStartupScript(this.GetType(), "HideErrorMessage", hideErrorMessageScript, true);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Features/Receptionists/Receptionist.aspx"));
        }
    }
}