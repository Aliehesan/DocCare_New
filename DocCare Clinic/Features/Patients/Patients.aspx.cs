using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using DocCare_Clinic.Config;
using System.Data.SqlClient;
using System.Data;

namespace DocCare_Clinic
{
    public partial class Patients : System.Web.UI.Page
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

                if (userRole == "Doctor")
                {
                    HideAdminMenus();
                    BindDoctorPatients("");
                }
                else
                {
                    BindAllPatient("");
                }
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string userRole = Session["UserRole"] as string;
            string searchText = txtSearch.Text.Trim();
            if (userRole == "Doctor")
            {
                HideAdminMenus();
                BindDoctorPatients(searchText);
            }
            else
            {
                BindAllPatient(searchText);
            }
        }

        private void HideAdminMenus()
        {
            // Find menu controls inside MasterPage
            Control menuReceptionist = Master.FindControl("menuReceptionist");
            Control menuDoctor = Master.FindControl("menuDoctor");
            Control menuDepartment = Master.FindControl("menuDepartment");

            if (menuReceptionist != null) menuReceptionist.Visible = false;
            if (menuDoctor != null) menuDoctor.Visible = false;
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

        private void BindDoctorPatients(string searchQuery)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                if (Session["DoctorId"] != null)
                {
                    int doctorId = Convert.ToInt32(Session["DoctorId"]);

                    string sqlQuery = @"
                SELECT DISTINCT 
                        p.PatientID, 
                        p.PatientName, 
                        p.PatientCode, 
                        p.DateOfBirth, 
                        p.Gender, 
                        p.Status, 
                        COALESCE(u.Mobile, p.Mobile, 'N/A') AS Mobile
                    FROM 
                        Appointments a
                    INNER JOIN 
                        Patients p ON a.PatientID = p.PatientID 
                    LEFT JOIN 
                        Users u ON p.UserID = u.UserID
                    WHERE 
                        a.DoctorID = @DoctorID
                        AND p.IsDeleted = 0
                    ";

                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        // Name, Patient ID ("DC-00029", "dc29") or mobile
                        sqlQuery += @" AND (p.PatientName LIKE @SearchQuery OR p.PatientCode LIKE @SearchQuery
                         OR (@TypedPatientID IS NOT NULL AND p.PatientID = @TypedPatientID)
                         OR p.Mobile LIKE @SearchQuery OR u.Mobile LIKE @SearchQuery)";
                    }

                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                        if (!string.IsNullOrEmpty(searchQuery))
                        {
                            cmd.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                            int? typed = PatientCode.Parse(searchQuery);
                            cmd.Parameters.AddWithValue("@TypedPatientID", typed.HasValue ? (object)typed.Value : DBNull.Value);
                        }

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            gvPatients.DataSource = dt;
                            gvPatients.DataBind();
                        }
                    }
                }
                else
                {
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }
            }
        }

        private void BindAllPatient(string searchQuery)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sqlQuery = @"
                SELECT 
                    p.PatientID, 
                    p.PatientName, 
                    p.PatientCode, 
                    p.DateOfBirth, 
                    p.Gender, 
                    p.Status, 
                    COALESCE(u.Mobile, p.Mobile, 'N/A') AS Mobile
                FROM 
                    Patients p
                LEFT JOIN 
                    Users u ON p.UserID = u.UserID
                WHERE 
                    p.IsDeleted = 0";

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    // Name, Patient ID ("DC-00029", "dc29") or mobile
                        sqlQuery += @" AND (p.PatientName LIKE @SearchQuery OR p.PatientCode LIKE @SearchQuery
                         OR (@TypedPatientID IS NOT NULL AND p.PatientID = @TypedPatientID)
                         OR p.Mobile LIKE @SearchQuery OR u.Mobile LIKE @SearchQuery)";
                }

                using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                {
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        cmd.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                            int? typed = PatientCode.Parse(searchQuery);
                            cmd.Parameters.AddWithValue("@TypedPatientID", typed.HasValue ? (object)typed.Value : DBNull.Value);
                    }

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        gvPatients.DataSource = dt;
                        gvPatients.DataBind();
                    }
                }
            }
        }



        protected void gvPatients_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPatients.PageIndex = e.NewPageIndex;
            string userRole = Session["UserRole"] as string;
            if (userRole == "Doctor")
            {
                HideAdminMenus();
                BindDoctorPatients(txtSearch.Text.Trim());
            }
            else
            {
                BindAllPatient(txtSearch.Text.Trim());
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

        protected void gvPatients_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string patientID = e.CommandArgument.ToString();
            string userRole = Session["UserRole"] as string;
            if (e.CommandName == "EditDoctor")
            {
                Response.Redirect(ResolveUrl("~/Features/Patients/EditPatient.aspx") + "?ID=" + patientID);
            }
            else if (e.CommandName == "DeleteDoctor")
            {
                if (userRole == "Doctor")
                {
                    SoftDeletePatient(patientID);
                    BindDoctorPatients("");
                }
                else
                {
                    SoftDeletePatient(patientID);
                    BindAllPatient("");
                }

            }
            else if (e.CommandName == "ViewDoctor")
            {
                Response.Redirect(ResolveUrl("~/Features/Patients/PatientProfile.aspx") + "?ID=" + patientID);
            }
        }


        private void SoftDeletePatient(string patientID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sqlQuery = "UPDATE Patients SET IsDeleted = 1 WHERE PatientID = @PatientID";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientID);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        protected void ddlActiveInactive_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddl.NamingContainer;

            if (gvPatients.DataKeys.Count > row.RowIndex) // Ensure index exists
            {
                int PatientID = Convert.ToInt32(gvPatients.DataKeys[row.RowIndex].Value);
                string newStatus = ddl.SelectedValue;

                string query = "UPDATE Patients SET Status = @Status WHERE PatientID = @PatientID";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.Parameters.AddWithValue("@PatientID", PatientID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                // Refresh GridView
                BindAllPatient("");
                gvPatients.DataBind();
            }
        }


        protected void gvPatients_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Calculate the correct row number based on pagination
                int serialNo = gvPatients.PageIndex * gvPatients.PageSize + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = serialNo.ToString();

                DropDownList ddlActiveInactive = (DropDownList)e.Row.FindControl("ddlActiveInactive");
                if (ddlActiveInactive != null)
                {
                    string currentStatus = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                    ddlActiveInactive.SelectedValue = currentStatus;

                    // Disable dropdown for doctors
                    if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Doctor")
                    {
                        ddlActiveInactive.Enabled = false;
                    }
                }
            }
        }
    }
}