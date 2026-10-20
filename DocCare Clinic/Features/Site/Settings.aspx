<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="Settings.aspx.cs" Inherits="DocCare_Clinic.Settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrapper">
        <div class="content container-fluid">
            <!-- Page Header -->
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item">
                                <asp:HyperLink ID="lnkDashboard" runat="server">Dashboard</asp:HyperLink></li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Settings</li>
                        </ul>
                    </div>
                </div>
            </div>
            <!-- Settings Menu Links -->
            <div class="settings-menu-links">
                <ul class="nav nav-tabs menu-tabs">
                    <li class="nav-item"><a class="nav-link" href="<%= ResolveUrl("~/Features/Accounts/ChangePassword.aspx") %>">Change Password</a>
                    </li>
                </ul>
            </div>
        </div>
    </div>
</asp:Content>
