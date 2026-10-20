<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true" CodeBehind="EditAppointment.aspx.cs" Inherits="DocCare_Clinic.EditAppointment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/Features/Appointments/Appointments.aspx") %>">Appointment</a></li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Edit Appointment</li>
                        </ul>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <asp:Panel ID="Panel1" runat="server">
                                <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>
                                <asp:Label ID="lblSuccess" runat="server" CssClass="text-success"></asp:Label>

                                    <div class="row">
                                        <div class="col-12">
                                            <div class="form-heading">
                                                <h4>Patient Details</h4>
                                            </div>
                                        </div>

                                        <div class="col-12 col-md-6 col-xl-6">
                                            <div class="input-block local-forms">
                                                <label>Full Name <span class="login-danger">*</span></label>
                                                <asp:HiddenField ID="HiddenFieldID" runat="server" />
                                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-12 col-md-6 col-xl-4">
                                            <div class="input-block select-gender">
                                                <label class="gen-label">Gender<span class="login-danger">*</span></label>
                                                <div class="form-check-inline">
                                                    <asp:RadioButton ID="rbMale" runat="server" GroupName="gender" Text="Male" Cssclass="form-check-label" />
                                                </div>
                                                <div class="form-check-inline">
                                                    <asp:RadioButton ID="rbFemale" runat="server" GroupName="gender" Text="Female" Cssclass="form-check-label" />
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-12 col-md-6 col-xl-6">
                                            <div class="input-block local-forms">
                                                <label>Mobile <span class="login-danger">*</span></label>
                                                <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-12 col-md-6 col-xl-6">
                                            <div class="input-block local-forms">
                                                <label>Email <span class="login-danger">*</span></label>
                                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-12 col-sm-12">
                                            <div class="input-block local-forms">
                                                <label>Address <span class="login-danger">*</span></label>
                                                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-12">
                                            <div class="form-heading">
                                                <h4>Appointment Details</h4>
                                            </div>
                                        </div>

                                        <div class="col-12 col-md-6 col-xl-5">
                                            <div class="input-block local-forms cal-icon">
                                                <label>Date of Appointment <span class="login-danger">*</span></label>
                                                <asp:TextBox ID="txtAppointmentDate" runat="server" CssClass="form-control datetimepicker"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-12 col-md-6 col-xl-4">
                                            <div class="input-block local-forms">
                                                <label>Time <span class="login-danger">*</span></label>
                                                <asp:TextBox ID="txtAppointmentTime" runat="server" CssClass="form-control" TextMode="Time"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-12 col-md-6 col-xl-6">
                                            <div class="input-block local-forms">
                                                <label>Consulting Doctor <span class="login-danger">*</span></label>
                                                <asp:DropDownList ID="ddlConsultDoctor" runat="server" CssClass="form-control select">
                                                    <asp:ListItem Text="Select Consulting Doctor" Value="" />
                                                    <asp:ListItem Text="Dr. Bernardo James" Value="Dr. Bernardo James" />
                                                    <asp:ListItem Text="Dr. Andrea Lalema" Value="Dr. Andrea Lalema" />
                                                    <asp:ListItem Text="Dr. William Stephin" Value="Dr. William Stephin" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-12 col-md-6 col-xl-6">
                                            <div class="input-block local-forms">
                                                <label>Treatment <span class="login-danger">*</span></label>
                                                <asp:TextBox ID="txtTreatment" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-12">
                                            <div class="doctor-submit text-end">
                                                <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary submit-form me-2" Text="Submit" />
                                                <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-primary cancel-form" Text="Cancel" />
                                            </div>
                                        </div>
                                    </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="notification-box">
                            <div class="msg-sidebar notifications msg-noti">
                                <div class="topnav-dropdown-header">
                                    <span>Messages</span>
                                </div>
                                <div class="drop-scroll msg-list-scroll" id="msg_list">
                                    <ul class="list-box">
                                        <li>
                                            <a href="javascript:void(0);">
                                                <div class="list-item">
                                                    <div class="list-left">
                                                        <span class="avatar">R</span>
                                                    </div>
                                                    <div class="list-body">
                                                        <span class="message-author">Richard Miles</span>
                                                        <span class="message-time">12:28 AM</span>
                                                        <span class="message-content">Lorem ipsum dolor sit amet, consectetur adipiscing</span>
                                                    </div>
                                                </div>
                                            </a>
                                        </li>
                                        <!-- Add similar messages as needed -->
                                    </ul>
                                </div>
                                <div class="topnav-dropdown-footer">
                                    <a href="javascript:void(0);">See all messages</a>
                                </div>
                            </div>
                        </div>
    </div>
</asp:Content>


