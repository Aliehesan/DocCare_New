using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

namespace DocCare_Clinic
{
    public partial class EditReceptionist : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Get the ReceptionistID from the query string
                string idString = Request.QueryString["Id"];

                // Declare the variable for the ID
                int id;

                // Check if the "Id" parameter is present and is a valid integer
                if (int.TryParse(idString, out id))
                {
                    // Fetch receptionist data and bind it to the text fields
                    var receptionist = GetReceptionistById(id);
                    if (receptionist != null)
                    {
                        txtName.Text = receptionist.Name;
                        txtMobile.Text = receptionist.Mobile;
                        txtEmail.Text = receptionist.Email;

                        // Store the ID in ViewState for updating later
                        ViewState["ReceptionistID"] = id;
                    }
                    else
                    {
                        // Handle the case where the receptionist is not found
                        // Optionally, you can show a message that the record was not found
                        Response.Redirect(ResolveUrl("~/Features/Receptionists/Receptionist.aspx"));  // Redirect back if receptionist is not found
                    }
                }
                else
                {
                    // Handle the case where the "Id" query parameter is missing or invalid
                    // Optionally, show an error message
                    Response.Redirect(ResolveUrl("~/Features/Receptionists/Receptionist.aspx"));  // Redirect back to the list page or show an error
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ViewState["ReceptionistID"] != null)
            {
                // Retrieve the ID from ViewState
                int id = Convert.ToInt32(ViewState["ReceptionistID"]);

                // Collect updated data from the text fields
                string name = txtName.Text.Trim();
                string mobile = txtMobile.Text.Trim();
                string email = txtEmail.Text.Trim();

                // Update the database
                bool isUpdated = UpdateReceptionist(id, name, mobile, email);

                if (isUpdated)
                {
                    // Redirect back to Receptionist.aspx with a success message
                    Response.Redirect(ResolveUrl("~/Features/Receptionists/Receptionist.aspx"));
                }
                else
                {
                    // Show an error message
                    ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Failed to update receptionist.');", true);
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Features/Receptionists/Receptionist.aspx"));
        }

        private Receptionist GetReceptionistById(int id)
        {
            // Receptionist object to store the data
            Receptionist receptionist = null;

            // SQL query to fetch the receptionist by ID
            string query = "SELECT Name, Mobile, Email FROM Receptionists WHERE ReceptionistID = @ReceptionistID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ReceptionistID", id);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Populate the receptionist object
                                receptionist = new Receptionist
                                {
                                    Name = reader["Name"].ToString(),
                                    Mobile = reader["Mobile"].ToString(),
                                    Email = reader["Email"].ToString()
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions (e.g., log the error)
                        throw new Exception("An error occurred while fetching receptionist data.", ex);
                    }
                }
            }

            return receptionist;
        }

        private bool UpdateReceptionist(int id, string name, string mobile, string email)
        {
            string query = "UPDATE Receptionists SET Name = @Name, Mobile = @Mobile, Email = @Email WHERE ReceptionistID = @ReceptionistID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Adding parameters to prevent SQL injection
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Mobile", mobile);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@ReceptionistID", id);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();  // Executes the update query

                        // If rows were affected, return true (update successful)
                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                        else
                        {
                            // If no rows were updated (maybe invalid ID), return false
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions (e.g., log the error)
                        throw new Exception("An error occurred while updating the receptionist data.", ex);
                    }
                }
            }
        }
    }
}