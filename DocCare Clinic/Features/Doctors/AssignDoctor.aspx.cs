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
    public partial class AssignDoctor : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request.QueryString["ReceptionistId"]))
                {
                    Response.Redirect(ResolveUrl("~/Features/Receptionists/Receptionist.aspx"));
                }
                else
                {
                    BindReceptionistName();
                    LoadReceptionists();
                    LoadDoctors();
                }
            }
        }

        private void BindReceptionistName()
        {
            int receptionistId;
            if (int.TryParse(Request.QueryString["ReceptionistId"], out receptionistId))
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT ReceptionistID, Name FROM Receptionists WHERE ReceptionistID = @ReceptionistID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ReceptionistID", receptionistId);

                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            ddlReceptionists.Items.Clear();
                            ddlReceptionists.Items.Add(new ListItem(reader["Name"].ToString(), reader["ReceptionistID"].ToString()));
                            ddlReceptionists.SelectedIndex = 0;
                        }
                        else
                        {
                            ddlReceptionists.Items.Clear();
                            ddlReceptionists.Items.Add(new ListItem("Receptionist Not Found", ""));
                        }
                    }
                }
            }
            else
            {
                ddlReceptionists.Items.Clear();
                ddlReceptionists.Items.Add(new ListItem("Select Receptionist", ""));
            }
        }

        private void LoadReceptionists()
        {
            string query = "SELECT ReceptionistID, Name FROM Receptionists Where Status = 'Active'";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlReceptionists.DataSource = reader;
                    ddlReceptionists.DataTextField = "Name";
                    ddlReceptionists.DataValueField = "ReceptionistID";
                    ddlReceptionists.DataBind();
                }
            }

            ddlReceptionists.Items.Insert(0, new ListItem("Select Receptionist", ""));
        }

        private void LoadDoctors()
        {
            string receptionistId = Request.QueryString["ReceptionistId"]; // Get current receptionist
            string query = @"
        SELECT d.DoctorId, d.Name, d.Degree, d.Specialization, d.ImageUrl, 
               (SELECT ReceptionistID FROM ReceptionistDoctorAssignments WHERE DoctorID = d.DoctorId) AS AssignedReceptionist
        FROM Doctors d Where IsDeleted = 0";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    lvDoctors.DataSource = reader;
                    lvDoctors.DataBind();
                }
            }
        }

        protected void btnAssign_Click(object sender, EventArgs e)
        {
            int receptionistId;
            if (int.TryParse(Request.QueryString["ReceptionistId"], out receptionistId))
            {
                List<int> selectedDoctors = ViewState["SelectedDoctors"] as List<int> ?? new List<int>();

                // ✅ Unassign all doctors if none are selected
                if (selectedDoctors.Count == 0)
                {
                    UnassignAllDoctorsFromReceptionist(receptionistId);
                    ShowSuccessMessageAndRedirect("✅ All doctors unassigned from the receptionist.", ResolveUrl("~/Features/Receptionists/Receptionist.aspx"));
                    return;
                }

                // ✅ Assign selected doctors
                AssignDoctorsToReceptionist(receptionistId, selectedDoctors);
                ShowSuccessMessageAndRedirect("✅ Doctors successfully assigned to the receptionist.", ResolveUrl("~/Features/Receptionists/Receptionist.aspx"));
            }
            else
            {
                ShowErrorMessage("❌ Invalid receptionist.");
            }
        }

        private void UnassignAllDoctorsFromReceptionist(int receptionistId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string deleteQuery = "DELETE FROM ReceptionistDoctorAssignments WHERE ReceptionistID = @ReceptionistID";
                using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@ReceptionistID", receptionistId);
                    deleteCmd.ExecuteNonQuery();
                }
            }
        }

        private void AssignDoctorsToReceptionist(int receptionistId, List<int> doctorIds)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (int doctorId in doctorIds)
                {
                    // ✅ Check if doctor is already assigned to any receptionist
                    string checkQuery = "SELECT COUNT(*) FROM ReceptionistDoctorAssignments WHERE DoctorID = @DoctorID";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count == 0) // ✅ Doctor is not assigned → Assign to receptionist
                        {
                            string insertQuery = "INSERT INTO ReceptionistDoctorAssignments (ReceptionistID, DoctorID) VALUES (@ReceptionistID, @DoctorID)";
                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@ReceptionistID", receptionistId);
                                insertCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            // ✅ Show Success Message + Redirect Automatically
            ShowSuccessMessageAndRedirect("✅ Doctors successfully assigned to the receptionist.", ResolveUrl("~/Features/Receptionists/Receptionist.aspx"));
        }

        protected void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            // Just update selection - don't modify the database yet
            CheckBox chk = (CheckBox)sender;
            ListViewItem item = (ListViewItem)chk.NamingContainer;
            HiddenField hfDoctorId = (HiddenField)item.FindControl("hfDoctorId");

            int doctorId;
            if (int.TryParse(hfDoctorId.Value, out doctorId))
            {
                if (chk.Checked)
                {
                    // Store in ViewState so btnAssign_Click can use it
                    List<int> selectedDoctors = ViewState["SelectedDoctors"] as List<int> ?? new List<int>();
                    if (!selectedDoctors.Contains(doctorId))
                    {
                        selectedDoctors.Add(doctorId);
                    }
                    ViewState["SelectedDoctors"] = selectedDoctors;
                }
                else
                {
                    // Remove from ViewState if unchecked
                    List<int> selectedDoctors = ViewState["SelectedDoctors"] as List<int> ?? new List<int>();
                    selectedDoctors.Remove(doctorId);
                    ViewState["SelectedDoctors"] = selectedDoctors;
                }
            }
        }

        protected bool IsDoctorAssigned(string doctorId)
        {
            string receptionistId = Request.QueryString["ReceptionistId"];
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM ReceptionistDoctorAssignments WHERE ReceptionistID = @ReceptionistID AND DoctorID = @DoctorID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ReceptionistID", receptionistId);
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        protected bool IsDoctorAssignedToAnotherReceptionist(string doctorId)
        {
            string receptionistId = Request.QueryString["ReceptionistId"];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT ReceptionistID FROM ReceptionistDoctorAssignments WHERE DoctorID = @DoctorID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    conn.Open();
                    object assignedReceptionist = cmd.ExecuteScalar();

                    return assignedReceptionist != null && assignedReceptionist.ToString() != receptionistId;
                }
            }
        }

        private void ShowSuccessMessageAndRedirect(string message, string redirectUrl)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                "alert('" + message + "'); window.location='" + redirectUrl + "';", true);
        }

        private void ShowErrorMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + message + "');", true);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Features/Receptionists/Receptionist.aspx"));
        }
    }
}