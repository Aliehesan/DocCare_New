using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;

namespace DocCare_Clinic
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                return;
            }
            if (!IsPostBack)
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

        // This method handles the password change when the user clicks the "Submit" button
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string userRole = Session["UserRole"] as string;
            string userId = GetUserIdFromSession(userRole); 

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                return;
            }

            string oldPassword = txtOldPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                lblErrorMessage.Text = "All fields are required.";
                return;
            }

            if (newPassword != confirmPassword)
            {
                lblErrorMessage.Text = "New Password and Confirm Password must match.";
                return;
            }

            // Validate the old password
            if (ValidateOldPassword(userId, oldPassword, userRole))
            {
                string hashedNewPassword = HashPassword(newPassword);

                // Update password in respective table
                string updateResult = UpdatePassword(userId, hashedNewPassword, userRole);
                lblErrorMessage.Text = updateResult;

                if (updateResult == "Password updated successfully.")
                {
                    if (userRole == "SuperAdmin")
                    {
                        Response.Redirect(ResolveUrl("~/Features/Admin/AdminDashBoard.aspx"));

                    }
                    else if (userRole == "Doctor")
                    {
                        Response.Redirect(ResolveUrl("~/Features/Doctors/DoctorDashBoard.aspx"));
                    }
                    else
                    {
                        Response.Redirect(ResolveUrl("~/Features/Receptionists/ReceptionistDashboard.aspx"));                        
                    }
                }
            }
            else
            {
                lblErrorMessage.Text = "Old password is incorrect.";
            }
        }

        // Method to validate the old password
       private bool ValidateOldPassword(string userId, string oldPassword, string userRole)
        {
            string tableName = GetTableName(userRole);
            string idColumn = GetIdColumn(userRole);

            string query = "SELECT Password FROM " + tableName + " WHERE " + idColumn + " = @UserID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        object result = cmd.ExecuteScalar();
                        string storedPassword = result as string;

                        return !string.IsNullOrEmpty(storedPassword) && storedPassword == HashPassword(oldPassword);
                    }
                }
                catch (Exception)
                {
                    lblErrorMessage.Text = "*Error validating old password";
                    return false;
                }
            }
        }

        private string UpdatePassword(string userId, string hashedNewPassword, string userRole)
        {
            string tableName = GetTableName(userRole);
            string idColumn = GetIdColumn(userRole);

            string query = "UPDATE " + tableName + " SET Password = @Password WHERE " + idColumn + " = @UserID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@Password", hashedNewPassword);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0 ? "Password updated successfully." : "Error updating password.";
                    }
                }
                catch (Exception ex)
                {
                    return "An error occurred: " + ex.Message;
                }
            }
        }


        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private string GetTableName(string userRole)
        {
            switch (userRole)
            {
                case "Doctor":
                    return "Doctors";
                case "Receptionist":
                    return "Receptionists";
                default:
                    return "Admin";
            }
        }

        private string GetIdColumn(string userRole)
        {
            switch (userRole)
            {
                case "Doctor":
                    return "DoctorID";
                case "Receptionist":
                    return "ReceptionistID";
                default:
                    return "UserID";
            }
        }

        private string GetUserIdFromSession(string userRole)
        {
            if (userRole == "Doctor" && Session["DoctorId"] != null)
            {
                return Session["DoctorId"].ToString();
            }
            else if (userRole == "Receptionist" && Session["ReceptionistId"] != null)
            {
                return Session["ReceptionistId"].ToString();
            }
            else if (Session["UserID"] != null)
            {
                return Session["UserID"].ToString();
            }

            return null;
        }
    }
}