using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    public partial class AppointmentReport : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            // Paged by default; only the Print request (btnPrint_Click) turns paging off for itself
            gvAppointmentReport.AllowPaging = true;

            if (!IsPostBack)
            {
                string userRole = Session["UserRole"] as string;
                if (string.IsNullOrEmpty(userRole))
                {
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }

                if (userRole == "Receptionist")
                {
                    HideAdminMenus();
                }
                LoadDoctors();
                LoadReceptionists();
                BindAppointmentReport();
            }
        }

        private void HideAdminMenus()
        {
            // Find menu controls inside MasterPage
            Control menuReceptionist = Master.FindControl("menuReceptionist");
            Control menuDepartment = Master.FindControl("menuDepartment");

            if (menuReceptionist != null) menuReceptionist.Visible = false;
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

        private void LoadDoctors()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT DoctorID, Name FROM Doctors ORDER BY Name";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlDoctorList.DataSource = dt;
                ddlDoctorList.DataTextField = "Name";
                ddlDoctorList.DataValueField = "DoctorID";
                ddlDoctorList.DataBind();
                ddlDoctorList.Items.Insert(0, new ListItem("All", ""));
            }
        }

        private void LoadReceptionists()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT ReceptionistID, Name FROM Receptionists ORDER BY Name", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlBookedBy.DataSource = dt;
                ddlBookedBy.DataTextField = "Name";
                ddlBookedBy.DataValueField = "ReceptionistID";
                ddlBookedBy.DataBind();
                ddlBookedBy.Items.Insert(0, new ListItem("All", ""));
            }
        }

        // One query for everything on the page (cards, doctor summary, table, export, print)
        private DataTable LoadReport()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Fee: what the appointment stored at booking. Older walk-ins have none, so they fall back
                // to the doctor's current fee and are flagged FeeEstimated.
                string query = @"
                    SELECT
                        a.ID AS AppointmentID,
                        a.AppointDate,
                        a.AppointTime,
                        a.TokenNo,
                        a.Status,
                        p.PatientName, p.PatientCode,
                        COALESCE(NULLIF(p.Mobile, ''), u.Mobile, 'N/A') AS PatientMobile,
                        COALESCE(u.Mobile, 'N/A') AS UserMobile,
                        COALESCE(u.Email, 'Not Provided') AS UserEmail,
                        COALESCE(u.Name, 'Walk-in Patient') AS UserName,
                        CASE WHEN a.UserID IS NULL THEN 'Walk-in' ELSE 'Online' END AS BookingType,
                        COALESCE(br.Name, ba.FullName, bd.Name, '') AS BookedByName,
                        CASE a.BookedByRole WHEN 'SuperAdmin' THEN 'Admin' ELSE COALESCE(a.BookedByRole, '') END AS BookedByRole,
                        COALESCE(d.Name, 'No Doctor Assigned') AS DoctorName,
                        COALESCE(d.Specialization, 'Not Available') AS Specialization,
                        COALESCE(a.TotalFee, d.Charges, 0) AS Fee,
                        CASE WHEN a.TotalFee IS NULL THEN 1 ELSE 0 END AS FeeEstimated,
                        ISNULL(a.PaidAmount, 0) AS PaidAmount,
                        CASE WHEN a.TotalFee IS NULL THEN COALESCE(d.Charges, 0) ELSE ISNULL(a.DueAmount, 0) END AS DueAmount,
                        a.DeletedAt
                    FROM Appointments a
                    INNER JOIN Patients p ON a.PatientID = p.PatientID
                    LEFT JOIN Users u ON a.UserID = u.UserID
                    LEFT JOIN Doctors d ON a.DoctorID = d.DoctorID
                    -- Staff member who booked a walk-in (see add_booked_by_columns.sql)
                    LEFT JOIN Receptionists br ON a.BookedByRole = 'Receptionist' AND br.ReceptionistID = a.BookedByID
                    LEFT JOIN Admin ba ON a.BookedByRole = 'SuperAdmin' AND ba.UserID = a.BookedByID
                    LEFT JOIN Doctors bd ON a.BookedByRole = 'Doctor' AND bd.DoctorID = a.BookedByID
                    WHERE
                        (@From IS NULL OR a.AppointDate >= @From)
                        AND (@To IS NULL OR a.AppointDate <= @To)
                        AND (@Status = '' OR a.Status = @Status)
                        AND (@DoctorID IS NULL OR a.DoctorID = @DoctorID)
                        AND (@Type = '' OR (@Type = 'Online' AND a.UserID IS NOT NULL) OR (@Type = 'Walk-in' AND a.UserID IS NULL))
                        AND (@BookedBy IS NULL OR (a.BookedByRole = 'Receptionist' AND a.BookedByID = @BookedBy))
                        AND (@Search = '' OR p.PatientName LIKE @Like OR p.PatientCode LIKE @Like
                             OR (@TypedPatientID IS NOT NULL AND p.PatientID = @TypedPatientID) OR a.TokenNo LIKE @Like OR p.Mobile LIKE @Like OR u.Mobile LIKE @Like)
                    ORDER BY a.AppointDate DESC, a.AppointTime DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    DateTime d;
                    cmd.Parameters.AddWithValue("@From", DateTime.TryParse(txtFromDate.Text, out d) ? (object)d.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@To", DateTime.TryParse(txtToDate.Text, out d) ? (object)d.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", ddlAppointmentStatus.SelectedValue);
                    int id;
                    cmd.Parameters.AddWithValue("@DoctorID", int.TryParse(ddlDoctorList.SelectedValue, out id) ? (object)id : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Type", ddlBookingType.SelectedValue);
                    cmd.Parameters.AddWithValue("@BookedBy", int.TryParse(ddlBookedBy.SelectedValue, out id) ? (object)id : DBNull.Value);
                    string search = txtSearch.Text.Trim();
                    cmd.Parameters.AddWithValue("@Search", search);
                    cmd.Parameters.AddWithValue("@Like", "%" + search + "%");
                    int? typedPatient = PatientCode.Parse(search);
                    cmd.Parameters.AddWithValue("@TypedPatientID", typedPatient.HasValue ? (object)typedPatient.Value : DBNull.Value);

                    DataTable dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    return dt;
                }
            }
        }

        private void BindAppointmentReport()
        {
            try
            {
                DataTable dt = LoadReport();
                gvAppointmentReport.DataSource = dt;
                gvAppointmentReport.DataBind();
                BindSummary(dt);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "reportError",
                    "swalAlert('error', 'Report Error', '" + HttpUtility.JavaScriptStringEncode(ex.Message) + "');", true);
            }
        }

        // Cards and per-doctor table, for exactly the rows the filters returned
        private void BindSummary(DataTable dt)
        {
            int total = dt.Rows.Count, completed = 0, upcoming = 0, cancelled = 0, noShow = 0, online = 0;
            decimal revenue = 0, advance = 0, advanceCancelled = 0;
            var doctors = new SortedDictionary<string, decimal[]>(); // total, completed, upcoming, cancelled, revenue, no show

            foreach (DataRow r in dt.Rows)
            {
                string status = r["Status"].ToString();
                decimal fee = Convert.ToDecimal(r["Fee"]), paid = Convert.ToDecimal(r["PaidAmount"]);
                string doctor = r["DoctorName"].ToString();
                if (!doctors.ContainsKey(doctor)) doctors[doctor] = new decimal[6];
                decimal[] s = doctors[doctor];
                s[0]++;

                if (r["BookingType"].ToString() == "Online") online++;
                advance += paid;

                if (status == AppointmentStatus.Completed) { completed++; revenue += fee; s[1]++; s[4] += fee; }
                else if (status == AppointmentStatus.Cancelled) { cancelled++; advanceCancelled += paid; s[3]++; }
                else if (status == AppointmentStatus.NoShow) { noShow++; s[5]++; }
                else { upcoming++; s[2]++; }
            }

            litTotal.Text = total.ToString();
            litTotalSub.Text = online + " online &middot; " + (total - online) + " walk-in";
            litCompleted.Text = completed.ToString();
            litUpcoming.Text = upcoming.ToString();
            litCancelled.Text = cancelled.ToString();
            litCancelRate.Text = total == 0 ? "&nbsp;" : Math.Round(cancelled * 100m / total, 1) + "% of appointments";
            litNoShow.Text = noShow.ToString();
            litRevenue.Text = Money(revenue);
            litAdvance.Text = Money(advance);
            litAdvanceSub.Text = advanceCancelled > 0 ? "incl. " + Money(advanceCancelled) + " on cancelled (refund due)" : "20% paid at booking";

            DataTable summary = new DataTable();
            summary.Columns.Add("Doctor"); summary.Columns.Add("Total"); summary.Columns.Add("Completed");
            summary.Columns.Add("Upcoming"); summary.Columns.Add("Cancelled"); summary.Columns.Add("Revenue"); summary.Columns.Add("NoShow");
            foreach (var kv in doctors)
                summary.Rows.Add(kv.Key, kv.Value[0], kv.Value[1], kv.Value[2], kv.Value[3], Money(kv.Value[4]), kv.Value[5]);
            if (doctors.Count > 1)
                summary.Rows.Add("All doctors", total, completed, upcoming, cancelled, Money(revenue), noShow);
            gvDoctorSummary.DataSource = summary;
            gvDoctorSummary.DataBind();

            RegisterCharts(dt, doctors);

            litPrintFilters.Text = HttpUtility.HtmlEncode(DescribeFilters()) + " &middot; printed " + DateTime.Now.ToString("dd MMM yyyy, hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
        }

        // Counts for the three charts, from the same filtered rows as the cards and tables
        private void RegisterCharts(DataTable dt, SortedDictionary<string, decimal[]> doctors)
        {
            var dates = dt.Rows.Cast<DataRow>().Select(r => Convert.ToDateTime(r["AppointDate"]).Date).ToList();

            // Per day (every day in the range, zeros included); more than two months -> per month
            DateTime from, to;
            DateTime f, t;
            from = DateTime.TryParse(txtFromDate.Text, out f) ? f.Date : (dates.Count > 0 ? dates.Min() : DateTime.Today);
            to = DateTime.TryParse(txtToDate.Text, out t) ? t.Date : (dates.Count > 0 ? dates.Max() : DateTime.Today);
            if (to < from) to = from;
            bool byMonth = (to - from).TotalDays > 62;
            var dayLabels = new List<string>();
            var dayValues = new List<int>();
            if (byMonth)
            {
                for (DateTime m = new DateTime(from.Year, from.Month, 1); m <= to; m = m.AddMonths(1))
                {
                    dayLabels.Add(m.ToString("MMM yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    dayValues.Add(dates.Count(d => d.Year == m.Year && d.Month == m.Month));
                }
            }
            else
            {
                for (DateTime d = from; d <= to; d = d.AddDays(1))
                {
                    dayLabels.Add(d.ToString("dd MMM", System.Globalization.CultureInfo.InvariantCulture));
                    dayValues.Add(dates.Count(x => x == d));
                }
            }

            string[] statusOrder = { AppointmentStatus.Pending, AppointmentStatus.Confirmed, AppointmentStatus.Completed, AppointmentStatus.Cancelled, AppointmentStatus.NoShow };
            var data = new
            {
                byMonth = byMonth,
                days = new { labels = dayLabels, values = dayValues },
                doctors = new { labels = doctors.Keys.Select(k => "Dr. " + k).ToList(), values = doctors.Values.Select(v => (int)v[0]).ToList() },
                statuses = new { labels = statusOrder, values = statusOrder.Select(s => dt.Rows.Cast<DataRow>().Count(r => r["Status"].ToString() == s)).ToList() }
            };
            string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data);
            ScriptManager.RegisterStartupScript(this, GetType(), "reportCharts",
                "if (window.renderReportCharts) renderReportCharts(" + json + ");", true);
        }

        private string DescribeFilters()
        {
            var parts = new List<string>();
            parts.Add(txtFromDate.Text == "" && txtToDate.Text == "" ? "All dates"
                : (txtFromDate.Text == "" ? "..." : DateTime.Parse(txtFromDate.Text).ToString("dd MMM yyyy")) + " to " +
                  (txtToDate.Text == "" ? "..." : DateTime.Parse(txtToDate.Text).ToString("dd MMM yyyy")));
            if (ddlAppointmentStatus.SelectedValue != "") parts.Add("Status: " + ddlAppointmentStatus.SelectedItem.Text);
            if (ddlDoctorList.SelectedValue != "") parts.Add("Doctor: " + ddlDoctorList.SelectedItem.Text);
            if (ddlBookingType.SelectedValue != "") parts.Add(ddlBookingType.SelectedItem.Text);
            if (ddlBookedBy.SelectedValue != "") parts.Add("Booked by: " + ddlBookedBy.SelectedItem.Text);
            if (txtSearch.Text.Trim() != "") parts.Add("Search: " + txtSearch.Text.Trim());
            return string.Join(" · ", parts);
        }

        protected string Money(object amount)
        {
            decimal value = amount == null || amount == DBNull.Value ? 0 : Convert.ToDecimal(amount);
            return "&#8377;" + value.ToString("N0");
        }

        protected string FormatTime(object time)
        {
            return time is TimeSpan ? SlotService.Format((TimeSpan)time) : "Not allotted";
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            gvAppointmentReport.PageIndex = 0;
            BindAppointmentReport();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtFromDate.Text = "";
            txtToDate.Text = "";
            ddlAppointmentStatus.SelectedIndex = 0;
            ddlDoctorList.SelectedIndex = 0;
            ddlBookingType.SelectedIndex = 0;
            ddlBookedBy.SelectedIndex = 0;
            txtSearch.Text = "";
            gvAppointmentReport.PageIndex = 0;
            BindAppointmentReport();
        }

        // Today / This Week (Monday to Sunday) / This Month / All Dates
        protected void QuickRange_Command(object sender, CommandEventArgs e)
        {
            DateTime today = DateTime.Today;
            DateTime? from = null, to = null;
            switch (e.CommandArgument.ToString())
            {
                case "today": from = to = today; break;
                case "week":
                    from = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
                    to = from.Value.AddDays(6);
                    break;
                case "month":
                    from = new DateTime(today.Year, today.Month, 1);
                    to = from.Value.AddMonths(1).AddDays(-1);
                    break;
            }
            txtFromDate.Text = from.HasValue ? from.Value.ToString("yyyy-MM-dd") : "";
            txtToDate.Text = to.HasValue ? to.Value.ToString("yyyy-MM-dd") : "";
            gvAppointmentReport.PageIndex = 0;
            BindAppointmentReport();
        }

        protected void gvAppointmentReport_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAppointmentReport.PageIndex = e.NewPageIndex;
            BindAppointmentReport();
        }

        // Excel opens CSV directly; UTF-8 with BOM keeps the ₹ sign and names readable
        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dt = LoadReport();
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("Appointment Report," + Csv(DescribeFilters()));
            csv.AppendLine();
            csv.AppendLine("ID,Date,Time,Token,Patient,Patient ID,Patient Mobile,Booking Type,Booked By / Account,Doctor,Specialization,Status,Fee (Rs),Paid Online (Rs),Due at Clinic (Rs),Fee Note");
            foreach (DataRow r in dt.Rows)
            {
                string bookedBy = r["BookingType"].ToString() == "Online"
                    ? r["UserName"].ToString()
                    : (r["BookedByName"].ToString() == "" ? "Front desk" : r["BookedByName"] + " (" + r["BookedByRole"] + ")");
                csv.AppendLine(string.Join(",", new[] {
                    r["AppointmentID"].ToString(),
                    Convert.ToDateTime(r["AppointDate"]).ToString("dd-MM-yyyy"),
                    Csv(FormatTime(r["AppointTime"])),
                    Csv(r["TokenNo"].ToString()),
                    Csv(r["PatientName"].ToString()),
                    Csv(r["PatientCode"].ToString()),
                    Csv(r["PatientMobile"].ToString()),
                    r["BookingType"].ToString(),
                    Csv(bookedBy),
                    Csv(r["DoctorName"].ToString()),
                    Csv(r["Specialization"].ToString()),
                    r["Status"].ToString(),
                    Convert.ToDecimal(r["Fee"]).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                    Convert.ToDecimal(r["PaidAmount"]).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                    Convert.ToDecimal(r["DueAmount"]).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                    Convert.ToInt32(r["FeeEstimated"]) == 1 ? "estimate (doctor's current fee)" : ""
                }));
            }

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.ContentEncoding = Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment; filename=AppointmentReport_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv");
            Response.BinaryWrite(Encoding.UTF8.GetPreamble());
            Response.Write(csv.ToString());
            Response.Flush();
            Response.SuppressContent = true;
            Context.ApplicationInstance.CompleteRequest();
        }

        static string Csv(string value)
        {
            if (value == null) return "";
            // quote fields with separators; neutralise values Excel would run as formulas
            if (value.Length > 0 && "=+-@".IndexOf(value[0]) >= 0) value = "'" + value;
            return value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0 ? "\"" + value.Replace("\"", "\"\"") + "\"" : value;
        }

        // Print / Save as PDF: every matching row on one page, then the browser's print dialog
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            gvAppointmentReport.AllowPaging = false;
            BindAppointmentReport();
            ScriptManager.RegisterStartupScript(this, GetType(), "print",
                "window.addEventListener('load', function () { setTimeout(function () { window.print(); }, 400); });", true);
        }
    }
}
