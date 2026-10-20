using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    public partial class BookAppointment : System.Web.UI.Page
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

                if (userRole == "Receptionist")
                {
                    HideAdminMenus(); // Hide Super Admin menus                                                     
                }
                txtAppointDate.Text = DateTime.Now.ToString("yyyy-MM-dd"); // Set today's date
                BindDoctorsWithSchedule();
            }           
        }

        // Staff member at the desk, from the IDs set at login (see Login.aspx.cs)
        private int? GetBookedBy(out string role)
        {
            role = Session["UserRole"] as string;
            string idKey = role == "Receptionist" ? "ReceptionistID"
                         : role == "Doctor" ? "DoctorID"
                         : role == "SuperAdmin" ? "UserID"
                         : null;

            int id;
            if (idKey != null && Session[idKey] != null && int.TryParse(Session[idKey].ToString(), out id))
                return id;

            role = null;
            return null;
        }

        private void HideAdminMenus()
        {
            // Find menu controls inside MasterPage
            Control menuReceptionist = Master.FindControl("menuReceptionist");
            Control menuDepartment = Master.FindControl("menuDepartment");

            if (menuReceptionist != null) menuReceptionist.Visible = false;
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

        private void BindDoctorsWithSchedule()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Doctors working today (weekly hours) and not on leave today; weekday 0 = Sunday (1900-01-07 was a Sunday)
                string query = @"
            SELECT d.DoctorID, d.Name
            FROM Doctors d
            INNER JOIN DoctorWorkingHours w ON w.DoctorID = d.DoctorID AND w.IsWorking = 1
                                           AND w.DayOfWeek = DATEDIFF(DAY, '19000107', CAST(GETDATE() AS DATE)) % 7
            WHERE d.IsDeleted = 0
              AND NOT EXISTS (SELECT 1 FROM DoctorLeave l WHERE l.DoctorID = d.DoctorID AND l.LeaveDate = CAST(GETDATE() AS DATE))
            ORDER BY d.Name";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlConsultDoctor.DataSource = dt;
                ddlConsultDoctor.DataTextField = "Name";
                ddlConsultDoctor.DataValueField = "DoctorID";
                ddlConsultDoctor.DataBind();

                ddlConsultDoctor.Items.Insert(0, new ListItem("-- Select Doctor --", "0"));
            }
        }

        // Walk-in: patient is already at the desk, so SlotService uses the short walk-in buffer.
        // Returns "hh:mm:ss", or null with a reason the receptionist can read.
        private string GetNextAvailableSlot(int doctorID, out string reason)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                TimeSpan? slot = SlotService.GetNextAvailableSlot(con, doctorID, SlotService.WalkInBufferMinutes, out reason);
                return slot.HasValue ? slot.Value.ToString(@"hh\:mm\:ss") : null;
            }
        }

        // Show the next free slot as soon as a doctor is picked, before patient details are typed in
        protected void ddlConsultDoctor_SelectedIndexChanged(object sender, EventArgs e)
        {
            int doctorID;
            if (!int.TryParse(ddlConsultDoctor.SelectedValue, out doctorID) || doctorID == 0)
            {
                lblSlotInfo.Text = "";
                return;
            }

            string reason;
            string slot = GetNextAvailableSlot(doctorID, out reason);
            if (slot != null)
            {
                lblSlotInfo.Text = "Next available slot: " + SlotService.Format(TimeSpan.Parse(slot));
                lblSlotInfo.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblSlotInfo.Text = reason;
                lblSlotInfo.ForeColor = System.Drawing.Color.Red;
            }
        }

        // ── Returning patient ─────────────────────────────────────────────────

        const string PatientColumns = @"p.PatientID, p.PatientCode, p.PatientName, p.Gender, p.Age, p.DateOfBirth,
                                        COALESCE(NULLIF(p.Mobile, ''), u.Mobile) AS Mobile, COALESCE(NULLIF(p.Address, ''), u.Address) AS Address";

        // By Patient ID (exact) or by mobile number (every active patient on that number, e.g. a family)
        protected void btnFindPatient_Click(object sender, EventArgs e)
        {
            string text = txtFindPatient.Text.Trim();
            rptMatches.DataSource = null;
            rptMatches.DataBind();
            int? id = PatientCode.Parse(text);
            string mobile = new string(text.Where(char.IsDigit).ToArray());
            if (!id.HasValue && mobile.Length < 6)
            {
                ShowFindResult("Type a Patient ID like DC-00029, or the mobile number.", false);
                return;
            }

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT " + PatientColumns + @" FROM Patients p LEFT JOIN Users u ON u.UserID = p.UserID
                WHERE ISNULL(p.IsDeleted, 0) = 0
                  AND (p.PatientID = @ID OR (@ID IS NULL AND (p.Mobile = @Mobile OR u.Mobile = @Mobile)))
                ORDER BY p.PatientName", con))
            {
                cmd.Parameters.AddWithValue("@ID", id.HasValue ? (object)id.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Mobile", mobile);
                new SqlDataAdapter(cmd).Fill(dt);
            }

            if (dt.Rows.Count == 0)
                ShowFindResult("No patient found for " + HttpUtility.HtmlEncode(text) + ". Fill in the details below to register a new patient.", false);
            else if (dt.Rows.Count == 1)
                UsePatient(dt.Rows[0]);
            else
            {
                ShowFindResult(dt.Rows.Count + " patients use this number. Choose one:", true);
                rptMatches.DataSource = dt;
                rptMatches.DataBind();
            }
        }

        protected void rptMatches_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            DataRow row = e.CommandName == "Pick" && int.TryParse(e.CommandArgument.ToString(), out id) ? LoadPatient(id) : null;
            if (row != null) UsePatient(row);
        }

        protected void lnkNewPatient_Click(object sender, EventArgs e)
        {
            hdnExistingPatientID.Value = "";
            pnlReturning.Visible = false;
            pnlFind.Visible = true;
            txtFindPatient.Text = txtName.Text = txtDob.Text = txtAge.Text = txtMobile.Text = txtAddress.Text = "";
            rbMale.Checked = rbFemale.Checked = false;
            SetPersonFieldsLocked(false);
            ShowFindResult("", false);
        }

        DataRow LoadPatient(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT " + PatientColumns +
                " FROM Patients p LEFT JOIN Users u ON u.UserID = p.UserID WHERE p.PatientID = @ID AND ISNULL(p.IsDeleted, 0) = 0", con))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                new SqlDataAdapter(cmd).Fill(dt);
            }
            return dt.Rows.Count == 1 ? dt.Rows[0] : null;
        }

        // Fills the form from the record; name, birth date and gender belong to the record and are not edited here
        void UsePatient(DataRow p)
        {
            hdnExistingPatientID.Value = p["PatientID"].ToString();
            litReturningName.Text = HttpUtility.HtmlEncode(p["PatientName"]);
            litReturningCode.Text = HttpUtility.HtmlEncode(p["PatientCode"]);
            txtName.Text = p["PatientName"].ToString();
            txtDob.Text = p["DateOfBirth"] == DBNull.Value ? "" : Convert.ToDateTime(p["DateOfBirth"]).ToString("yyyy-MM-dd");
            txtAge.Text = p["Age"].ToString();
            rbMale.Checked = p["Gender"].ToString() == "Male";
            rbFemale.Checked = p["Gender"].ToString() == "Female";
            txtMobile.Text = p["Mobile"].ToString();
            txtAddress.Text = p["Address"].ToString();
            SetPersonFieldsLocked(true);
            pnlFind.Visible = false;
            pnlReturning.Visible = true;
            rptMatches.DataSource = null;
            rptMatches.DataBind();
        }

        void SetPersonFieldsLocked(bool locked)
        {
            txtName.ReadOnly = txtDob.ReadOnly = locked;
            rbMale.Enabled = rbFemale.Enabled = !locked;
        }

        void ShowFindResult(string html, bool ok)
        {
            lblFindResult.Text = html;
            lblFindResult.ForeColor = ok ? System.Drawing.Color.FromArgb(0x06, 0x5f, 0x46) : System.Drawing.Color.FromArgb(0x9a, 0x34, 0x12);
        }

        // The patient to book: the one found above, else an existing record with the same name and mobile, else none (new)
        int? ExistingPatientId(SqlConnection con, SqlTransaction tx, string name, string mobile)
        {
            int id;
            if (int.TryParse(hdnExistingPatientID.Value, out id) && LoadPatient(id) != null) return id;
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(mobile)) return null;
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1 p.PatientID FROM Patients p LEFT JOIN Users u ON u.UserID = p.UserID
                WHERE ISNULL(p.IsDeleted, 0) = 0 AND LTRIM(RTRIM(p.PatientName)) = @Name
                  AND (p.Mobile = @Mobile OR u.Mobile = @Mobile)
                ORDER BY p.PatientID DESC", con, tx))
            {
                cmd.Parameters.AddWithValue("@Name", name.Trim());
                cmd.Parameters.AddWithValue("@Mobile", mobile.Trim());
                object found = cmd.ExecuteScalar();
                return found == null ? (int?)null : Convert.ToInt32(found);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string patientName = txtName.Text;
            string gender = rbMale.Checked ? "Male" : "Female";
            string mobile = txtMobile.Text;
            string address = txtAddress.Text;
            int doctorID = Convert.ToInt32(ddlConsultDoctor.SelectedValue);
            DateTime today = DateTime.Today;

            if (doctorID == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "noDoctor",
                    "swalAlert('warning', 'Select Doctor', 'Please select a consulting doctor.');", true);
                return;
            }

            // Fetch next available slot
            string slotReason;
            string appointTime = GetNextAvailableSlot(doctorID, out slotReason);
            if (appointTime == null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "noSlots",
                    "swalAlert('warning', 'No Slots Available', '" + slotReason.Replace("'", "\\'") + "');", true);
                return;
            }

            // Validate and parse Date of Birth
            DateTime dob;
            if (!DateTime.TryParse(txtDob.Text, out dob))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "invalidDob",
                    "swalAlert('error', 'Invalid Date', 'Invalid Date of Birth. Please enter a valid date.');", true);
                return;
            }

            // Calculate age
            int age = DateTime.Now.Year - dob.Year;
            if (DateTime.Now.Date < dob.Date.AddYears(age))
            {
                age--; // Adjust for birthdays later in the year
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // A returning patient keeps their record (and Patient ID); only the contact details are refreshed
                    int patientID;
                    int? existing = ExistingPatientId(con, transaction, patientName, mobile);
                    if (existing.HasValue)
                    {
                        patientID = existing.Value;
                        SqlCommand updateCmd = new SqlCommand(@"
            UPDATE Patients SET Mobile = COALESCE(NULLIF(@Mobile, ''), Mobile), Address = COALESCE(NULLIF(@Address, ''), Address)
            WHERE PatientID = @PatientID", con, transaction);
                        updateCmd.Parameters.AddWithValue("@Mobile", mobile.Trim());
                        updateCmd.Parameters.AddWithValue("@Address", address.Trim());
                        updateCmd.Parameters.AddWithValue("@PatientID", patientID);
                        updateCmd.ExecuteNonQuery();
                        patientName = Convert.ToString(new SqlCommand("SELECT PatientName FROM Patients WHERE PatientID = " + patientID, con, transaction).ExecuteScalar());
                    }
                    else
                    {
                        string patientQuery = @"
            INSERT INTO Patients (PatientName, Gender, Mobile, Address, DateOfBirth, Age, Status, IsDeleted)
            VALUES (@PatientName, @Gender, @Mobile, @Address, @DOB, @Age, @Status, 0);
            SELECT SCOPE_IDENTITY();";
                        SqlCommand patientCmd = new SqlCommand(patientQuery, con, transaction);
                        patientCmd.Parameters.AddWithValue("@PatientName", patientName);
                        patientCmd.Parameters.AddWithValue("@Gender", gender);
                        patientCmd.Parameters.AddWithValue("@Mobile", mobile);
                        patientCmd.Parameters.AddWithValue("@Address", address);
                        patientCmd.Parameters.AddWithValue("@DOB", dob);
                        patientCmd.Parameters.AddWithValue("@Age", age);
                        patientCmd.Parameters.AddWithValue("@Status", "Active");

                        patientID = Convert.ToInt32(patientCmd.ExecuteScalar());
                    }

                    // Next token for this doctor today (same numbering as online bookings)
                    string nextToken = SlotService.NextToken(con, transaction, doctorID, DateTime.Today);


                    // Convert appointTime to TimeSpan
                    TimeSpan appointTimeSpan;
                    if (!TimeSpan.TryParse(appointTime, out appointTimeSpan))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "invalidSlot",
                            "swalAlert('error', 'Invalid Time Slot', 'Invalid time slot format: " + appointTime.Replace("'", "\\'") + ".');", true);
                        return;
                    }

                    // Insert appointment
                    string appointmentQuery = @"
            INSERT INTO Appointments (PatientID, DoctorID, TokenNo, AppointDate, AppointTime, Status, BookedByID, BookedByRole,
                                      TotalFee, PaidAmount, DueAmount)
            SELECT @PatientID, @DoctorID, @TokenNo, @AppointDate, @AppointTime, 'Pending', @BookedByID, @BookedByRole,
                   -- walk-ins pay the doctor's fee in full at the clinic; stored now so reports keep today's fee
                   ISNULL(d.Charges, 0), 0, ISNULL(d.Charges, 0)
            FROM Doctors d WHERE d.DoctorID = @DoctorID";
                    SqlCommand appointmentCmd = new SqlCommand(appointmentQuery, con, transaction);
                    appointmentCmd.Parameters.AddWithValue("@PatientID", patientID);
                    appointmentCmd.Parameters.AddWithValue("@DoctorID", doctorID);
                    appointmentCmd.Parameters.AddWithValue("@TokenNo", nextToken);
                    appointmentCmd.Parameters.AddWithValue("@AppointDate", today); // Pass as DATE
                    appointmentCmd.Parameters.AddWithValue("@AppointTime", appointTimeSpan); // Pass as TIME

                    // Record which staff member booked this walk-in
                    string bookedByRole;
                    int? bookedByID = GetBookedBy(out bookedByRole);
                    appointmentCmd.Parameters.AddWithValue("@BookedByID", (object)bookedByID ?? DBNull.Value);
                    appointmentCmd.Parameters.AddWithValue("@BookedByRole", (object)bookedByRole ?? DBNull.Value);

                    appointmentCmd.ExecuteNonQuery();
                    transaction.Commit();

                    // Confirm with the booking details, then go to the Appointments list
                    string details = HttpUtility.HtmlEncode(patientName) + " is booked with <b>" +
                                     HttpUtility.HtmlEncode(ddlConsultDoctor.SelectedItem.Text) + "</b><br/>" +
                                     "Patient ID: <b>" + PatientCode.Of(patientID) + "</b> &nbsp;&middot;&nbsp; " +
                                     "Time: <b>" + SlotService.Format(appointTimeSpan) + "</b> &nbsp;&middot;&nbsp; Token: <b>" +
                                     HttpUtility.HtmlEncode(nextToken) + "</b>";
                    string successScript =
                        "Swal.fire({ icon: 'success', title: 'Appointment Booked!', html: '" + HttpUtility.JavaScriptStringEncode(details) + "'," +
                        " timer: 3500, timerProgressBar: true, confirmButtonText: 'Go to Appointments', confirmButtonColor: '#2E37A4'," +
                        " allowOutsideClick: false, customClass: { popup: 'swal-modern-popup' } })" +
                        ".then(function () { window.location.href = '" + ResolveUrl("~/Features/Appointments/Appointments.aspx") + "'; });";
                    ClientScript.RegisterStartupScript(this.GetType(), "bookSuccess", successScript, true);
                }
                catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
                {
                    // UX_Appointments_ActiveSlot: an online patient took this exact time a moment earlier
                    if (transaction.Connection != null)
                    {
                        transaction.Rollback();
                    }
                    ClientScript.RegisterStartupScript(this.GetType(), "bookTaken",
                        "swalAlert('warning', 'Time Just Taken', 'That time was booked a moment ago. Please press Submit again for the next free time.');", true);
                }
                catch (Exception ex)
                {
                    if (transaction.Connection != null)
                    {
                        transaction.Rollback();
                    }
                    ClientScript.RegisterStartupScript(this.GetType(), "bookError",
                        "swalAlert('error', 'Booking Error', '" + ex.Message.Replace("'", "\\'") + "');", true);
                }
            }
        }

        private void clear()
        {
            txtName.Text = "";
            txtDob.Text = "";
            txtAge.Text = "";
            txtMobile.Text = "";
            txtAddress.Text = "";
            txtAppointDate.Text = "";
            ddlConsultDoctor.SelectedValue = "0"; // "-- Select Doctor --"
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Features/Appointments/Appointments.aspx"));
        }
    }
}