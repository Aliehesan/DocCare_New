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
                // Today by default; any day in the booking window for phone bookings and follow-ups
                txtAppointDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
                txtAppointDate.Attributes["max"] = DateTime.Today.AddDays(SlotService.BookingWindowDays).ToString("yyyy-MM-dd");
                txtAppointDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                PrefillFromLink();
                BindDoctorsWithSchedule();
                LoadTimes();
            }
        }

        // "Book" from the Follow-ups list: ?patientId=&doctorId=&date= fill in the patient, doctor and day
        void PrefillFromLink()
        {
            int id;
            DataRow p = int.TryParse(Request.QueryString["patientId"], out id) ? LoadPatient(id) : null;
            if (p != null) UsePatient(p);
            DateTime day;
            if (DateTime.TryParse(Request.QueryString["date"], out day))
                txtAppointDate.Text = ClampDate(day).ToString("yyyy-MM-dd");
            ViewState["PrefillDoctor"] = Request.QueryString["doctorId"];
        }

        static DateTime ClampDate(DateTime day)
        {
            if (day.Date < DateTime.Today) return DateTime.Today;
            DateTime last = DateTime.Today.AddDays(SlotService.BookingWindowDays);
            return day.Date > last ? last : day.Date;
        }

        DateTime ChosenDate
        {
            get { DateTime d; return DateTime.TryParse(txtAppointDate.Text, out d) ? ClampDate(d) : DateTime.Today; }
        }

        protected void txtAppointDate_TextChanged(object sender, EventArgs e)
        {
            txtAppointDate.Text = ChosenDate.ToString("yyyy-MM-dd");
            BindDoctorsWithSchedule();
            LoadTimes();
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
            string keep = ViewState["PrefillDoctor"] as string ?? ddlConsultDoctor.SelectedValue;
            ViewState["PrefillDoctor"] = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Doctors working on the chosen day (weekly hours) and not on leave then; weekday 0 = Sunday (1900-01-07 was a Sunday)
                string query = @"
            SELECT d.DoctorID, d.Name
            FROM Doctors d
            INNER JOIN DoctorWorkingHours w ON w.DoctorID = d.DoctorID AND w.IsWorking = 1
                                           AND w.DayOfWeek = DATEDIFF(DAY, '19000107', @Day) % 7
            WHERE d.IsDeleted = 0
              AND NOT EXISTS (SELECT 1 FROM DoctorLeave l WHERE l.DoctorID = d.DoctorID AND l.LeaveDate = @Day)
            ORDER BY d.Name";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.Add("@Day", SqlDbType.Date).Value = ChosenDate;
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlConsultDoctor.Items.Clear();
                ddlConsultDoctor.DataSource = dt;
                ddlConsultDoctor.DataTextField = "Name";
                ddlConsultDoctor.DataValueField = "DoctorID";
                ddlConsultDoctor.DataBind();

                ddlConsultDoctor.Items.Insert(0, new ListItem(dt.Rows.Count == 0 ? "-- No doctor works on this day --" : "-- Select Doctor --", "0"));
                // keep the doctor when they also work on the new day
                if (keep != null && ddlConsultDoctor.Items.FindByValue(keep) != null) ddlConsultDoctor.SelectedValue = keep;
            }
        }

        protected void ddlConsultDoctor_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTimes();
        }

        // Free times of the doctor on the chosen day as chips; today the next one is preselected (walk-in at the desk,
        // short buffer). For a returning patient, times that clash with their other appointments are left out.
        void LoadTimes()
        {
            string keep = rblTime.SelectedValue;
            rblTime.Items.Clear();
            int doctorID;
            if (!int.TryParse(ddlConsultDoctor.SelectedValue, out doctorID) || doctorID == 0)
            {
                SlotInfo(ddlConsultDoctor.Items.Count <= 1 ? "No doctor works on " + SlotService.FormatDate(ChosenDate) + "." : "Choose a doctor to see the free times.", false);
                return;
            }

            string reason;
            List<TimeSpan> free;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                free = SlotService.GetFreeSlots(con, doctorID, ChosenDate, SlotService.WalkInBufferMinutes, out reason);
            }
            int patient;
            string clash = null;
            if (int.TryParse(hdnExistingPatientID.Value, out patient))
                free = free.Where(t =>
                {
                    string c = SlotService.PatientConflict(patient, txtName.Text, doctorID, ChosenDate, t, null);
                    if (c != null && clash == null) clash = c;
                    return c == null;
                }).ToList();

            foreach (TimeSpan t in free)
                rblTime.Items.Add(new ListItem(SlotService.Format(t), t.ToString(@"hh\:mm")));
            if (free.Count == 0)
            {
                SlotInfo(clash ?? reason ?? "No free times on this day.", false);
                return;
            }
            rblTime.SelectedValue = rblTime.Items.FindByValue(keep) != null ? keep : rblTime.Items[0].Value;
            SlotInfo(free.Count + (free.Count == 1 ? " free time" : " free times") + " on " + SlotService.FormatDate(ChosenDate) +
                     (ChosenDate == DateTime.Today ? " (next one selected)" : ""), true);
        }

        void SlotInfo(string text, bool ok)
        {
            lblSlotInfo.Text = "&middot; " + HttpUtility.HtmlEncode(text);
            lblSlotInfo.CssClass = "slot-info " + (ok ? "ok" : "none");
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
            LoadTimes();   // a new patient has no other appointments to clash with
            UpdatePanel1.Update();
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
            if (IsPostBack) { LoadTimes(); UpdatePanel1.Update(); }
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
            DateTime today = ChosenDate;   // the appointment day (named today for the walk-in code below)

            if (doctorID == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "noDoctor",
                    "swalAlert('warning', 'Select Doctor', 'Please select a consulting doctor.');", true);
                return;
            }

            // The chosen time must still be free (someone may have booked it online meanwhile)
            TimeSpan chosen;
            string slotReason = null;
            bool slotFree = TimeSpan.TryParse(rblTime.SelectedValue, out chosen);
            if (slotFree)
                using (SqlConnection sc = new SqlConnection(connectionString))
                {
                    sc.Open();
                    slotFree = SlotService.IsSlotFree(sc, doctorID, today, chosen, SlotService.WalkInBufferMinutes, out slotReason);
                }
            if (!slotFree)
            {
                LoadTimes();
                ClientScript.RegisterStartupScript(this.GetType(), "noSlots",
                    "swalAlert('warning', 'Choose a Time', '" + HttpUtility.JavaScriptStringEncode(slotReason ?? "Please choose a time for the appointment.") + "');", true);
                return;
            }
            string appointTime = chosen.ToString(@"hh\:mm\:ss");

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

            // Returning patient: not with this doctor again that day, and no overlap with their other appointments
            int? knownPatient;
            using (SqlConnection pc = new SqlConnection(connectionString))
            {
                pc.Open();
                knownPatient = ExistingPatientId(pc, null, patientName, mobile);
            }
            if (knownPatient.HasValue)
            {
                string clash = SlotService.PatientConflict(knownPatient.Value, patientName, doctorID, today, chosen, null);
                if (clash != null)
                {
                    LoadTimes();
                    ClientScript.RegisterStartupScript(this.GetType(), "patientClash",
                        "swalAlert('warning', 'Already Booked', '" + HttpUtility.JavaScriptStringEncode(clash) + "');", true);
                    return;
                }
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
                                     "When: <b>" + (today == DateTime.Today ? "Today" : SlotService.FormatDate(today)) + ", " + SlotService.Format(appointTimeSpan) + "</b> &nbsp;&middot;&nbsp; Token: <b>" +
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