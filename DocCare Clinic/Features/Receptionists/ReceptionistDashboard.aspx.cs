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
    public partial class ReceptionistDashboard : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAppointmentsCount();
                LoadNewPatientsCount();
                LoadPatientVisitByGender();
                //BindDepartmentData();
            }
        }

        private void LoadAppointmentsCount()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Appointments";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    AppointmentsCount.Text = count.ToString(); // Bind to Literal
                }
            }
        }

        private void LoadPatientVisitByGender()
        {
            int maleCount = 0;
            int femaleCount = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT Gender, COUNT(*) AS TotalCount
        FROM Patients
        GROUP BY Gender";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr["Gender"].ToString() == "Male")
                        {
                            maleCount = Convert.ToInt32(dr["TotalCount"]);
                        }
                        else if (dr["Gender"].ToString() == "Female")
                        {
                            femaleCount = Convert.ToInt32(dr["TotalCount"]);
                        }
                    }
                }
            }

            int totalPatients = maleCount + femaleCount;
            double malePercentage = (totalPatients == 0) ? 0 : (maleCount * 100.0) / totalPatients;
            double femalePercentage = (totalPatients == 0) ? 0 : (femaleCount * 100.0) / totalPatients;

            // ✅ Inject Chart Data Without Lag
            ClientScript.RegisterStartupScript(this.GetType(), "drawChart",
            "<script>" +
            "document.getElementById('malePercentage').innerText = '" + malePercentage.ToString("0.0") + "%';" +
            "document.getElementById('femalePercentage').innerText = '" + femalePercentage.ToString("0.0") + "%';" +
            "drawChart(" + malePercentage.ToString("0.0") + ", " + femalePercentage.ToString("0.0") + ");" +
            "</script>", false);
        }

        private void LoadNewPatientsCount()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Patients";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    NewPatientsCount.Text = count.ToString();
                }
            }
        }
    }
}