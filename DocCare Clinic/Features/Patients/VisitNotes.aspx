<%@ Page Title="Visit Notes" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="VisitNotes.aspx.cs" Inherits="DocCare_Clinic.VisitNotes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .visit-head { display: flex; flex-wrap: wrap; gap: 18px 40px; align-items: flex-start; justify-content: space-between; }
        .visit-head h3 { font-size: 20px; font-weight: 700; color: #111827; margin: 0 0 4px; }
        .visit-head .sub { font-size: 13.5px; color: #6b7280; }
        .visit-head .when { font-size: 14px; color: #2E37A4; font-weight: 600; margin-top: 6px; }
        .visit-status { display: inline-block; padding: 4px 12px; border-radius: 20px; font-size: 12.5px; font-weight: 600; background: #eef2ff; color: #2E37A4; margin-left: 6px; }
        .allergy-banner { display: flex; gap: 10px; align-items: flex-start; background: #fef2f2; border: 1px solid #fecaca; color: #991b1b;
                          border-radius: 12px; padding: 12px 16px; margin: 0 0 18px; font-size: 14px; }
        .allergy-banner i { margin-top: 3px; }
        .section-title { font-size: 16px; font-weight: 700; color: #111827; margin: 0 0 4px; }
        .section-note { font-size: 12.5px; color: #6b7280; margin: 0 0 14px; }
        .vitals-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 14px; }
        .field label { display: block; font-size: 13px; font-weight: 600; color: #374151; margin-bottom: 6px; }
        .field .form-control { min-height: 42px; }
        .field textarea.form-control { min-height: 84px; }
        .rx-table { width: 100%; border-collapse: separate; border-spacing: 0 8px; }
        .rx-table th { font-size: 12.5px; font-weight: 600; color: #6b7280; padding: 0 6px; text-align: left; white-space: nowrap; }
        .rx-table td { padding: 0 6px; vertical-align: middle; }
        .rx-table .form-control { min-height: 40px; }
        .rx-remove { color: #b91c1c !important; font-size: 13px; font-weight: 600; white-space: nowrap; }
        .rx-add { font-size: 13.5px; font-weight: 600; color: #2E37A4 !important; }
        .readonly-note { background: #f9fafb; border: 1px solid #e5e7eb; border-radius: 10px; padding: 10px 14px; font-size: 13px; color: #4b5563; margin-bottom: 16px; }
        .visit-actions { display: flex; flex-wrap: wrap; gap: 10px; justify-content: flex-end; margin-top: 18px; }
        .recorded { font-size: 12px; color: #6b7280; margin-top: 8px; }
        @media (max-width: 767px) { .rx-table thead { display: none; } .rx-table tr { display: grid; grid-template-columns: 1fr 1fr; gap: 6px; margin-bottom: 12px; } }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><asp:HyperLink ID="lnkPatient" runat="server">Patient</asp:HyperLink></li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Visit Notes</li>
                        </ul>
                    </div>
                </div>
            </div>

            <%-- Who and when --%>
            <div class="card">
                <div class="card-body visit-head">
                    <div>
                        <h3><asp:Literal ID="litPatient" runat="server" /></h3>
                        <div class="sub"><asp:Literal ID="litPatientSub" runat="server" /></div>
                    </div>
                    <div>
                        <h3 style="font-size: 17px;"><asp:Literal ID="litDoctor" runat="server" /></h3>
                        <div class="when"><asp:Literal ID="litWhen" runat="server" /></div>
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnlAllergy" runat="server" Visible="false" CssClass="allergy-banner">
                <i class="fa-solid fa-triangle-exclamation"></i>
                <span><strong>Allergies:</strong> <asp:Literal ID="litAllergies" runat="server" /></span>
            </asp:Panel>

            <asp:Panel ID="pnlReadOnly" runat="server" Visible="false" CssClass="readonly-note">
                <asp:Literal ID="litReadOnly" runat="server" />
            </asp:Panel>

            <%-- Vitals: reception or the doctor --%>
            <div class="card">
                <div class="card-body">
                    <div class="section-title">Vitals</div>
                    <div class="section-note">Leave a box empty if it was not measured.</div>
                    <div class="vitals-grid">
                        <div class="field"><label for="<%= txtBp.ClientID %>">Blood pressure (mmHg)</label>
                            <asp:TextBox ID="txtBp" runat="server" CssClass="form-control" MaxLength="9" placeholder="120/80" /></div>
                        <div class="field"><label for="<%= txtPulse.ClientID %>">Pulse (per min)</label>
                            <asp:TextBox ID="txtPulse" runat="server" CssClass="form-control" TextMode="Number" placeholder="72" /></div>
                        <div class="field"><label for="<%= txtTemp.ClientID %>">Temperature (&deg;F)</label>
                            <asp:TextBox ID="txtTemp" runat="server" CssClass="form-control" placeholder="98.6" MaxLength="5" /></div>
                        <div class="field"><label for="<%= txtWeight.ClientID %>">Weight (kg)</label>
                            <asp:TextBox ID="txtWeight" runat="server" CssClass="form-control" placeholder="65" MaxLength="5" /></div>
                        <div class="field"><label for="<%= txtSpO2.ClientID %>">SpO2 (%)</label>
                            <asp:TextBox ID="txtSpO2" runat="server" CssClass="form-control" TextMode="Number" placeholder="98" /></div>
                    </div>
                    <div class="recorded"><asp:Literal ID="litVitalsBy" runat="server" /></div>
                </div>
            </div>

            <%-- Doctor's notes: never sent to reception --%>
            <asp:Panel ID="pnlClinical" runat="server" CssClass="card">
                <div class="card-body">
                    <div class="section-title">Consultation</div>
                    <div class="section-note">Seen only by doctors and the admin.</div>
                    <div class="row">
                        <div class="col-md-6 field mb-3"><label for="<%= txtComplaint.ClientID %>">Complaint / symptoms</label>
                            <asp:TextBox ID="txtComplaint" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="1000" placeholder="e.g. Fever and cough for 3 days" /></div>
                        <div class="col-md-6 field mb-3"><label for="<%= txtDiagnosis.ClientID %>">Diagnosis</label>
                            <asp:TextBox ID="txtDiagnosis" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="1000" placeholder="e.g. Viral upper respiratory infection" /></div>
                    </div>

                    <div class="section-title" style="font-size: 15px; margin-top: 6px;">Prescription</div>
                    <div class="table-responsive">
                        <table class="rx-table">
                            <thead><tr><th>Medicine</th><th>Dose</th><th>How often</th><th>Days</th><th>Instructions</th><th></th></tr></thead>
                            <tbody>
                                <asp:Repeater ID="rptRx" runat="server" OnItemCommand="rptRx_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td style="min-width: 190px;"><asp:TextBox ID="txtMedicine" runat="server" CssClass="form-control" MaxLength="150" Text='<%# Eval("Medicine") %>' placeholder="Medicine name" /></td>
                                            <td style="min-width: 110px;"><asp:TextBox ID="txtDose" runat="server" CssClass="form-control" MaxLength="60" Text='<%# Eval("Dose") %>' placeholder="500 mg" /></td>
                                            <td style="min-width: 120px;"><asp:TextBox ID="txtFrequency" runat="server" CssClass="form-control" MaxLength="60" Text='<%# Eval("Frequency") %>' placeholder="1-0-1" list="freqList" /></td>
                                            <td style="width: 90px;"><asp:TextBox ID="txtDays" runat="server" CssClass="form-control" TextMode="Number" Text='<%# Eval("Days") %>' placeholder="5" /></td>
                                            <td style="min-width: 150px;"><asp:TextBox ID="txtInstructions" runat="server" CssClass="form-control" MaxLength="200" Text='<%# Eval("Instructions") %>' placeholder="After food" list="instrList" /></td>
                                            <td><asp:LinkButton ID="lnkRemove" runat="server" CssClass="rx-remove" CommandName="Remove" CommandArgument='<%# Container.ItemIndex %>' CausesValidation="false" Visible='<%# CanWriteClinical %>'>Remove</asp:LinkButton></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <asp:LinkButton ID="lnkAddRx" runat="server" CssClass="rx-add" OnClick="lnkAddRx_Click" CausesValidation="false">+ Add medicine</asp:LinkButton>
                    <datalist id="freqList">
                        <option value="1-0-1">Morning and night</option>
                        <option value="1-1-1">Morning, afternoon and night</option>
                        <option value="1-0-0">Morning</option>
                        <option value="0-0-1">Night</option>
                        <option value="0-1-0">Afternoon</option>
                        <option value="SOS">Only when needed</option>
                    </datalist>
                    <datalist id="instrList">
                        <option value="After food"></option>
                        <option value="Before food"></option>
                        <option value="With water"></option>
                        <option value="At bedtime"></option>
                    </datalist>

                    <div class="row" style="margin-top: 16px;">
                        <div class="col-md-8 field mb-3"><label for="<%= txtAdvice.ClientID %>">Advice / notes</label>
                            <asp:TextBox ID="txtAdvice" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="2000" placeholder="e.g. Rest, drink plenty of fluids, avoid cold drinks" /></div>
                        <div class="col-md-4 field mb-3"><label for="<%= txtFollowUp.ClientID %>">Follow-up date</label>
                            <asp:TextBox ID="txtFollowUp" runat="server" CssClass="form-control" TextMode="Date" /></div>
                    </div>
                    <div class="recorded"><asp:Literal ID="litNotesBy" runat="server" /></div>
                </div>
            </asp:Panel>

            <div class="visit-actions">
                <asp:HyperLink ID="lnkPrint" runat="server" CssClass="btn btn-outline-primary" Target="_blank" Visible="false">
                    <i class="fa-solid fa-print"></i>&nbsp; Print prescription</asp:HyperLink>
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
                <asp:Button ID="btnSaveComplete" runat="server" CssClass="btn btn-success" Text="Save &amp; Mark Completed" OnClick="btnSaveComplete_Click" />
            </div>
        </div>
    </div>
</asp:Content>
