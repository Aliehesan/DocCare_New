using System;
using System.Globalization;
using System.Web;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    /// <summary>Printable prescription for one visit (?AppointmentID=). Doctors and the admin only (Global.asax).</summary>
    public partial class Prescription : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int id;
            MedicalRecords.Visit v = int.TryParse(Request.QueryString["AppointmentID"], out id) ? MedicalRecords.LoadVisit(id) : null;
            if (v == null || !MedicalRecords.SeesClinical(Session["UserRole"] as string))
            {
                Response.Redirect(ResolveUrl("~/Features/Appointments/Appointments.aspx"));
                return;
            }

            MedicalRecords.Notes n = MedicalRecords.LoadNotes(id);
            MedicalRecords.Health h = MedicalRecords.LoadHealth(v.PatientID);
            var rx = MedicalRecords.LoadRx(id);
            Func<string, string> enc = HttpUtility.HtmlEncode;

            Title = "Prescription - " + v.Patient + " - " + v.Date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
            litLocation.Text = enc(v.Location);
            litDoctor.Text = enc(v.Doctor);
            litSpecialization.Text = enc(v.Specialization);
            litPatient.Text = enc(v.Patient);
            litCode.Text = enc(v.PatientCode);
            litAgeSex.Text = (v.Age > 0 ? v.Age + " y" : "") + (string.IsNullOrEmpty(v.Gender) ? "" : " / " + enc(v.Gender));
            litDate.Text = v.Date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
            litToken.Text = enc(v.Token);
            litPrinted.Text = DateTime.Now.ToString("dd MMM yyyy, hh:mm tt", CultureInfo.InvariantCulture);
            litSign.Text = enc(v.Doctor);

            pnlAllergy.Visible = !string.IsNullOrWhiteSpace(h.Allergies);
            litAllergies.Text = enc(h.Allergies);
            pnlVitals.Visible = n.HasVitals;
            litVitals.Text = enc(n.VitalsLine());
            pnlComplaint.Visible = !string.IsNullOrWhiteSpace(n.Complaint);
            litComplaint.Text = enc(n.Complaint);
            pnlDiagnosis.Visible = !string.IsNullOrWhiteSpace(n.Diagnosis);
            litDiagnosis.Text = enc(n.Diagnosis);
            pnlAdvice.Visible = !string.IsNullOrWhiteSpace(n.Advice);
            litAdvice.Text = enc(n.Advice);
            pnlFollow.Visible = n.FollowUpDate.HasValue;
            litFollow.Text = n.FollowUpDate.HasValue ? SlotService.FormatDate(n.FollowUpDate.Value) : "";

            rptRx.DataSource = rx;
            rptRx.DataBind();
            rptRx.Visible = rx.Count > 0;
            pnlNoRx.Visible = rx.Count == 0;
        }
    }
}
