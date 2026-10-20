using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace DocCare_Clinic
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RegisterButton_Click(object sender, EventArgs e)
        {            
            // Validate if all fields are filled
            if (string.IsNullOrEmpty(txtName.Text) ||
                string.IsNullOrEmpty(txtEmail.Text) ||
                string.IsNullOrEmpty(txtPassword.Text) ||
                string.IsNullOrEmpty(txtConfirmPassword.Text) ||               
                string.IsNullOrEmpty(txtMobile.Text) ||
                string.IsNullOrEmpty(txtAddress.Text))
                //rblGender.SelectedIndex == -1)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "valFields",
                    "swalAlert('warning', 'Required Fields', 'Please fill in all the fields.');", true);
                return;
            }


            // Validate email format
            if (!IsValidEmail(txtEmail.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "valEmail",
                    "swalAlert('warning', 'Invalid Email', 'Invalid email format.');", true);
                return;
            }

            // Validate password strength
            if (!IsPasswordValid(txtPassword.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "valPwd",
                    "swalAlert('warning', 'Weak Password', 'Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, and one digit.');", true);
                return;
            }

            // Validate password and confirm password match
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "valMatch",
                    "swalAlert('warning', 'Password Mismatch', 'Passwords do not match.');", true);
                return;
            }

            // Validate terms checkbox
            if (!TermsCheckBox.Checked)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "valTerms",
                    "swalAlert('warning', 'Terms Required', 'You must agree to the terms and conditions.');", true);
                return;
            }

            // Hash the password
            string hashedPassword = HashPassword(txtPassword.Text);

            // Save to the patients table in the database
            string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;

            // One account per email (login looks users up by email)
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand check = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", connection))
            {
                check.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                connection.Open();
                if ((int)check.ExecuteScalar() > 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "valDuplicate",
                        "swalAlert('warning', 'Email Already Registered', 'An account with this email already exists. Please log in instead.');", true);
                    return;
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (Name, Mobile, Email, Address, Password) " +
               "VALUES (@Name, @Mobile, @Email, @Address, @Password)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", txtName.Text);
                    command.Parameters.AddWithValue("@Email", txtEmail.Text);
                    command.Parameters.AddWithValue("@Password", hashedPassword); // Store the hashed password
                    //command.Parameters.AddWithValue("@DateOfBirth", txtDob.Text);
                    //command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@Mobile", txtMobile.Text);
                    command.Parameters.AddWithValue("@Address", txtAddress.Text);
                    //command.Parameters.AddWithValue("@Gender", rblGender.SelectedValue);

                    try
                    {
                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "regSuccess",
                                "swalSuccessRedirect('Registration Successful!', 'Your account has been created. Please log in to continue.', '" + ResolveUrl("~/Features/Accounts/Login.aspx") + "');", true);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "regFail",
                                "swalAlert('error', 'Registration Failed', 'Registration failed. Please try again later.');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "regError",
                            "swalAlert('error', 'Error', '" + ex.Message.Replace("'", "\\'") + "');", true);
                    }
                }
            }
        }

        // Function to hash passwords using SHA-256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash); // Convert hash to Base64 for storage
            }
        }

        // Function to validate password
        private bool IsPasswordValid(string password)
        {
            string pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        // Function to validate email
        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            return Regex.IsMatch(email, emailPattern);
        }
    }
}