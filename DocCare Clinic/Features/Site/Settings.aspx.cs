using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocCare_Clinic
{
    public partial class Settings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string userRole = Session["UserRole"] as string;

                if (string.IsNullOrEmpty(userRole))
                {
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }

                if (userRole == "Doctor")
                {
                    lnkDashboard.NavigateUrl = ResolveUrl("~/Features/Doctors/DoctorDashBoard.aspx"); // Redirect to doctor dashboard
                }
                else if (userRole == "Receptionist")
                {
                    lnkDashboard.NavigateUrl = ResolveUrl("~/Features/Receptionists/ReceptionistDashboard.aspx"); // Redirect to receptionist dashboard
                }
                else
                {
                    lnkDashboard.NavigateUrl = ResolveUrl("~/Features/Admin/AdminDashBoard.aspx"); // Default Admin Dashboard
                }
            }
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
    }
}