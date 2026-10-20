<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true" CodeBehind="EditProfile.aspx.cs" Inherits="DocCare_Clinic.edit_profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="page-wrapper">
            <div class="content">
                <!-- Page Header -->
                <div class="page-header">
                    <div class="row">
                        <div class="col-sm-12">
                            <ul class="breadcrumb">
                                <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/Features/Site/Default.aspx") %>">Dashboard</a></li>
                                <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                                <li class="breadcrumb-item active">Edit Profile</li>
                            </ul>
                        </div>
                    </div>
                </div>

                <!-- Edit Profile Form -->
   
                            <!-- Basic Information -->
                            <div class="card-box">
                                <h3 class="card-title">Basic Information</h3>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="profile-img-wrap">
                                            <asp:Image ID="ProfileImage" runat="server" CssClass="inline-block" ImageUrl="assets/img/user.jpg" AlternateText="User" />
                                            <div class="fileupload btn">
                                                <span class="btn-text">Edit</span>
                                                <asp:FileUpload ID="ProfileUpload" runat="server" CssClass="upload" />
                                            </div>
                                        </div>
                                        <div class="profile-basic">
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-block local-forms">
                                                        <label class="focus-label">First Name</label>
                                                        <asp:TextBox ID="FirstName" runat="server" CssClass="form-control floating" Text="John" />
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-block local-forms">
                                                        <label class="focus-label">Last Name</label>
                                                        <asp:TextBox ID="LastName" runat="server" CssClass="form-control floating" Text="Doe" />
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-block local-forms">
                                                        <label class="focus-label">Birth Date</label>
                                                        <div class="cal-icon">
                                                            <asp:TextBox ID="BirthDate" runat="server" CssClass="form-control floating datetimepicker" Text="05/06/1985" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-block local-forms">
                                                        <label class="focus-label">Gender</label>
                                                        <asp:DropDownList ID="Gender" runat="server" CssClass="form-control select">
                                                            <asp:ListItem Text="Select Gender" Value="" />
                                                            <asp:ListItem Text="Male" Value="Male" />
                                                            <asp:ListItem Text="Female" Value="Female" Selected="True" />
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Contact Information -->
                            <div class="card-box">
                                <h3 class="card-title">Contact Information</h3>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-block local-forms">
                                            <label class="focus-label">Address</label>
                                            <asp:TextBox ID="Address" runat="server" CssClass="form-control floating" Text="4487 Snowbird Lane" />
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="input-block local-forms">
                                            <label class="focus-label">State</label>
                                            <asp:TextBox ID="State" runat="server" CssClass="form-control floating" Text="New York" />
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="input-block local-forms">
                                            <label class="focus-label">Country</label>
                                            <asp:TextBox ID="Country" runat="server" CssClass="form-control floating" Text="United States" />
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="input-block local-forms">
                                            <label class="focus-label">Pin Code</label>
                                            <asp:TextBox ID="PinCode" runat="server" CssClass="form-control floating" Text="10523" />
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="input-block local-forms">
                                            <label class="focus-label">Phone Number</label>
                                            <asp:TextBox ID="PhoneNumber" runat="server" CssClass="form-control floating" Text="631-889-3206" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Education Information -->
                            <div class="card-box">
                                <h3 class="card-title">Education Information</h3>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="input-block local-forms">
                                            <label class="focus-label">Institution</label>
                                            <asp:TextBox ID="Institution" runat="server" CssClass="form-control floating" Text="Oxford University" />
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="input-block local-forms">
                                            <label class="focus-label">Subject</label>
                                            <asp:TextBox ID="Subject" runat="server" CssClass="form-control floating" Text="Computer Science" />
                                        </div>
                                    </div>
                                    <!-- More fields... -->
                                </div>
                            </div>

                            <!-- Save Button -->
                            <div class="text-center">
                                <asp:Button ID="SaveButton" runat="server" CssClass="btn btn-primary submit-btn mb-4" Text="Save" />
                            </div>
            </div>
            <div class="notification-box">
                <div class="msg-sidebar notifications msg-noti">
                    <div class="topnav-dropdown-header">
                        <span>Messages</span>
                    </div>
                    <div class="drop-scroll msg-list-scroll" id="msg_list">
                        <asp:Repeater ID="rptMessages" runat="server">
                            <ItemTemplate>
                                <li>
                                    <a href="javascript:void(0);">
                                        <div class="list-item">
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
                                    </a>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="topnav-dropdown-footer">
                        <a href="javascript:void(0);">See all messages</a>
                    </div>
                </div>
            </div>
        </div>
</asp:Content>
