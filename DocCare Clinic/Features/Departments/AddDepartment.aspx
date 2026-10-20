<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="AddDepartment.aspx.cs" Inherits="DocCare_Clinic.AddDepartment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item">
                                <asp:HyperLink ID="lnkDepartment" runat="server" NavigateUrl="~/Features/Departments/Departments.aspx">Department</asp:HyperLink>
                            </li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Add Department </li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <asp:Panel ID="pnlAddDepartment" runat="server">
                                <div class="row">
                                    <div class="col-12">
                                        <div class="form-heading">
                                            <h4>
                                                Add Department</h4>
                                        </div>
                                    </div>
                                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" ForeColor="Red"
                                                Font-Size="13px" Style="margin-bottom: 10px; display: block;"></asp:Label>                                   
                                    <div class="col-12 col-md-6 col-xl-6">
                                        <div class="input-block local-forms">
                                            <label for="txtDepartmentName">
                                                Department Name <span class="login-danger">*</span></label>
                                            <asp:TextBox ID="txtDepartmentName" CssClass="form-control" runat="server" Placeholder="Enter department name"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-12 col-md-6 col-xl-6">
                                        <div class="input-block local-forms">
                                            <label for="txtDepartmentHead">
                                                Department Head <span class="login-danger">*</span></label>
                                            <asp:TextBox ID="txtDepartmentHead" CssClass="form-control" runat="server" Placeholder="Enter department head"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-12 col-sm-12">
                                        <div class="input-block local-forms">
                                            <label for="txtDescription">
                                                Description <span class="login-danger">*</span></label>
                                            <asp:TextBox ID="txtDescription" CssClass="form-control" TextMode="MultiLine" Rows="3"
                                                runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-12 col-md-6 col-xl-6">
                                        <div class="input-block local-forms">
                                            <label for="txtDepartmentDate">
                                                Department Date <span class="login-danger">*</span></label>
                                            <asp:TextBox ID="txtDepartmentDate" CssClass="form-control" TextMode="Date" runat="server"
                                                Placeholder="Select date"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-12 col-md-6 col-xl-6">
                                        <div class="input-block select-gender">
                                            <label class="gen-label">
                                                Status <span class="login-danger">*</span></label>
                                            <asp:RadioButtonList ID="rblStatus" runat="server" RepeatDirection="Horizontal" CssClass="form-check-label">
                                                <asp:ListItem Text="Active" Value="Active" class="form-check-inline"></asp:ListItem>
                                                <asp:ListItem Text="InActive" Value="InActive"></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>
                                    <div class="col-12">
                                        <div class="doctor-submit text-end">
                                            <asp:Button ID="btnCreateDepartment" Text="Create Department" CssClass="btn btn-primary submit-form me-2"
                                                runat="server" OnClick="btnCreateDepartment_Click" />
                                            <asp:Button ID="btnCancel" Text="Cancel" CssClass="btn btn-primary cancel-form" runat="server"
                                                OnClick="btnCancel_Click" />
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
                    <asp:Repeater ID="rptMessages" runat="server">
                        <ItemTemplate>
                            <li><a href="javascript:void(0);">
                                <div class="list-item <%# Convert.ToBoolean(Eval("IsNewMessage")) ? "new-message" : "" %>">
                                    <div class="list-left">
                                        <span class="avatar">
                                            <%# Eval("Avatar") %></span>
                                    </div>
                                    <div class="list-body">
                                        <span class="message-author">
                                            <%# Eval("Author") %></span> <span class="message-time">
                                                <%# Eval("Time") %></span>
                                        <div class="clearfix">
                                        </div>
                                        <span class="message-content">
                                            <%# Eval("MessageContent") %></span>
                                    </div>
                                </div>
                            </a></li>
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
