using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocCare_Clinic
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clear all session variables
            Session.Clear();
            Session.Abandon();
            // Redirect to login page
            Response.Redirect(ResolveUrl("~/Features/Site/Default.aspx"));
        }
    }
}