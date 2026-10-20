using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocCare_Clinic
{
    public partial class ReceptionistAdmin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Abandon the session
            Session.Abandon();

            // Optionally clear session-specific cookies
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            }

            // Redirect to the login page
            Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
        }
    }
}