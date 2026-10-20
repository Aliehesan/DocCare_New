<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="PatientProfile.aspx.cs" Inherits="DocCare_Clinic.PatientProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item">
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Features/Patients/Patients.aspx">Patients</asp:HyperLink>
                            </li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Patients Profile </li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="about-info">
                                        <h4>
                                            Patient Profile <span></span>
                                        </h4>
                                    </div>
                                    <div class="doctor-profile-head">
                                        <div class="profile-bg-img">
                                            <asp:Image ID="imgBackground" runat="server" CssClass="profile-bg-img" ImageUrl="assets/img/profile-bg.jpg"
                                                AlternateText="Profile Background" />
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-6 col-xl-4 col-md-4">
                                                <div class="profile-user-box">
                                                    <div class="profile-user-img">
                                                        <img src="assets/img/profile-user-01.jpg" alt="Profile" />
                                                    </div>
                                                    <div class="names-profiles">
                                                        <h4>
                                                            <asp:Label ID="lblPatientName" runat="server"></asp:Label></h4>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <!-- Left Column -->
                        <div class="col-lg-4">
                            <!-- About Me Section -->
                            <div class="doctor-personals-grp">
                                <div class="card">
                                    <div class="card-body">
                                        <div class="heading-detail">
                                            <h4 class="mb-3">
                                                About me</h4>
                                        </div>
                                        <div class="about-me-list">
                                            <ul class="list-space">
                                                <li>
                                                    <h4>
                                                        Gender</h4>
                                                    <span>
                                                        <asp:Label ID="lblGender" runat="server"></asp:Label></span> </li>
                                                <li>
                                                    <h4>
                                                        Age</h4>
                                                    <span>
                                                        <asp:Label ID="lblAge" runat="server"></asp:Label></span> </li>
                                                <li>
                                                    <h4>
                                                        Date Of Birth</h4>
                                                    <span>
                                                        <asp:Label ID="lblDob" runat="server"></asp:Label></span> </li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!-- Right Column -->
                        <div class="col-lg-8">
                            <div class="doctor-personals-grp">
                                <!-- About Me and Settings Tab -->
                                <div class="card">
                                    <div class="card-body">
                                        <div class="tab-content-set">
                                            <ul class="nav">
                                                <li><a href="#" class="active">About me</a></li>
                                            </ul>
                                        </div>
                                        <!-- Personal Details -->
                                        <div class="personal-list-out">
                                            <div class="row">
                                                <div class="col-xl-3 col-md-6">
                                                    <div class="detail-personal">
                                                        <h2>
                                                            Full Name</h2>
                                                        <h3>
                                                            <asp:Label ID="lblFullName" runat="server"></asp:Label></h3>
                                                    </div>
                                                </div>
                                                <div class="col-xl-3 col-md-6">
                                                    <div class="detail-personal">
                                                        <h2>
                                                            Mobile</h2>
                                                        <h3>
                                                            <asp:Label ID="lblMobile" runat="server"></asp:Label></h3>
                                                    </div>
                                                </div>
                                                <div class="col-xl-3 col-md-6">
                                                    <div class="detail-personal">
                                                        <h2>
                                                            Email</h2>
                                                        <h3>
                                                            <asp:Label ID="lblEmail" runat="server"></asp:Label></h3>
                                                    </div>
                                                </div>
                                                <div class="col-xl-3 col-md-6">
                                                    <div class="detail-personal">
                                                        <h2>
                                                            Address</h2>
                                                        <h3>
                                                            <asp:Label ID="lblAddress" runat="server"></asp:Label></h3>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- Medical History Table -->
                                        <div class="card">
                                            <div class="card-header">
                                                <h4 class="card-title">
                                                    Medical History
                                                </h4>
                                            </div>
                                            <div class="card-body p-0 table-dash">
                                                <asp:GridView ID="gvMedicalHistory" runat="server" CssClass="table datatable custom-table patient-profile-table"
                                                    AutoGenerateColumns="False" EmptyDataText="No Medical Record Found." AllowPaging="True"
                                                    PageSize="5" AllowSorting="True" OnPageIndexChanging="gvMedicalHistory_PageIndexChanging"
                                                    OnSorting="gvMedicalHistory_Sorting">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Date" SortExpression="Date">
                                                            <ItemTemplate>
                                                                <%# Eval("Date", "{0:dd-MMM-yyyy}") %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField HeaderText="Consultant Doctor" DataField="Doctor" SortExpression="Doctor" />
                                                        <asp:TemplateField HeaderText="Charges ($)" SortExpression="Charges">
                                                            <ItemTemplate>
                                                                <%# String.Format("{0:C2}", Eval("Charges")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
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
                        <li><a href="javascript:void(0);">
                            <div class="list-item">
                                <div class="list-left">
                                    <span class="avatar">R</span>
                                </div>
                                <div class="list-body">
                                    <span class="message-author">Richard Miles</span> <span class="message-time">12:28 AM</span>
                                    <span class="message-content">Lorem ipsum dolor sit amet, consectetur adipiscing</span>
                                </div>
                            </div>
                        </a></li>
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
