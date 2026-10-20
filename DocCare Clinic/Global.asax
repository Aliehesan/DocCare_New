<%@ Application Language="C#" %>
<%@ Import Namespace="System.Collections.Generic" %>

<script runat="server">
    // Pages use <base href="~/"> so plain asset links resolve from the site root.
    // Side effect: any URL ASP.NET renders *relative to the page* is also resolved
    // from the root by the browser, and breaks. Two places do that:
    //   1. the form's action ("./Page.aspx")       -> postbacks go to /Page.aspx
    //   2. server controls with "~/" URLs           -> e.g. "~/Features/Accounts/Register.aspx"
    //      are rendered as "Register.aspx" on Login.aspx -> /Register.aspx
    // Both are rewritten to absolute paths before the page renders.
    // Day-before reminder emails (webConfig/ReminderService.cs)
    void Application_Start(object sender, EventArgs e)
    {
        DocCare_Clinic.Config.ReminderService.Start();
    }

    void Application_End(object sender, EventArgs e)
    {
        DocCare_Clinic.Config.ReminderService.Stop();
    }

    void Application_PreRequestHandlerExecute(object sender, EventArgs e)
    {
        var page = Context.Handler as System.Web.UI.Page;
        if (page == null) return;

        // Links in reminder emails need the site's address (webConfig/SiteAddress.cs)
        DocCare_Clinic.Config.SiteAddress.Remember(Request);

        if (!IsAllowed(page)) return;

        page.PreRender += (s, args) =>
        {
            if (page.Form != null)
                page.Form.Action = page.Request.RawUrl;
        };

        // PreRenderComplete: after data binding, so grid/repeater rows are included
        page.PreRenderComplete += (s, args) => MakeAppRelativeUrlsAbsolute(page);
    }

    // ── Page access by role ─────────────────────────────────────────────────────
    // Session["UserRole"] is set at login: SuperAdmin, Receptionist, Doctor or User (patient).
    // Every page must be listed here. A page that is not listed is admin-only,
    // so a new page stays closed until it is given its roles.
    const string Admin = "SuperAdmin", Receptionist = "Receptionist", Doctor = "Doctor", Patient = "User";
    static readonly string[] Everyone = { "*" };
    static readonly string[] AllStaff = { Admin, Receptionist, Doctor };

    static readonly Dictionary<string, string[]> PageRoles = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        // Public site
        { "~/Default.aspx", Everyone },
        { "~/Features/Site/Default.aspx", Everyone },
        { "~/Features/Site/About.aspx", Everyone },
        { "~/Features/Site/Blog.aspx", Everyone },
        { "~/Features/Site/Contact.aspx", Everyone },
        { "~/Features/Site/Service.aspx", Everyone },
        { "~/Features/Accounts/Login.aspx", Everyone },
        { "~/Features/Accounts/Logout.aspx", Everyone },
        { "~/Features/Accounts/Register.aspx", Everyone },
        { "~/Features/Accounts/Forget-Password.aspx", Everyone },
        { "~/Features/Doctors/Doctors-Team.aspx", Everyone },
        { "~/Features/Doctors/Doctor-details.aspx", Everyone },

        // Patient
        { "~/Features/Accounts/UserProfile.aspx", new[] { Patient } },
        { "~/Features/Accounts/EditUser.aspx", new[] { Patient } },
        { "~/Features/Accounts/ChangePasswordUser.aspx", new[] { Patient } },
        { "~/Features/Appointments/Confirm-Appointment.aspx", new[] { Patient } },
        { "~/Features/Appointments/MyAppointment.aspx", new[] { Patient } },
        { "~/Features/Appointments/Reschedule.aspx", new[] { Patient } },
        { "~/Features/Appointments/BookAppointments.aspx", new[] { Patient } },

        // All staff
        { "~/Features/Accounts/ChangePassword.aspx", AllStaff },
        { "~/Features/Accounts/EditProfile.aspx", AllStaff },
        { "~/Features/Site/Settings.aspx", AllStaff },
        { "~/Features/Appointments/Appointments.aspx", AllStaff },
        { "~/Features/Appointments/EditAppointment.aspx", AllStaff },
        { "~/Features/Patients/Patients.aspx", AllStaff },
        { "~/Features/Patients/AddPatient.aspx", AllStaff },
        { "~/Features/Patients/EditPatient.aspx", AllStaff },
        { "~/Features/Patients/PatientProfile.aspx", AllStaff },
        // Medical record of one visit: what each role may see or change is decided in MedicalRecords
        { "~/Features/Patients/VisitNotes.aspx", AllStaff },
        { "~/Features/Patients/Prescription.aspx", new[] { Admin, Doctor } },
        // Combining two records of the same person
        { "~/Features/Patients/MergePatients.aspx", new[] { Admin, Receptionist } },
        // Follow-ups to call about: date, doctor and patient only (doctors see their own patients)
        { "~/Features/Appointments/FollowUps.aspx", AllStaff },
        // Advances to give back on cancelled appointments
        { "~/Features/Appointments/Refunds.aspx", new[] { Admin, Receptionist } },
        { "~/Features/Doctors/Doctors.aspx", AllStaff },
        { "~/Features/Doctors/DoctorProfile.aspx", AllStaff },
        { "~/Features/Doctors/DoctorsAppoinment.aspx", AllStaff },

        // Admin + receptionist (front desk)
        { "~/Features/Appointments/BookAppointment.aspx", new[] { Admin, Receptionist } },
        { "~/Features/Appointments/AppointmentReport.aspx", new[] { Admin, Receptionist } },
        { "~/Features/Doctors/Manage_Doctor.aspx", new[] { Admin, Receptionist } },
        { "~/Features/Receptionists/ReceptionistDashboard.aspx", new[] { Receptionist } },

        // Doctor (own dashboard, profile and schedule; admin may also edit doctors)
        { "~/Features/Doctors/DoctorDashBoard.aspx", new[] { Doctor } },
        // doctors: their own hours; reception/admin: a chosen doctor (?DoctorID=, from Manage Doctor)
        { "~/Features/Scheduling/Schedule.aspx", new[] { Doctor, Receptionist, Admin } },
        { "~/Features/Doctors/EditDoctor.aspx", new[] { Admin, Doctor } },
        { "~/Features/Doctors/DoctorSetting.aspx", new[] { Admin, Doctor } },

        // Admin only
        { "~/Features/Admin/AdminDashBoard.aspx", new[] { Admin } },
        { "~/Features/Doctors/AddDoctor.aspx", new[] { Admin } },
        { "~/Features/Doctors/AssignDoctor.aspx", new[] { Admin } },
        { "~/Features/Receptionists/Receptionist.aspx", new[] { Admin } },
        { "~/Features/Receptionists/AddReceptionist.aspx", new[] { Admin } },
        { "~/Features/Receptionists/EditReceptionist.aspx", new[] { Admin } },
        { "~/Features/Departments/Departments.aspx", new[] { Admin } },
        { "~/Features/Departments/AddDepartment.aspx", new[] { Admin } },
        { "~/Features/Departments/EditDepartment.aspx", new[] { Admin } },
    };

    // Redirects (and returns false) when the current user may not open this page
    bool IsAllowed(System.Web.UI.Page page)
    {
        string path = Request.AppRelativeCurrentExecutionFilePath;
        string[] roles;
        if (!PageRoles.TryGetValue(path, out roles))
            roles = new[] { Admin };
        if (roles == Everyone) return true;

        string role = Context.Session != null ? Context.Session["UserRole"] as string : null;
        if (role != null && Array.IndexOf(roles, role) >= 0) return true;

        // Not logged in -> login page (and back here afterwards, e.g. from a link in an email);
        // logged in with the wrong role -> that role's home page
        string target = page.ResolveUrl(HomeFor(role));
        if (role == null) target += "?ReturnUrl=" + HttpUtility.UrlEncode(Request.RawUrl);
        Response.Redirect(target, false);
        Context.ApplicationInstance.CompleteRequest();
        return false;
    }

    static string HomeFor(string role)
    {
        switch (role)
        {
            case Admin: return "~/Features/Admin/AdminDashBoard.aspx";
            case Receptionist: return "~/Features/Receptionists/ReceptionistDashboard.aspx";
            case Doctor: return "~/Features/Doctors/DoctorDashBoard.aspx";
            case Patient: return "~/Features/Site/Default.aspx";
            default: return "~/Features/Accounts/Login.aspx";
        }
    }

    static void MakeAppRelativeUrlsAbsolute(Control parent)
    {
        foreach (Control c in parent.Controls)
        {
            var link = c as HyperLink;
            if (link != null)
            {
                link.NavigateUrl = Absolute(c, link.NavigateUrl);
                link.ImageUrl = Absolute(c, link.ImageUrl);
            }

            var image = c as System.Web.UI.WebControls.Image;   // also ImageButton
            if (image != null) image.ImageUrl = Absolute(c, image.ImageUrl);

            var button = c as IButtonControl;                    // Button, LinkButton, ImageButton
            if (button != null) button.PostBackUrl = Absolute(c, button.PostBackUrl);

            var anchor = c as System.Web.UI.HtmlControls.HtmlAnchor;
            if (anchor != null) anchor.HRef = Absolute(c, anchor.HRef);

            var img = c as System.Web.UI.HtmlControls.HtmlImage;
            if (img != null) img.Src = Absolute(c, img.Src);

            var css = c as System.Web.UI.HtmlControls.HtmlLink;
            if (css != null) css.Href = Absolute(c, css.Href);

            if (c.HasControls()) MakeAppRelativeUrlsAbsolute(c);
        }
    }

    static string Absolute(Control c, string url)
    {
        return !string.IsNullOrEmpty(url) && url.StartsWith("~/") ? c.ResolveUrl(url) : url;
    }
</script>
