using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace DocCare_Clinic
{
    public partial class Manage_Doctor : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string userRole = Session["UserRole"] as string;
                if (string.IsNullOrEmpty(userRole))
                {
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }

                if (userRole == "Receptionist")
                {
                    HideAdminMenus(); // Hide Super Admin menus
                    BindReceptionistDoctor(""); // Bind only assigned doctors                                        
                }
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

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {

            string searchText = txtSearch.Text.Trim();
            BindReceptionistDoctor(searchText);

        }

        private void BindReceptionistDoctor(string searchKeyword)
        {
            // Get the logged-in Receptionist's ID from the session
            int receptionistId = Convert.ToInt32(Session["ReceptionistID"]); // Ensure UserID is ReceptionistID

            string query = @"
                SELECT d.DoctorID, d.Name, dep.Department AS DepartmentName, d.Specialization, d.Mobile, 
                       d.Email
                FROM Doctors d
                INNER JOIN ReceptionistDoctorAssignments rda ON d.DoctorID = rda.DoctorID
                INNER JOIN Departments dep ON d.DepartmentID = dep.DepartmentID
                WHERE rda.ReceptionistID = @ReceptionistID";

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query += " AND (d.Name LIKE @Keyword OR dep.Department LIKE @Keyword OR d.Specialization LIKE @Keyword)";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        cmd.Parameters.AddWithValue("@Keyword", "%" + searchKeyword + "%");
                    }

                    cmd.Parameters.AddWithValue("@ReceptionistID", receptionistId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvManageDoctors.DataSource = dt;
                    gvManageDoctors.DataBind();
                }
            }           
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            string searchKeyword = txtSearch.Text.Trim();
            BindReceptionistDoctor(searchKeyword);
        }

        protected void btnViewAppointments_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string doctorId = e.CommandArgument.ToString();
                Response.Redirect(ResolveUrl("~/Features/Doctors/DoctorsAppoinment.aspx") + "?DoctorID=" + doctorId);
            }
        }

        protected void btnViewSchedule_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string doctorId = e.CommandArgument.ToString();
                Response.Redirect(ResolveUrl("~/Features/Scheduling/Schedule.aspx") + "?DoctorID=" + doctorId);
            }
        }

        protected void gvManageDoctors_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvManageDoctors.PageIndex = e.NewPageIndex;
            BindReceptionistDoctor(txtSearch.Text.Trim());
        }

        protected void gvManageDoctors_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Calculate the correct row number based on pagination
                int serialNo = gvManageDoctors.PageIndex * gvManageDoctors.PageSize + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = serialNo.ToString();                           
            }
        }
    }
}