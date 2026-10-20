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
    public partial class Default : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["UserType"] == null || Session["UserType"].ToString() != "Admin")
            {
                // Store a flag in session that this redirect is from Admin attempt
                Session["RedirectFromAdmin"] = true;
                Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
            }
            if (!IsPostBack)
            {
                LoadFigures();
                LoadPatientVisitByGender();
            }
        }

        // Live dashboard figures. "This month" is compared with the same days of last month
        // (1st up to today's date), so a half-finished month is not compared with a whole one.
        private void LoadFigures()
        {
            string query = @"
                DECLARE @today DATE = CAST(GETDATE() AS DATE);
                DECLARE @m0 DATE = DATEFROMPARTS(YEAR(@today), MONTH(@today), 1);
                DECLARE @m1 DATE = DATEADD(MONTH, -1, @m0);
                DECLARE @m1End DATE = DATEADD(MONTH, -1, DATEADD(DAY, 1, @today));   -- same days last month (exclusive)
                SELECT
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate = @today AND Status <> 'Cancelled') AS TodayTotal,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate = @today AND Status = 'Completed') AS TodayCompleted,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate = @today AND Status IN ('Pending', 'Confirmed')) AS TodayUpcoming,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate > @today AND AppointDate <= DATEADD(DAY, 7, @today) AND Status IN ('Pending', 'Confirmed')) AS Next7,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate >= @m0 AND AppointDate <= @today AND Status <> 'Cancelled') AS MonthAppts,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate >= @m1 AND AppointDate < @m1End AND Status <> 'Cancelled') AS LastMonthAppts,
                    (SELECT ISNULL(SUM(COALESCE(a.TotalFee, d.Charges, 0)), 0) FROM Appointments a JOIN Doctors d ON d.DoctorID = a.DoctorID
                      WHERE a.Status = 'Completed' AND a.AppointDate >= @m0 AND a.AppointDate <= @today) AS MonthRevenue,
                    (SELECT ISNULL(SUM(COALESCE(a.TotalFee, d.Charges, 0)), 0) FROM Appointments a JOIN Doctors d ON d.DoctorID = a.DoctorID
                      WHERE a.Status = 'Completed' AND a.AppointDate >= @m1 AND a.AppointDate < @m1End) AS LastMonthRevenue,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate >= @m0 AND AppointDate <= @today AND Status = 'Cancelled') AS MonthCancelled,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate >= @m0 AND AppointDate <= @today AND Status = 'No Show') AS MonthNoShow,
                    (SELECT COUNT(*) FROM Appointments WHERE AppointDate >= @m0 AND AppointDate <= @today) AS MonthAll,
                    (SELECT COUNT(*) FROM Doctors WHERE IsDeleted = 0) AS DoctorsCount,
                    (SELECT COUNT(*) FROM Patients WHERE ISNULL(IsDeleted, 0) = 0) AS PatientsCount,
                    (SELECT COUNT(*) FROM Users WHERE CreatedAt >= @m0) AS NewAccounts;

                -- booked appointments per day, last 14 days (zeros filled in below)
                SELECT AppointDate, COUNT(*) AS N FROM Appointments
                WHERE AppointDate > DATEADD(DAY, -14, CAST(GETDATE() AS DATE)) AND AppointDate <= CAST(GETDATE() AS DATE) AND Status <> 'Cancelled'
                GROUP BY AppointDate;";

            var perDay = new Dictionary<DateTime, int>();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    r.Read();
                    int todayTotal = Convert.ToInt32(r["TodayTotal"]), monthAll = Convert.ToInt32(r["MonthAll"]);
                    int cancelled = Convert.ToInt32(r["MonthCancelled"]), noShow = Convert.ToInt32(r["MonthNoShow"]);
                    decimal revenue = Convert.ToDecimal(r["MonthRevenue"]);

                    litToday.Text = todayTotal.ToString();
                    litTodaySub.Text = r["TodayCompleted"] + " completed &middot; " + r["TodayUpcoming"] + " to come";
                    litNext7.Text = r["Next7"].ToString();
                    AppointmentsCount.Text = r["MonthAppts"].ToString();
                    litMonthChange.Text = Change(Convert.ToDecimal(r["MonthAppts"]), Convert.ToDecimal(r["LastMonthAppts"]));
                    litRevenue.Text = "&#8377;" + revenue.ToString("N0");
                    litRevenueChange.Text = Change(revenue, Convert.ToDecimal(r["LastMonthRevenue"]));
                    litCancelled.Text = cancelled + " &middot; " + noShow;
                    litCancelledSub.Text = monthAll == 0 ? "this month" : Math.Round((cancelled + noShow) * 100m / monthAll) + "% of this month&rsquo;s bookings";
                    NewPatientsCount.Text = r["DoctorsCount"] + " &middot; " + r["PatientsCount"];
                    litPeopleSub.Text = r["NewAccounts"] + " new patient " + (Convert.ToInt32(r["NewAccounts"]) == 1 ? "account" : "accounts") + " this month";

                    r.NextResult();
                    while (r.Read()) perDay[Convert.ToDateTime(r["AppointDate"]).Date] = Convert.ToInt32(r["N"]);
                }
            }

            var labels = new List<string>();
            var values = new List<int>();
            for (DateTime d = DateTime.Today.AddDays(-13); d <= DateTime.Today; d = d.AddDays(1))
            {
                labels.Add(d.ToString("dd MMM", System.Globalization.CultureInfo.InvariantCulture));
                values.Add(perDay.ContainsKey(d) ? perDay[d] : 0);
            }
            var json = new System.Web.Script.Serialization.JavaScriptSerializer();
            ClientScript.RegisterStartupScript(GetType(), "last14",
                "window.addEventListener('load', function () { drawLast14Days(" + json.Serialize(labels) + ", " + json.Serialize(values) + "); });", true);
        }

        // "▲ 40% vs same days last month" (green up / red down), from real numbers
        static string Change(decimal current, decimal previous)
        {
            if (previous == 0)
                return current == 0 ? "same as last month" : "<span class=\"passive-view\"><i class=\"feather-arrow-up-right me-1\"></i>new</span> vs last month";
            decimal pct = Math.Round((current - previous) * 100m / previous);
            string css = pct >= 0 ? "passive-view" : "negative-view";
            string arrow = pct >= 0 ? "feather-arrow-up-right" : "feather-arrow-down-right";
            return "<span class=\"" + css + "\"><i class=\"" + arrow + " me-1\"></i>" + Math.Abs(pct) + "%</span> vs same days last month";
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