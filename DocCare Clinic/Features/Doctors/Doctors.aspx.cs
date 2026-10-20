using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;


namespace DocCare_Clinic
{
    public partial class Doctors : System.Web.UI.Page
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
                    HideColumn();
                    addDoctorButton.Visible = false;
                    lblBreadcrumb.Text = "Assign Doctors";
                }
                else
                {
                    if (userRole == "Doctor")
                        addDoctorButton.Visible = false; // doctors can only view the list
                    BindDoctorsGrid(""); // For Admins, show all doctors
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
            BindDoctorsGrid(searchText);

        }

        private void HideColumn()
        {
            if (gvDoctors.Columns.Count > 0)
            {
                gvDoctors.Columns[4].Visible = false;
                gvDoctors.Columns[7].Visible = false;
                gvDoctors.Columns[8].Visible = false;
                gvDoctors.Columns[9].Visible = false;  
            }
        }

       
        private void BindDoctorsGrid(string searchKeyword)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                        SELECT d.DoctorID, d.Name, 
                               ISNULL(CASE WHEN dp.Status = 'Active' THEN dp.Department ELSE NULL END, 'N/A') AS DepartmentName, 
                               d.Specialization, d.Degree, d.Mobile, d.Email, 
                               d.Gender, d.Charges, d.JoinDate, d.ImageUrl
                        FROM Doctors d
                        LEFT JOIN Departments dp ON d.DepartmentID = dp.DepartmentID
                        WHERE d.IsDeleted = 0";

                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    query += " AND (d.Name LIKE @Keyword OR dp.Department LIKE @Keyword OR d.Specialization LIKE @Keyword)";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        cmd.Parameters.AddWithValue("@Keyword", "%" + searchKeyword + "%");
                    }

                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gvDoctors.DataSource = dt;
                        gvDoctors.DataBind();
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            string searchKeyword = txtSearch.Text.Trim();
            BindDoctorsGrid(searchKeyword);
        }

        protected void gvDoctors_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Only admins manage doctors here. These commands also put the chosen doctor in
            // Session["DoctorId"], which for a logged-in doctor is their own identity.
            if ((Session["UserRole"] as string) != "SuperAdmin")
                return;

            if (e.CommandName == "EditDoctor")
            {
                int doctorId = Convert.ToInt32(e.CommandArgument);
                Session["DoctorId"] = doctorId;
                Response.Redirect(ResolveUrl("~/Features/Doctors/EditDoctor.aspx"));
            }
            else if (e.CommandName == "DeleteDoctor")
            {
                int doctorId = Convert.ToInt32(e.CommandArgument);
                DeleteDoctor(doctorId);
                BindDoctorsGrid(string.Empty);
            }
            else if (e.CommandName == "ViewDoctor")
            {
                int doctorId = Convert.ToInt32(e.CommandArgument);
                Session["DoctorId"] = doctorId;
                Response.Redirect(ResolveUrl("~/Features/Doctors/DoctorProfile.aspx"));
            }
        }

        private void DeleteDoctor(int doctorId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Doctors SET IsDeleted = 1 WHERE DoctorId = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", doctorId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void gvDoctors_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDoctors.PageIndex = e.NewPageIndex;
            BindDoctorsGrid(txtSearch.Text.Trim());
        }

        protected void gvDoctors_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Calculate the correct row number based on pagination
                int serialNo = gvDoctors.PageIndex * gvDoctors.PageSize + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = serialNo.ToString();

                string userRole = Session["UserRole"] as string;
                Button btnEdit = (Button)e.Row.FindControl("btnEdit");
                Button btnDelete = (Button)e.Row.FindControl("btnDelete");
                Button btnView = (Button)e.Row.FindControl("btnView");

                if (userRole == "Doctor")
                {
                    // View-only list: Edit/View would switch Session["DoctorId"] to another doctor
                    if (btnEdit != null) btnEdit.Visible = false;
                    if (btnDelete != null) btnDelete.Visible = false;
                    if (btnView != null) btnView.Visible = false;
                }
                else if (userRole == "Receptionist")
                {
                    // Hide Edit & Delete buttons
                    if (btnEdit != null) btnEdit.Visible = false;
                    if (btnDelete != null) btnDelete.Visible = false;

                    // Convert "View" button into a styled text inside the Actions column
                    if (btnView != null)
                    {
                        TableCell actionsCell = (TableCell)btnView.Parent; // Get the correct cell
                        actionsCell.Controls.Clear(); // Remove existing button

                        string doctorId = btnView.CommandArgument;

                        HyperLink linkView = new HyperLink();
                        linkView.Text = "View"; // Display text
                        linkView.NavigateUrl = ResolveUrl("~/Features/Doctors/DoctorProfile.aspx") + "?DoctorId=" + doctorId;
                        linkView.CssClass = "styled-view-link"; // Apply CSS class

                        actionsCell.Controls.Add(linkView); // Add to correct cell
                    }
                }
            }
        }

    }

}