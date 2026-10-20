<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="BookAppointment.aspx.cs" Inherits="DocCare_Clinic.BookAppointment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .find-patient { background: #f5f7ff; border: 1px solid #dfe3fb; border-radius: 12px; padding: 14px 16px; margin: 0 0 22px; }
        .find-patient label { display: block; font-weight: 600; font-size: 14px; color: #2E37A4; margin-bottom: 8px; }
        .find-row { display: flex; gap: 10px; max-width: 520px; }
        .find-row .form-control { min-height: 42px; }
        .find-result { display: block; font-size: 13px; margin-top: 8px; color: #6b7280; }
        .find-matches { display: flex; flex-wrap: wrap; gap: 8px; margin-top: 8px; }
        .find-match { border: 1.5px solid #c7d2fe; background: #fff; border-radius: 10px; padding: 7px 12px; color: #111827 !important; text-decoration: none; }
        .find-match:hover { border-color: #2E37A4; }
        .find-match span { display: block; font-size: 12px; color: #6b7280; }
        .returning-patient { display: flex; flex-wrap: wrap; justify-content: space-between; gap: 8px; align-items: center;
                             background: #ecfdf5; border: 1px solid #a7f3d0; color: #065f46; border-radius: 12px; padding: 12px 16px; margin: 0 0 22px; font-size: 14px; }
        .returning-patient a { font-size: 13px; font-weight: 600; color: #2E37A4; }
        .time-picker { margin: 0 0 20px; }
        .time-picker-label { font-size: 14px; font-weight: 600; color: #333448; margin-bottom: 8px; }
        .slot-info { font-size: 13px; font-weight: 500; margin-left: 8px; }
        .slot-info.ok { color: #047857; }
        .slot-info.none { color: #b91c1c; }
        .time-chips { display: flex; flex-wrap: wrap; gap: 8px; }
        .time-chips input[type="radio"] { position: absolute; opacity: 0; width: 1px; height: 1px; }
        .time-chips label { display: inline-block; padding: 8px 14px; border-radius: 10px; border: 1.5px solid #c7d2fe; background: #eef2ff;
                            color: #2E37A4; font-size: 13.5px; font-weight: 600; cursor: pointer; margin: 0; }
        .time-chips label:hover { border-color: #2E37A4; }
        .time-chips input[type="radio"]:checked + label { background: #2E37A4; border-color: #2E37A4; color: #fff; }
        .time-chips input[type="radio"]:focus-visible + label { outline: 2px solid #2E37A4; outline-offset: 2px; }
    </style>
    <div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/Features/Appointments/Appointments.aspx") %>">Appointment </a></li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Book Appointment</li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-12">
                                    <div class="form-heading">
                                        <h4>
                                            Patient Details</h4>
                                    </div>
                                </div>
                                <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" ForeColor="Red"
                                        Font-Size="13px" Style="margin-bottom: 10px; display: block;"></asp:Label>

                                <%-- Returning patient: find the existing record so the patient keeps one Patient ID --%>
                                <div class="col-12">
                                    <asp:Panel ID="pnlFind" runat="server" CssClass="find-patient" DefaultButton="btnFindPatient">
                                        <label for="<%= txtFindPatient.ClientID %>">Returning patient?</label>
                                        <div class="find-row">
                                            <asp:TextBox ID="txtFindPatient" runat="server" CssClass="form-control" MaxLength="20"
                                                placeholder="Patient ID (e.g. DC-00029) or mobile number"></asp:TextBox>
                                            <asp:Button ID="btnFindPatient" runat="server" CssClass="btn btn-primary" Text="Find" OnClick="btnFindPatient_Click" CausesValidation="false" />
                                        </div>
                                        <asp:Label ID="lblFindResult" runat="server" CssClass="find-result"></asp:Label>
                                        <asp:Repeater ID="rptMatches" runat="server" OnItemCommand="rptMatches_ItemCommand">
                                            <HeaderTemplate><div class="find-matches"></HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkPick" runat="server" CssClass="find-match" CommandName="Pick"
                                                    CommandArgument='<%# Eval("PatientID") %>' CausesValidation="false">
                                                    <strong><%# HttpUtility.HtmlEncode(Eval("PatientName")) %></strong>
                                                    <span><%# Eval("PatientCode") %> &middot; <%# Eval("Gender") %>, <%# Eval("Age") %> yrs</span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                            <FooterTemplate></div></FooterTemplate>
                                        </asp:Repeater>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlReturning" runat="server" Visible="false" CssClass="returning-patient">
                                        <span>Returning patient: <strong><asp:Literal ID="litReturningName" runat="server" /></strong>
                                            &middot; Patient ID <strong><asp:Literal ID="litReturningCode" runat="server" /></strong></span>
                                        <asp:LinkButton ID="lnkNewPatient" runat="server" OnClick="lnkNewPatient_Click" CausesValidation="false">Book a new patient instead</asp:LinkButton>
                                    </asp:Panel>
                                    <asp:HiddenField ID="hdnExistingPatientID" runat="server" />
                                </div>
                                <div class="col-12 col-md-6 col-xl-4">
                                    <div class="input-block local-forms">
                                        <label>
                                            Name <span class="login-danger">*</span></label>
                                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                                    </div>
                                </div>
                                <div class="col-12 col-md-6 col-xl-4">
                                    <div class="input-block local-forms">
                                        <label>
                                            Date Of Birth <span class="login-danger">*</span>
                                        </label>
                                        <asp:TextBox ID="txtDob" CssClass="form-control" runat="server" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-12 col-md-6 col-xl-4">
                                    <div class="input-block local-forms">
                                        <label>
                                            Age <span class="login-danger">*</span>
                                        </label>
                                        <asp:TextBox ID="txtAge" CssClass="form-control" runat="server" Placeholder="Enter Age"
                                            ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-12 col-md-6 col-xl-2">
                                    <div class="input-block select-gender">
                                        <label class="gen-label">
                                            Gender<span class="login-danger">*</span></label>
                                        <div class="form-check-inline">
                                            <asp:RadioButton ID="rbMale" runat="server" GroupName="gender" Text="Male" CssClass="form-check-label" />
                                        </div>
                                        <div class="form-check-inline">
                                            <asp:RadioButton ID="rbFemale" runat="server" GroupName="gender" Text="Female" CssClass="form-check-label" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-12 col-md-6 col-xl-4">
                                    <div class="input-block local-forms">
                                        <label>
                                            Mobile <span class="login-danger">*</span></label>
                                        <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" />
                                    </div>
                                </div>
                                <div class="col-12 col-sm-6">
                                    <div class="input-block local-forms">
                                        <label>
                                            Address <span class="login-danger">*</span></label>
                                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine"
                                            Rows="3" />
                                    </div>
                                </div>
                                <div class="col-12">
                                    <div class="form-heading">
                                        <h4>
                                            Appointment Details</h4>
                                    </div>
                                </div>
                                <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
                                </asp:ScriptManagerProxy>
                                <%-- Today (walk-in) or a later day (phone, follow-up): date -> doctors working that day -> free times --%>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col-12 col-md-4 col-xl-3">
                                                <div class="input-block local-forms">
                                                    <label>Date of Appointment <span class="login-danger">*</span></label>
                                                    <asp:TextBox ID="txtAppointDate" CssClass="form-control" runat="server" TextMode="Date"
                                                        AutoPostBack="true" OnTextChanged="txtAppointDate_TextChanged"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-12 col-md-8 col-xl-5">
                                                <div class="input-block local-forms">
                                                    <label>Consulting Doctor <span class="login-danger">*</span></label>
                                                    <asp:DropDownList ID="ddlConsultDoctor" runat="server" CssClass="form-control select"
                                                        AutoPostBack="true" OnSelectedIndexChanged="ddlConsultDoctor_SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-12">
                                                <div class="time-picker">
                                                    <div class="time-picker-label">Time <span class="login-danger">*</span>
                                                        <asp:Label ID="lblSlotInfo" runat="server" CssClass="slot-info"></asp:Label></div>
                                                    <asp:RadioButtonList ID="rblTime" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="time-chips" />
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div class="col-12">
                                    <div class="doctor-submit text-end">
                                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary submit-form me-2"
                                            OnClick="btnSubmit_Click" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-primary cancel-form"
                                            OnClick="btnCancel_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
    document.addEventListener('DOMContentLoaded', function () {
        const dobInput = document.getElementById('<%= txtDob.ClientID %>');
        const ageInput = document.getElementById('<%= txtAge.ClientID %>');

        dobInput.addEventListener('change', function () {
            const dob = new Date(dobInput.value);
            if (!isNaN(dob)) {
                const today = new Date();
                let age = today.getFullYear() - dob.getFullYear();
                const monthDiff = today.getMonth() - dob.getMonth();
                const dayDiff = today.getDate() - dob.getDate();

                // Adjust age if the birthday hasn't occurred yet this year
                if (monthDiff < 0 || (monthDiff === 0 && dayDiff < 0)) {
                    age--;
                }

                ageInput.value = age; // Set the calculated age
            } else {
                ageInput.value = ''; // Clear age if DOB is invalid
            }
        });
    });
    </script>
</asp:Content>
