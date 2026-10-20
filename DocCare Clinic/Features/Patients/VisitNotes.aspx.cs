using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocCare_Clinic.Config;

namespace DocCare_Clinic
{
    /// <summary>
    /// One visit's medical record (?AppointmentID=): vitals, and for doctors the consultation and prescription.
    /// Access rules are in MedicalRecords; reception never receives the consultation section.
    /// </summary>
    public partial class VisitNotes : System.Web.UI.Page
    {
        string Role { get { return Session["UserRole"] as string; } }

        int AppointmentId
        {
            get { int id; return int.TryParse(Request.QueryString["AppointmentID"], out id) ? id : 0; }
        }

        int SessionDoctorId
        {
            get { int id; return Session["DoctorID"] != null && int.TryParse(Session["DoctorID"].ToString(), out id) ? id : 0; }
        }

        MedicalRecords.Visit visit;

        // The visit happened (or is happening) and this person may write that part of it
        bool IsVisitDay { get { return visit != null && MedicalRecords.IsVisit(visit.Status, visit.Date); } }
        bool CanWriteVitals
        {
            get
            {
                if (!IsVisitDay || !MedicalRecords.WritesVitals(Role)) return false;
                return Role != "Doctor" || visit.DoctorID == SessionDoctorId;   // a doctor: own patients' visits
            }
        }
        protected bool CanWriteClinical { get { return IsVisitDay && Role == "Doctor" && visit.DoctorID == SessionDoctorId; } }

        List<MedicalRecords.RxItem> Rx
        {
            get { return (ViewState["Rx"] as List<MedicalRecords.RxItem>) ?? new List<MedicalRecords.RxItem>(); }
            set { ViewState["Rx"] = value; }
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Role == "Doctor") MasterPageFile = ResolveUrl("~/Features/Doctors/DoctorAdmin.Master");
            else if (Role == "Receptionist") MasterPageFile = ResolveUrl("~/Features/Receptionists/ReceptionistAdmin.Master");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            visit = AppointmentId > 0 ? MedicalRecords.LoadVisit(AppointmentId) : null;
            if (visit == null)
            {
                Response.Redirect(ResolveUrl("~/Features/Appointments/Appointments.aspx"));
                return;
            }

            // Reception never gets the consultation section, not even hidden in the page
            pnlClinical.Visible = MedicalRecords.SeesClinical(Role);

            if (!IsPostBack)
            {
                ShowHeader();
                ShowNotes();
            }
            SetModes();
        }

        void ShowHeader()
        {
            lnkPatient.Text = HttpUtility.HtmlEncode(visit.Patient);
            lnkPatient.NavigateUrl = "~/Features/Patients/PatientProfile.aspx?Id=" + visit.PatientID;
            litPatient.Text = HttpUtility.HtmlEncode(visit.Patient) + " <span class=\"patient-code\" style=\"display:inline; font-size:13px;\">" + HttpUtility.HtmlEncode(visit.PatientCode) + "</span>";
            litPatientSub.Text = HttpUtility.HtmlEncode(visit.Gender) + (visit.Age > 0 ? ", " + visit.Age + " years" : "");
            litDoctor.Text = HttpUtility.HtmlEncode(visit.Doctor) + (string.IsNullOrEmpty(visit.Specialization) ? "" : " &middot; " + HttpUtility.HtmlEncode(visit.Specialization));
            litWhen.Text = SlotService.FormatDate(visit.Date) + (visit.Time.HasValue ? " at " + SlotService.Format(visit.Time.Value) : "") +
                           (string.IsNullOrEmpty(visit.Token) ? "" : " &middot; Token " + HttpUtility.HtmlEncode(visit.Token)) +
                           " <span class=\"visit-status\">" + HttpUtility.HtmlEncode(visit.Status) + "</span>";

            MedicalRecords.Health h = MedicalRecords.LoadHealth(visit.PatientID);
            pnlAllergy.Visible = !string.IsNullOrWhiteSpace(h.Allergies);
            litAllergies.Text = HttpUtility.HtmlEncode(h.Allergies);
        }

