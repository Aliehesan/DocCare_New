<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="AddDoctor.aspx.cs" Inherits="DocCare_Clinic.AddDoctor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrapper">
        <div class="content">
            <!-- Breadcrumb -->
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item">
                                <asp:HyperLink ID="lnkDoctors" NavigateUrl="~/Features/Doctors/Doctors.aspx" runat="server">Doctors</asp:HyperLink>
                            </li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Add Doctor</li>
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
                                <div id="frmDoctorDetails" enctype="multipart/form-data">
                                    <div class="row">
                                        <div class="col-12">
                                            <div class="form-heading">
                                                <h4>
                                                    Doctor Details</h4>
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
                                        <!-- Designation -->
                                        <div class="col-12 col-md-6 col-xl-4">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblDesignation" AssociatedControlID="ddlDesignation" runat="server"
                                                    Text="Designation *"></asp:Label>
                                                <asp:DropDownList ID="ddlDesignation" CssClass="form-control select" runat="server">
                                                    <asp:ListItem Value="" Text="Select Designation" Enabled="true" Selected="True" />
                                                    <asp:ListItem Value="Intern" Text="Intern" />
                                                    <asp:ListItem Value="Junior Resident" Text="Junior Resident" />
                                                    <asp:ListItem Value="Senior Resident" Text="Senior Resident" />
                                                    <asp:ListItem Value="Fellow" Text="Fellow" />
                                                    <asp:ListItem Value="Attending Physician" Text="Attending Physician" />
                                                    <asp:ListItem Value="Consultant" Text="Consultant" />
                                                    <asp:ListItem Value="Head of Department" Text="Head of Department" />
                                                    <asp:ListItem Value="Medical Director" Text="Medical Director" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <!-- Department -->
                                        <div class="col-12 col-md-6 col-xl-4">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblDepartment" AssociatedControlID="ddlDepartment" runat="server"
                                                    Text="Department *"></asp:Label>
                                                <asp:DropDownList ID="ddlDepartment" CssClass="form-control select" runat="server"
                                                    AppendDataBoundItems="true">
                                                    <asp:ListItem Value="" Text="Select Department" Enabled="true" Selected="True" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <!-- Specialization -->
                                        <div class="col-12 col-md-6 col-xl-4">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblSpecialization" AssociatedControlID="ddlSpecialization" runat="server"
                                                    Text="Specialization *"></asp:Label>
                                                <asp:DropDownList ID="ddlSpecialization" CssClass="form-control select" runat="server">
                                                    <asp:ListItem Value="" Text="Select Specialization" Enabled="true" Selected="True" />
                                                    <asp:ListItem Value="Surgeon" Text="Surgeon" />
                                                    <asp:ListItem Value="Cardiologist" Text="Cardiologist" />
                                                    <asp:ListItem Value="Dermatology" Text="Dermatology" />
                                                    <asp:ListItem Value="Neurologist" Text="Neurologist" />
                                                    <asp:ListItem Value="General surgery" Text="General surgery" />
                                                    <asp:ListItem Value="Geriatrics" Text="Geriatrics" />
                                                    <asp:ListItem Value="Neurosurgery" Text="Neurosurgery" />
                                                    <asp:ListItem Value="Orthopedist" Text="Orthopedist" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <!-- Degree -->
                                        <div class="col-12 col-md-6 col-xl-4">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblDegree" AssociatedControlID="txtDegree" runat="server" Text="Degree *"></asp:Label>
                                                <asp:TextBox ID="txtDegree" CssClass="form-control" runat="server" placeholder="Enter degree"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- YearofCompletion -->
                                        <div class="col-12 col-md-6 col-xl-4">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblYearofCompletion" AssociatedControlID="txtYearOfCompletion" runat="server" Text="Year of Completion *"></asp:Label>
                                                <asp:TextBox ID="txtYearOfCompletion" CssClass="form-control" runat="server" placeholder="Enter Year of Completion"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Experience -->
                                        <div class="col-12 col-md-6 col-xl-2">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblExperience" AssociatedControlID="txtExperience" runat="server" Text="Experience *"></asp:Label>
                                                <asp:TextBox ID="txtExperience" CssClass="form-control" runat="server" placeholder="Enter Experience"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Hospital -->
                                        <div class="col-12 col-md-6 col-xl-5">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblHospital" AssociatedControlID="txtHospital" runat="server" Text="Hospital *"></asp:Label>
                                                <asp:TextBox ID="txtHospital" CssClass="form-control" runat="server" placeholder="Enter Hospital"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Institute -->
                                        <div class="col-12 col-md-6 col-xl-5">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblInstitute" AssociatedControlID="txtInstitute" runat="server" Text="Institute *"></asp:Label>
                                                <asp:TextBox ID="txtInstitute" CssClass="form-control" runat="server" placeholder="Enter Institute"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Location -->
                                        <div class="col-12 col-md-6 col-xl-4">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblLocation" AssociatedControlID="txtLocation" runat="server" Text="Location *"></asp:Label>
                                                <asp:TextBox ID="txtLocation" CssClass="form-control" runat="server" placeholder="Enter Your Location" TextMode="MultiLine"></asp:TextBox>
                                            </div>
                                        </div>                                        
                                        <!-- Email -->
                                        <div class="col-12 col-md-6 col-xl-4">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblEmail" AssociatedControlID="txtEmail" runat="server" Text="Email *"></asp:Label>
                                                <asp:TextBox ID="txtEmail" CssClass="form-control" runat="server" placeholder="Enter email"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Password -->
                                        <div class="col-12 col-md-6 col-xl-3">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblPassword" AssociatedControlID="txtPassword" runat="server" Text="Password *"></asp:Label>
                                                <asp:TextBox ID="txtPassword" CssClass="form-control" runat="server" TextMode="Password"
                                                    placeholder="Enter password"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Mobile -->
                                        <div class="col-12 col-md-6 col-xl-3">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblMobile" AssociatedControlID="txtMobile" runat="server" Text="Mobile *"></asp:Label>
                                                <asp:TextBox ID="txtMobile" CssClass="form-control" runat="server" placeholder="Enter mobile"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Charges -->
                                        <div class="col-12 col-md-6 col-xl-3">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblCharges" AssociatedControlID="txtCharges" runat="server" Text="Charges *"></asp:Label>
                                                <asp:TextBox ID="txtCharges" CssClass="form-control" runat="server" placeholder="Enter charges"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%--Upload image--%>
                                        <div class="col-12 col-md-6 col-xl-6">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblImage" AssociatedControlID="fileUpload" runat="server" Text="Profile Image *"></asp:Label>
                                                <asp:FileUpload ID="fileUpload" CssClass="form-control" runat="server" />
                                            </div>
                                        </div>
                                        <!-- Joining Date -->
                                        <div class="col-12 col-md-6 col-xl-3">
                                            <div class="input-block local-forms">
                                                <asp:Label ID="lblJoinDate" AssociatedControlID="txtJoinDate" runat="server" Text="Joining Date *"></asp:Label>
                                                <asp:TextBox ID="txtJoinDate" CssClass="form-control" runat="server" TextMode="Date"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Gender -->
                                        <div class="col-12 col-md-6 col-xl-3">
                                            <div class="input-block select-gender">
                                                <label class="gen-label">
                                                    Gender<span class="login-danger">*</span></label>
                                                <asp:RadioButtonList ID="rblGender" runat="server" RepeatDirection="Horizontal" CssClass="form-check-label">
                                                    <asp:ListItem Text="Male" Value="Male" class="form-check-inline"></asp:ListItem>
                                                    <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                                                </asp:RadioButtonList>
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
