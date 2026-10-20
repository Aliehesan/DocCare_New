using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    public partial class MyAppointment : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

        const int PageSize = 5;

        // Upcoming = Pending/Confirmed from today on; Past = finished, or an active one whose day has gone; Cancelled
        string Tab
        {
            get { return ViewState["Tab"] as string; }
            set { ViewState["Tab"] = value; }
        }

        int PageIndex
        {
            get { return ViewState["Page"] == null ? 0 : (int)ViewState["Page"]; }
            set { ViewState["Page"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int UserId;
                if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out UserId))
                {
                    BindAppointments(UserId); // Call the method with a valid patientId
                }
                else
                {
                    // Redirect to an error page or login page if the session is invalid
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }
            }
        }

        int CurrentUserId()
        {
            int id;
            return Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out id) ? id : 0;
        }

        protected void Tab_Command(object sender, CommandEventArgs e)
        {
            Tab = e.CommandArgument.ToString();
            PageIndex = 0;
            BindAppointments(CurrentUserId());
        }

        protected void Page_Command(object sender, CommandEventArgs e)
        {
            string arg = e.CommandArgument.ToString();
            PageIndex = arg == "prev" ? PageIndex - 1 : arg == "next" ? PageIndex + 1 : int.Parse(arg);
            BindAppointments(CurrentUserId());
            // Back to the top of the list after changing page
            ScriptManager.RegisterStartupScript(this, GetType(), "toList",
                "var l = document.querySelector('.appt-tabs'); if (l) l.scrollIntoView({ behavior: 'smooth', block: 'start' });", true);
        }

        static string TabOf(DataRow r)
        {
            string status = r["Status"].ToString();
            if (status == AppointmentStatus.Cancelled) return "cancelled";
            if (!AppointmentStatus.IsFinal(status) && Convert.ToDateTime(r["AppointDate"]).Date >= DateTime.Today) return "upcoming";
            return "past";
        }

        // ── Follow-ups the doctor advised, not booked yet ──

        void BindFollowUps(int userID)
        {
            DataTable dt = MedicalRecords.OpenFollowUps(userId: userID);
            rptFollowUps.DataSource = dt;
            rptFollowUps.DataBind();
            rptFollowUps.Visible = dt.Rows.Count > 0;
        }

        protected string FollowUpWhen(object item)
        {
            DateTime d = Convert.ToDateTime(((DataRowView)item)["FollowUpDate"]).Date;
            if (d < DateTime.Today) return "<span class=\"followup-overdue\">was due on " + SlotService.FormatDate(d) + "</span>";
            return "around " + SlotService.FormatDate(d) + (d == DateTime.Today ? " (today)" : "");
        }

        // Book now (with doctor, date and patient chosen), or when booking opens for a far-away date
        protected string FollowUpAction(object item)
        {
            DataRowView r = (DataRowView)item;
            DateTime d = Convert.ToDateTime(r["FollowUpDate"]).Date;
            DateTime opens = d.AddDays(-SlotService.BookingWindowDays);
            if (opens > DateTime.Today)
                return "<span class=\"followup-later\">Booking opens on " + SlotService.FormatDate(opens) + "</span>";
            string url = ResolveUrl(MedicalRecords.FollowUpBookingPath(Convert.ToInt32(r["DoctorID"]), d, Convert.ToInt32(r["PatientID"])));
            return "<a class=\"followup-book\" href=\"" + HttpUtility.HtmlAttributeEncode(url) + "\"><i class=\"fa-regular fa-calendar-plus\"></i>&nbsp; Book follow-up</a>";
        }

        private void BindAppointments(int userID)
        {
            BindFollowUps(userID);
            string query = @"
                SELECT 
                    A.ID AS AppointmentID, 
                    D.Name AS DoctorName, 
                    D.Specialization, 
                    D.ImageUrl AS DoctorImage,  
                    DP.Location AS Address, 
                    A.AppointDate,                   
                    CASE
                        WHEN A.AppointTime IS NOT NULL
                        THEN RIGHT(CONVERT(VARCHAR(20), CAST(A.AppointTime AS DATETIME), 100), 7)
                        ELSE 'Not Allocated'
                    END AS AppointTime,
                    A.AppointTime AS AppointTimeRaw,
                    A.Status,
                    ISNULL(A.TokenNo, '') AS TokenNo,
                    ISNULL(A.PaidAmount, 0) AS PaidAmount,
                    ISNULL(A.DueAmount, 0) AS DueAmount,
                    ISNULL(A.TotalFee, 0) AS TotalFee,
                    ISNULL(A.PaymentStatus, '') AS PaymentStatus,
                    ISNULL(A.PaymentId, '') AS PaymentId,
                    COALESCE(P.PatientName, U.Name, 'Patient') AS PatientName, ISNULL(P.PatientCode, '') AS PatientCode,
                    ISNULL(P.Age, 0) AS PatientAge,
                    ISNULL(P.Gender, '') AS PatientGender
                FROM Appointments A
                JOIN Doctors D ON A.DoctorID = D.DoctorID
                LEFT JOIN DoctorProfile DP ON D.DoctorID = DP.DoctorID
                LEFT JOIN Patients P ON A.PatientID = P.PatientID
                LEFT JOIN Users U ON A.UserID = U.UserID
                WHERE A.UserID = @UserID
                ORDER BY A.AppointDate DESC, A.ID DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserID", userID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Nothing booked at all: the first-booking card instead of tabs
                bool any = dt.Rows.Count > 0;
                pnlNoAppointments.Visible = !any;
                pnlList.Visible = any;
                if (!any) return;

                var rows = dt.Rows.Cast<DataRow>().ToList();
                var counts = new Dictionary<string, int> { { "upcoming", 0 }, { "past", 0 }, { "cancelled", 0 } };
                foreach (DataRow r in rows) counts[TabOf(r)]++;
                if (Tab == null) Tab = counts["upcoming"] > 0 ? "upcoming" : "past";   // open on what is coming next

                // Upcoming: soonest first; past and cancelled: newest first
                Func<DataRow, DateTime> start = r => Convert.ToDateTime(r["AppointDate"]).Date +
                    (r["AppointTimeRaw"] == DBNull.Value ? TimeSpan.Zero : (TimeSpan)r["AppointTimeRaw"]);
                var inTab = rows.Where(r => TabOf(r) == Tab);
                inTab = Tab == "upcoming" ? inTab.OrderBy(start) : inTab.OrderByDescending(start);
                List<DataRow> list = inTab.ToList();

                int pages = Math.Max(1, (int)Math.Ceiling(list.Count / (double)PageSize));
                PageIndex = Math.Min(Math.Max(PageIndex, 0), pages - 1);
                DataTable page = dt.Clone();
                foreach (DataRow r in list.Skip(PageIndex * PageSize).Take(PageSize)) page.ImportRow(r);
                rptAppointments.DataSource = page;
                rptAppointments.DataBind();

                SetTab(tabUpcoming, "upcoming", "Upcoming", counts["upcoming"]);
                SetTab(tabPast, "past", "Past", counts["past"]);
                SetTab(tabCancelled, "cancelled", "Cancelled", counts["cancelled"]);

                // Empty tab
                pnlTabEmpty.Visible = list.Count == 0;
                litTabEmpty.Text = Tab == "upcoming" ? "No upcoming appointments." : Tab == "past" ? "No past appointments yet." : "No cancelled appointments.";
                lnkTabEmptyBook.Visible = Tab == "upcoming";

                // Pager (only when there is more than one page)
                pnlPager.Visible = pages > 1;
                litPagerInfo.Text = list.Count == 0 ? "" : "Showing " + (PageIndex * PageSize + 1) + "&ndash;" +
                                    Math.Min((PageIndex + 1) * PageSize, list.Count) + " of " + list.Count;
                lnkPrev.Enabled = PageIndex > 0;
                lnkNext.Enabled = PageIndex < pages - 1;
                lnkPrev.CssClass = "pager-btn" + (lnkPrev.Enabled ? "" : " disabled");
                lnkNext.CssClass = "pager-btn" + (lnkNext.Enabled ? "" : " disabled");
                rptPages.DataSource = Enumerable.Range(0, pages).Select(i => new { Index = i, Label = (i + 1).ToString(), Css = "pager-btn" + (i == PageIndex ? " active" : "") });
                rptPages.DataBind();
            }
        }

        void SetTab(LinkButton tab, string key, string label, int count)
        {
            tab.Text = label + " <span class=\"tab-count\">" + count + "</span>";
            tab.CssClass = "appt-tab" + (Tab == key ? " active" : "");
        }

        // Fee line: what is still due only matters for an appointment that is still coming
        protected string FeeLine(object item)
        {
            DataRowView r = (DataRowView)item;
            decimal total = Convert.ToDecimal(r["TotalFee"]), paid = Convert.ToDecimal(r["PaidAmount"]), due = Convert.ToDecimal(r["DueAmount"]);
            if (total <= 0) return "";
            string status = r["Status"].ToString();
            string html = "<strong>Fee: &#8377;" + total.ToString("N0") + "</strong>";
            if (paid > 0)
                html += " &bull; <span class='fee-paid'><i class='fa-solid fa-circle-check'></i> " +
                        (status == AppointmentStatus.Cancelled ? "Advance paid" : "Paid online") + ": &#8377;" + paid.ToString("N0") + "</span>";
            if (!AppointmentStatus.IsFinal(status) && due > 0)
                html += " &bull; <span class='fee-due'><i class='fa-solid fa-clock'></i> Due at clinic: &#8377;" + due.ToString("N0") + "</span>";
            return "<div class='appt-meta-item fee-line'><i class='fa-solid fa-receipt'></i><span>" + html + "</span></div>";
        }

        protected bool IsActive(object status)
        {
            return !AppointmentStatus.IsFinal(Convert.ToString(status));
        }

        // ── Card helpers (status badge, Cancel / Reschedule buttons) ──

        protected string StatusClass(object status)
        {
            switch (Convert.ToString(status))
            {
                case AppointmentStatus.Confirmed: return "status-badge-confirmed";
                case AppointmentStatus.Completed: return "status-badge-completed";
                case AppointmentStatus.Cancelled: return "status-badge-cancelled";
                case AppointmentStatus.NoShow: return "status-badge-noshow";
                default: return "status-badge-pending";
            }
        }

        protected string StatusIcon(object status)
        {
            switch (Convert.ToString(status))
            {
                case AppointmentStatus.Confirmed: return "fa-check";
                case AppointmentStatus.Completed: return "fa-circle-check";
                case AppointmentStatus.Cancelled: return "fa-xmark";
                case AppointmentStatus.NoShow: return "fa-user-xmark";
                default: return "fa-clock";
            }
        }

        // Pending/Confirmed and more than PatientCutoffHours away: the patient may cancel or reschedule online
        protected bool CanChangeOnline(object status, object date, object time)
        {
            if (AppointmentStatus.IsFinal(Convert.ToString(status))) return false;
            DateTime? start = AppointmentStatus.StartOf(date, time);
            return !start.HasValue || AppointmentStatus.PatientCanChange(start.Value);
        }

        protected string CancelText(object status, object date, object time)
        {
            string s = Convert.ToString(status);
            if (AppointmentStatus.IsFinal(s)) return s;
            return CanChangeOnline(status, date, time) ? "Cancel Appointment" : "Call clinic to cancel";
        }

        protected string CancelCss(object status, object date, object time)
        {
            if (Convert.ToString(status) == AppointmentStatus.Completed) return "completed-btn-modern";
            return CanChangeOnline(status, date, time) ? "cancel-btn-modern" : "cancelled-btn-modern";
        }

        protected void CancelAppointment(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int appointmentID = Convert.ToInt32(btn.CommandArgument);

            int userID;
            if (Session["UserId"] == null || !int.TryParse(Session["UserId"].ToString(), out userID))
                return;

            // Same rules as the staff pages; a patient may only cancel their own Pending/Confirmed appointments
            string message;
            bool cancelled = AppointmentStatus.TryChange(appointmentID, AppointmentStatus.Cancelled, "User", userID, out message);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "cancelAppt",
                "swalAlert('" + (cancelled ? "success" : "warning") + "', '" + (cancelled ? "Appointment Cancelled" : "Not Cancelled") + "', '" +
                HttpUtility.JavaScriptStringEncode(cancelled ? "Your appointment has been cancelled." : message) + "');", true);

            BindAppointments(userID);   // stays on the same tab and page
        }
    }
}