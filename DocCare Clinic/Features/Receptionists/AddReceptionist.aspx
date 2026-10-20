<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="AddReceptionist.aspx.cs" Inherits="DocCare_Clinic.AddReceptionist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--<style>
    .custom-listbox {
        border: 2px solid #E9EAF6; 
        border-radius: 7px;       
        background-color: #ffffff;         
        padding: 7px;                         
    }    
</style>--%>
    <div class="page-wrapper">
        <div class="content">
            <!-- Breadcrumb -->
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item">
                                <asp:HyperLink ID="lnkDoctors" NavigateUrl="~/Features/Receptionists/Receptionist.aspx" runat="server">Receptionists</asp:HyperLink>
                            </li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Add Receptionist</li>
                        </ul>
                    </div>
                </div>
            </div>
            <!-- Doctor Form -->
            <div class="row">
                <div class="col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <asp:Panel ID="DoctorFormPanel" runat="server">
                                <div id="frmDoctorDetails">
                                    <div class="row">
                                        <div class="col-12">
                                            <div class="form-heading">
                                                <h4>
                                                    Receptionist Details</h4>
                                            </div>
                                        </div>
                                        <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" ForeColor="Red"
                                            Font-Size="13px" Style="margin-bottom: 10px; display: block;"></asp:Label>
                                        <!-- Name -->
                                        <div class="col-12 col-md-6 col-xl-4">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblName" AssociatedControlID="txtName" runat="server" Text="Name *"></asp:Label>
                                                <asp:TextBox ID="txtName" CssClass="form-control" runat="server" placeholder="Enter name"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Mobile -->
                                        <div class="col-12 col-md-6 col-xl-6">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblMobile" AssociatedControlID="txtMobile" runat="server" Text="Mobile *"></asp:Label>
                                                <asp:TextBox ID="txtMobile" CssClass="form-control" runat="server" placeholder="Enter mobile"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Email -->
                                        <div class="col-12 col-md-6 col-xl-6">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblEmail" AssociatedControlID="txtEmail" runat="server" Text="Email *"></asp:Label>
                                                <asp:TextBox ID="txtEmail" CssClass="form-control" runat="server" placeholder="Enter email"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Password -->
                                        <div class="col-12 col-md-6 col-xl-6">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblPassword" AssociatedControlID="txtPassword" runat="server" Text="Password *"></asp:Label>
                                                <asp:TextBox ID="txtPassword" CssClass="form-control" runat="server" TextMode="Password"
                                                    placeholder="Enter password"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Submit and Cancel Buttons -->
                                        <div class="col-12">
                                            <div class="doctor-submit text-end">
                                                <asp:Button ID="btnSubmit" CssClass="btn btn-primary submit-form me-2" Text="Submit"
                                                    runat="server" OnClick="btnSubmit_Click" />
                                                <asp:Button ID="btnCancel" CssClass="btn btn-primary cancel-form" Text="Cancel" runat="server"
                                                    OnClick="btnCancel_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Notification Box -->
            <div class="notification-box">
                <div class="msg-sidebar notifications msg-noti">
                    <div class="topnav-dropdown-header">
                        <span>Messages</span>
                    </div>
                    <div class="drop-scroll msg-list-scroll" id="msg_list">
                        <ul class="list-box">
                            <asp:Repeater ID="rptMessages" runat="server">
                                <ItemTemplate>
                                    <li>
                                        <asp:HyperLink ID="HyperLink1" NavigateUrl='<%# Eval("ChatUrl") %>' runat="server">
                                                                <div class="list-item <%# Eval("IsNewMessage") %>">
                                                                    <div class="list-left">
                                                                        <span class="avatar"><%# Eval("Avatar") %></span>
                                                                    </div>
                                                                    <div class="list-body">
                                                                        <span class="message-author"><%# Eval("Author") %></span>
                                                                        <span class="message-time"><%# Eval("Time") %></span>
                                                                        <div class="clearfix"></div>
                                                                        <span class="message-content"><%# Eval("Content") %></span>
                                                                    </div>
                                                                </div>
                                        </asp:HyperLink>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                    <div class="topnav-dropdown-footer">
                        <asp:HyperLink ID="lnkSeeAllMessages" NavigateUrl="javascript:void(0);" runat="server">See all messages</asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
