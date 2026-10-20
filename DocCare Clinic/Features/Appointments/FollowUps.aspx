<%@ Page Title="Follow-ups" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="FollowUps.aspx.cs" Inherits="DocCare_Clinic.FollowUps" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .fu-note { font-size: 13px; color: #6b7280; margin: -4px 0 14px; }
        .fu-bar { display: flex; flex-wrap: wrap; align-items: center; gap: 8px; margin-bottom: 14px; }
        .fu-chip { padding: 7px 14px; border-radius: 20px; border: 1.5px solid #c7d2fe; background: #eef2ff; color: #2E37A4 !important;
                   font-size: 13px; font-weight: 600; text-decoration: none; white-space: nowrap; }
        .fu-chip.active { background: #2E37A4; border-color: #2E37A4; color: #fff !important; }
        .fu-count { margin-left: auto; font-size: 13px; color: #6b7280; }
        .fu-table { width: 100%; border-collapse: collapse; font-size: 14px; }
        .fu-table th { font-size: 12.5px; color: #6b7280; font-weight: 600; text-align: left; padding: 10px 12px; border-bottom: 1px solid #e5e7eb; white-space: nowrap; }
        .fu-table td { padding: 12px; border-bottom: 1px solid #f1f2f6; vertical-align: middle; }
        .fu-due { font-weight: 600; color: #111827; white-space: nowrap; }
        .fu-badge { display: inline-block; margin-top: 3px; padding: 2px 9px; border-radius: 10px; font-size: 11.5px; font-weight: 600; }
        .fu-badge.overdue { background: #fef3c7; color: #92400e; }
        .fu-badge.today { background: #dcfce7; color: #166534; }
        .fu-badge.soon { background: #eef2ff; color: #2E37A4; }
        .fu-small { display: block; font-size: 12px; color: #6b7280; }
        .fu-done { font-size: 13px; font-weight: 600; color: #b91c1c !important; white-space: nowrap; }
        .fu-empty { padding: 26px 12px; text-align: center; color: #6b7280; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/Features/Appointments/Appointments.aspx") %>">Appointment</a></li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Follow-ups</li>
                        </ul>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-body">
                    <div class="form-heading"><h4>Follow-ups</h4></div>
                    <p class="fu-note">Patients the doctor asked to see again who have not booked yet. Booking a visit with that doctor removes them from the list.
                        Patients with an email address get a reminder <%= DocCare_Clinic.Config.ReminderService.FollowUpReminderDaysBefore %> days before the date.</p>

                    <div class="fu-bar">
                        <asp:LinkButton ID="lnkWeek" runat="server" CssClass="fu-chip" CommandArgument="week" OnCommand="Filter_Command">Due this week</asp:LinkButton>
                        <asp:LinkButton ID="lnkOverdue" runat="server" CssClass="fu-chip" CommandArgument="overdue" OnCommand="Filter_Command">Overdue</asp:LinkButton>
                        <asp:LinkButton ID="lnkAll" runat="server" CssClass="fu-chip" CommandArgument="all" OnCommand="Filter_Command">All open</asp:LinkButton>
                        <span class="fu-count"><asp:Literal ID="litCount" runat="server" /></span>
                    </div>

                    <div class="table-responsive">
                        <table class="fu-table">
                            <thead>
                                <tr><th>Due</th><th>Patient</th><th>Mobile</th><th>Doctor</th><th>Last visit</th><th>Reminder</th><th></th></tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptFollowUps" runat="server" OnItemCommand="rptFollowUps_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><span class="fu-due"><%# Eval("FollowUpDate", "{0:ddd, dd MMM yyyy}") %></span><br /><%# DueBadge(Eval("FollowUpDate")) %></td>
                                            <td><a href='<%# ResolveUrl("~/Features/Patients/PatientProfile.aspx?Id=" + Eval("PatientID")) %>'><%# HttpUtility.HtmlEncode(Eval("PatientName")) %></a>
                                                <span class="patient-code"><%# Eval("PatientCode") %></span></td>
                                            <td><%# MobileLink(Eval("Mobile")) %></td>
                                            <td>Dr. <%# HttpUtility.HtmlEncode(DocCare_Clinic.Config.DoctorName.WithoutTitle(Eval("Doctor").ToString())) %>
                                                <span class="fu-small"><%# HttpUtility.HtmlEncode(Eval("Specialization")) %></span></td>
                                            <td><%# Eval("VisitDate", "{0:dd MMM yyyy}") %></td>
                                            <td><%# ReminderText(Container.DataItem) %></td>
                                            <td><asp:LinkButton ID="lnkClose" runat="server" CssClass="fu-done" CommandName="Close" CommandArgument='<%# Eval("AppointmentID") %>'
                                                    OnClientClick="return confirm('Remove this follow-up from the list? Use this when it is no longer needed (for example the patient declined).');"
                                                    ToolTip="The follow-up is no longer needed">Not needed</asp:LinkButton></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="fu-empty">
                            <asp:Literal ID="litEmpty" runat="server" />
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
