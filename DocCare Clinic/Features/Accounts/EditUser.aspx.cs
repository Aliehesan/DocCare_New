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
    public partial class EditUser : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    int UserID = Convert.ToInt32(Session["UserID"]);
                    LoadUserDetails(UserID);
                }
                else
                {
                    // Redirect to login or error page if session is missing
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }
            }
        }

        private void LoadUserDetails(int UserID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Name, Email, Mobile, Address FROM Users WHERE UserId = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", UserID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Populate textboxes with the fetched data
                    txtName.Text = reader["Name"].ToString();
                    txtAddress.Text = reader["Address"].ToString();
                    txtMobile.Text = reader["Mobile"].ToString();
                    txtEmail.Text = reader["Email"].ToString();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string address = txtAddress.Text;
            string mobile = txtMobile.Text;
            string email = txtEmail.Text;

            // Logic to save data to the database
            string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Users SET Name = @Name, Mobile = @Mobile, Email = @Email, Address = @Address WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);                
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@Mobile", mobile);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            Session["Name"] = txtName.Text;
            Session["SuccessMessage"] = "Profile details updated successfully!";

            // Provide feedback or redirect
            Response.Redirect(ResolveUrl("~/Features/Accounts/UserProfile.aspx"));
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Features/Accounts/UserProfile.aspx"));
        }
    }
}