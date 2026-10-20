using System;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    /// <summary>Compare two patient records and merge the duplicate into the one kept (PatientMerge).</summary>
    public partial class MergePatients : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if ((Session["UserRole"] as string) == "Receptionist") MasterPageFile = ResolveUrl("~/Features/Receptionists/ReceptionistAdmin.Master");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindSuggestions();
        }

        void BindSuggestions()
        {
            DataTable dt = PatientMerge.Suggestions();
            rptSuggest.DataSource = dt;
            rptSuggest.DataBind();
            pnlNoSuggest.Visible = dt.Rows.Count == 0;
        }

        // Suggest keeping the record with an online account, then the one with more appointments
        protected string KeepFirst(object item)
        {
            DataRowView r = (DataRowView)item;
            bool bFirst = (r["AUser"] == DBNull.Value && r["BUser"] != DBNull.Value) ||
                          (r["AUser"] == DBNull.Value == (r["BUser"] == DBNull.Value) && Convert.ToInt32(r["BVisits"]) > Convert.ToInt32(r["AVisits"]));
            return bFirst ? r["BID"] + "," + r["AID"] : r["AID"] + "," + r["BID"];
        }

        protected void rptSuggest_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string[] ids = e.CommandArgument.ToString().Split(',');
            txtKeep.Text = PatientCode.Of(ids[0]);
            txtDup.Text = PatientCode.Of(ids[1]);
            Preview();
        }

        protected void btnPreview_Click(object sender, EventArgs e) { Preview(); }

        protected void btnSwap_Click(object sender, EventArgs e)
        {
            string k = txtKeep.Text;
            txtKeep.Text = txtDup.Text;
            txtDup.Text = k;
            Preview();
        }

        static DataRow Find(string text)
        {
            int? id = PatientCode.Parse(text);
            int plain;
            if (!id.HasValue && int.TryParse((text ?? "").Trim(), out plain)) id = plain;   // a bare record number also works here
            return id.HasValue ? PatientMerge.Load(id.Value) : null;
        }

        // Both records side by side, and whether they can be merged
        bool Preview()
        {
            DataRow keep = Find(txtKeep.Text), dup = Find(txtDup.Text);
            pnlPreview.Visible = true;
            litKeep.Text = CardHtml(keep, txtKeep.Text);
            litDup.Text = CardHtml(dup, txtDup.Text);
            string problem = PatientMerge.Problem(keep, dup);
            pnlProblem.Visible = problem != null;
            litProblem.Text = HttpUtility.HtmlEncode(problem);
            pnlExplain.Visible = btnMerge.Visible = btnSwap.Visible = problem == null;
            if (problem != null) { btnSwap.Visible = keep != null && dup != null; return false; }

            int n = Convert.ToInt32(dup["Appointments"]);
            litMoveCount.Text = n == 0 ? "No appointments" : n + (n == 1 ? " appointment" : " appointments") + " of " + HttpUtility.HtmlEncode(dup["PatientCode"]);
            litKeepCode.Text = litKeepCode2.Text = HttpUtility.HtmlEncode(keep["PatientCode"]);
            litDupCode.Text = HttpUtility.HtmlEncode(dup["PatientCode"]);
            txtKeep.Text = keep["PatientCode"].ToString();
            txtDup.Text = dup["PatientCode"].ToString();
            return true;
        }

        static string CardHtml(DataRow p, string typed)
        {
            Func<object, string> e = o => HttpUtility.HtmlEncode(Convert.ToString(o));
            if (p == null)
                return "<div class=\"name\">Not found</div><p class=\"mg-none\">No active patient " + e(typed) + ".</p>";
            Func<string, object, string> row = (label, value) =>
                "<dt>" + label + "</dt><dd>" + (Convert.ToString(value).Trim() == "" ? "<span style=\"color:#9ca3af;\">none</span>" : e(value)) + "</dd>";
            return "<div class=\"name\">" + e(p["PatientName"]) + " <span class=\"patient-code\" style=\"display:inline;\">" + e(p["PatientCode"]) + "</span></div><dl>" +
                row("Date of birth", p["DateOfBirth"] == DBNull.Value ? "" : Convert.ToDateTime(p["DateOfBirth"]).ToString("dd MMM yyyy", CultureInfo.InvariantCulture)) +
                row("Gender", p["Gender"]) + row("Mobile", p["Mobile"]) + row("Email", p["Email"]) + row("Address", p["Address"]) +
                row("Online account", p["UserID"] == DBNull.Value ? "" : (p["AccountName"] + " (#" + p["UserID"] + ")")) +
                row("Appointments", p["Appointments"] + (p["LastVisit"] == DBNull.Value ? "" : " · last " + Convert.ToDateTime(p["LastVisit"]).ToString("dd MMM yyyy", CultureInfo.InvariantCulture))) +
                row("Visit notes", p["Notes"]) + row("Health summary", Convert.ToInt32(p["HasHealth"]) == 1 ? "recorded" : "") + "</dl>";
        }

        protected void btnMerge_Click(object sender, EventArgs e)
        {
            DataRow keep = Find(txtKeep.Text), dup = Find(txtDup.Text);
            string problem = PatientMerge.Problem(keep, dup);   // checked again: the records may have changed since the preview
            if (problem != null)
            {
                Preview();
                Alert("warning", "Not Merged", problem);
                return;
            }
            int moved = PatientMerge.Merge(Convert.ToInt32(keep["PatientID"]), Convert.ToInt32(dup["PatientID"]), MedicalRecords.StaffName(Session));
            string keptCode = keep["PatientCode"].ToString(), dupCode = dup["PatientCode"].ToString();
            txtKeep.Text = txtDup.Text = "";
            pnlPreview.Visible = false;
            BindSuggestions();
            string profile = ResolveUrl("~/Features/Patients/PatientProfile.aspx?Id=" + keep["PatientID"]);
            ClientScript.RegisterStartupScript(GetType(), "merged",
                "Swal.fire({ icon: 'success', title: 'Records Merged', text: '" + HttpUtility.JavaScriptStringEncode(dupCode + " is merged into " + keptCode + " (" + moved +
                (moved == 1 ? " appointment" : " appointments") + " moved).") + "', showCancelButton: true, confirmButtonText: 'Open " + keptCode + "', cancelButtonText: 'Close'," +
                " confirmButtonColor: '#2E37A4' }).then(function (r) { if (r.isConfirmed) window.location.href = '" + HttpUtility.JavaScriptStringEncode(profile) + "'; });", true);
        }

        void Alert(string icon, string title, string text)
        {
            ClientScript.RegisterStartupScript(GetType(), "mergeMsg",
                "swalAlert('" + icon + "', '" + HttpUtility.JavaScriptStringEncode(title) + "', '" + HttpUtility.JavaScriptStringEncode(text) + "');", true);
        }
    }
}
