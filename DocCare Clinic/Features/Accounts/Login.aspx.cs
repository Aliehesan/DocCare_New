using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace DocCare_Clinic
{
    public partial class Login : System.Web.UI.Page
    {
        // ─── Connection string from Web.config ────────────────────────────────────
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

        // ─── Page Load ────────────────────────────────────────────────────────────
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user email is already saved via "Remember Me" cookie
                if (Request.Cookies["UserEmail"] != null)
                {
                    txtEmail.Text = Request.Cookies["UserEmail"].Value;
                    chkRememberMe.Checked = true;
                }

                // Check if redirected from AdminDashboard
                if (Session["RedirectFromAdmin"] != null && (bool)Session["RedirectFromAdmin"])
                {
                    signUpParagraph.Visible = false;
                    lnkSignUp.Visible = false;
                    Session.Remove("RedirectFromAdmin");
                }
            }
        }

        // ─── Login Button Click ───────────────────────────────────────────────────
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email          = txtEmail.Text.Trim();
            string password       = txtPassword.Text.Trim();
            string hashedPassword = HashPassword(password);

            string userType;
            string userId = GetUserId(email, hashedPassword, out userType);

            if (!string.IsNullOrEmpty(userId) && !userId.StartsWith("ERROR:"))
            {
                Session["UserID"]   = userId;
                Session["UserType"] = userType;

                // ── Admin ──────────────────────────────────────────────────────────
                if (userType == "Admin")
                {
                    Session["UserRole"] = "SuperAdmin";
                    Session["UserName"] = GetAdminName(email);   // ← uses SP
                    HandleRememberMe(email);
                    Response.Redirect(ResolveUrl("~/Features/Admin/AdminDashBoard.aspx"));
                }

                // ── Doctor ─────────────────────────────────────────────────────────
                else if (userType == "Doctor")
                {
                    string doctorId, doctorName;
                    GetDoctorDetails(email, hashedPassword, out doctorId, out doctorName); // ← uses SP

                    Session["DoctorID"]   = doctorId;
                    Session["DoctorName"] = doctorName;
                    Session["UserRole"]   = "Doctor";
                    HandleRememberMe(email);
                    Response.Redirect(ResolveUrl("~/Features/Doctors/DoctorDashBoard.aspx"));
                }

                // ── Receptionist ───────────────────────────────────────────────────
                else if (userType == "Receptionist")
                {
                    string receptionistId, receptionistName;
                    GetReceptionistDetails(email, hashedPassword, out receptionistId, out receptionistName); // ← uses SP

                    Session["ReceptionistID"]   = receptionistId;
                    Session["ReceptionistName"] = receptionistName;
                    Session["UserRole"]         = "Receptionist";
                    HandleRememberMe(email);
                    Response.Redirect(ResolveUrl("~/Features/Receptionists/ReceptionistDashboard.aspx"));
                }

                // ── Patient / User ─────────────────────────────────────────────────
                else if (userType == "User")
                {
                    string patientId, patientName;
                    GetPatientDetails(email, out patientId, out patientName);  // ← uses SP

                    Session["UserID"]   = patientId;
                    Session["Name"]     = patientName;
                    Session["UserRole"] = "User";
                    HandleRememberMe(email);
                    Response.Redirect(ResolveUrl("~/Features/Site/Default.aspx"));
                }
            }
            else
            {
                // Show error on page
                lblErrorMessage.Text = userId != null && userId.StartsWith("ERROR:")
                    ? "*" + userId
                    : "*Invalid email or password. Please try again.";

                lblErrorMessage.Visible = true;

                string hideScript = @"
                    setTimeout(function() {
                        var el = document.getElementById('" + lblErrorMessage.ClientID + @"');
                        if (el) el.style.display = 'none';
                    }, 5000);";
                ClientScript.RegisterStartupScript(this.GetType(), "HideError", hideScript, true);
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        //  STORED PROCEDURE METHODS
        //  Each method below calls ONE stored procedure — no raw SQL here
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Calls sp_LoginUser — checks all 4 tables and returns UserID + Role.
        /// </summary>
        private string GetUserId(string email, string hashedPassword, out string userType)
        {
            string userId = null;
            userType = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_LoginUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;   // ← KEY LINE
                        cmd.Parameters.AddWithValue("@Email",    email);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userId   = reader[0].ToString();
                                userType = reader[1].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    try { System.IO.File.AppendAllText(Server.MapPath("~/Logs/errors.txt"), ex.ToString()); } catch { }
                    return "ERROR: " + ex.Message;
                }
            }

            return userId;
        }

        /// <summary>
        /// Calls sp_GetAdminName — returns FullName of Admin by Email.
        /// </summary>
        private string GetAdminName(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetAdminName", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);

                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : "Admin";
                }
            }
        }

        /// <summary>
        /// Calls sp_GetDoctorByEmail — returns DoctorID and Name in one DB call.
        /// </summary>
        private void GetDoctorDetails(string email, string hashedPassword,
                                      out string doctorId, out string doctorName)
        {
            doctorId   = string.Empty;
            doctorName = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetDoctorByEmail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email",    email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            doctorId   = reader["DoctorID"].ToString();
                            doctorName = reader["Name"].ToString();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calls sp_GetReceptionistByEmail — returns ReceptionistID and Name in one DB call.
        /// </summary>
        private void GetReceptionistDetails(string email, string hashedPassword,
                                            out string receptionistId, out string receptionistName)
        {
            receptionistId   = string.Empty;
            receptionistName = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetReceptionistByEmail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email",    email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            receptionistId   = reader["ReceptionistID"].ToString();
                            receptionistName = reader["Name"].ToString();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calls sp_GetPatientByEmail — returns UserID and Name for a patient.
        /// </summary>
        private void GetPatientDetails(string email, out string patientId, out string patientName)
        {
            patientId   = string.Empty;
            patientName = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetPatientByEmail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            patientId   = reader["UserID"].ToString();
                            patientName = reader["Name"].ToString();
                        }
                    }
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        //  HELPER METHODS
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Handles the "Remember Me" cookie logic.
        /// </summary>
        private void HandleRememberMe(string email)
        {
            if (chkRememberMe.Checked)
            {
                HttpCookie userCookie = new HttpCookie("UserEmail", email)
                {
                    Expires = DateTime.Now.AddDays(7)
                };
                Response.Cookies.Add(userCookie);
            }
            else
            {
                if (Request.Cookies["UserEmail"] != null)
                    Response.Cookies["UserEmail"].Expires = DateTime.Now.AddDays(-1);
            }
        }

        /// <summary>
        /// Hashes a plain-text password using SHA-256 and returns Base64 string.
        /// </summary>
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash  = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}