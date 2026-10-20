using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace DocCare_Clinic
{
    public partial class ChangePasswordUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if PatientId is available in the query string
                string patientId = Request.QueryString["UserID"];

                if (!string.IsNullOrEmpty(patientId))
                {
                    // Store PatientId in the session if it's in the query string
                    Session["UserID"] = patientId;
                }
                else if (Session["UserID"] == null)
                {
                    // Redirect to login if PatientId is missing in both query string and session
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                    return;
                }
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            // Retrieve PatientId from the session
            string userId = Session["UserID"] as string;

            if (string.IsNullOrEmpty(userId))
            {
                Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                return;
            }

            string oldPassword = txtCurrentPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ShowMessage("All fields are required.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                ShowMessage("New Password and Confirm Password must match.");
                return;
            }

            // Validate the old password
            string validateOldPasswordResult = ValidateOldPassword(userId, oldPassword);
            if (!string.IsNullOrEmpty(validateOldPasswordResult))
            {
                // Hash the new password before storing
                string hashedNewPassword = HashPassword(newPassword);

                // Update the password
                string updatePasswordResult = UpdatePassword(userId, hashedNewPassword);
                if (updatePasswordResult == "Password updated successfully.")
                {
                    ShowMessage(updatePasswordResult);
                    Response.Redirect(ResolveUrl("~/Features/Accounts/UserProfile.aspx"));
                }
                else
                {
                    ShowMessage(updatePasswordResult); // Show error message from UpdatePassword
                }
            }
            else
            {
                ShowMessage("Old password is incorrect.");
            }
        }

        // Method to show messages to the user via SweetAlert2
        private void ShowMessage(string message)
        {
            string safeMsg = message.Replace("'", "\\'");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "swalMsg",
                "swalAlert('error', 'Password Change', '" + safeMsg + "');", true);
        }

        // Method to validate the old password
        private string ValidateOldPassword(string userId, string oldPassword)
        {
            string isValid = string.Empty;

            string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

            string query = "SELECT Password FROM Users WHERE UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);

                        // Retrieve the hashed password from the database
                        object result = cmd.ExecuteScalar();
                        string storedPassword = result as string;

                        // Hash the provided old password
                        string hashedOldPassword = HashPassword(oldPassword);

                        // Compare the hashed passwords
                        if (!string.IsNullOrEmpty(storedPassword) && storedPassword == hashedOldPassword)
                        {
                            isValid = storedPassword; // Password matches
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while validating the old password: " + ex.Message);
                }
            }

            return isValid;
        }

        // Function to hash the password using SHA-256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash); // Convert hash to Base64 for storage/validation
            }
        }


        // Method to update the password in the database
        private string UpdatePassword(string userId, string hashedNewPassword)
        {
            string result = string.Empty;

            string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "UPDATE Users SET Password = @Password WHERE UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@Password", hashedNewPassword); // Password is already hashed

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            result = "Password updated successfully.";
                        }
                        else
                        {
                            result = "No user found with the provided UserID.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = "An error occurred while updating the password: " + ex.Message;
                }
            }

            return result;
        }
    }
}