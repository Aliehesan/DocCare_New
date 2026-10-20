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
    public partial class Doctor_details : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadDoctorDetails();
        }

        private void LoadDoctorDetails()
        {
            string doctorId = Request.QueryString["DoctorID"];
            if (string.IsNullOrEmpty(doctorId))
            {
                Response.Redirect(ResolveUrl("~/Features/Doctors/Doctors-Team.aspx")); // Redirect if no ID is provided
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT d.Name, d.ImageUrl, d.Degree, d.Specialization, d.Charges, dp.Experience 
                     FROM Doctors d
                     INNER JOIN DoctorProfile dp ON d.DoctorId = dp.DoctorId
                     WHERE d.DoctorId = @DoctorId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblName.Text = reader["Name"].ToString();
                    lblAbout.Text = reader["Name"].ToString();
                    imgDoctor.ImageUrl = reader["ImageUrl"].ToString();
                    lblDegree.Text = reader["Degree"].ToString();
                    lblSpeclist.Text = reader["Specialization"].ToString();
                    // Rupees, Indian digit grouping (500, 1,500), no paise
                    lblCharges.Text = reader["Charges"] == DBNull.Value ? "0"
                        : Convert.ToDecimal(reader["Charges"]).ToString("N0", new System.Globalization.CultureInfo("en-IN"));
                    lblExperience.Text = reader["Experience"].ToString() + " Years";
                    // Set the value of HiddenField to the doctorId
                    hfDoctorId.Value = doctorId; // Ensure this line is present
                }
                else
                {
                    Response.Redirect(ResolveUrl("~/Features/Doctors/Doctors-Team.aspx")); // Redirect if doctor not found
                }
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            // Check if the session exists
            if (Session["UserID"] == null)
            {
                // Redirect to the login page if the session is not found
                Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
            }
            string doctorId = hfDoctorId.Value;  // Retrieve the doctorId from the HiddenField

            if (!string.IsNullOrEmpty(doctorId))
            {
                string redirectUrl = ResolveUrl("~/Features/Appointments/Confirm-Appointment.aspx") + "?doctorId=" + doctorId;
                Response.Redirect(redirectUrl);
            }
            else
            {
                Response.Write("Error: Doctor ID is missing.");
            }
        }
    }
}