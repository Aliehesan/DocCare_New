<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs" Inherits="DocCare_Clinic.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrapper">
        <!-- Page Content -->
        <div class="content container-fluid">
            <!-- Breadcrumb -->
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item">
                                <asp:HyperLink ID="lnkDashboard" NavigateUrl="~/Features/Admin/AdminDashBoard.aspx" runat="server">Dashboard</asp:HyperLink>
                            </li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Change Password</li>
                        </ul>
                    </div>
                </div>
            </div>
            <!-- Change Password Form -->
            <div class="row">
                <div class="col-lg-8">
                    <div class="card">
                        <div class="card-body">
                            <asp:Panel ID="ChangePasswordPanel" runat="server">
                                <h4 class="page-title">
                                    Change Password</h4>
                                <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" ForeColor="Red"
                                    Font-Size="13px" Style="margin-bottom: 10px; display: block;"></asp:Label>
                                <div class="row">
                                    <div class="col-12 col-md-6 col-xl-12">
                                        <div class="input-block local-forms">
                                            <asp:Label ID="lblOldPassword" AssociatedControlID="txtOldPassword" CssClass="form-label"
                                                runat="server" Text="Old Password"></asp:Label>
                                            <asp:TextBox ID="txtOldPassword" CssClass="form-control" TextMode="Password" runat="server"
                                                placeholder="Enter old password"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-12 col-md-6 col-xl-6">
                                        <div class="input-block local-forms">
                                            <asp:Label ID="lblNewPassword" AssociatedControlID="txtNewPassword" CssClass="form-label"
                                                runat="server" Text="New Password"></asp:Label>
                                            <asp:TextBox ID="txtNewPassword" CssClass="form-control" TextMode="Password" runat="server"
                                                placeholder="Enter new password"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-12 col-md-6 col-xl-6">
                                        <div class="input-block local-forms">
                                            <asp:Label ID="lblConfirmPassword" AssociatedControlID="txtConfirmPassword" CssClass="form-label"
                                                runat="server" Text="Confirm Password"></asp:Label>
                                            <asp:TextBox ID="txtConfirmPassword" CssClass="form-control" TextMode="Password"
                                                runat="server" placeholder="Confirm new password"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-12 text-end">
                                        <asp:Button ID="btnSubmit" CssClass="btn btn-primary submit-form" Text="Submit" runat="server"
                                            OnClick="btnSubmit_Click" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
