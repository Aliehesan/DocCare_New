using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    public partial class Confirm_Appointment : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear any stale appointment confirmation messages so they never show on Confirm-Appointment page
                if (Session["SuccessMessage"] != null && Session["SuccessMessage"].ToString().Contains("Appointment Confirmed"))
                {
                    Session["SuccessMessage"] = null;
                }

                // Auto-select logged-in patient's details (if PatientID exists in session)
                if (Session["UserId"] != null)
                {
                    string UserId = Session["UserId"].ToString();
                    LoadPatientsRadioButtonList(UserId);
                    LoadUserDetails(UserId);
                }
                else
                {
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }
                hdnSelectedPatientId.Value = "";
                DateTime today = DateTime.Today;

                // Any date in the booking window; ShowSlotAvailability moves it to the doctor's next free day
                txtAppointmentDate.Text = today.ToString("yyyy-MM-dd");
                txtAppointmentDate.Attributes["min"] = today.ToString("yyyy-MM-dd");
                txtAppointmentDate.Attributes["max"] = today.AddDays(SlotService.BookingWindowDays).ToString("yyyy-MM-dd");
               
                // Load doctor details from query string
                string doctorId = Request.QueryString["doctorId"];
                if (!string.IsNullOrEmpty(doctorId))
                {
                    LoadDoctorDetails(doctorId);
                    ShowSlotAvailability(doctorId);
                }
            }

            // Always ensure booked patient IDs for today are up-to-date
            string docId = Request.QueryString["doctorId"];
            if (!string.IsNullOrEmpty(docId))
            {
                LoadBookedPatientIds(docId);
            }

            // The contact card is the logged-in account holder, whichever family member is selected
            // (the selected value is a PatientID, not a UserID, so it must never be looked up in Users)
            if (IsPostBack && Session["UserId"] != null)
            {
                LoadUserDetails(Session["UserId"].ToString());
            }
        }

        // The patient belongs to the logged-in account (the ID comes from the browser and can be changed there)
        private bool IsOwnPatient(int patientId)
        {
            if (Session["UserId"] == null) return false;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Patients WHERE PatientID = @PatientID AND UserID = @UserID AND ISNULL(IsDeleted, 0) = 0", con))
            {
                cmd.Parameters.AddWithValue("@PatientID", patientId);
                cmd.Parameters.AddWithValue("@UserID", Session["UserId"].ToString());
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Button btnEdit = (Button)sender;
            int patientID;
            if (Session["UserId"] == null || !int.TryParse(btnEdit.CommandArgument, out patientID) || !IsOwnPatient(patientID)) return;
            string UserId = Session["UserId"].ToString();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Patients WHERE PatientID = @PatientID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientID);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtEditName.Text = reader["PatientName"].ToString();
                        txtEditDob.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("yyyy-MM-dd");
                        txtEditAge.Text = reader["Age"].ToString();
                        //txtEditMobile.Text = reader["Mobile"].ToString();
                        //txtEditEmail.Text = reader["Email"].ToString();
                        rblEditGender.SelectedValue = reader["Gender"].ToString();
                        //txtEditAddress.Text = reader["Address"].ToString();
                        hdnSelectedPatientId.Value = patientID.ToString();
                    }
                }
            }
            LoadUserDetails(UserId);
            // Open modal using JavaScript
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Popup", "openEditModal();", true);
        }

        protected void btnUpdatePatient_Click(object sender, EventArgs e)
        {
            int patientID;
            if (Session["UserId"] == null || !int.TryParse(hdnSelectedPatientId.Value, out patientID) || !IsOwnPatient(patientID))
            {
                Session["ErrorMessage"] = "Please choose one of your own patients to edit.";
                Response.Redirect(Request.RawUrl);
                return;
            }
            string UserId = Session["UserId"].ToString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Patients SET PatientName = @Name, DateOfBirth = @Dob, Age = @Age, Gender = @Gender WHERE PatientID = @PatientID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", txtEditName.Text);
                    cmd.Parameters.AddWithValue("@Dob", txtEditDob.Text);
                    cmd.Parameters.AddWithValue("@Age", txtEditAge.Text);
                    //cmd.Parameters.AddWithValue("@Mobile", txtEditMobile.Text);
                    //cmd.Parameters.AddWithValue("@Email", txtEditEmail.Text);
                    cmd.Parameters.AddWithValue("@Gender", rblEditGender.SelectedValue);
                    //cmd.Parameters.AddWithValue("@Address", txtEditAddress.Text);
                    cmd.Parameters.AddWithValue("@PatientID", patientID);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            Session["SuccessMessage"] = "Patient updated successfully!";
            Session["RedirectToDefault"] = "false";
            Session["ErrorMessage"] = null;
            Response.Redirect(Request.RawUrl);
        }

        // Load patients into the radio button list
        private void LoadPatientsRadioButtonList(string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT PatientID, PatientCode, PatientName, Age FROM Patients WHERE UserID = @UserID AND (IsDeleted = 0 OR IsDeleted IS NULL)", con);
                cmd.Parameters.AddWithValue("@UserID", UserId);
                SqlDataReader reader = cmd.ExecuteReader();

                rptPatients.DataSource = reader;
                rptPatients.DataBind();
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Button btnDelete = (Button)sender;
            int patientID = Convert.ToInt32(btnDelete.CommandArgument);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "UPDATE Patients SET IsDeleted = 1 WHERE PatientID = @PatientID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientID);
                    cmd.ExecuteNonQuery();
                }
            }

            Session["SuccessMessage"] = "Patient removed successfully!";
            Session["RedirectToDefault"] = "false";
            Session["ErrorMessage"] = null;
            Response.Redirect(Request.RawUrl);
        }


        private void LoadUserDetails(string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                lblPatientName.Text = lblPhone.Text = lblEmail.Text = lblAddress.Text = "";
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Name, Mobile, Email, Address FROM Users WHERE UserID = @UserID", con);
                cmd.Parameters.AddWithValue("@UserID", UserId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblPatientName.Text = reader["Name"].ToString();
                    lblPhone.Text = reader["Mobile"].ToString();
                    lblEmail.Text = reader["Email"].ToString();
                    lblAddress.Text = reader["Address"].ToString();
                }
                else
                {
                    Error.Text = "Patient not found.";
                    lblPhone.Text = lblEmail.Text = lblAddress.Text = "";
                }
            }
        }

        private void LoadDoctorDetails(string doctorId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Name, Specialization, Charges FROM Doctors WHERE DoctorID = @DoctorID", con);
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblDoctorName.Text = reader["Name"].ToString();
                    lblSpecialty.Text = reader["Specialization"].ToString();

                    decimal totalFee = 0;
                    if (reader["Charges"] != DBNull.Value)
                    {
                        decimal.TryParse(reader["Charges"].ToString(), out totalFee);
                    }

                    decimal advanceFee = Math.Round(totalFee * 0.20m, 2);
                    decimal dueFee = totalFee - advanceFee;
                    long advancePaise = (long)Math.Round(advanceFee * 100m);

                    lblCharges.Text = "&#8377;" + totalFee.ToString("N0");

                    hdnTotalFee.Value = totalFee.ToString("F2");
                    hdnPaidAmount.Value = advanceFee.ToString("F2");
                    hdnDueAmount.Value = dueFee.ToString("F2");
                    hdnAmountPaise.Value = advancePaise.ToString();
                }
                else
                {
                    lblDoctorName.Text = "Doctor not found.";
                    lblSpecialty.Text = lblCharges.Text = "";
                    hdnTotalFee.Value = "0";
                    hdnPaidAmount.Value = "0";
                    hdnDueAmount.Value = "0";
                    hdnAmountPaise.Value = "0";
                }
            }
        }

        // Open the page on the doctor's next day with a free slot (the times themselves are loaded by GetSlots)
        private void ShowSlotAvailability(string doctorId)
        {
            int id;
            if (!int.TryParse(doctorId, out id)) return;

            DateTime? next;
            TimeSpan firstSlot;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                next = SlotService.GetNextAvailableDate(con, id, SlotService.OnlineBufferMinutes, out firstSlot);
            }

            hdnSlotAvailable.Value = next.HasValue ? "1" : "0";
            if (next.HasValue)
            {
                txtAppointmentDate.Text = next.Value.ToString("yyyy-MM-dd");
                lblSlotStatus.Text = "Next available: " + SlotService.FormatDate(next.Value) + " at " + SlotService.Format(firstSlot);
                lblSlotStatus.CssClass = "slot-status slot-ok";
            }
            else
            {
                lblSlotStatus.Text = "This doctor has no free appointments in the next " + SlotService.BookingWindowDays + " days.";
                lblSlotStatus.CssClass = "slot-status slot-none";
            }
        }

        static bool TryParseDate(string value, out DateTime date)
        {
            return DateTime.TryParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
                                          System.Globalization.DateTimeStyles.None, out date);
        }

        // Free times on a date for the "Choose a Time" buttons, plus which of the family is already booked that day
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static object GetSlots(int doctorId, string date)
        {
            DateTime day;
            if (!TryParseDate(date, out day))
                return new { available = false, slots = new object[0], message = "Please choose a valid date.", bookedPatientIds = new string[0] };

            string reason;
            List<TimeSpan> free;
            List<string> booked = new List<string>();
            string cs = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                free = SlotService.GetFreeSlots(con, doctorId, day, SlotService.OnlineBufferMinutes, out reason);
                using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT PatientID FROM Appointments WHERE DoctorID = @DoctorID AND AppointDate = @Date AND Status <> 'Cancelled'", con))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    cmd.Parameters.AddWithValue("@Date", day);
                    using (SqlDataReader r = cmd.ExecuteReader())
                        while (r.Read()) booked.Add(r["PatientID"].ToString());
                }
            }
            return new
            {
                available = free.Count > 0,
                dateLabel = SlotService.FormatDate(day),
                slots = free.Select(t => new { value = t.ToString(@"hh\:mm"), label = SlotService.Format(t) }).ToArray(),
                message = reason,
                bookedPatientIds = booked.ToArray()
            };
        }

        // Called by the Pay button right before payment opens: is the chosen time still free?
        [System.Web.Services.WebMethod(EnableSession = true)]
        public static object CheckSlot(int doctorId, string date, string slot)
        {
            DateTime day;
            TimeSpan time;
            if (!TryParseDate(date, out day) || !TimeSpan.TryParse(slot, out time))
                return new { available = false, message = "Please choose a date and time first." };

            string reason;
            bool free;
            string cs = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                free = SlotService.IsSlotFree(con, doctorId, day, time, SlotService.OnlineBufferMinutes, out reason);
            }
            return new
            {
                available = free,
                slot = SlotService.Format(time),
                dateLabel = SlotService.FormatDate(day),
                message = reason
            };
        }

        private void LoadBookedPatientIds(string doctorId)
        {
            List<string> bookedIds = new List<string>();
            DateTime today;
            if (!TryParseDate(txtAppointmentDate.Text, out today)) today = DateTime.Today;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT DISTINCT PatientID FROM Appointments WHERE DoctorID = @DoctorID AND AppointDate = @Today AND Status <> 'Cancelled'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                cmd.Parameters.AddWithValue("@Today", today);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookedIds.Add(reader["PatientID"].ToString());
                }
            }
            hdnBookedPatientIds.Value = string.Join(",", bookedIds.ToArray());
        }

        protected void btnConfirmAppointment_Click(object sender, EventArgs e)
        {
            string patientId = hdnSelectedPatientId.Value;
            string doctorId = Request.QueryString["doctorId"];
            if (Session["UserId"] == null)
            {
                Session["ErrorMessage"] = "User is not logged in. Please log in again.";
                return;
            }
            string UserId = Session["UserId"].ToString();
            string paymentRef = hdnRazorpayPaymentId.Value;
            try
            {
                int doctor, patient;
                if (!int.TryParse(patientId, out patient) || !int.TryParse(doctorId, out doctor) || !IsOwnPatient(patient))
                {
                    Session["ErrorMessage"] = "Please select a patient.";
                    return;
                }
                DateTime appointDate;
                TimeSpan appointTime;
                if (!TryParseDate(txtAppointmentDate.Text, out appointDate) || !TimeSpan.TryParse(hdnSelectedSlot.Value, out appointTime))
                {
                    Session["ErrorMessage"] = "Please choose a date and time for the appointment.";
                    return;
                }
                LoadUserDetails(UserId);

                string newTokenNo;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Step 1: one appointment per patient with this doctor per day
                    string checkDuplicateQuery = @"
                        SELECT COUNT(*) FROM Appointments
                        WHERE PatientID = @PatientID
                        AND DoctorID = @DoctorID
                        AND AppointDate = @Date
                        AND Status <> 'Cancelled'";  // Ignore cancelled appointments

                    SqlCommand checkDuplicateCmd = new SqlCommand(checkDuplicateQuery, con);
                    checkDuplicateCmd.Parameters.AddWithValue("@PatientID", patientId);
                    checkDuplicateCmd.Parameters.AddWithValue("@DoctorID", doctor);
                    checkDuplicateCmd.Parameters.AddWithValue("@Date", appointDate);
                    if ((int)checkDuplicateCmd.ExecuteScalar() > 0)
                    {
                        Session["ErrorMessage"] = "This patient already has an appointment with this doctor on " + SlotService.FormatDate(appointDate) + ".";
                        return;
                    }

                    // Step 2: the chosen time must still be free (working hours, leave, 45-minute buffer today, max patients)
                    string slotReason;
                    if (!SlotService.IsSlotFree(con, doctor, appointDate, appointTime, SlotService.OnlineBufferMinutes, out slotReason))
                    {
                        // Checked before payment too, so this only happens if it was taken while paying
                        Session["ErrorMessage"] = string.IsNullOrEmpty(paymentRef)
                            ? slotReason
                            : "Sorry, that time was taken while you were paying. Payment reference: " + paymentRef + ". Please contact the clinic for a refund.";
                        return;
                    }

                    // Step 3: token restarts for each doctor each day
                    newTokenNo = SlotService.NextToken(con, null, doctor, appointDate);

                    // Step 4: insert the appointment with the payment details
                    string paymentId = !string.IsNullOrEmpty(paymentRef) ? paymentRef : ("pay_sim_" + DateTime.UtcNow.Ticks.ToString());
                    decimal totalFee = 0;
                    decimal paidAmount = 0;
                    decimal dueAmount = 0;

                    decimal.TryParse(hdnTotalFee.Value, out totalFee);
                    decimal.TryParse(hdnPaidAmount.Value, out paidAmount);
                    decimal.TryParse(hdnDueAmount.Value, out dueAmount);

                    if (totalFee > 0 && paidAmount == 0)
                    {
                        paidAmount = Math.Round(totalFee * 0.20m, 2);
                        dueAmount = totalFee - paidAmount;
                    }

                    string query = @"INSERT INTO Appointments
                        (UserID, PatientID, DoctorID, AppointDate, AppointTime, TokenNo, Status, TotalFee, PaidAmount, DueAmount, PaymentId, PaymentStatus)
                        VALUES
                        (@UserID, @PatientID, @DoctorID, @AppointDate, @AppointTime, @TokenNo, @Status, @TotalFee, @PaidAmount, @DueAmount, @PaymentId, @PaymentStatus);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserID", UserId);
                    cmd.Parameters.AddWithValue("@PatientID", patientId);
                    cmd.Parameters.AddWithValue("@DoctorID", doctor);
                    cmd.Parameters.AddWithValue("@AppointDate", appointDate);
                    cmd.Parameters.AddWithValue("@AppointTime", appointTime);
                    cmd.Parameters.AddWithValue("@TokenNo", newTokenNo);
                    cmd.Parameters.AddWithValue("@Status", "Confirmed");
                    cmd.Parameters.AddWithValue("@TotalFee", totalFee);
                    cmd.Parameters.AddWithValue("@PaidAmount", paidAmount);
                    cmd.Parameters.AddWithValue("@DueAmount", dueAmount);
                    cmd.Parameters.AddWithValue("@PaymentId", paymentId);
                    cmd.Parameters.AddWithValue("@PaymentStatus", "Advance Paid (20%)");

                    int newAppointmentId;
                    try
                    {
                        newAppointmentId = (int)cmd.ExecuteScalar();
                    }
                    catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
                    {
                        // UX_Appointments_ActiveSlot: someone booked this exact time a moment earlier
                        Session["ErrorMessage"] = "Sorry, that time was booked by someone else a moment ago." +
                            (string.IsNullOrEmpty(paymentRef) ? " Please choose another time." : " Payment reference: " + paymentRef + ". Please contact the clinic for a refund.");
                        return;
                    }
                    // Paid online: email the booking details without making the patient wait for Gmail.
                    // If it fails the booking still stands and stays "not notified" on Doctor's Appointments.
                    AppointmentNotifier.SendInBackground(newAppointmentId, AppointmentNotifier.EmailKind.Details);
                }

                // Success message
                decimal finalPaid = 0;
                decimal finalDue = 0;
                decimal.TryParse(hdnPaidAmount.Value, out finalPaid);
                decimal.TryParse(hdnDueAmount.Value, out finalDue);

                Session["SuccessMessage"] = "Appointment Confirmed for " + SlotService.FormatDate(appointDate) + " at " + SlotService.Format(appointTime) +
                    " (token " + newTokenNo + ")! &#8377;" + finalPaid.ToString("N0") + " advance paid online. Remaining &#8377;" + finalDue.ToString("N0") +
                    " is payable at the clinic. The appointment details are being emailed to you.";
                Session["ErrorMessage"] = null;
                Session["RedirectToDefault"] = "false";
                Response.Redirect(ResolveUrl("~/Features/Appointments/MyAppointment.aspx"));
            }
            catch (System.Threading.ThreadAbortException)
            {
                // Ignore thread abort on Response.Redirect
            }
            catch (Exception ex)
            {
                Session["ErrorMessage"] = "Error: " + ex.Message.Replace("'", "");
                LoadUserDetails(UserId);
            }
        }

        protected void btnSaveDependent_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Text = ""; // Clear previous error messages
            lblErrorMessage.Visible = false;
            string doctorId = Request.QueryString["doctorId"];
            if (!string.IsNullOrEmpty(doctorId))
            {
                LoadDoctorDetails(doctorId);
            }

            if (Session["UserId"] == null)
            {
                Session["ErrorMessage"] = "User is not logged in. Please log in again.";
                return;
            }


            string UserId = Session["UserId"].ToString();

            if (string.IsNullOrEmpty(txtName.Text.Trim()) ||
                string.IsNullOrEmpty(txtDob.Text.Trim()) ||
                rblGender.SelectedIndex == -1)
            {
                Session["ErrorMessage"] = "Please fill in all the fields.";
                return;
            }

            DateTime dob;
            if (!DateTime.TryParse(txtDob.Text.Trim(), out dob))
            {
                Session["ErrorMessage"] = "Please enter a valid Date of Birth.";
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
                return;
            }

            int parsedAge;
            if (int.TryParse(txtAge.Text.Trim(), out parsedAge) && parsedAge >= 0)
            {
                age = parsedAge;
            }

            bool isSuccess = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Patients (UserID, DoctorID, PatientName, DateOfBirth, Age, Gender, Status) " +
                               "VALUES (@UserId, @doctorId, @Name, @DateOfBirth, @Age, @Gender,'Active')";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", UserId);
                    command.Parameters.AddWithValue("@DoctorID", doctorId);
                    command.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    command.Parameters.AddWithValue("@DateOfBirth", txtDob.Text.Trim());
                    command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@Gender", rblGender.SelectedValue);

                    try
                    {
                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            isSuccess = true;
                            Session["SuccessMessage"] = "Patient added successfully!";
                            Session["RedirectToDefault"] = "false";
                            Session["ErrorMessage"] = null;
                        }
                        else
                        {
                            Session["ErrorMessage"] = "Add failed. Please try again later.";
                            Session["SuccessMessage"] = null;
                        }
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                        // Ignore thread abort during redirection
                    }
                    catch (Exception ex)
                    {
                        Session["ErrorMessage"] = "Error: " + ex.Message;
                        Session["SuccessMessage"] = null;
                    }
                }
            }

            if (isSuccess)
            {
                Response.Redirect(Request.RawUrl);
            }
        }

        // ✅ **Function to Hide Error Message After 5 Seconds**
        private void HideErrorMessage()
        {
            string script = @"
        setTimeout(function() {
            var errorLabel = document.getElementById('" + lblErrorMessage.ClientID + @"');
            if (errorLabel) {
                errorLabel.style.display = 'none';
            }
        }, 5000);";
            ClientScript.RegisterStartupScript(this.GetType(), "HideErrorMessage", script, true);
        }

        // Function to validate email
        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            return Regex.IsMatch(email, emailPattern);
        }
    }
}