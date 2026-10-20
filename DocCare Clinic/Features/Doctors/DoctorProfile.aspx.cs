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
    public partial class DoctorProfile : System.Web.UI.Page
    {
        private string doctorDesignation = "";
        string email = "";
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

                int doctorId = 0;

                if (userRole == "Doctor")
                {
                    HideAdminMenus();
                    lblBreadcrumb.Text = "My Profile";
                    HyperLink1.NavigateUrl = ResolveUrl("~/Features/Doctors/DoctorDashBoard.aspx");
                    lblDashBoard.Text = "Dashboard";

                    if (Session["DoctorId"] != null)
                    {
                        doctorId = Convert.ToInt32(Session["DoctorId"]);
                    }
                    else
                    {
                        Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                    }
                }
                else if (userRole == "Receptionist")
                {
                    lblBreadcrumb.Text = "Doctor Profile";
                    HyperLink1.NavigateUrl = ResolveUrl("~/Features/Doctors/Doctors.aspx");
                    lblDashBoard.Text = "Doctors";

                    if (Request.QueryString["DoctorId"] != null)
                    {
                        doctorId = Convert.ToInt32(Request.QueryString["DoctorId"]);
                    }
                    else
                    {
                        Response.Redirect(ResolveUrl("~/Features/Doctors/Doctors.aspx"));
                    }
                }
                else
                {
                    lblBreadcrumb.Text = "Doctor Profile";
                    HyperLink1.NavigateUrl = ResolveUrl("~/Features/Doctors/Doctors.aspx");
                    lblDashBoard.Text = "Doctors";

                    if (Session["DoctorId"] != null)
                    {
                        doctorId = Convert.ToInt32(Session["DoctorId"]);
                    }
                    else
                    {
                        Response.Redirect(ResolveUrl("~/Features/Doctors/Doctors.aspx"));
                    }
                }

                // Load profile only if doctorId is found
                if (doctorId > 0)
                {
                    LoadDoctorProfile(doctorId);
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

        private void LoadDoctorProfile(int doctorID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT d.Name, d.Gender, d.Specialization, d.Mobile , d.Email , d.Degree , d.ImageUrl, 
                       dp.AboutMe, dp.Designation, dp.Location, dp.Year_of_Completion, dp.Experience, dp.Hospital, dp.Institute
                FROM Doctors d
                INNER JOIN DoctorProfile dp ON d.DoctorID = dp.DoctorID
                WHERE d.DoctorID = @DoctorID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@DoctorID", doctorID);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        lblDoctorName.Text = reader["Name"].ToString();
                        lblFullName.Text = reader["Name"].ToString();
                        lblGender.Text = reader["Gender"].ToString();
                        lblSpecialization.Text = reader["Specialization"].ToString();
                        lblMobile.Text = reader["Mobile"].ToString();
                        email = reader["Email"].ToString();
                        lblDegree.Text = reader["Degree"].ToString();
                        lblAboutMe.Text = reader["AboutMe"].ToString();
                        doctorDesignation = reader["Designation"].ToString();
                        lblLocation.Text = reader["Location"].ToString();
                        lblEducation.Text = reader["Year_of_Completion"].ToString();
                        lblExperience.Text = reader["Experience"].ToString();
                        lblHospital.Text = reader["Hospital"].ToString();
                        lblInstitute.Text = reader["Institute"].ToString();
                        string imagePath = reader["ImageUrl"].ToString();
                        imgDoctor.ImageUrl = string.IsNullOrEmpty(imagePath) ? "~/assets/img/profiles/" : imagePath;
                    }
                    conn.Close();
                    emailLink.Attributes["href"] = "mailto:" + email;
                    lblEmail.Text = email;
                    lblDesignation.Text = doctorDesignation;
                    lblDesignation1.Text = doctorDesignation;
                    lblDesignation2.Text = doctorDesignation;
                }
            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
            }
        }
    }
}