        void ShowNotes()
        {
            MedicalRecords.Notes n = MedicalRecords.LoadNotes(AppointmentId);
            txtBp.Text = n.BloodPressure;
            txtPulse.Text = n.Pulse.HasValue ? n.Pulse.ToString() : "";
            txtTemp.Text = n.TemperatureF.HasValue ? n.TemperatureF.Value.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture) : "";
            txtWeight.Text = n.WeightKg.HasValue ? n.WeightKg.Value.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture) : "";
            txtSpO2.Text = n.SpO2.HasValue ? n.SpO2.ToString() : "";
            litVitalsBy.Text = n.VitalsAt.HasValue ? "Recorded by " + HttpUtility.HtmlEncode(n.VitalsBy) + " on " + n.VitalsAt.Value.ToString("dd MMM yyyy, hh:mm tt", System.Globalization.CultureInfo.InvariantCulture) : "";

            if (!pnlClinical.Visible) return;
            txtComplaint.Text = n.Complaint;
            txtDiagnosis.Text = n.Diagnosis;
            txtAdvice.Text = n.Advice;
            txtFollowUp.Text = n.FollowUpDate.HasValue ? n.FollowUpDate.Value.ToString("yyyy-MM-dd") : "";
            litNotesBy.Text = n.NotesAt.HasValue ? "Last saved " + n.NotesAt.Value.ToString("dd MMM yyyy, hh:mm tt", System.Globalization.CultureInfo.InvariantCulture) : "";

            List<MedicalRecords.RxItem> rx = MedicalRecords.LoadRx(AppointmentId);
            if (rx.Count == 0 && CanWriteClinical) rx.Add(new MedicalRecords.RxItem());
            BindRx(rx);
            lnkPrint.Visible = n.HasClinical || rx.Any(r => !string.IsNullOrWhiteSpace(r.Medicine));
            lnkPrint.NavigateUrl = "~/Features/Patients/Prescription.aspx?AppointmentID=" + AppointmentId;
        }

        // What can be typed and which buttons show, for this person and this visit
        void SetModes()
        {
            bool vitals = CanWriteVitals, clinical = CanWriteClinical;
            foreach (TextBox t in new[] { txtBp, txtPulse, txtTemp, txtWeight, txtSpO2 }) t.ReadOnly = !vitals;
            foreach (TextBox t in new[] { txtComplaint, txtDiagnosis, txtAdvice, txtFollowUp }) t.ReadOnly = !clinical;
            foreach (RepeaterItem item in rptRx.Items)
                foreach (string id in new[] { "txtMedicine", "txtDose", "txtFrequency", "txtDays", "txtInstructions" })
                    ((TextBox)item.FindControl(id)).ReadOnly = !clinical;
            lnkAddRx.Visible = clinical;

            btnSave.Visible = vitals || clinical;
            btnSave.Text = clinical ? "Save" : "Save Vitals";
            btnSaveComplete.Visible = clinical && !AppointmentStatus.IsFinal(visit.Status);

            string why = null;
            if (visit.Status == AppointmentStatus.Cancelled || visit.Status == AppointmentStatus.NoShow)
                why = "This appointment is " + visit.Status + ", so there is no visit to record.";
            else if (visit.Date.Date > DateTime.Today)
                why = "This appointment is on " + SlotService.FormatDate(visit.Date) + ". Notes can be added from the day of the visit.";
            else if (Role == "SuperAdmin")
                why = "Read only: notes are written by the doctor and vitals by the doctor or reception.";
            else if (Role == "Doctor" && !clinical)
                why = "Read only: this visit is with " + HttpUtility.HtmlEncode(visit.Doctor) + ".";
            pnlReadOnly.Visible = why != null;
            litReadOnly.Text = why;
        }

        // ── Prescription rows ─────────────────────────────────────────────────

        void BindRx(List<MedicalRecords.RxItem> rx)
        {
            Rx = rx;
            rptRx.DataSource = rx;
            rptRx.DataBind();
        }

        List<MedicalRecords.RxItem> ReadRx()
        {
            var list = new List<MedicalRecords.RxItem>();
            foreach (RepeaterItem item in rptRx.Items)
            {
                int days;
                list.Add(new MedicalRecords.RxItem
                {
                    Medicine = ((TextBox)item.FindControl("txtMedicine")).Text.Trim(),
                    Dose = ((TextBox)item.FindControl("txtDose")).Text.Trim(),
                    Frequency = ((TextBox)item.FindControl("txtFrequency")).Text.Trim(),
                    Days = int.TryParse(((TextBox)item.FindControl("txtDays")).Text, out days) && days > 0 && days <= 365 ? days : (int?)null,
                    Instructions = ((TextBox)item.FindControl("txtInstructions")).Text.Trim()
                });
            }
            return list;
        }

        protected void lnkAddRx_Click(object sender, EventArgs e)
        {
            if (!CanWriteClinical) return;
            var rx = ReadRx();
            rx.Add(new MedicalRecords.RxItem());
            BindRx(rx);
            SetModes();
        }

        protected void rptRx_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Remove" || !CanWriteClinical) return;
            var rx = ReadRx();
            int i = int.Parse(e.CommandArgument.ToString());
            if (i >= 0 && i < rx.Count) rx.RemoveAt(i);
            if (rx.Count == 0) rx.Add(new MedicalRecords.RxItem());
            BindRx(rx);
            SetModes();
        }

        // ── Saving ────────────────────────────────────────────────────────────

        protected void btnSave_Click(object sender, EventArgs e) { Save(false); }
        protected void btnSaveComplete_Click(object sender, EventArgs e) { Save(true); }

        void Save(bool complete)
        {
            bool vitals = CanWriteVitals, clinical = CanWriteClinical;
            if (!vitals && !clinical) { Alert("warning", "Not Saved", "You cannot change this visit."); return; }

            var problems = new List<string>();
            var n = new MedicalRecords.Notes();
            if (vitals) MedicalRecords.ParseVitals(txtBp.Text, txtPulse.Text, txtTemp.Text, txtWeight.Text, txtSpO2.Text, n, problems);

            List<MedicalRecords.RxItem> rx = null;
            if (clinical)
            {
                n.Complaint = Limit(txtComplaint.Text, 1000);
                n.Diagnosis = Limit(txtDiagnosis.Text, 1000);
                n.Advice = Limit(txtAdvice.Text, 2000);
                DateTime follow;
                if (txtFollowUp.Text.Trim() != "")
                {
                    if (!DateTime.TryParse(txtFollowUp.Text, out follow) || follow.Date <= visit.Date.Date)
                        problems.Add("Follow-up date must be after the visit");
                    else n.FollowUpDate = follow.Date;
                }
                rx = ReadRx();
                var lines = rx.Where(r => r.Medicine != "").ToList();
                if (rx.Any(r => r.Medicine == "" && (r.Dose != "" || r.Frequency != "" || r.Days.HasValue || r.Instructions != "")))
                    problems.Add("Each prescription line needs the medicine name");
                rx = lines;
                if (complete && string.IsNullOrWhiteSpace(n.Diagnosis) && rx.Count == 0)
                    problems.Add("Add a diagnosis or a prescription before marking the visit completed");
            }

            if (problems.Count > 0)
            {
                Alert("warning", "Please check", string.Join(". ", problems) + ".");
                return;
            }

            // Vitals only when they changed, so "recorded by" stays with whoever took them (often reception)
            MedicalRecords.Notes before = MedicalRecords.LoadNotes(AppointmentId);
            bool vitalsChanged = before.BloodPressure != n.BloodPressure || before.Pulse != n.Pulse || before.TemperatureF != n.TemperatureF ||
                                 before.WeightKg != n.WeightKg || before.SpO2 != n.SpO2;
            if (vitals && vitalsChanged) MedicalRecords.SaveVitals(AppointmentId, n, MedicalRecords.StaffName(Session));
            if (clinical) MedicalRecords.SaveClinical(AppointmentId, n, rx, SessionDoctorId);

            string done = clinical ? "The visit notes are saved." : "The vitals are saved.";
            if (complete)
            {
                string message;
                if (AppointmentStatus.TryChange(AppointmentId, AppointmentStatus.Completed, Role, null, out message))
                    done = "The visit notes are saved and the appointment is marked Completed.";
                else
                    done += " " + message;
            }
            Session["VisitNotesMessage"] = done;
            Response.Redirect(Request.RawUrl);   // reload with the saved values
        }

        static string Limit(string text, int max)
        {
            text = (text ?? "").Trim();
            return text.Length > max ? text.Substring(0, max) : text;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            string msg = Session["VisitNotesMessage"] as string;
            if (msg != null)
            {
                Session["VisitNotesMessage"] = null;
                Alert("success", "Saved", msg);
            }
        }

        void Alert(string icon, string title, string text)
        {
            ClientScript.RegisterStartupScript(GetType(), "visitMsg",
                "swalAlert('" + icon + "', '" + HttpUtility.JavaScriptStringEncode(title) + "', '" + HttpUtility.JavaScriptStringEncode(text) + "');", true);
        }
    }
}
