using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace DocCare_Clinic
{
    public partial class AddDepartment : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCreateDepartment_Click(object sender, EventArgs e)
        {
            // Get input values
            string departmentName = txtDepartmentName.Text.Trim();
            string departmentHead = txtDepartmentHead.Text.Trim();
            string description = txtDescription.Text.Trim();
            string departmentDate = txtDepartmentDate.Text.Trim();
            string status = rblStatus.SelectedValue; // Active or Inactive

            // Validate required fields
            if (string.IsNullOrEmpty(departmentName) || string.IsNullOrEmpty(departmentHead) ||
                string.IsNullOrEmpty(description) || string.IsNullOrEmpty(departmentDate) || string.IsNullOrEmpty(status))
            {
                lblErrorMessage.Text = "*Please fill in all required fields.";
            }

            // Insert department into the database
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Departments (Department, DepartmentHead, Description, Date, Status) " +
                               "VALUES (@Departmentname, @DepartmentHead, @Description, @DepartmentDate, @Status)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
                    cmd.Parameters.AddWithValue("@DepartmentHead", departmentHead);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@DepartmentDate", departmentDate);
                    cmd.Parameters.AddWithValue("@Status", status);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        lblErrorMessage.Text = "Department added successfully!";
                        lblErrorMessage.ForeColor = System.Drawing.Color.Green;
                        lblErrorMessage.Visible = true;
                        ClearFields();

                        string script = "setTimeout(function() { window.location.href = '" + ResolveUrl("~/Features/Departments/Departments.aspx") + "'; }, 2000);";
                        ClientScript.RegisterStartupScript(this.GetType(), "Redirect", script, true);

                    }
                    catch (Exception ex)
                    {
                        lblErrorMessage.Text = "An error occurred: " + ex.Message;
                        lblErrorMessage.Visible = true;
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearFields();
            Response.Redirect(ResolveUrl("~/Features/Departments/Departments.aspx"));
        }

        private void ClearFields()
        {
            txtDepartmentName.Text = "";
            txtDepartmentHead.Text = "";
            txtDescription.Text = "";
            txtDepartmentDate.Text = "";
            rblStatus.ClearSelection();
        }
    }
}