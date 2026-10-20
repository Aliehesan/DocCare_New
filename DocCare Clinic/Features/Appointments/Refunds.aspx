<%@ Page Title="Refunds" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="Refunds.aspx.cs" Inherits="DocCare_Clinic.Refunds" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .rf-note { font-size: 13px; color: #6b7280; margin: -4px 0 14px; }
        .rf-bar { display: flex; flex-wrap: wrap; align-items: center; gap: 8px; margin-bottom: 14px; }
        .rf-chip { padding: 7px 14px; border-radius: 20px; border: 1.5px solid #c7d2fe; background: #eef2ff; color: #2E37A4 !important;
                   font-size: 13px; font-weight: 600; text-decoration: none; white-space: nowrap; }
        .rf-chip.active { background: #2E37A4; border-color: #2E37A4; color: #fff !important; }
        .rf-total { margin-left: auto; font-size: 13.5px; color: #4b5563; }
        .rf-total strong { color: #b45309; }
        .rf-table { width: 100%; border-collapse: collapse; font-size: 14px; }
        .rf-table th { font-size: 12.5px; color: #6b7280; font-weight: 600; text-align: left; padding: 10px 12px; border-bottom: 1px solid #e5e7eb; white-space: nowrap; }
        .rf-table td { padding: 12px; border-bottom: 1px solid #f1f2f6; vertical-align: middle; }
        .rf-amount { font-weight: 700; color: #111827; white-space: nowrap; }
        .rf-small { display: block; font-size: 12px; color: #6b7280; }
        .rf-ref { font-family: Consolas, monospace; font-size: 12px; color: #4b5563; word-break: break-all; }
        .rf-action { display: flex; gap: 8px; align-items: center; min-width: 300px; }
        .rf-action .form-control { min-height: 36px; font-size: 13px; }
        .rf-done { font-size: 12.5px; color: #047857; font-weight: 600; }
        .rf-empty { padding: 26px 12px; text-align: center; color: #6b7280; }
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
                            <li class="breadcrumb-item active">Refunds</li>
                        </ul>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-body">
                    <div class="form-heading"><h4>Refunds</h4></div>
                    <p class="rf-note">Cancelled appointments where the patient paid the online advance. Return the money (UPI, cash ...),
                        then mark it refunded here with a note such as the UPI reference, so nobody is refunded twice or forgotten.</p>

                    <div class="rf-bar">
                        <asp:LinkButton ID="lnkDue" runat="server" CssClass="rf-chip" CommandArgument="due" OnCommand="Tab_Command">Due</asp:LinkButton>
                        <asp:LinkButton ID="lnkDone" runat="server" CssClass="rf-chip" CommandArgument="done" OnCommand="Tab_Command">Refunded</asp:LinkButton>
                        <span class="rf-total"><asp:Literal ID="litTotal" runat="server" /></span>
                    </div>

                    <div class="table-responsive">
                        <table class="rf-table">
                            <thead>
                                <tr><th>Patient</th><th>Mobile</th><th>Doctor</th><th>Appointment</th><th>Paid online</th><th>Payment reference</th>
                                    <th><asp:Literal ID="litActionHead" runat="server" /></th></tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptRefunds" runat="server" OnItemCommand="rptRefunds_ItemCommand" OnItemDataBound="rptRefunds_ItemDataBound">
                                    <ItemTemplate>
                                        <tr>
                                            <td><a href='<%# ResolveUrl("~/Features/Patients/PatientProfile.aspx?Id=" + Eval("PatientID")) %>'><%# HttpUtility.HtmlEncode(Eval("PatientName")) %></a>
                                                <span class="patient-code"><%# Eval("PatientCode") %></span></td>
                                            <td><%# MobileLink(Eval("Mobile")) %></td>
                                            <td>Dr. <%# HttpUtility.HtmlEncode(DocCare_Clinic.Config.DoctorName.WithoutTitle(Eval("Doctor").ToString())) %></td>
                                            <td><%# Eval("AppointDate", "{0:dd MMM yyyy}") %><span class="rf-small">Token <%# HttpUtility.HtmlEncode(Eval("TokenNo")) %></span></td>
                                            <td class="rf-amount">&#8377;<%# string.Format("{0:N0}", Eval("PaidAmount")) %></td>
                                            <td class="rf-ref"><%# HttpUtility.HtmlEncode(Eval("PaymentId")) %></td>
                                            <td>
                                                <asp:Panel ID="pnlMark" runat="server" CssClass="rf-action">
                                                    <asp:TextBox ID="txtNote" runat="server" CssClass="form-control" MaxLength="200" placeholder="Note, e.g. UPI ref" />
                                                    <asp:Button ID="btnMark" runat="server" CssClass="btn btn-primary btn-sm" Text="Mark refunded" CommandName="Mark"
                                                        CommandArgument='<%# Eval("ID") %>' OnClientClick="return confirm('Mark this advance as refunded? Do this after the money has been returned.');" />
                                                </asp:Panel>
                                                <asp:Literal ID="litDone" runat="server" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="rf-empty">
                            <asp:Literal ID="litEmpty" runat="server" />
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
