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
    public partial class DoctorDashBoard : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["UserRole"] == null)
            {
                Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
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
                    LoadAppointmentData();
                    LoadPatientData();
                    LoadEarnings();
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

        private void LoadAppointmentData()
        {
            string doctorId = Session["DoctorID"].ToString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // Query to count completed appointments and total appointments
                    string query = @"
                    SELECT 
                        COUNT(CASE WHEN Status = 'Completed' THEN 1 END) AS CompletedAppointments,
                        COUNT(*) AS TotalAppointments
                    FROM Appointments
                    WHERE DoctorID = @DoctorID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int completedAppointments = Convert.ToInt32(reader["CompletedAppointments"]);
                                int totalAppointments = Convert.ToInt32(reader["TotalAppointments"]);

                                lblAppointments.Text = completedAppointments.ToString();
                                lblTotalAppointments.Text = "/" + totalAppointments.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblAppointments.Text = "Error";
                    lblTotalAppointments.Text = "/Error";
                    // Log error if needed
                }
            }
        }

        private void LoadPatientData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    int doctorId = Convert.ToInt32(Session["DoctorID"]);

                    string query = @"
                    SELECT 
                        COUNT(CASE WHEN Status = 'Active' THEN 1 END) AS ActivePatients,
                        COUNT(*) AS TotalPatients
                    FROM Patients
                    WHERE DoctorID = @DoctorID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DoctorID", doctorId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int activePatients = Convert.ToInt32(reader["ActivePatients"]);
                                int totalPatients = Convert.ToInt32(reader["TotalPatients"]);
                                int inactivePatients = totalPatients - activePatients;

                                lblActivePatients.Text = activePatients.ToString();
                                lblTotalPatients.Text = "/" + totalPatients.ToString();
                                lblInactivePatients.Text = inactivePatients.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblActivePatients.Text = "Error";
                    lblTotalPatients.Text = "/Error";
                    lblInactivePatients.Text = "Error";
                }
            }
        }

        private void LoadEarnings()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // Replace with actual Doctor ID (e.g., from session)
                    int doctorId = Convert.ToInt32(Session["DoctorID"]);

                    // ✅ Define the queries **before using them**
                    string totalEarningsQuery = @"
                    SELECT SUM(D.Charges) AS TotalEarnings
                    FROM Appointments A
                    INNER JOIN Doctors D ON A.DoctorID = D.DoctorID
                    WHERE A.Status = 'Completed' AND A.DoctorID = @DoctorID";

                    string todayEarningsQuery = @"
                    SELECT SUM(D.Charges) AS TodayEarnings
                    FROM Appointments A
                    INNER JOIN Doctors D ON A.DoctorID = D.DoctorID
                    WHERE A.Status = 'Completed' AND A.DoctorID = @DoctorID 
                    AND CAST(A.AppointDate AS DATE) = CAST(GETDATE() AS DATE)";

                    // ✅ Use the queries inside the same method
                    using (SqlCommand totalCmd = new SqlCommand(totalEarningsQuery, con))
                    using (SqlCommand todayCmd = new SqlCommand(todayEarningsQuery, con))
                    {
                        totalCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                        todayCmd.Parameters.AddWithValue("@DoctorID", doctorId);

                        object totalResult = totalCmd.ExecuteScalar();
                        object todayResult = todayCmd.ExecuteScalar();

                        decimal totalEarnings = (totalResult != DBNull.Value && totalResult != null) ? Convert.ToDecimal(totalResult) : 0;
                        decimal todayEarnings = (todayResult != DBNull.Value && todayResult != null) ? Convert.ToDecimal(todayResult) : 0;

                        lblEarnings.Text = totalEarnings.ToString("N0", new System.Globalization.CultureInfo("en-IN"));
                        lblTodayEarnings.Text = " &#8377;" + todayEarnings.ToString("N0", new System.Globalization.CultureInfo("en-IN"));
                    }
                }
                catch (Exception ex)
                {
                    lblEarnings.Text = "Error";
                    lblTodayEarnings.Text = "Error";
                }
            }
        }
    }
}