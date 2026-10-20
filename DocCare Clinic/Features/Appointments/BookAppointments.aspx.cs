using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace DocCare_Clinic
{
    public partial class BookAppointments : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if the doctorId exists in the query string
                if (Request.QueryString["doctorId"] != null)
                {
                    string doctorId = Request.QueryString["doctorId"];

                    // Load doctor details using the doctorId
                    LoadDoctorDetails(doctorId);

                    // Load available tokens for this doctor
                    LoadAvailableTokens(doctorId);
                }
                else
                {
                    // Handle case where doctorId is not available in the query string
                    Response.Redirect(ResolveUrl("~/Features/Doctors/Doctors-Team.aspx")); // Or show an error message
                }
            }
        }

        private void LoadDoctorDetails(string doctorId)
        {

            if (string.IsNullOrEmpty(doctorId))
            {
                Response.Redirect(ResolveUrl("~/Features/Doctors/Doctors-Team.aspx"));
                return;
            }
            // Convert doctorId to integer for SQL query
            int doctorIdInt = Convert.ToInt32(doctorId);  // Use the correct query string parameter name                
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Doctors WHERE DoctorID = @DoctorID", con);
                cmd.Parameters.AddWithValue("@DoctorID", doctorIdInt);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    imgDoctor.ImageUrl = reader["ImageUrl"].ToString();
                    lblDoctorName.Text = reader["Name"].ToString();
                    lblSpecialization.Text = reader["Specialization"].ToString();
                    lblDepartment.Text = reader["Department"].ToString();
                    // Set the value of the HiddenField
                    hfDoctorId.Value = doctorId;
                }
                else
                {
                    // Handle case if doctor details are not found
                    lblDoctorName.Text = "Doctor not found.";
                    lblSpecialization.Text = "";
                    lblDepartment.Text = "";
                    imgDoctor.ImageUrl = ""; // Set a default or error image if needed
                }
            }
        }

        private void LoadAvailableTokens(string doctorId)
        {
            // Convert doctorId to integer for SQL query
            int doctorIdInt = Convert.ToInt32(doctorId);  // Use the correct query string parameter name
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT TokenID, TokenName, IsAvailable FROM Tokens WHERE DoctorID = @DoctorID", con);
                cmd.Parameters.AddWithValue("@DoctorID", doctorIdInt);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Bind the tokens to a Repeater control or any other UI control
                rptTokens.DataSource = dt;
                rptTokens.DataBind();
            }
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            // Check if a token has been selected
            string selectedToken = hfSelectedToken.Value;
            Response.Write("Selected Token: " + selectedToken);
            if (!string.IsNullOrEmpty(selectedToken))
            {
                // Retrieve patientId dynamically from session
                int patientId = 0;

                // Ensure the PatientID exists in the session
                if (Session["PatientID"] != null)
                {
                    patientId = Convert.ToInt32(Session["PatientID"]);
                }
                else
                {
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                    return; // Exit if PatientID is not found in the session
                }

                // Retrieve the doctorId from the query string
                string doctorId = Request.QueryString["doctorId"];
                int doctorIdInt; // Declare doctorIdInt here

                // Ensure doctorId is valid and convert it to integer
                if (int.TryParse(doctorId, out doctorIdInt)) // Corrected: Pass doctorIdInt as an out parameter
                {
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connectionString))
                        {
                            con.Open();

                            // SQL query for inserting the appointment into the database
                            string query = "INSERT INTO Appointments (PatientID, DoctorID, TokenName, AppointDate, AppointTime) " +
                                           "VALUES (@PatientID, @DoctorID, @TokenName, GETDATE(), '09:00:00')";

                            SqlCommand cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@PatientID", patientId);
                            cmd.Parameters.AddWithValue("@DoctorID", doctorIdInt);
                            cmd.Parameters.AddWithValue("@TokenName", selectedToken);

                            cmd.ExecuteNonQuery();

                            // Update the IsAvailable field in the Tokens table to false
                            string updateTokenQuery = "UPDATE Tokens SET IsAvailable = 0 WHERE DoctorID = @DoctorID AND TokenName = @TokenName";
                            SqlCommand updateCmd = new SqlCommand(updateTokenQuery, con);
                            updateCmd.Parameters.AddWithValue("@DoctorID", doctorIdInt);
                            updateCmd.Parameters.AddWithValue("@TokenName", selectedToken);
                            updateCmd.ExecuteNonQuery();
                        }

                        // Redirect to confirmation page after successful appointment booking
                        Response.Redirect(ResolveUrl("~/Features/Appointments/Confirm-Appointment.aspx"));
                        LoadAvailableTokens(doctorId);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception (you could display a message, log to a file, etc.)
                        Response.Write("An error occurred while booking the appointment: {ex.Message}");
                    }
                }
                else
                {
                    // Handle the case where the doctorId is invalid
                    Response.Write("Invalid Doctor ID.");
                }
            }
            else
            {
                // If no token is selected, show an error message
                Response.Write("Please select a token before continuing.");
            }
        }

        protected void rptTokens_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectToken")
            {
                // Retrieve the TokenName from the selected item
                string tokenName = e.CommandArgument.ToString();

                // Store the selected token in the session
                Session["SelectedToken"] = tokenName;

                // You can now bind the Repeater again to update the visual state
                string doctorId = Request.QueryString["doctorId"];
                LoadAvailableTokens(doctorId); // Rebind to highlight the selected token
            }
        }
    }
}