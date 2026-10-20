using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using TaskManagement.config;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    public partial class DoctorsAppoinment : System.Web.UI.Page
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
                    HideAdminMenus();
                    BindDoctorAppointments("");
                }
            }
        }

        protected string GetStatusCssClass(string status)
        {
            switch (status.ToLower())
            {
                case "confirmed":
                    return "status-blue"; // Blue color for Booked
                case "completed":
                    return "status-green"; // Green color for Complete
                case "cancelled":
                    return "status-red"; // Red color for Cancelled
                case "no show":
                    return "status-gray"; // Grey for No Show
                default:
                    return "status-default"; // Default color for other statuses
            }
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

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {

            string searchText = txtSearch.Text.Trim();
            BindDoctorAppointments(searchText);

        }

        private void BindDoctorAppointments(string searchKeyword)
        {
            string doctorId = Request.QueryString["DoctorID"];
            if (string.IsNullOrEmpty(doctorId))
            {
                Response.Redirect(ResolveUrl("~/Features/Doctors/Manage_Doctor.aspx"));
                return;
            }

            // Working days from the weekly hours (not leave days): the next 7 days in full, plus any
            // later day in the booking window that already has bookings. Weekday 0 = Sunday (1900-01-07 was a Sunday).
            string query = @"
            WITH Days AS (
                SELECT CAST(ISNULL(@FilterDate, GETDATE()) AS DATE) AS D, 0 AS N
                UNION ALL
                SELECT CAST(DATEADD(DAY, 1, D) AS DATE), N + 1 FROM Days WHERE N < @Window AND @FilterDate IS NULL
            )
            SELECT dd.D AS AppointDate, w.StartTime, w.EndTime, w.SlotDuration, w.BreakStart, w.BreakEnd, 
                   ISNULL(a.ID, '-') AS AppointmentID, ISNULL(a.AppointTime, '') AS AppointTime,
                   ISNULL(a.Status, 'Not Booked') AS Status, 
                   ISNULL(p.PatientName, 'N/A') AS PatientName, p.PatientCode,
                   a.NotifiedAt,
                   COALESCE(NULLIF(u.Email, ''), NULLIF(pu.Email, ''), NULLIF(p.Email, ''), '') AS PatientEmail
            FROM Days dd
            INNER JOIN DoctorWorkingHours w ON w.DoctorID = @DoctorID AND w.IsWorking = 1
                                            AND w.DayOfWeek = DATEDIFF(DAY, '19000107', dd.D) % 7
            LEFT JOIN Appointments a ON a.DoctorID = @DoctorID
                                      AND a.AppointDate = dd.D
                                      AND w.StartTime <= a.AppointTime AND w.EndTime > a.AppointTime
                                      AND a.Status <> 'Cancelled'
            LEFT JOIN Patients p ON a.PatientID = p.PatientID
            LEFT JOIN Users u ON a.UserID = u.UserID
            LEFT JOIN Users pu ON p.UserID = pu.UserID
            WHERE NOT EXISTS (SELECT 1 FROM DoctorLeave l WHERE l.DoctorID = @DoctorID AND l.LeaveDate = dd.D)
              AND (@FilterDate IS NOT NULL OR dd.N < 7
                   OR EXISTS (SELECT 1 FROM Appointments x WHERE x.DoctorID = @DoctorID AND x.AppointDate = dd.D AND x.Status <> 'Cancelled'))";

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query += " AND (p.PatientName LIKE @Keyword OR p.PatientCode LIKE @Keyword OR (@TypedPatientID IS NOT NULL AND p.PatientID = @TypedPatientID))";
            }

            query += " ORDER BY dd.D, w.StartTime";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    cmd.Parameters.AddWithValue("@Window", SlotService.BookingWindowDays);
                    DateTime filterDate;
                    bool oneDay = DateTime.TryParse(txtFilterDate.Text, out filterDate);
                    cmd.Parameters.Add("@FilterDate", SqlDbType.Date).Value = oneDay ? (object)filterDate.Date : DBNull.Value;

                    // What the grid shows, and why a chosen day can be empty
                    lblShowing.Text = oneDay ? "Showing " + SlotService.FormatDate(filterDate)
                                             : "Showing the next 7 days, plus later days that have bookings";
                    gvDoctorAppointments.EmptyDataText = "No Record Found";
                    int doctor;
                    if (oneDay && int.TryParse(doctorId, out doctor))
                        using (SqlConnection hc = new SqlConnection(connectionString))
                        {
                            hc.Open();
                            SlotService.DayHours h = SlotService.GetDayHours(hc, doctor, filterDate);
                            if (!h.Working) gvDoctorAppointments.EmptyDataText = h.NotWorkingReason;
                        }
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        cmd.Parameters.AddWithValue("@Keyword", "%" + searchKeyword + "%");
                        int? typed = PatientCode.Parse(searchKeyword);
                        cmd.Parameters.AddWithValue("@TypedPatientID", typed.HasValue ? (object)typed.Value : DBNull.Value);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    DataTable allSlotsTable = new DataTable();
                    allSlotsTable.Columns.Add("AppointmentID", typeof(string));
                    allSlotsTable.Columns.Add("PatientName", typeof(string));
                    allSlotsTable.Columns.Add("PatientCode", typeof(string));
                    allSlotsTable.Columns.Add("AppointDate", typeof(string));
                    allSlotsTable.Columns.Add("TimeSlot", typeof(string));
                    allSlotsTable.Columns.Add("Status", typeof(string));
                    allSlotsTable.Columns.Add("NotifiedAt", typeof(object));
                    allSlotsTable.Columns.Add("PatientEmail", typeof(string));
                    allSlotsTable.Columns.Add("SortKey", typeof(string));   // "yyyy-MM-dd HH:mm", for date then time order

                    // Patient ID per booked appointment, filled in after the slots are built
                    var codes = new Dictionary<string, string>();
                    foreach (DataRow row in dt.Rows)
                        if (row["AppointmentID"].ToString() != "-") codes[row["AppointmentID"].ToString()] = row["PatientCode"].ToString();

                    foreach (DataRow row in dt.Rows)
                    {
                        TimeSpan startTime = row["StartTime"] != DBNull.Value ? (TimeSpan)row["StartTime"] : TimeSpan.Zero;
                        TimeSpan endTime = row["EndTime"] != DBNull.Value ? (TimeSpan)row["EndTime"] : TimeSpan.Zero;
                        int slotDuration = row["SlotDuration"] != DBNull.Value ? Convert.ToInt32(row["SlotDuration"]) : 0;
                        TimeSpan? breakStart = row["BreakStart"] != DBNull.Value ? (TimeSpan?)row["BreakStart"] : null;
                        TimeSpan? breakEnd = row["BreakEnd"] != DBNull.Value ? (TimeSpan?)row["BreakEnd"] : null;
                        string appointDate = row["AppointDate"] != DBNull.Value ? Convert.ToDateTime(row["AppointDate"]).ToString("yyyy-MM-dd") : "";
                        string appointmentID = row["AppointmentID"] != DBNull.Value ? row["AppointmentID"].ToString() : "-";
                        string patientName = row["PatientName"] != DBNull.Value ? row["PatientName"].ToString() : "N/A";
                        string appointTime = row["AppointTime"] != DBNull.Value ? row["AppointTime"].ToString() : "";
                        string status = row["Status"] != DBNull.Value ? row["Status"].ToString() : "Not Booked";
                        object notifiedAt = row["NotifiedAt"];
                        string patientEmail = row["PatientEmail"].ToString();

                        GetTimeSlot(allSlotsTable, appointmentID, patientName, appointDate, startTime, endTime, slotDuration, breakStart, breakEnd, appointTime, status, notifiedAt, patientEmail);
                    }

                    foreach (DataRow slot in allSlotsTable.Rows)
                    {
                        string code;
                        slot["PatientCode"] = codes.TryGetValue(slot["AppointmentID"].ToString(), out code) ? code : "";
                    }
                    AddOffHoursBookings(allSlotsTable, doctorId, oneDay ? filterDate.Date : (DateTime?)null, searchKeyword);
                    allSlotsTable.DefaultView.Sort = "SortKey";
                    allSlotsTable = allSlotsTable.DefaultView.ToTable();

                    gvDoctorAppointments.DataSource = allSlotsTable;
                    gvDoctorAppointments.DataBind();
                }
            }
        }
        private void GetTimeSlot(DataTable allSlotsTable, string appointmentID, string patientName, string appointDate, 
            TimeSpan startTime, TimeSpan endTime, int slotDuration, TimeSpan? breakStart, TimeSpan? breakEnd,
            string appointTime, string status, object notifiedAt, string patientEmail)
        {
            // Same slots patients can book: whole slots that finish by the end time, none during the break
            foreach (TimeSpan currentSlot in SlotService.SlotTimes(startTime, endTime, slotDuration, breakStart, breakEnd))
            {
                TimeSpan nextSlot = currentSlot.Add(TimeSpan.FromMinutes(slotDuration));

                string timeSlotStr = string.Format("{0} - {1}",
                    SlotService.Format(currentSlot),
                    SlotService.Format(nextSlot));

                // Check for existing slots
                DataRow[] existingRows = allSlotsTable.Select(string.Format("TimeSlot = '{0}' AND AppointDate = '{1}'", timeSlotStr, appointDate));
                if (existingRows.Length > 0)
                {
                    if (!string.IsNullOrEmpty(appointTime))
                    {
                        TimeSpan parsedAppointTime;
                        if (TimeSpan.TryParse(appointTime, out parsedAppointTime))
                        {
                            if (parsedAppointTime == currentSlot)
                            {
                                existingRows[0]["AppointmentID"] = appointmentID;
                                existingRows[0]["PatientName"] = patientName;
                                existingRows[0]["Status"] = status;
                                existingRows[0]["NotifiedAt"] = notifiedAt;
                                existingRows[0]["PatientEmail"] = patientEmail;
                            }
                        }
                    }
                    continue;
                }

                // Determine slot status
                bool isBooked = false;
                if (!string.IsNullOrEmpty(appointTime))
                {
                    TimeSpan parsedAppointTime;
                    if (TimeSpan.TryParse(appointTime, out parsedAppointTime))
                    {
                        isBooked = parsedAppointTime == currentSlot;
                    }
                }

                // Add new row
                DataRow slotRow = allSlotsTable.NewRow();
                slotRow["AppointmentID"] = isBooked ? appointmentID : "-";
                slotRow["PatientName"] = isBooked ? patientName : "N/A";
                slotRow["AppointDate"] = appointDate;
                slotRow["TimeSlot"] = timeSlotStr;
                slotRow["Status"] = isBooked ? status : "Not Booked";
                slotRow["NotifiedAt"] = isBooked ? notifiedAt : DBNull.Value;
                slotRow["PatientEmail"] = isBooked ? patientEmail : "";
                slotRow["SortKey"] = appointDate + " " + currentSlot.ToString(@"hh\:mm");

                allSlotsTable.Rows.Add(slotRow);
            }
        }

        const string OffHoursLabel = "Outside working hours";

        // Bookings that are not on one of the slot times above (the hours or break changed after the patient
        // booked and the clinic kept the appointment, or a day off was set): never hide a patient who is coming.
        private void AddOffHoursBookings(DataTable allSlotsTable, string doctorId, DateTime? oneDay, string searchKeyword)
        {
            var shown = new HashSet<string>(allSlotsTable.Rows.Cast<DataRow>()
                .Select(r => r["AppointmentID"].ToString()).Where(id => id != "-"));

            string query = @"
                SELECT a.ID, a.AppointDate, a.AppointTime, a.Status, a.NotifiedAt, ISNULL(p.PatientName, 'N/A') AS PatientName, p.PatientCode,
                       COALESCE(NULLIF(u.Email, ''), NULLIF(pu.Email, ''), NULLIF(p.Email, ''), '') AS PatientEmail
                FROM Appointments a
                LEFT JOIN Patients p ON a.PatientID = p.PatientID
                LEFT JOIN Users u ON a.UserID = u.UserID
                LEFT JOIN Users pu ON p.UserID = pu.UserID
                WHERE a.DoctorID = @DoctorID AND a.Status <> 'Cancelled' AND a.AppointTime IS NOT NULL
                  AND (@OneDay IS NULL AND a.AppointDate >= CAST(GETDATE() AS DATE) OR a.AppointDate = @OneDay)
                  AND NOT EXISTS (SELECT 1 FROM DoctorLeave l WHERE l.DoctorID = a.DoctorID AND l.LeaveDate = a.AppointDate)";
            if (!string.IsNullOrEmpty(searchKeyword)) query += " AND (p.PatientName LIKE @Keyword OR p.PatientCode LIKE @Keyword OR (@TypedPatientID IS NOT NULL AND p.PatientID = @TypedPatientID))";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                cmd.Parameters.Add("@OneDay", SqlDbType.Date).Value = oneDay.HasValue ? (object)oneDay.Value : DBNull.Value;
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    cmd.Parameters.AddWithValue("@Keyword", "%" + searchKeyword + "%");
                    int? typed = PatientCode.Parse(searchKeyword);
                    cmd.Parameters.AddWithValue("@TypedPatientID", typed.HasValue ? (object)typed.Value : DBNull.Value);
                }
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        if (shown.Contains(r["ID"].ToString())) continue;
                        TimeSpan time = (TimeSpan)r["AppointTime"];
                        string date = Convert.ToDateTime(r["AppointDate"]).ToString("yyyy-MM-dd");
                        DataRow row = allSlotsTable.NewRow();
                        row["AppointmentID"] = r["ID"].ToString();
                        row["PatientName"] = r["PatientName"].ToString();
                        row["PatientCode"] = r["PatientCode"].ToString();
                        row["AppointDate"] = date;
                        row["TimeSlot"] = SlotService.Format(time) + " (" + OffHoursLabel + ")";
                        row["Status"] = r["Status"].ToString();
                        row["NotifiedAt"] = r["NotifiedAt"];
                        row["PatientEmail"] = r["PatientEmail"].ToString();
                        row["SortKey"] = date + " " + time.ToString(@"hh\:mm");
                        allSlotsTable.Rows.Add(row);
                    }
            }
        }

        protected void NotifyPatient_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int appointmentId;
            if (!int.TryParse(btn.CommandArgument, out appointmentId))
            {
                Session["ErrorMessage"] = "This slot has no appointment to notify.";
                Response.Redirect(Request.RawUrl);
                return;
            }

            // Same email as the Appointments list (doctor, time slot, token, fees...)
            string message;
            if (AppointmentNotifier.NotifyPatient(appointmentId, out message))
            {
                // A pending booking is confirmed once the patient has been told
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("UPDATE Appointments SET Status = 'Confirmed' WHERE ID = @ID AND Status = 'Pending'", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", appointmentId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                Session["SuccessMessage"] = HttpUtility.JavaScriptStringEncode(message);
            }
            else
            {
                Session["ErrorMessage"] = HttpUtility.JavaScriptStringEncode(message);
            }

            Response.Redirect(Request.RawUrl);
        }

        protected void txtFilterDate_TextChanged(object sender, EventArgs e)
        {
            gvDoctorAppointments.PageIndex = 0;
            BindDoctorAppointments(txtSearch.Text.Trim());
        }

        protected void lnkNext7Days_Click(object sender, EventArgs e)
        {
            txtFilterDate.Text = "";
            gvDoctorAppointments.PageIndex = 0;
            BindDoctorAppointments(txtSearch.Text.Trim());
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            string searchKeyword = txtSearch.Text.Trim();
            BindDoctorAppointments(searchKeyword);
        }

        protected void gvDoctorAppointments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDoctorAppointments.PageIndex = e.NewPageIndex;
            BindDoctorAppointments(txtSearch.Text.Trim());
        }

        protected void gvDoctorAppointments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Calculate the correct row number based on pagination
                int serialNo = gvDoctorAppointments.PageIndex * gvDoctorAppointments.PageSize + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = serialNo.ToString();

                if (DataBinder.Eval(e.Row.DataItem, "TimeSlot").ToString().Contains(OffHoursLabel))
                {
                    e.Row.Style["background"] = "#fff7ed";
                    e.Row.ToolTip = "Booked before the working hours changed; the appointment was kept at its time";
                }

                Button btnNotify = (Button)e.Row.FindControl("btnNotify");
                Label lblNotified = (Label)e.Row.FindControl("lblNotified");
                string appointmentId = DataBinder.Eval(e.Row.DataItem, "AppointmentID").ToString();
                string status = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                object notifiedAt = DataBinder.Eval(e.Row.DataItem, "NotifiedAt");

                // The details email is about an upcoming visit: nothing to send for empty slots or for
                // visits that are already over (Completed/Cancelled patients get their own automatic email)
                btnNotify.Visible = appointmentId != "-" && status != "Not Booked" && !AppointmentStatus.IsFinal(status);

                // Walk-ins usually have no email address
                if (btnNotify.Visible && DataBinder.Eval(e.Row.DataItem, "PatientEmail").ToString() == "")
                {
                    btnNotify.Visible = false;
                    lblNotified.Text = "No email on file";
                    lblNotified.Style["color"] = "#9ca3af";
                    lblNotified.Visible = true;
                }

                // Already emailed (by the front desk, or automatically after online payment)
                if (btnNotify.Visible && notifiedAt != null && notifiedAt != DBNull.Value)
                {
                    btnNotify.Text = "Resend";
                    lblNotified.Text = "Notified &middot; " + Convert.ToDateTime(notifiedAt).ToString("dd MMM, hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                    lblNotified.Visible = true;
                }
            }
        }
    }
}