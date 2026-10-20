using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    public partial class MyAppointment : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int UserId;
                if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out UserId))
                {
                    BindAppointments(UserId); // Call the method with a valid patientId
                }
                else
                {
                    // Redirect to an error page or login page if the session is invalid
                    Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
                }
            }
        }

        private void BindAppointments(int userID)
        {
            string query = @"
                SELECT 
                    A.ID AS AppointmentID, 
                    D.Name AS DoctorName, 
                    D.Specialization, 
                    D.ImageUrl AS DoctorImage,  
                    DP.Location AS Address, 
                    A.AppointDate,                   
                    CASE
                        WHEN A.AppointTime IS NOT NULL
                        THEN RIGHT(CONVERT(VARCHAR(20), CAST(A.AppointTime AS DATETIME), 100), 7)
                        ELSE 'Not Allocated'
                    END AS AppointTime,
                    A.AppointTime AS AppointTimeRaw,
                    A.Status,
                    ISNULL(A.PaidAmount, 0) AS PaidAmount,
                    ISNULL(A.DueAmount, 0) AS DueAmount,
                    ISNULL(A.TotalFee, 0) AS TotalFee,
                    ISNULL(A.PaymentStatus, '') AS PaymentStatus,
                    ISNULL(A.PaymentId, '') AS PaymentId,
                    COALESCE(P.PatientName, U.Name, 'Patient') AS PatientName, ISNULL(P.PatientCode, '') AS PatientCode,
                    ISNULL(P.Age, 0) AS PatientAge,
                    ISNULL(P.Gender, '') AS PatientGender
                FROM Appointments A
                JOIN Doctors D ON A.DoctorID = D.DoctorID
                LEFT JOIN DoctorProfile DP ON D.DoctorID = DP.DoctorID
                LEFT JOIN Patients P ON A.PatientID = P.PatientID
                LEFT JOIN Users U ON A.UserID = U.UserID
                WHERE A.UserID = @UserID
                ORDER BY A.AppointDate DESC, A.ID DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserID", userID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    rptAppointments.DataSource = dt;
                    rptAppointments.DataBind();
                    pnlNoAppointments.Visible = false;
                }
                else
                {
                    rptAppointments.DataSource = null;
                    rptAppointments.DataBind();
                    pnlNoAppointments.Visible = true;
                }
            }
        }

        // ── Card helpers (status badge, Cancel / Reschedule buttons) ──

        protected string StatusClass(object status)
        {
            switch (Convert.ToString(status))
            {
                case AppointmentStatus.Confirmed: return "status-badge-confirmed";
                case AppointmentStatus.Completed: return "status-badge-completed";
                case AppointmentStatus.Cancelled: return "status-badge-cancelled";
                case AppointmentStatus.NoShow: return "status-badge-noshow";
                default: return "status-badge-pending";
            }
        }

        protected string StatusIcon(object status)
        {
            switch (Convert.ToString(status))
            {
                case AppointmentStatus.Confirmed: return "fa-check";
                case AppointmentStatus.Completed: return "fa-circle-check";
                case AppointmentStatus.Cancelled: return "fa-xmark";
                case AppointmentStatus.NoShow: return "fa-user-xmark";
                default: return "fa-clock";
            }
        }

        // Pending/Confirmed and more than PatientCutoffHours away: the patient may cancel or reschedule online
        protected bool CanChangeOnline(object status, object date, object time)
        {
            if (AppointmentStatus.IsFinal(Convert.ToString(status))) return false;
            DateTime? start = AppointmentStatus.StartOf(date, time);
            return !start.HasValue || AppointmentStatus.PatientCanChange(start.Value);
        }

        protected string CancelText(object status, object date, object time)
        {
            string s = Convert.ToString(status);
            if (AppointmentStatus.IsFinal(s)) return s;
            return CanChangeOnline(status, date, time) ? "Cancel Appointment" : "Call clinic to cancel";
        }

        protected string CancelCss(object status, object date, object time)
        {
            if (Convert.ToString(status) == AppointmentStatus.Completed) return "completed-btn-modern";
            return CanChangeOnline(status, date, time) ? "cancel-btn-modern" : "cancelled-btn-modern";
        }

        protected void CancelAppointment(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int appointmentID = Convert.ToInt32(btn.CommandArgument);

            int userID;
            if (Session["UserId"] == null || !int.TryParse(Session["UserId"].ToString(), out userID))
                return;

            // Same rules as the staff pages; a patient may only cancel their own Pending/Confirmed appointments
            string message;
            bool cancelled = AppointmentStatus.TryChange(appointmentID, AppointmentStatus.Cancelled, "User", userID, out message);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "cancelAppt",
                "swalAlert('" + (cancelled ? "success" : "warning") + "', '" + (cancelled ? "Appointment Cancelled" : "Not Cancelled") + "', '" +
                HttpUtility.JavaScriptStringEncode(cancelled ? "Your appointment has been cancelled." : message) + "');", true);

            BindAppointments(userID);
        }
    }
}