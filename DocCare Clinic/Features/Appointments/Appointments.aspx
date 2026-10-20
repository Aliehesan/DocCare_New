<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="Appointments.aspx.cs" Inherits="DocCare_Clinic.Appointments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .custom-icon-delete
        {
            background: url('assets/img/icons/delete-icon.png') no-repeat center center;
            background-size: contain;
            width: 16px;
            height: 24px;
            display: inline-block;
            border: none;
            margin-left: 25px;
        }
        
        .pagination
        {
            text-align: center;
            margin-top: 20px;
        }
        .pagination a, .pagination span
        {
            color: #6c757d;
            background-color: #f8f9fa;
            border: 1px solid #dee2e6;
            padding: 6px 12px;
            margin: 0 2px;
            border-radius: 5px;
            text-decoration: none;
            transition: 0.3s;
        }
        .pagination a:hover
        {
            background-color: #007bff;
            color: white;
            border-color: #007bff;
        }
        .pagination .current
        {
            background-color: #007bff;
            color: white;
            border-color: #007bff;
        }
        .pagination a:disabled
        {
            background-color: #e9ecef;
            color: #6c757d;
            border-color: #dee2e6;
            cursor: not-allowed;
        }
        .status-pending
        {
            background-color: gray !important;
            color: white !important;
            font-weight: bold;
            border-radius: 5px;
            padding: 5px;
        }
        
        .status-confirmed
        {
            background-color: #007bff !important;
            color: white !important;
            font-weight: bold;
            border-radius: 5px;
            padding: 5px;
        }
        
        .status-completed
        {
            background-color: green !important;
            color: white !important;
            font-weight: bold;
            border-radius: 5px;
            padding: 5px;
        }
        
        .status-cancelled
        {
            background-color: red !important;
            color: white !important;
            font-weight: bold;
            border-radius: 5px;
            padding: 5px;
        }

        .status-noshow
        {
            background-color: #6b7280 !important;
            color: white !important;
            font-weight: bold;
            border-radius: 5px;
            padding: 5px;
        }
        
        .date-filter-bar { display: flex; flex-wrap: wrap; align-items: center; gap: 8px; margin: 0 0 14px; padding: 0 20px; }
        .date-filter-bar .form-control { width: 175px; min-height: 40px; }
        .date-chip { padding: 7px 14px; border-radius: 20px; border: 1.5px solid #c7d2fe; background: #eef2ff;
                     color: #2E37A4 !important; font-size: 13px; font-weight: 600; text-decoration: none; white-space: nowrap; }
        .date-chip:hover { border-color: #2E37A4; }
        .date-chip.active { background: #2E37A4; border-color: #2E37A4; color: #fff !important; }
        .showing-label { margin-left: auto; font-size: 13px; color: #6b7280; }
        .notes-link { display: inline-flex; align-items: center; gap: 5px; padding: 4px 10px; border-radius: 8px; border: 1.5px solid #c7d2fe;
                      background: #eef2ff; color: #2E37A4 !important; font-size: 12.5px; font-weight: 600; text-decoration: none; white-space: nowrap; }
        .notes-link:hover { border-color: #2E37A4; }        @media (max-width: 767px) { .showing-label { margin-left: 0; width: 100%; } }

        .status-dropdown
        {
            width: 120px;
            text-align: center;
            font-size: 14px;
        }
    </style>
    <!-- Page Wrapper -->
    <div class="page-wrapper">
        <div class="content">
            <!-- Page Header -->
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/Features/Appointments/Appointments.aspx") %>">Appointment </a></li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Appointment List</li>
                        </ul>
                    </div>
                </div>
            </div>
            <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
            </asp:ScriptManagerProxy>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <!-- Appointment List -->
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="card card-table show-entire">
                                <div class="card-body">
                                    <!-- Page Table Header -->
                                    <div class="page-table-header mb-2">
                                        <div class="row align-items-center">
                                            <div class="col">
                                                <div class="doctor-table-blk">
                                                    <h3>
                                                        Appointment List</h3>
                                                    <div class="top-nav-search table-search-blk">
                                                        <asp:TextBox ID="txtSearch" CssClass="form-control" runat="server" Placeholder="Name, Patient ID, mobile, token"
                                                            AutoPostBack="false" onkeyup="SearchPatients()" ClientIDMode="Static"></asp:TextBox>
                                                        <asp:Button ID="btnHiddenSearch" runat="server" Style="display: none;" OnClick="txtSearch_TextChanged" />
                                                        <div style="background-color: transparent; border-color: transparent; min-height: 45px;
                                                            padding: 7px 10px; position: absolute; top: 16px; display: -webkit-box; display: -ms-flexbox;
                                                            display: flex; -webkit-box-align: center; -ms-flex-align: center; align-items: center;">
                                                        </div>
                                                        <div style="display: inline-block; margin-top: -33px; margin-left: 10px;">
                                                            <svg width="20" height="20" viewbox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                                                <path opacity="0.4" d="M9.17508 16.6807C13.3218 16.6807 16.6834 13.3191 16.6834 9.1724C16.6834 5.02566 13.3218 1.66406 9.17508 1.66406C5.02834 1.66406 1.66675 5.02566 1.66675 9.1724C1.66675 13.3191 5.02834 16.6807 9.17508 16.6807Z" fill="#2E37A4" />
                                                                                <path d="M18.325 15.7917C18.05 15.2833 17.4666 15 16.6833 15C16.0916 15 15.5833 15.2417 15.2833 15.6583C14.9833 16.075 14.9166 16.6333 15.1 17.1917C15.4583 18.275 16.0833 18.5167 16.425 18.5583C16.475 18.5667 16.525 18.5667 16.5833 18.5667C16.95 18.5667 17.5166 18.4083 18.0666 17.5833C18.5083 16.9417 18.5916 16.3 18.325 15.7917Z" fill="#2E37A4" />
                                                                            </svg>
                                                        </div>
                                                    </div>
                                                    <div class="doctor-search-blk">
                                                        <div class="add-group">
                                                            <asp:Panel ID="btnBookAppointment" runat="server">
                                                                <a href="<%= ResolveUrl("~/Features/Appointments/BookAppointment.aspx") %>" class="btn btn-primary add-pluss " title="Book walk-in appointment">
                                                                    <svg width="15" height="15" viewbox="0 0 15 15" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                                        <path d="M13.8158 6.31579H8.68421V1.18421C8.68421 0.536842 8.14737 0 7.5 0C6.85263 0 6.31579 0.536842 6.31579 1.18421V6.31579H1.18421C0.536842 6.31579 0 6.85263 0 7.5C0 8.14737 0.536842 8.68421 1.18421 8.68421H6.31579V13.8158C6.31579 14.4632 6.85263 15 7.5 15C8.14737 15 8.68421 14.4632 8.68421 13.8158V8.68421H13.8158C14.4632 8.68421 15 8.14737 15 7.5C15 6.85263 14.4632 6.31579 13.8158 6.31579Z" fill="#2E37A4"/>
                                                                        </svg>
                                                                </a>
                                                            </asp:Panel>
                                                            <a href="<%= ResolveUrl("~/Features/Appointments/Appointments.aspx") %>" class="btn btn-primary doctor-refresh ms-2" style="margin-left: 5px !important;">
                                                                <svg width="15" height="15" viewbox="0 0 15 15" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                                        <path d="M14.1567 6.6875C13.6904 6.6875 13.324 7.0539 13.324 7.52022C13.3129 9.07463 12.7133 10.5291 11.6252 11.6172C10.526 12.7164 9.06047 13.327 7.50606 13.327C5.95165 13.327 4.48604 12.7164 3.38685 11.6172C3.35354 11.5839 3.33135 11.5506 3.29804 11.5173H4.21957C4.67479 11.5173 5.05229 11.1398 5.05229 10.6846C5.05229 10.2293 4.67479 9.85183 4.21957 9.85183H1.2773C0.82208 9.85183 0.44458 10.2293 0.44458 10.6846V13.6268C0.44458 14.082 0.82208 14.4595 1.2773 14.4595C1.73252 14.4595 2.11002 14.082 2.11002 13.6268V12.6831C2.14333 12.7164 2.17663 12.7608 2.20994 12.8052C3.62001 14.2153 5.50753 15.0036 7.50606 15.0036C9.50458 15.0036 11.3921 14.2264 12.8021 12.8052C14.2011 11.4062 14.9783 9.52985 14.9894 7.53132C14.9894 7.065 14.6119 6.6986 14.1567 6.6875Z" fill="#2E37A4"/>
                                                                        <path d="M0.832965 8.21616C1.28819 8.21616 1.65458 7.84977 1.66568 7.39455C1.68789 5.87345 2.29855 4.44117 3.36443 3.36419C4.46362 2.265 5.9292 1.65434 7.48361 1.65434C9.03802 1.65434 10.5036 2.265 11.6028 3.36419C11.6361 3.39749 11.6583 3.4308 11.6916 3.46411H10.7701C10.3149 3.46411 9.93738 3.84161 9.93738 4.29683C9.93738 4.75205 10.3149 5.12955 10.7701 5.12955H13.7124C14.1676 5.12955 14.5451 4.75205 14.5451 4.29683V1.36566C14.5451 0.91044 14.1676 0.53294 13.7124 0.53294C13.2571 0.53294 12.8796 0.91044 12.8796 1.36566V2.32051C12.8463 2.2872 12.813 2.24279 12.7797 2.19838C11.3697 0.788308 9.48213 0 7.48361 0C5.48508 0 3.59759 0.777205 2.18752 2.19838C0.799658 3.57514 0.0224517 5.41823 0.000245814 7.37234C-0.0108571 7.82756 0.355529 8.20506 0.821851 8.21616C0.832954 8.21616 0.832965 8.21616 0.832965 8.21616Z" fill="#2E37A4"/>
                                                                        </svg>
                                                            </a>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <%-- One day (opens on today) or all dates; works together with the search --%>
                                    <div class="date-filter-bar">
                                        <asp:TextBox ID="txtFilterDate" runat="server" TextMode="Date" CssClass="form-control"
                                            AutoPostBack="true" OnTextChanged="txtFilterDate_TextChanged" ToolTip="Show one day"></asp:TextBox>
                                        <asp:LinkButton ID="lnkToday" runat="server" CssClass="date-chip" OnClick="lnkToday_Click">Today</asp:LinkButton>
                                        <asp:LinkButton ID="lnkTomorrow" runat="server" CssClass="date-chip" OnClick="lnkTomorrow_Click">Tomorrow</asp:LinkButton>
                                        <asp:LinkButton ID="lnkAllDates" runat="server" CssClass="date-chip" OnClick="lnkAllDates_Click">All dates</asp:LinkButton>
                                        <asp:Label ID="lblShowing" runat="server" CssClass="showing-label"></asp:Label>
                                    </div>
                                    <!-- Appointment Table -->
                                    <div class="table-responsive">
                                        <asp:GridView ID="GridViewAppointments" runat="server" CssClass="table border-0 custom-table comman-table datatable mb-0"
                                            AutoGenerateColumns="False" OnRowCommand="GridViewAppointments_RowCommand" EmptyDataText="No Appointment Found"
                                            AllowPaging="true" PageSize="7" OnPageIndexChanging="GridViewAppointments_PageIndexChanging"
                                            OnRowDataBound="GridViewAppointments_RowDataBound" DataKeyNames="ID">
                                            <Columns>
                                                <asp:TemplateField HeaderText="#">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSerialNo" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Name">
                                                    <ItemTemplate>
                                                        <%# HttpUtility.HtmlEncode(Eval("Name")) %>
                                                        <span class="patient-code"><%# Eval("PatientCode") %></span>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="ConsultingDoctor" HeaderText="Consulting Doctor" SortExpression="ConsultingDoctor" />
                                                <asp:BoundField DataField="Mobile" HeaderText="Mobile" SortExpression="Mobile" />
                                                <asp:TemplateField HeaderText="Appoinement Date">
                                                    <ItemTemplate>
                                                        <%# Convert.ToDateTime(Eval("AppointDate")).ToString("dd-MMM-yyyy")%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="TokenNo" HeaderText="Token Number" SortExpression="TokenNo" />
                                                <asp:TemplateField HeaderText="Booking Type">
                                                    <ItemTemplate>
                                                        <span class='custom-badge <%# Eval("BookingType").ToString() == "Walk-in" ? "status-orange" : "status-blue" %>'>
                                                            <%# Eval("BookingType").ToString() == "Walk-in" ? "Walk-in" : "Online" %>
                                                        </span>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Time">
                                                    <ItemTemplate>
                                                        <%# ConvertTime(Eval("AppointTime")) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"
                                                            CssClass='<%# GetStatusCssClass(Eval("Status").ToString()) %>'>
                                                            <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                                                            <asp:ListItem Text="Confirmed" Value="Confirmed"></asp:ListItem>
                                                            <asp:ListItem Text="Completed" Value="Completed"></asp:ListItem>
                                                            <asp:ListItem Text="Cancelled" Value="Cancelled"></asp:ListItem>
                                                            <asp:ListItem Text="No Show" Value="No Show"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Actions">
                                                    <ItemTemplate>
                                                        <%-- Visit notes / vitals for a visit that happened or is happening (see MedicalRecords) --%>
                                                        <asp:HyperLink ID="lnkNotes" runat="server" CssClass="notes-link" Visible="false"
                                                            NavigateUrl='<%# "~/Features/Patients/VisitNotes.aspx?AppointmentID=" + Eval("ID") %>' />
                                                        <asp:Button ID="btnDelete" runat="server" CssClass="custom-icon-delete" CommandArgument='<%# Eval("ID") %>'
                                                            CommandName="DeleteAppointment" OnClientClick="return confirm('Remove this appointment from the list? It will stay in the Appointment Report.');" ToolTip="Remove from list (kept in Appointment Report)" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerStyle CssClass="pagination" />
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                </ContentTemplate>
                <%--<Triggers>
                    <asp:PostBackTrigger ControlID="GridViewAppointments" />
                </Triggers>--%>
            </asp:UpdatePanel>
        </div>
    </div>
    <script type="text/javascript">
        function SearchPatients() {
            clearTimeout(window.searchTimeout);
            window.searchTimeout = setTimeout(function () {
                __doPostBack('<%= btnHiddenSearch.UniqueID %>', '');
            }, 500); // Delay search by 500ms to avoid excessive postbacks
        }
    </script>
    <script type="text/javascript">
    </script>
</asp:Content>
