using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    /// <summary>Online advances on cancelled appointments: still to refund, and already refunded (Refunds service).</summary>
    public partial class Refunds : System.Web.UI.Page
    {
        string Tab
        {
            get { return (ViewState["Tab"] as string) ?? "due"; }
            set { ViewState["Tab"] = value; }
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if ((Session["UserRole"] as string) == "Receptionist") MasterPageFile = ResolveUrl("~/Features/Receptionists/ReceptionistAdmin.Master");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) Bind();
        }

        protected void Tab_Command(object sender, CommandEventArgs e)
        {
            Tab = e.CommandArgument.ToString();
            Bind();
        }

        void Bind()
        {
            bool done = Tab == "done";
            DataTable dt = DocCare_Clinic.Config.Refunds.List(done);
            rptRefunds.DataSource = dt;
            rptRefunds.DataBind();
            pnlEmpty.Visible = dt.Rows.Count == 0;
            litEmpty.Text = done ? "Nothing has been marked refunded yet." : "No refunds are due.";
            litActionHead.Text = done ? "Refunded" : "";

            decimal sum = dt.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["PaidAmount"]));
            litTotal.Text = dt.Rows.Count == 0 ? "" : done
                ? dt.Rows.Count + (dt.Rows.Count == 1 ? " refund" : " refunds") + " &middot; &#8377;" + sum.ToString("N0")
                : dt.Rows.Count + " due &middot; <strong>&#8377;" + sum.ToString("N0") + "</strong> to refund";
            lnkDue.CssClass = "rf-chip" + (done ? "" : " active");
            lnkDone.CssClass = "rf-chip" + (done ? " active" : "");
        }

        protected void rptRefunds_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            DataRowView r = (DataRowView)e.Item.DataItem;
            bool refunded = r["RefundedAt"] != DBNull.Value;
            e.Item.FindControl("pnlMark").Visible = !refunded;
            if (refunded)
                ((Literal)e.Item.FindControl("litDone")).Text = "<span class=\"rf-done\">" + Convert.ToDateTime(r["RefundedAt"]).ToString("dd MMM yyyy, hh:mm tt", System.Globalization.CultureInfo.InvariantCulture) +
                    "</span><span class=\"rf-small\">by " + HttpUtility.HtmlEncode(r["RefundedBy"]) + "</span>" +
                    (r["RefundNote"].ToString() == "" ? "" : "<span class=\"rf-small\">" + HttpUtility.HtmlEncode(r["RefundNote"]) + "</span>");
        }

        protected void rptRefunds_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (e.CommandName != "Mark" || !int.TryParse(e.CommandArgument.ToString(), out id)) return;
            string note = ((TextBox)e.Item.FindControl("txtNote")).Text;
            bool ok = DocCare_Clinic.Config.Refunds.MarkRefunded(id, note, MedicalRecords.StaffName(Session));
            ClientScript.RegisterStartupScript(GetType(), "refund", ok
                ? "swalAlert('success', 'Refund Recorded', 'The advance is marked as refunded.');"
                : "swalAlert('warning', 'Not Changed', 'This refund was already recorded.');", true);
            Bind();
        }

        protected string MobileLink(object value)
        {
            string m = Convert.ToString(value).Trim();
            return m == "" ? "<span class=\"rf-small\">No mobile</span>" : "<a href=\"tel:" + HttpUtility.HtmlAttributeEncode(m) + "\">" + HttpUtility.HtmlEncode(m) + "</a>";
        }
    }
}
