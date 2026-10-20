using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    /// <summary>
    /// Open follow-ups for the front desk to call about (MedicalRecords.OpenFollowUps): only the date, doctor
    /// and patient, never the diagnosis. Doctors see their own patients.
    /// </summary>
    public partial class FollowUps : System.Web.UI.Page
    {
        string Role { get { return Session["UserRole"] as string; } }

        string Filter
        {
            get { return (ViewState["Filter"] as string) ?? "week"; }
            set { ViewState["Filter"] = value; }
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Role == "Doctor") MasterPageFile = ResolveUrl("~/Features/Doctors/DoctorAdmin.Master");
            else if (Role == "Receptionist") MasterPageFile = ResolveUrl("~/Features/Receptionists/ReceptionistAdmin.Master");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) Bind();
        }

        protected void Filter_Command(object sender, CommandEventArgs e)
        {
            Filter = e.CommandArgument.ToString();
            Bind();
        }

        void Bind()
        {
            int doctor;
            int? doctorId = Role == "Doctor" && Session["DoctorID"] != null && int.TryParse(Session["DoctorID"].ToString(), out doctor) ? doctor : (int?)null;
            DateTime? dueBy = Filter == "week" ? DateTime.Today.AddDays(7) : Filter == "overdue" ? DateTime.Today.AddDays(-1) : (DateTime?)null;
            DataTable dt = MedicalRecords.OpenFollowUps(doctorId: doctorId, dueBy: dueBy);

            rptFollowUps.DataSource = dt;
            rptFollowUps.DataBind();
            pnlEmpty.Visible = dt.Rows.Count == 0;
            litEmpty.Text = Filter == "week" ? "No follow-ups are due this week." : Filter == "overdue" ? "No overdue follow-ups." : "There are no open follow-ups.";
            int overdue = dt.Rows.Cast<DataRow>().Count(r => Convert.ToDateTime(r["FollowUpDate"]).Date < DateTime.Today);
            litCount.Text = dt.Rows.Count + (dt.Rows.Count == 1 ? " patient" : " patients") + (overdue > 0 && Filter != "overdue" ? " &middot; " + overdue + " overdue" : "");

            lnkWeek.CssClass = "fu-chip" + (Filter == "week" ? " active" : "");
            lnkOverdue.CssClass = "fu-chip" + (Filter == "overdue" ? " active" : "");
            lnkAll.CssClass = "fu-chip" + (Filter == "all" ? " active" : "");
        }

        protected void rptFollowUps_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (e.CommandName != "Close" || !int.TryParse(e.CommandArgument.ToString(), out id)) return;

            // A doctor may only close follow-ups for their own patients
            DataTable row = MedicalRecords.OpenFollowUps(appointmentId: id);
            int doctor;
            bool allowed = row.Rows.Count == 1 && (Role != "Doctor" ||
                           (Session["DoctorID"] != null && int.TryParse(Session["DoctorID"].ToString(), out doctor) && doctor == Convert.ToInt32(row.Rows[0]["DoctorID"])));
            bool closed = allowed && MedicalRecords.CloseFollowUp(id, MedicalRecords.StaffName(Session));
            ClientScript.RegisterStartupScript(GetType(), "fuClosed", closed
                ? "swalAlert('success', 'Removed', 'The follow-up is marked as not needed.');"
                : "swalAlert('warning', 'Not Changed', 'This follow-up is no longer open.');", true);
            Bind();
        }

        protected string DueBadge(object value)
        {
            DateTime d = Convert.ToDateTime(value).Date;
            int days = (d - DateTime.Today).Days;
            if (days < 0) return "<span class=\"fu-badge overdue\">Overdue " + (-days) + (days == -1 ? " day" : " days") + "</span>";
            if (days == 0) return "<span class=\"fu-badge today\">Today</span>";
            return "<span class=\"fu-badge soon\">In " + days + (days == 1 ? " day" : " days") + "</span>";
        }

        protected string MobileLink(object value)
        {
            string m = Convert.ToString(value).Trim();
            return m == "" ? "<span class=\"fu-small\">No mobile</span>" : "<a href=\"tel:" + HttpUtility.HtmlAttributeEncode(m) + "\">" + HttpUtility.HtmlEncode(m) + "</a>";
        }

        protected string ReminderText(object item)
        {
            DataRowView r = (DataRowView)item;
            if (r["FollowUpReminderSentAt"] != DBNull.Value)
                return "Emailed " + Convert.ToDateTime(r["FollowUpReminderSentAt"]).ToString("dd MMM", System.Globalization.CultureInfo.InvariantCulture);
            if (r["Email"].ToString().Trim() == "") return "<span class=\"fu-small\">No email &middot; please call</span>";
            DateTime d = Convert.ToDateTime(r["FollowUpDate"]).Date;
            return d < DateTime.Today ? "<span class=\"fu-small\">Not emailed</span>"
                 : "<span class=\"fu-small\">Email on " + SlotService.FormatDate(d.AddDays(-ReminderService.FollowUpReminderDaysBefore) < DateTime.Today ? DateTime.Today : d.AddDays(-ReminderService.FollowUpReminderDaysBefore)) + "</span>";
        }
    }
}
