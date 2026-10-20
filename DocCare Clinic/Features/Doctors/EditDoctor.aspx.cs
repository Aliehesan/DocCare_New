using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    public partial class EditDoctor : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        string userRole = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            userRole = Session["UserRole"] as string;
            if (!IsPostBack)
            {
                BindDepartmentDropdown();
                if (string.IsNullOrEmpty(userRole))
                {
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }

                int doctorId = 0;

                if (userRole == "Doctor")
                {
                    if (Session["DoctorId"] != null)
                    {
                        HideAdminMenus();
                        lblBreadcrumb.Text = "Edit Profile";
                        HyperLink1.NavigateUrl = ResolveUrl("~/Features/Doctors/DoctorDashBoard.aspx");
                        lblDashBoard.Text = "Dashboard";

                        doctorId = Convert.ToInt32(Session["DoctorId"]);
                    }
                    else
                    {
                        Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                    }
                }
                else if (userRole == "SuperAdmin")
                {
                    if (Session["DoctorId"] != null)
                    {
                        lblBreadcrumb.Text = "Doctor Profile";
                        HyperLink1.NavigateUrl = ResolveUrl("~/Features/Doctors/Doctors.aspx");
                        lblDashBoard.Text = "Doctors";
                        doctorId = Convert.ToInt32(Session["DoctorId"]);
                    }
                    else
                    {
                        Response.Redirect(ResolveUrl("~/Features/Doctors/Doctors.aspx"));
                    }
                }

                if (doctorId > 0)
                {
                    LoadDoctorDetails(doctorId);
                }
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
            userRole = Session["UserRole"] as string;

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

        private void BindDepartmentDropdown()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT DepartmentID, Department FROM Departments WHERE Status = 'Active'";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlDepartment.DataSource = reader;
                ddlDepartment.DataTextField = "Department";
                ddlDepartment.DataValueField = "DepartmentID";
                ddlDepartment.DataBind();

                conn.Close();
            }
            ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
        }

        private void LoadDoctorDetails(int doctorId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        d.DoctorId, 
                        d.Name, 
                        d.Mobile, 
                        d.Email, 
                        d.Degree, 
                        d.Specialization, 
                        d.Charges, 
                        ISNULL(d.DepartmentID, 0) AS DepartmentID, 
                        ISNULL(dep.Department, 'N/A') AS Department, 
                        d.JoinDate, 
                        d.Gender,
                        dp.Designation, 
                        dp.Location, 
                        dp.Institute, 
                        dp.Hospital, 
                        dp.Experience, 
                        dp.Year_Of_Completion,
                        ISNULL(d.ImageUrl, '~/assets/img/profiles/default-profile.png') AS ProfileImage
                    FROM 
                        Doctors d
                    LEFT JOIN 
                        Departments dep ON d.DepartmentID = dep.DepartmentID
                    LEFT JOIN 
                        DoctorProfile dp ON d.DoctorId = dp.DoctorId
                    WHERE 
                        d.DoctorId = @DoctorId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtName.Text = reader["Name"].ToString();
                        txtMobile.Text = reader["Mobile"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        txtDegree.Text = reader["Degree"].ToString();
                        txtCharges.Text = reader["Charges"].ToString();
                        txtLocation.Text = reader["Location"].ToString();
                        txtInstitute.Text = reader["Institute"].ToString();
                        txtHospital.Text = reader["Hospital"].ToString();
                        txtExperience.Text = reader["Experience"].ToString();
                        txtYearOfCompletion.Text = reader["Year_Of_Completion"].ToString();
                        imgProfile.Src = reader["ProfileImage"].ToString();
                        imgProfile.Attributes["alt"] = reader["Name"].ToString();

                        if (reader["JoinDate"] != DBNull.Value)
                        {
                            txtJoinDate.Text = Convert.ToDateTime(reader["JoinDate"]).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            txtJoinDate.Text = "";
                        }

                        ListItem itemDesignation = ddlDesignation.Items.FindByValue(reader["Designation"].ToString());
                        if (itemDesignation != null)
                        {
                            ddlDesignation.SelectedValue = reader["Designation"].ToString();
                        }
                        else
                        {
                            ddlDesignation.SelectedIndex = 0;
                        }

                        ListItem item = ddlDepartment.Items.FindByValue(reader["DepartmentID"].ToString());
                        if (item != null)
                        {
                            ddlDepartment.SelectedValue = reader["DepartmentID"].ToString();
                        }
                        else
                        {
                            ddlDepartment.SelectedIndex = 0;
                        }

                        ListItem itemSpecial = ddlSpecialization.Items.FindByText(reader["Specialization"].ToString());
                        if (itemSpecial != null)
                        {
                            ddlSpecialization.SelectedValue = itemSpecial.Value;
                        }
                        else
                        {
                            ddlSpecialization.SelectedIndex = 0;
                        }

                        if (!string.IsNullOrEmpty(reader["Gender"].ToString()))
                        {
                            rblGender.SelectedValue = reader["Gender"].ToString();
                        }
                        else
                        {
                            rblGender.SelectedIndex = -1;
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            userRole = Session["UserRole"] as string;

            if (string.IsNullOrEmpty(userRole))
            {
                Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
            }

            string name = DoctorName.WithoutTitle(txtName.Text); // pages add "Dr. " themselves
            string mobile = txtMobile.Text.Trim();
            string email = txtEmail.Text.Trim();
            string degree = txtDegree.Text.Trim();
            string specialization = ddlSpecialization.SelectedValue;
            string department = ddlDepartment.SelectedValue;
            string chargesText = txtCharges.Text.Trim();
            string joinDateText = txtJoinDate.Text.Trim();
            string gender = rblGender.SelectedValue;
            string designation = ddlDesignation.SelectedValue;
            string location = txtLocation.Text.Trim();
            string institute = txtInstitute.Text.Trim();
            string hospital = txtHospital.Text.Trim();
            string experience = txtExperience.Text.Trim();
            string yearofCompletion = txtYearOfCompletion.Text.Trim();

            decimal charges;
            DateTime joinDate;

            // ✅ Validation Check
            if ((string.IsNullOrEmpty(name) ||
                    string.IsNullOrEmpty(department) ||
                    string.IsNullOrEmpty(specialization) ||
                    string.IsNullOrEmpty(degree) ||
                    string.IsNullOrEmpty(txtExperience.Text) ||
                    string.IsNullOrEmpty(txtHospital.Text) ||
                    string.IsNullOrEmpty(txtInstitute.Text) ||
                    string.IsNullOrEmpty(designation) ||
                    string.IsNullOrEmpty(txtYearOfCompletion.Text) ||
                    string.IsNullOrEmpty(txtLocation.Text) ||
                    string.IsNullOrEmpty(mobile) ||
                    string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(chargesText) ||
                    !decimal.TryParse(chargesText, out charges) ||
                    string.IsNullOrEmpty(joinDateText) ||
                    !DateTime.TryParse(joinDateText, out joinDate) ||
                    string.IsNullOrEmpty(gender)))
            {
                // ✅ Show error message if validation fails
                lblErrorMessage.Text = "*Please fill out all fields correctly.";
                lblErrorMessage.Visible = true;
                return;
            }

            // ✅ Convert Join Date (if not empty)
            if (!DateTime.TryParse(joinDateText, out joinDate))
            {
                joinDate = DateTime.MinValue;
            }
            string imagePath = "";

            // ✅ Handle Image Upload
            if (fileUpload.HasFile)
            {
                // ✔ Upload new image
                string fileName = Path.GetFileName(fileUpload.PostedFile.FileName);
                imagePath = "~/assets/img/profiles/" + fileName;
                fileUpload.SaveAs(Server.MapPath(imagePath));
            }
            else
            {
                // ✔ Keep old image if no new image uploaded
                imagePath = imgProfile.Src;
            }

            // ✅ Get Doctor ID from QueryString
            int doctorId = Convert.ToInt32(Session["DoctorId"]);

            // ✅ Update Query
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string updateQuery = @"
            UPDATE Doctors 
            SET Name = @Name, 
                Mobile = @Mobile, 
                Email = @Email, 
                Degree = @Degree, 
                Specialization = @Specialization, 
                DepartmentID = @DepartmentID, 
                Charges = @Charges, 
                JoinDate = @JoinDate, 
                Gender = @Gender,
                ImageUrl = @Image
            WHERE DoctorId = @DoctorId";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Mobile", mobile);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Degree", degree);
                    cmd.Parameters.AddWithValue("@Specialization", specialization);
                    cmd.Parameters.AddWithValue("@DepartmentID", department);
                    cmd.Parameters.AddWithValue("@Charges", charges);
                    cmd.Parameters.AddWithValue("@Gender", gender);
                    cmd.Parameters.AddWithValue("@Image", imagePath);
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                    // ✅ Handle Join Date (Null or Not)
                    if (string.IsNullOrEmpty(joinDateText))
                    {
                        cmd.Parameters.AddWithValue("@JoinDate", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@JoinDate", joinDate);
                    }

                    // ✅ Execute Query
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Session["DoctorName"] = name;
                    conn.Close();
                }
                string updateDoctorProfileQuery = @"
            IF EXISTS (SELECT 1 FROM DoctorProfile WHERE DoctorId = @DoctorId)
            BEGIN
                UPDATE DoctorProfile
                SET Designation = @Designation, 
                    Location = @Location, 
                    Institute = @Institute, 
                    Hospital = @Hospital, 
                    Experience = @Experience, 
                    Year_Of_Completion = @YearOfCompletion               
                WHERE DoctorId = @DoctorId
            END
            ELSE
            BEGIN
                INSERT INTO DoctorProfile (DoctorId, Designation, Location, Institute, Hospital, Experience, Year_Of_Completion)
                VALUES (@DoctorId, @Designation, @Location, @Institute, @Hospital, @Experience, @YearOfCompletion)
            END";

                using (SqlCommand cmd = new SqlCommand(updateDoctorProfileQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    cmd.Parameters.AddWithValue("@Designation", designation);
                    cmd.Parameters.AddWithValue("@Location", location);
                    cmd.Parameters.AddWithValue("@Institute", institute);
                    cmd.Parameters.AddWithValue("@Hospital", hospital);
                    cmd.Parameters.AddWithValue("@Experience", experience);
                    cmd.Parameters.AddWithValue("@YearOfCompletion", yearofCompletion);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // ✅ Hide Error Message After Success
            lblErrorMessage.Visible = false;

            if (userRole == "Doctor")
            {
                // Redirect Doctor to their Dashboard
                Response.Redirect(ResolveUrl("~/Features/Doctors/DoctorDashBoard.aspx"));
            }
            else if (userRole == "SuperAdmin")
            {
                // Redirect SuperAdmin to the Doctors list page
                Response.Redirect(ResolveUrl("~/Features/Doctors/Doctors.aspx"));
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (userRole == "Doctor")
            {
                // Redirect Doctor to their Dashboard
                Response.Redirect(ResolveUrl("~/Features/Doctors/DoctorDashBoard.aspx"));
            }
            else if (userRole == "SuperAdmin")
            {
                // Redirect SuperAdmin to the Doctors list page
                Response.Redirect(ResolveUrl("~/Features/Doctors/Doctors.aspx"));
            }
        }
    }
}