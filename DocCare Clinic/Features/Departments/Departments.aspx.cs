//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.HtmlControls;
//using System.Configuration;
//using System.Data.SqlClient;
//using System.Data;

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
    public partial class Departments : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDepartments("");
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {

            string searchText = txtSearch.Text.Trim();
            LoadDepartments(searchText);

        }

        private void LoadDepartments(string search = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Departments";

                if (!string.IsNullOrEmpty(search))
                {
                    query += " WHERE Department LIKE @Search OR DepartmentHead LIKE @Search";
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        cmd.Parameters.AddWithValue("@Search", "%" + search + "%");
                    }

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        gvDepartments.DataSource = dt;
                        gvDepartments.DataBind();
                    }
                }
            }
        }
        protected string GetStatusCssClass(string status)
        {
            switch (status.ToUpper())
            {
                case "ACTIVE":
                    return "status-green"; // Green color for Active
                case "INACTIVE":
                    return "status-red"; // Red color for Inactive      
                default:
                    return "status-gray"; // Default style for unknown statuses
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadDepartments(txtSearch.Text.Trim());
        }

        protected void gvDepartments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditDoctor")
            {
                int departmentId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect(ResolveUrl("~/Features/Departments/EditDepartment.aspx") + "?DepartmentID=" + departmentId);
            }
            else if (e.CommandName == "DeleteDoctor")
            {
                int departmentId = Convert.ToInt32(e.CommandArgument);
                DeleteDepartment(departmentId);
            }
        }

        private void DeleteDepartment(int departmentId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Departments WHERE DepartmentID = @DepartmentID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            // Reload the grid after deletion
            LoadDepartments();
        }

        protected void gvDepartments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDepartments.PageIndex = e.NewPageIndex;
            LoadDepartments(txtSearch.Text.Trim());
        }

        protected void gvDepartments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Calculate the correct row number based on pagination
                int serialNo = gvDepartments.PageIndex * gvDepartments.PageSize + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = serialNo.ToString();

                DropDownList ddlActiveInactive = (DropDownList)e.Row.FindControl("ddlActiveInactive");
                if (ddlActiveInactive != null)
                {
                    string currentStatus = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                    ddlActiveInactive.SelectedValue = currentStatus;
                }
            }
        }

        protected void ddlActiveInactive_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddl.NamingContainer;

            if (gvDepartments.DataKeys.Count > row.RowIndex) // Ensure index exists
            {
                int DepartmentID = Convert.ToInt32(gvDepartments.DataKeys[row.RowIndex].Value);
                string newStatus = ddl.SelectedValue;

                string query = "UPDATE Departments SET Status = @Status WHERE DepartmentID = @DepartmentID";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.Parameters.AddWithValue("@DepartmentID", DepartmentID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                // Refresh GridView
                LoadDepartments("");
                gvDepartments.DataBind();
            }
        }
    }
}