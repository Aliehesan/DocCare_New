<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="PatientProfile.aspx.cs" Inherits="DocCare_Clinic.PatientProfile" %>
<%@ Import Namespace="DocCare_Clinic.Config" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .mh-card { margin-top: 18px; }
        .mh-header { display: flex; justify-content: space-between; align-items: center; gap: 10px; flex-wrap: wrap; }
        .mh-link { font-size: 13.5px; font-weight: 600; color: #2E37A4 !important; }
        .mh-next { font-size: 13px; color: #6b7280; }
        .mh-allergy { display: flex; gap: 10px; align-items: flex-start; background: #fef2f2; border: 1px solid #fecaca; color: #991b1b;
                      border-radius: 12px; padding: 12px 16px; margin-bottom: 18px; font-size: 14px; }
        .mh-allergy i { margin-top: 3px; }
        .mh-facts { display: grid; grid-template-columns: repeat(auto-fit, minmax(170px, 1fr)); gap: 14px 22px; }
        .mh-facts span { display: block; font-size: 12px; color: #6b7280; margin-bottom: 3px; }
        .mh-facts strong { font-size: 14px; color: #111827; font-weight: 600; white-space: pre-line; }
        .mh-facts strong.none { color: #9ca3af; font-weight: 500; }
        .mh-updated { font-size: 12px; color: #9ca3af; margin-top: 12px; }
        .mh-label { display: block; font-size: 13px; font-weight: 600; color: #374151; margin-bottom: 6px; }
        .mh-visit { border: 1px solid #eef0f7; border-radius: 12px; margin-bottom: 10px; background: #fff; }
        .mh-visit[open] { border-color: #c7d2fe; box-shadow: 0 4px 14px rgba(46, 55, 164, 0.06); }
        .mh-visit summary { display: grid; grid-template-columns: 110px 170px 1fr auto 70px; gap: 12px; align-items: center;
                            padding: 12px 16px; cursor: pointer; list-style: none; }
        .mh-visit summary::-webkit-details-marker { display: none; }
        .mh-visit summary:hover { background: #fafbff; border-radius: 12px; }
        .mh-date { font-weight: 600; color: #111827; font-size: 14px; }
        .mh-date small, .mh-doc small { display: block; font-weight: 400; color: #6b7280; font-size: 12px; }
        .mh-doc { font-size: 14px; color: #111827; }
        .mh-diag { font-size: 13px; color: #4b5563; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
        .mh-fee { font-size: 13.5px; font-weight: 600; color: #374151; text-align: right; }
        .mh-status { font-size: 12px; font-weight: 600; padding: 3px 10px; border-radius: 12px; background: #eef2ff; color: #2E37A4; white-space: nowrap; }
        .mh-status.st-Completed { background: #dcfce7; color: #166534; }
        .mh-status.st-NoShow { background: #f3f4f6; color: #4b5563; }
        .mh-status.st-Pending { background: #fef3c7; color: #92400e; }
        .mh-body { padding: 4px 16px 16px; border-top: 1px dashed #e5e7eb; font-size: 13.5px; }
        .mh-body h6 { font-size: 12px; text-transform: uppercase; letter-spacing: .5px; color: #2E37A4; margin: 12px 0 4px; }
        .mh-body p { margin: 0; color: #374151; white-space: pre-line; }
        .mh-body ol { margin: 0; padding-left: 20px; }
        .mh-body li { margin-bottom: 2px; }
        .mh-body .muted { color: #9ca3af; }
        .mh-actions { margin-top: 12px; display: flex; gap: 8px; flex-wrap: wrap; }
        .mh-empty { color: #6b7280; font-size: 14px; padding: 6px 2px; }
        .mh-pager { display: flex; justify-content: space-between; align-items: center; gap: 10px; flex-wrap: wrap; margin-top: 8px; font-size: 13px; color: #6b7280; }
        @media (max-width: 767px) {
            .mh-visit summary { grid-template-columns: 1fr auto; }
            .mh-diag { grid-column: 1 / -1; white-space: normal; }
            .mh-fee { text-align: left; }
        }
    </style>
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
                            <%-- Allergies are shown to every member of staff who opens the patient --%>
                            <asp:Panel ID="pnlAllergyBanner" runat="server" Visible="false" CssClass="mh-allergy">
                                <i class="fa-solid fa-triangle-exclamation"></i>
                                <span><strong>Allergies:</strong> <asp:Literal ID="litAllergyBanner" runat="server" /></span>
                            </asp:Panel>
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
                                        <%-- ── Health summary: permanent facts, kept up to date by the doctor ── --%>
                                        <div class="card mh-card">
                                            <div class="card-header mh-header">
                                                <h4 class="card-title">Health Summary</h4>
                                                <asp:LinkButton ID="lnkEditHealth" runat="server" CssClass="mh-link" OnClick="lnkEditHealth_Click" CausesValidation="false">
                                                    <i class="fa-regular fa-pen-to-square"></i> Edit</asp:LinkButton>
                                            </div>
                                            <div class="card-body">
                                                <asp:Panel ID="pnlHealthView" runat="server">
                                                    <div class="mh-facts">
                                                        <div><span>Blood group</span><strong><asp:Literal ID="litBloodGroup" runat="server" /></strong></div>
                                                        <div><span>Allergies</span><strong><asp:Literal ID="litAllergiesFact" runat="server" /></strong></div>
                                                        <div><span>Long-term conditions</span><strong><asp:Literal ID="litConditions" runat="server" /></strong></div>
                                                        <div><span>Regular medicines</span><strong><asp:Literal ID="litMedicines" runat="server" /></strong></div>
                                                        <div><span>Past surgeries</span><strong><asp:Literal ID="litSurgeries" runat="server" /></strong></div>
                                                    </div>
                                                    <div class="mh-updated"><asp:Literal ID="litHealthUpdated" runat="server" /></div>
                                                </asp:Panel>
                                                <asp:Panel ID="pnlHealthEdit" runat="server" Visible="false">
                                                    <div class="row">
                                                        <div class="col-md-3 mb-3"><label class="mh-label" for="<%= ddlBloodGroup.ClientID %>">Blood group</label>
                                                            <asp:DropDownList ID="ddlBloodGroup" runat="server" CssClass="form-control">
                                                                <asp:ListItem Value="">Not known</asp:ListItem>
                                                                <asp:ListItem>A+</asp:ListItem><asp:ListItem>A-</asp:ListItem>
                                                                <asp:ListItem>B+</asp:ListItem><asp:ListItem>B-</asp:ListItem>
                                                                <asp:ListItem>AB+</asp:ListItem><asp:ListItem>AB-</asp:ListItem>
                                                                <asp:ListItem>O+</asp:ListItem><asp:ListItem>O-</asp:ListItem>
                                                            </asp:DropDownList></div>
                                                        <div class="col-md-9 mb-3"><label class="mh-label" for="<%= txtAllergies.ClientID %>">Allergies (shown in red to all staff)</label>
                                                            <asp:TextBox ID="txtAllergies" runat="server" CssClass="form-control" MaxLength="500" placeholder="e.g. Penicillin, peanuts. Leave empty if none known." /></div>
                                                        <div class="col-md-4 mb-3"><label class="mh-label" for="<%= txtConditions.ClientID %>">Long-term conditions</label>
                                                            <asp:TextBox ID="txtConditions" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="500" placeholder="e.g. Type 2 diabetes, hypertension" /></div>
                                                        <div class="col-md-4 mb-3"><label class="mh-label" for="<%= txtMedicines.ClientID %>">Regular medicines</label>
                                                            <asp:TextBox ID="txtMedicines" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="500" placeholder="e.g. Metformin 500 mg twice a day" /></div>
                                                        <div class="col-md-4 mb-3"><label class="mh-label" for="<%= txtSurgeries.ClientID %>">Past surgeries</label>
                                                            <asp:TextBox ID="txtSurgeries" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="500" placeholder="e.g. Appendix removed (2018)" /></div>
                                                    </div>
                                                    <div class="text-end">
                                                        <asp:Button ID="btnCancelHealth" runat="server" CssClass="btn btn-outline-secondary me-2" Text="Cancel" OnClick="btnCancelHealth_Click" CausesValidation="false" />
                                                        <asp:Button ID="btnSaveHealth" runat="server" CssClass="btn btn-primary" Text="Save Health Summary" OnClick="btnSaveHealth_Click" />
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>

                                        <%-- ── Visits: newest first; each opens to its vitals and (for doctors / admin) the consultation ── --%>
                                        <div class="card mh-card">
                                            <div class="card-header mh-header">
                                                <h4 class="card-title">Medical History</h4>
                                                <span class="mh-next"><asp:Literal ID="litNextAppt" runat="server" /></span>
                                            </div>
                                            <div class="card-body">
                                                <asp:Repeater ID="rptVisits" runat="server">
                                                    <ItemTemplate>
                                                        <details class="mh-visit">
                                                            <summary>
                                                                <span class="mh-date"><%# Eval("AppointDate", "{0:dd MMM yyyy}") %><small><%# TimeText(Eval("AppointTime")) %></small></span>
                                                                <span class="mh-doc"><%# HttpUtility.HtmlEncode("Dr. " + DoctorName.WithoutTitle(Eval("Doctor").ToString())) %><small><%# HttpUtility.HtmlEncode(Eval("Specialization")) %></small></span>
                                                                <span class="mh-diag"><%# SummaryText(Container.DataItem) %></span>
                                                                <span class='mh-status <%# "st-" + Eval("Status").ToString().Replace(" ", "") %>'><%# Eval("Status") %></span>
                                                                <span class="mh-fee">&#8377;<%# string.Format("{0:N0}", Eval("Fee")) %></span>
                                                            </summary>
                                                            <div class="mh-body"><%# VisitDetails(Container.DataItem) %></div>
                                                        </details>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <asp:Panel ID="pnlNoVisits" runat="server" Visible="false" CssClass="mh-empty">
                                                    No visits yet. Visits appear here from the day of the appointment.
                                                </asp:Panel>
                                                <asp:Panel ID="pnlVisitPager" runat="server" CssClass="mh-pager">
                                                    <span><asp:Literal ID="litVisitPager" runat="server" /></span>
                                                    <span>
                                                        <asp:LinkButton ID="lnkVisitPrev" runat="server" CssClass="btn btn-outline-primary btn-sm" OnClick="lnkVisitPrev_Click" CausesValidation="false">&lsaquo; Newer</asp:LinkButton>
                                                        <asp:LinkButton ID="lnkVisitNext" runat="server" CssClass="btn btn-outline-primary btn-sm" OnClick="lnkVisitNext_Click" CausesValidation="false">Older &rsaquo;</asp:LinkButton>
                                                    </span>
                                                </asp:Panel>
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
