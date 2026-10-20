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
                LoadFigures();
                LoadPatientVisitByGender();
                //BindDepartmentData();
            }
        }

        // What the front desk needs today: today's and the coming week's bookings, follow-ups to call, refunds to give
        private void LoadFigures()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                DECLARE @today DATE = CAST(GETDATE() AS DATE);
                SELECT
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate = @today AND Status <> 'Cancelled') AS TodayTotal,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate = @today AND Status = 'Completed') AS TodayCompleted,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate = @today AND Status IN ('Pending', 'Confirmed')) AS TodayUpcoming,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate = @today AND Status IN ('Pending', 'Confirmed') AND UserID IS NULL) AS TodayWalkIn,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate > @today AND AppointDate <= DATEADD(DAY, 7, @today) AND Status IN ('Pending', 'Confirmed')) AS Next7,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate > @today AND AppointDate <= DATEADD(DAY, 7, @today) AND Status = 'Pending') AS Next7Pending", con))
            {
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    r.Read();
                    litToday.Text = r["TodayTotal"].ToString();
                    litTodaySub.Text = r["TodayCompleted"] + " completed &middot; " + r["TodayUpcoming"] + " to come";
                    litNext7.Text = r["Next7"].ToString();
                    int pending = Convert.ToInt32(r["Next7Pending"]);
                    litNext7Sub.Text = pending == 0 ? "upcoming bookings" : pending + " not confirmed yet";
                }
            }

            System.Data.DataTable followUps = DocCare_Clinic.Config.MedicalRecords.OpenFollowUps(dueBy: DateTime.Today.AddDays(7));
            int overdue = followUps.Rows.Cast<System.Data.DataRow>().Count(x => Convert.ToDateTime(x["FollowUpDate"]).Date < DateTime.Today);
            litFollowUps.Text = followUps.Rows.Count.ToString();
            litFollowUpsSub.Text = overdue > 0 ? "<span class=\"dash-warn\">" + overdue + " overdue</span> &middot; patients to call" : "patients to call or book";

            int refunds;
            decimal refundTotal;
            DocCare_Clinic.Config.Refunds.Due(out refunds, out refundTotal);
            litRefunds.Text = refunds.ToString();
            litRefundsSub.Text = refunds == 0 ? "nothing to give back" : "<span class=\"dash-warn\">&#8377;" + refundTotal.ToString("N0") + "</span> to give back";
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

    }
}