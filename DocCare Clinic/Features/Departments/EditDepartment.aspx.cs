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
    public partial class EditDepartment : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["DepartmentID"] != null)
                {
                    int departmentId;
                    if (int.TryParse(Request.QueryString["DepartmentID"], out departmentId))
                    {
                        LoadDepartmentDetails(departmentId);
                    }
                }
            }
        }

        private void LoadDepartmentDetails(int departmentId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Departments WHERE DepartmentID = @DepartmentID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtDepartmentName.Text = reader["Department"].ToString();
                        txtDepartmentHead.Text = reader["DepartmentHead"].ToString();
                        txtDescription.Text = reader["Description"].ToString();

                        // Format Date Properly
                        DateTime departmentDate;
                        if (DateTime.TryParse(reader["Date"].ToString(), out departmentDate))
                        {
                            txtDepartmentDate.Text = departmentDate.ToString("yyyy-MM-dd");
                        }

                        string status = reader["Status"].ToString();
                        rblStatus.SelectedValue = status;
                    }
                    con.Close();
                }
            }
        }


        protected void btnUpdateDepartment_Click(object sender, EventArgs e)
        {
            int departmentId;
            if (Request.QueryString["DepartmentID"] != null && int.TryParse(Request.QueryString["DepartmentID"], out departmentId))
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Departments SET Department = @Department, DepartmentHead = @DepartmentHead, " +
                                   "Description = @Description, Date = @Date, Status = @Status WHERE DepartmentID = @DepartmentID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Department", txtDepartmentName.Text.Trim());
                        cmd.Parameters.AddWithValue("@DepartmentHead", txtDepartmentHead.Text.Trim());
                        cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@Date", txtDepartmentDate.Text); // Ensure the format is correct
                        cmd.Parameters.AddWithValue("@Status", rblStatus.SelectedValue);
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();

                            lblErrorMessage.Text = "Department updated successfully!";
                            lblErrorMessage.ForeColor = System.Drawing.Color.Green;
                            lblErrorMessage.Visible = true;                            

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
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Features/Departments/Departments.aspx"));
        }
    }
}