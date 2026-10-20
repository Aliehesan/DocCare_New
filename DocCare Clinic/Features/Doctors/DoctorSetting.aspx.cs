using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocCare_Clinic
{
    public partial class DoctorSetting : System.Web.UI.Page
    {
        // Doctors get their own layout and menu (access itself is checked in Global.asax)
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if ((Session["UserRole"] as string) == "Doctor")
            {
                this.MasterPageFile = ResolveUrl("~/Features/Doctors/DoctorAdmin.Master");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}