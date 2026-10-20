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
                    LoadLast14Days();
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
                        COUNT(CASE WHEN Status <> 'Cancelled' THEN 1 END) AS TotalAppointments,
                        COUNT(CASE WHEN Status <> 'Cancelled' AND AppointDate = CAST(GETDATE() AS DATE) THEN 1 END) AS TodayAppointments
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
                                lblTodayAppointments.Text = reader["TodayAppointments"].ToString();
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
                    -- patients seen (a completed visit) out of all patients booked with this doctor,
                    -- and how many came to this doctor for the first time this month
                    SELECT
                        COUNT(DISTINCT CASE WHEN Status = 'Completed' THEN PatientID END) AS ActivePatients,
                        COUNT(DISTINCT PatientID) AS TotalPatients,
                        (SELECT COUNT(*) FROM (SELECT PatientID, MIN(AppointDate) AS FirstVisit FROM Appointments
                                               WHERE DoctorID = @DoctorID AND Status <> 'Cancelled' GROUP BY PatientID) f
                         WHERE f.FirstVisit >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AND f.FirstVisit <= CAST(GETDATE() AS DATE)) AS NewThisMonth
                    FROM Appointments
                    WHERE DoctorID = @DoctorID AND Status <> 'Cancelled'";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DoctorID", doctorId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int activePatients = Convert.ToInt32(reader["ActivePatients"]);
                                int totalPatients = Convert.ToInt32(reader["TotalPatients"]);

                                lblActivePatients.Text = activePatients.ToString();
                                lblTotalPatients.Text = "/" + totalPatients.ToString();
                                lblInactivePatients.Text = reader["NewThisMonth"].ToString();   // shown as "New this month"
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

        // Booked appointments per day for this doctor, last 14 days (zeros filled in)
        private void LoadLast14Days()
        {
            var perDay = new Dictionary<DateTime, int>();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT AppointDate, COUNT(*) AS N FROM Appointments
                WHERE DoctorID = @DoctorID AND Status <> 'Cancelled'
                  AND AppointDate > DATEADD(DAY, -14, CAST(GETDATE() AS DATE)) AND AppointDate <= CAST(GETDATE() AS DATE)
                GROUP BY AppointDate", con))
            {
                cmd.Parameters.AddWithValue("@DoctorID", Session["DoctorID"].ToString());
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) perDay[Convert.ToDateTime(r["AppointDate"]).Date] = Convert.ToInt32(r["N"]);
            }
            var labels = new List<string>();
            var values = new List<int>();
            for (DateTime d = DateTime.Today.AddDays(-13); d <= DateTime.Today; d = d.AddDays(1))
            {
                labels.Add(d.ToString("dd MMM", System.Globalization.CultureInfo.InvariantCulture));
                values.Add(perDay.ContainsKey(d) ? perDay[d] : 0);
            }
            var json = new System.Web.Script.Serialization.JavaScriptSerializer();
            ClientScript.RegisterStartupScript(GetType(), "doctor14",
                "window.addEventListener('load', function () { drawDoctor14Days(" + json.Serialize(labels) + ", " + json.Serialize(values) + "); });", true);
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
                    SELECT SUM(COALESCE(A.TotalFee, D.Charges)) AS TotalEarnings   -- the fee charged at the time
                    FROM Appointments A
                    INNER JOIN Doctors D ON A.DoctorID = D.DoctorID
                    WHERE A.Status = 'Completed' AND A.DoctorID = @DoctorID";

                    string todayEarningsQuery = @"
                    SELECT SUM(COALESCE(A.TotalFee, D.Charges)) AS TodayEarnings
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