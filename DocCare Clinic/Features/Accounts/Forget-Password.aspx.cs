using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using TaskManagement.config;
using System.Security.Cryptography;
using System.Text;

namespace DocCare_Clinic
{
    public partial class Forget_Password : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // ── Step 1: Send OTP ──────────────────────────────────────────────────────
        protected void btnSendOtp_Click(object sender, EventArgs e)
        {
            string enteredEmail = txtEmail.Text.Trim();

            if (checkEmailExist(enteredEmail))
            {
                // Store email in Session so it's available across all steps
                Session["ResetEmail"] = enteredEmail;

                // Generate OTP and store generation time in Session
                string otp = OTPService.GenerateOTP();
                Session["OTP"]            = otp;
                Session["OTPGeneratedAt"] = DateTime.Now;  // ✅ Store time for expiry check

                // Store OTP in HiddenField as backup
                hfOtp.Value = otp;

                // Send OTP email using the beautiful template
                EmailService emailService = new EmailService();
                string result = emailService.SendEmail(
                    enteredEmail,
                    "🏥 DocCare Clinic – Your Password Reset OTP",
                    EmailService.BuildOtpEmail(otp));

                if (result == "1")
                {
                    // Success — move to OTP verification view
                    mvForget.ActiveViewIndex = 1;
                }
                else
                {
                    // Email sending failed — show the actual error
                    lblErrorMessage.Text = "Failed to send OTP. Error: " + result;
                }
            }
            else
            {
                lblErrorMessage.Text = "This email address is not registered in our system.";
            }
        }

        // ── Step 2: Verify OTP ────────────────────────────────────────────────────
        protected void btnVerifyOtp_Click(object sender, EventArgs e)
        {
            string storedOtp      = Session["OTP"] != null ? Session["OTP"].ToString() : "";
            string userInputOtp   = txtOtp.Text.Trim();
            DateTime generatedAt  = Session["OTPGeneratedAt"] != null
                                        ? (DateTime)Session["OTPGeneratedAt"]
                                        : DateTime.MinValue;

            // ✅ Use DateTime.Now (not UtcNow) — consistent with how OTP time was stored
            if (OTPService.VerifyOTP(storedOtp, userInputOtp, generatedAt))
            {
                mvForget.ActiveViewIndex = 2;
            }
            else
            {
                lblErrorMessage1.Text = "OTP is incorrect or has expired. Please try again.";
            }
        }

        // ── Step 3: Change Password ───────────────────────────────────────────────
        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                lblErrorMessage2.Text = "Password and Confirm Password must match.";
                return;
            }

            // ✅ Read email from Session (NOT txtEmail.Text — that field is in View 0 and is cleared)
            string email          = Session["ResetEmail"] != null ? Session["ResetEmail"].ToString() : "";
            string hashedPassword = HashPassword(txtNewPassword.Text);

            if (string.IsNullOrEmpty(email))
            {
                lblErrorMessage2.Text = "Session expired. Please start over.";
                mvForget.ActiveViewIndex = 0;
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                bool passwordUpdated = false;

                // Update Admin
                SqlCommand cmdAdmin = new SqlCommand("UPDATE Admin SET Password=@pass WHERE Email=@email", conn);
                cmdAdmin.Parameters.AddWithValue("@pass",  hashedPassword);
                cmdAdmin.Parameters.AddWithValue("@email", email);
                if (cmdAdmin.ExecuteNonQuery() > 0) passwordUpdated = true;

                // Update Doctor (if not already updated)
                if (!passwordUpdated)
                {
                    SqlCommand cmdDoctor = new SqlCommand("UPDATE Doctors SET Password=@pass WHERE Email=@email", conn);
                    cmdDoctor.Parameters.AddWithValue("@pass",  hashedPassword);
                    cmdDoctor.Parameters.AddWithValue("@email", email);
                    if (cmdDoctor.ExecuteNonQuery() > 0) passwordUpdated = true;
                }

                // Update Receptionist
                if (!passwordUpdated)
                {
                    SqlCommand cmdRec = new SqlCommand("UPDATE Receptionists SET Password=@pass WHERE Email=@email", conn);
                    cmdRec.Parameters.AddWithValue("@pass",  hashedPassword);
                    cmdRec.Parameters.AddWithValue("@email", email);
                    if (cmdRec.ExecuteNonQuery() > 0) passwordUpdated = true;
                }

                // Update User/Patient
                if (!passwordUpdated)
                {
                    SqlCommand cmdUser = new SqlCommand("UPDATE Users SET Password=@pass WHERE Email=@email", conn);
                    cmdUser.Parameters.AddWithValue("@pass",  hashedPassword);
                    cmdUser.Parameters.AddWithValue("@email", email);
                    if (cmdUser.ExecuteNonQuery() > 0) passwordUpdated = true;
                }

                if (passwordUpdated)
                {
                    // Clear OTP session data
                    Session.Remove("OTP");
                    Session.Remove("OTPGeneratedAt");
                    Session.Remove("ResetEmail");
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }
                else
                {
                    lblErrorMessage2.Text = "Email not found in our system.";
                }
            }
        }

        // ── Email Existence Check ─────────────────────────────────────────────────
        public bool checkEmailExist(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // ✅ Added Admin table — was missing before!
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT Email FROM Admin        WHERE Email = @email
                    UNION
                    SELECT Email FROM Users        WHERE Email = @email
                    UNION
                    SELECT Email FROM Doctors      WHERE Email = @email
                    UNION
                    SELECT Email FROM Receptionists WHERE Email = @email", conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        return sdr.Read();
                    }
                }
            }
        }

        // ── Hash Password (SHA-256) ───────────────────────────────────────────────
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