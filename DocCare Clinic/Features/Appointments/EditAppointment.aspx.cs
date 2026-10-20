using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocCare_Clinic
{
    public partial class EditAppointment : System.Web.UI.Page
    {
        // Show each staff role its own layout and menu (access itself is checked in Global.asax)
        protected void Page_PreInit(object sender, EventArgs e)
        {
            string userRole = Session["UserRole"] as string;

            if (userRole == "Doctor")
            {
                this.MasterPageFile = ResolveUrl("~/Features/Doctors/DoctorAdmin.Master");
            }
            else if (userRole == "Receptionist")
            {
                this.MasterPageFile = ResolveUrl("~/Features/Receptionists/ReceptionistAdmin.Master");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}