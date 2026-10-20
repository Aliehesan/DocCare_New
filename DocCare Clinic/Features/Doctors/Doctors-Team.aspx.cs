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
    public partial class Doctors_Team : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDoctors();
            }
        }

        private void LoadDoctors(string searchTerm = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT DoctorID, Name, Specialization, ImageUrl FROM Doctors Where IsDeleted = 0";
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND (Name LIKE @SearchTerm OR Specialization LIKE @SearchTerm)";
                }
                
                query += " ORDER BY Name ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    }

                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Bind data to Repeater
                        if (dt.Rows.Count > 0)
                        {
                            rptDoctors.DataSource = dt;
                            rptDoctors.DataBind();
                            rptDoctors.Visible = true;
                            litNoDoctors.Text = "";
                        }
                        else
                        {
                            rptDoctors.Visible = false;
                            litNoDoctors.Text = "<span class='no-doctors-msg'>No Doctor Found matching your search.</span>";
                        }                        
                    }
                }
            }
        }

        protected void btnSearchHidden_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            LoadDoctors(searchTerm);
        }

        protected void BookAppointment_Click(object sender, EventArgs e)
        {
            // Check if the session exists
            if (Session["UserID"] == null)
            {
                // Redirect to the login page if the session is not found
                Response.Redirect(ResolveUrl("~/Features/Accounts/Login.aspx"));
            }
            Button btn = (Button)sender;
            string doctorId = btn.CommandArgument;

            if (!string.IsNullOrEmpty(doctorId))
            {
                string redirectUrl = ResolveUrl("~/Features/Doctors/Doctor-details.aspx") + "?DoctorID=" + doctorId;
                Response.Redirect(redirectUrl);
            }
            else
            {
                Response.Write("Error: Doctor ID is missing.");
            }
        }

        // Same rule as online booking: the first day in the booking window with a free time
        // (working hours, leave days, max patients, 45-minute travel buffer today)
        private bool CheckTokenAvailability(string doctorId, out string message)
        {
            int id;
            if (!int.TryParse(doctorId, out id))
            {
                message = "No appointments available.";
                return false;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                TimeSpan firstSlot;
                DateTime? next = SlotService.GetNextAvailableDate(con, id, SlotService.OnlineBufferMinutes, out firstSlot);
                message = next.HasValue
                    ? "Next available: " + (next.Value == DateTime.Today ? "Today" : next.Value == DateTime.Today.AddDays(1) ? "Tomorrow" : SlotService.FormatDate(next.Value))
                      + ", " + SlotService.Format(firstSlot)
                    : "No free appointments in the next " + SlotService.BookingWindowDays + " days.";
                return next.HasValue;
            }
        }

        protected void rptDoctors_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Button btnBookAppointment = (Button)e.Item.FindControl("btnBookAppointment");
                Label lblNoSlots = (Label)e.Item.FindControl("lblNoSlots");
                
                string doctorId = DataBinder.Eval(e.Item.DataItem, "DoctorID").ToString();
                string reason;
                bool isTokenAvailable = CheckTokenAvailability(doctorId, out reason);

                if (!isTokenAvailable)
                {
                    btnBookAppointment.Enabled = false;
                    lblNoSlots.Text = reason;
                    lblNoSlots.Visible = true;                    
                }
                else
                {
                    // e.g. "Next available: Tomorrow, 09:00 AM"
                    lblNoSlots.Text = reason;
                    lblNoSlots.CssClass = "next-available-label";
                    lblNoSlots.Visible = true;
                }
            }
        }
    }
}