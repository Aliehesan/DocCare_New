using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    public partial class Appointments : System.Web.UI.Page
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

                if (userRole == "Doctor")
                {
                    if (Session["DoctorID"] == null) // ✅ Redirect to login if session is missing
                    {
                        Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                        return;
                    }
                    // ✅ Hide Consulting Doctor column for Doctors
                    GridViewAppointments.Columns[2].Visible = false;
                    btnBookAppointment.Visible = false;
                }
                else if (userRole == "SuperAdmin")
                {
                    btnBookAppointment.Visible = false;
                }

                // Opens on today's appointments; "All dates" shows everything
                txtFilterDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                BindAppointments();
            }
            if ((Session["UserRole"] as string) == "Doctor") HideAdminMenus();
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            GridViewAppointments.PageIndex = 0;
            BindAppointments();
        }

        // ── Date filter ───────────────────────────────────────────────────────

        // The chosen day, or null for all dates
        DateTime? FilterDate
        {
            get
            {
                DateTime d;
                return DateTime.TryParse(txtFilterDate.Text, out d) ? d.Date : (DateTime?)null;
            }
        }

        void ShowDate(DateTime? date)
        {
            txtFilterDate.Text = date.HasValue ? date.Value.ToString("yyyy-MM-dd") : "";
            GridViewAppointments.PageIndex = 0;
            BindAppointments();
        }

        protected void txtFilterDate_TextChanged(object sender, EventArgs e) { ShowDate(FilterDate); }
        protected void lnkToday_Click(object sender, EventArgs e) { ShowDate(DateTime.Today); }
        protected void lnkTomorrow_Click(object sender, EventArgs e) { ShowDate(DateTime.Today.AddDays(1)); }
        protected void lnkAllDates_Click(object sender, EventArgs e) { ShowDate(null); }

        // The list for the current role, date and search. Doctors see only their own patients.
        // One day is in time (token) order; all dates are newest first.
        private void BindAppointments()
        {
            string userRole = Session["UserRole"] as string;
            object doctorId = userRole == "Doctor" ? Session["DoctorID"] : null;
            string search = txtSearch.Text.Trim();
            DateTime? date = FilterDate;

            string query = @"
                SELECT A.ID, P.PatientName AS Name, P.PatientCode, D.Name AS ConsultingDoctor,
                       COALESCE(U.Mobile, P.Mobile, 'N/A') AS Mobile, A.TokenNo, A.AppointDate, A.AppointTime, A.Status,
                       CASE WHEN A.UserID IS NULL THEN 'Walk-in' ELSE 'Online' END AS BookingType
                FROM Appointments A
                JOIN Patients P ON A.PatientID = P.PatientID
                LEFT JOIN Users U ON A.UserID = U.UserID
                JOIN Doctors D ON A.DoctorID = D.DoctorID
                WHERE A.DeletedAt IS NULL
                  AND (@DoctorID IS NULL OR A.DoctorID = @DoctorID)
                  AND (@Date IS NULL OR A.AppointDate = @Date)";
            // Patient ID: "DC-00029", "dc29" or "00029" finds that patient exactly
            int? typedPatient = PatientCode.Parse(search);
            if (search != "")
                query += @" AND (P.PatientName LIKE @Search
                         OR P.PatientCode LIKE @Search
                         OR (@TypedPatientID IS NOT NULL AND P.PatientID = @TypedPatientID)
                         OR D.Name LIKE @Search
                         OR A.TokenNo LIKE @Search
                         OR U.Mobile LIKE @Search
                         OR P.Mobile LIKE @Search)";
            query += date.HasValue ? " ORDER BY A.AppointTime, A.TokenNo" : " ORDER BY A.AppointDate DESC, A.AppointTime";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@DoctorID", doctorId ?? DBNull.Value);
                cmd.Parameters.Add("@Date", SqlDbType.Date).Value = date.HasValue ? (object)date.Value : DBNull.Value;
                if (search != "")
                {
                    cmd.Parameters.AddWithValue("@Search", "%" + search + "%");
                    cmd.Parameters.AddWithValue("@TypedPatientID", typedPatient.HasValue ? (object)typedPatient.Value : DBNull.Value);
                }
                new SqlDataAdapter(cmd).Fill(dt);
            }

            // What is shown, and why a day can be empty
            int count = dt.Rows.Count;
            lblShowing.Text = (date.HasValue ? "Showing " + SlotService.FormatDate(date.Value) : "Showing all dates") +
                              " &middot; " + count + (count == 1 ? " appointment" : " appointments") +
                              (search != "" ? " matching &ldquo;" + HttpUtility.HtmlEncode(search) + "&rdquo;" : "");
            GridViewAppointments.EmptyDataText = "No Appointment Found";
            if (date.HasValue && search == "")
            {
                GridViewAppointments.EmptyDataText = "No appointments on " + SlotService.FormatDate(date.Value) + ".";
                int doctor;
                if (doctorId != null && int.TryParse(doctorId.ToString(), out doctor))
                    using (SqlConnection hc = new SqlConnection(connectionString))
                    {
                        hc.Open();
                        SlotService.DayHours h = SlotService.GetDayHours(hc, doctor, date.Value);
                        if (!h.Working) GridViewAppointments.EmptyDataText += " " + h.NotWorkingReason;
                    }
            }

            // Highlight the quick button that matches the chosen day
            lnkToday.CssClass = "date-chip" + (date == DateTime.Today ? " active" : "");
            lnkTomorrow.CssClass = "date-chip" + (date == DateTime.Today.AddDays(1) ? " active" : "");
            lnkAllDates.CssClass = "date-chip" + (!date.HasValue ? " active" : "");

            GridViewAppointments.DataSource = dt;
            GridViewAppointments.DataBind();
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

        protected string GetStatusCssClass(string status)
        {
            switch (status)
            {
                case "Pending":
                    return "status-pending";  // Gray background
                case "Confirmed":
                    return "status-confirmed";  // Blue background
                case "Completed":
                    return "status-completed";  // Green background
                case "Cancelled":
                    return "status-cancelled";  // Red background
                case "No Show":
                    return "status-noshow";  // Grey background
                default:
                    return "status-default";  // Default styling
            }
        }

        protected string ConvertTime(object appointTime)
        {
            if (appointTime == null || string.IsNullOrEmpty(appointTime.ToString()))
            {
                return "N/A"; // ✅ Handle NULL values
            }

            TimeSpan time;
            if (TimeSpan.TryParse(appointTime.ToString(), out time)) // ✅ Fix syntax error
            {
                return SlotService.Format(time); // ✅ "09:00 AM" whatever the server's regional settings
            }

            return "Invalid Time"; // ✅ Handle incorrect formats
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            GridViewAppointments.PageIndex = 0;
            BindAppointments();
        }

        protected void GridViewAppointments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteAppointment")
            {
                int appointmentId = Convert.ToInt32(e.CommandArgument);
                string userRole = Session["UserRole"] as string;

                // Only hides the appointment from this list: the record stays for the Appointment Report
                // and the patient's history. Only finished appointments can be removed; an active one
                // still holds a time slot, so it must be cancelled (which frees the slot and emails the patient).
                // Doctors can only remove their own patients' appointments.
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Appointments SET DeletedAt = GETDATE()
                                     WHERE ID = @ID AND DeletedAt IS NULL
                                       AND Status IN ('Completed', 'Cancelled', 'No Show')
                                       AND (@DoctorID IS NULL OR DoctorID = @DoctorID)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", appointmentId);
                        cmd.Parameters.AddWithValue("@DoctorID", userRole == "Doctor" ? Session["DoctorID"] : DBNull.Value);
                        con.Open();
                        bool removed = cmd.ExecuteNonQuery() > 0;
                        ClientScript.RegisterStartupScript(this.GetType(), "removeAppt", removed
                            ? "swalAlert('success', 'Removed from List', 'The appointment is hidden from this list. It is still in the Appointment Report.');"
                            : "swalAlert('warning', 'Not Removed', 'Only completed or cancelled appointments can be removed. Cancel the appointment first.');", true);
                    }
                }
                BindAppointments();
            }
        }

        protected void GridViewAppointments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridViewAppointments.PageIndex = e.NewPageIndex;
            BindAppointments();
        }

        protected void GridViewAppointments_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.DataRow)
            {
                string userRole = Session["UserRole"] as string;
                if (userRole == "Doctor")
                {
                    int columnIndex = 2; // Index of "Consulting Doctor" column
                    e.Row.Cells[columnIndex].Visible = false;
                }
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Calculate the correct row number based on pagination
                int serialNo = GridViewAppointments.PageIndex * GridViewAppointments.PageSize + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = serialNo.ToString();

            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Remove (hide) is only offered for finished appointments, see DeleteAppointment
                Button btnDelete = (Button)e.Row.FindControl("btnDelete");
                string rowStatus = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                if (btnDelete != null)
                    btnDelete.Visible = AppointmentStatus.IsFinal(rowStatus);
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
                if (ddlStatus != null)
                {
                    string currentStatus = DataBinder.Eval(e.Row.DataItem, "Status").ToString();

                    // Ensure the dropdown contains "Confirmed"
                    ListItem selectedItem = ddlStatus.Items.FindByValue(currentStatus);
                    if (selectedItem != null)
                    {
                        ddlStatus.ClearSelection();
                        selectedItem.Selected = true;
                    }

                    // Offer only the statuses this role may move to next (AppointmentStatus rules);
                    // nothing left to choose (Completed, Cancelled, No Show, admin view) -> locked.
                    // No Show only appears once the appointment time has passed.
                    DateTime? start = AppointmentStatus.StartOf(DataBinder.Eval(e.Row.DataItem, "AppointDate"), DataBinder.Eval(e.Row.DataItem, "AppointTime"));
                    List<string> next = AppointmentStatus.NextFor(currentStatus, Session["UserRole"] as string, start);
                    foreach (ListItem item in ddlStatus.Items)
                        item.Enabled = item.Value == currentStatus || next.Contains(item.Value);
                    ddlStatus.Enabled = next.Count > 0;
                }
            }
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddl.NamingContainer;

            if (GridViewAppointments.DataKeys.Count > row.RowIndex) // Ensure index exists
            {
                int appointmentID = Convert.ToInt32(GridViewAppointments.DataKeys[row.RowIndex].Value);
                string newStatus = ddl.SelectedValue;
                string userRole = Session["UserRole"] as string;

                // Checked on the server against the current status (forward-only, Completed/Cancelled final)
                string message;
                bool changed = AppointmentStatus.TryChange(appointmentID, newStatus, userRole, null, out message);
                ClientScript.RegisterStartupScript(this.GetType(), "statusChange",
                    "swalAlert('" + (changed ? "success" : "warning") + "', '" + (changed ? "Status Updated" : "Not Changed") + "', '" +
                    HttpUtility.JavaScriptStringEncode(message) + "');", true);

                BindAppointments();
            }
        }
    }
}