using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocCare_Clinic
{
    public partial class UserProfile : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int UserId;
                if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out UserId))
                {
                    LoadUserDetails(UserId);
                    LoadRecentAppointments(UserId);
                    LoadFamilyMembers(UserId);
                }
                else
                {
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }
            }
        }

        private void LoadUserDetails(int UserId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Name, Email, Mobile, Address FROM Users WHERE UserId = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", UserId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Top identity card labels
                    lblName.Text = reader["Name"].ToString();
                    lblEmail.Text = reader["Email"].ToString();
                    lblPhone.Text = reader["Mobile"].ToString();

                    // Left column detail labels
                    lblNameDetail.Text = reader["Name"].ToString();
                    lblEmailDetail.Text = reader["Email"].ToString();
                    lblPhoneDetail.Text = reader["Mobile"].ToString();
                    lblAddress.Text = reader["Address"].ToString();
                }
            }
        }

        private void LoadRecentAppointments(int UserId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT TOP 3 
                                    a.AppointDate, a.AppointTime, a.Status,
                                    d.Name AS DoctorName, d.Specialization
                                 FROM Appointments a
                                 INNER JOIN Doctors d ON a.DoctorID = d.DoctorID
                                 WHERE a.UserID = @UserId
                                 ORDER BY a.AppointDate DESC, a.ID DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", UserId);

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    rptRecentAppts.DataSource = dt;
                    rptRecentAppts.DataBind();
                    lblNoAppts.Visible = false;
                }
                else
                {
                    lblNoAppts.Visible = true;
                }
            }
        }

        private void LoadFamilyMembers(int UserId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT PatientID, PatientCode, PatientName, DateOfBirth, Age, Gender 
                                 FROM Patients 
                                 WHERE UserID = @UserId AND (IsDeleted = 0 OR IsDeleted IS NULL)
                                 ORDER BY PatientID DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", UserId);

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    rptFamilyMembers.DataSource = dt;
                    rptFamilyMembers.DataBind();
                    lblNoFamily.Visible = false;
                }
                else
                {
                    rptFamilyMembers.DataSource = null;
                    rptFamilyMembers.DataBind();
                    lblNoFamily.Visible = true;
                }
            }
        }

        protected void btnSaveFamily_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            if (string.IsNullOrEmpty(txtFamilyName.Text.Trim()) ||
                string.IsNullOrEmpty(txtFamilyDob.Text.Trim()) ||
                rblFamilyGender.SelectedIndex == -1)
            {
                Session["ErrorMessage"] = "Please fill in all the family member fields.";
                Response.Redirect(Request.RawUrl);
                return;
            }

            DateTime dob;
            if (!DateTime.TryParse(txtFamilyDob.Text.Trim(), out dob))
            {
                Session["ErrorMessage"] = "Please enter a valid Date of Birth.";
                Response.Redirect(Request.RawUrl);
                return;
            }

            int age = DateTime.Today.Year - dob.Year;
            if (dob.Date > DateTime.Today.AddYears(-age))
            {
                age--;
            }
            if (age < 0)
            {
                Session["ErrorMessage"] = "Date of Birth cannot be in the future.";
                Response.Redirect(Request.RawUrl);
                return;
            }

            // Fallback to text box value if valid, otherwise use calculated age
            int parsedAge;
            if (int.TryParse(txtFamilyAge.Text.Trim(), out parsedAge) && parsedAge >= 0)
            {
                age = parsedAge;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Patients (UserID, PatientName, DateOfBirth, Age, Gender, Status, IsDeleted) 
                                 VALUES (@UserId, @Name, @DateOfBirth, @Age, @Gender, 'Active', 0)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Name", txtFamilyName.Text.Trim());
                cmd.Parameters.AddWithValue("@DateOfBirth", txtFamilyDob.Text.Trim());
                cmd.Parameters.AddWithValue("@Age", age);
                cmd.Parameters.AddWithValue("@Gender", rblFamilyGender.SelectedValue);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            Session["SuccessMessage"] = "Family member added successfully!";
            Response.Redirect(Request.RawUrl);
        }

        protected void btnEditFamily_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int patientId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT PatientID, PatientName, DateOfBirth, Age, Gender FROM Patients WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", patientId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtEditFamilyName.Text = reader["PatientName"].ToString();
                    txtEditFamilyDob.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("yyyy-MM-dd");
                    txtEditFamilyAge.Text = reader["Age"].ToString();
                    rblEditFamilyGender.SelectedValue = reader["Gender"].ToString();
                    hdnEditFamilyId.Value = patientId.ToString();
                }
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Popup", "openEditFamilyModal();", true);
        }

        protected void btnUpdateFamily_Click(object sender, EventArgs e)
        {
            int patientId = Convert.ToInt32(hdnEditFamilyId.Value);

            if (string.IsNullOrEmpty(txtEditFamilyName.Text.Trim()) ||
                string.IsNullOrEmpty(txtEditFamilyDob.Text.Trim()) ||
                rblEditFamilyGender.SelectedIndex == -1)
            {
                Session["ErrorMessage"] = "Please fill in all the family member fields.";
                Response.Redirect(Request.RawUrl);
                return;
            }

            DateTime dob;
            if (!DateTime.TryParse(txtEditFamilyDob.Text.Trim(), out dob))
            {
                Session["ErrorMessage"] = "Please enter a valid Date of Birth.";
                Response.Redirect(Request.RawUrl);
                return;
            }

            int age = DateTime.Today.Year - dob.Year;
            if (dob.Date > DateTime.Today.AddYears(-age))
            {
                age--;
            }
            if (age < 0)
            {
                Session["ErrorMessage"] = "Date of Birth cannot be in the future.";
                Response.Redirect(Request.RawUrl);
                return;
            }

            int parsedAge;
            if (int.TryParse(txtEditFamilyAge.Text.Trim(), out parsedAge) && parsedAge >= 0)
            {
                age = parsedAge;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Patients SET PatientName = @Name, DateOfBirth = @Dob, Age = @Age, Gender = @Gender WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", txtEditFamilyName.Text.Trim());
                cmd.Parameters.AddWithValue("@Dob", txtEditFamilyDob.Text.Trim());
                cmd.Parameters.AddWithValue("@Age", age);
                cmd.Parameters.AddWithValue("@Gender", rblEditFamilyGender.SelectedValue);
                cmd.Parameters.AddWithValue("@PatientID", patientId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            Session["SuccessMessage"] = "Family member updated successfully!";
            Response.Redirect(Request.RawUrl);
        }

        protected void btnDeleteFamily_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int patientId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Patients SET IsDeleted = 1 WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", patientId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            Session["SuccessMessage"] = "Family member removed successfully!";
            Response.Redirect(Request.RawUrl);
        }
    }
}