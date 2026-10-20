using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.Script.Serialization;
namespace DocCare_Clinic
{
    public partial class Receptionist : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindReceptionistGrid("");
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {

            string searchText = txtSearch.Text.Trim();
            BindReceptionistGrid(searchText);

        }

        private void BindReceptionistGrid(string searchKeyword = "")
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        R.ReceptionistID, 
                        R.Name AS ReceptionistName, 
                        R.Mobile, 
                        R.Email, 
                        R.Status,
                        ISNULL(STUFF((
                            SELECT ', ' + D.Name
                            FROM Doctors D
                            INNER JOIN ReceptionistDoctorAssignments RDA ON D.DoctorID = RDA.DoctorID
                            WHERE RDA.ReceptionistID = R.ReceptionistID
                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''), 'No Doctor Assigned') AS AssignedDoctors
                    FROM 
                        Receptionists R";

                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    query += " WHERE R.Name LIKE @Keyword";
                }

                query += " GROUP BY R.ReceptionistID, R.Name, R.Mobile, R.Email, R.Status";


                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        cmd.Parameters.AddWithValue("@Keyword", "%" + searchKeyword + "%");
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvReceptionist.DataSource = dt;
                    gvReceptionist.DataBind();
                }
            }
        }

        protected void gvReceptionist_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditReceptionist")
            {

                int id = Convert.ToInt32(e.CommandArgument);
                Response.Redirect(ResolveUrl("~/Features/Receptionists/EditReceptionist.aspx") + "?Id=" + id);
            }
        }

        protected void gvReceptionist_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Calculate the correct row number based on pagination
                int serialNo = gvReceptionist.PageIndex * gvReceptionist.PageSize + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = serialNo.ToString();
                // ✅ Color the Status Text Based On Status
                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                Button btnBlock = (Button)e.Row.FindControl("btnBlock");

                if (lblStatus != null && btnBlock != null)
                {
                    string status = lblStatus.Text;

                    // ✅ Set Text Color
                    if (status == "Active")
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Green;
                        btnBlock.CssClass = "custom-icon-block";
                        btnBlock.ToolTip = "Block";
                        btnBlock.OnClientClick = "return confirm('Are you sure you want to block this receptionist?');";
                    }
                    else if (status == "Blocked")
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        btnBlock.CssClass = "custom-icon-unblock";
                        btnBlock.ToolTip = "Unblock";
                        btnBlock.OnClientClick = "return confirm('Are you sure you want to unblock this receptionist?');";
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
                case "BLOCKED":
                    return "status-red"; // Red color for Inactive      
                default:
                    return "status-gray"; // Default style for unknown statuses
            }
        }

        protected void btnBlock_Command(object sender, CommandEventArgs e)
        {
            int receptionistID = Convert.ToInt32(e.CommandArgument);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // ✅ Check Current Status of the Receptionist
                string query = "SELECT Status FROM Receptionists WHERE ReceptionistID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", receptionistID);

                string status = cmd.ExecuteScalar().ToString();

                // ✅ Toggle Status
                if (status == "Active")
                {
                    query = "UPDATE Receptionists SET Status = 'Blocked' WHERE ReceptionistID = @ID";
                }
                else if (status == "Blocked")
                {
                    query = "UPDATE Receptionists SET Status = 'Active' WHERE ReceptionistID = @ID";
                }

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", receptionistID);
                cmd.ExecuteNonQuery();
            }

            // ✅ Refresh the GridView
            BindReceptionistGrid();
        }

        public string Name { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }



        public string Password { get; set; }

        protected void gvReceptionist_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvReceptionist.PageIndex = e.NewPageIndex;
            BindReceptionistGrid(txtSearch.Text.Trim());
        }


    }
}