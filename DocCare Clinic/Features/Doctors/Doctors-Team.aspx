<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Shared/DocCare.Master" AutoEventWireup="true"
    CodeBehind="Doctors-Team.aspx.cs" Inherits="DocCare_Clinic.Doctors_Team" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Doctors Available</title>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <style>
        .doctors-wrapper {
            background-color: #f4f7fb;
            padding: 50px 4%;
            border-radius: 30px;
            margin: 0 3% 60px 3%;
        }

        .doctors-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr); /* 4 cards per row to make them smaller */
            gap: 30px;
        }

        .doctor-card-modern {
            background: #ffffff;
            padding: 30px 20px; /* Reduced padding */
            border-radius: 25px; /* Slightly less curve for smaller cards */
            box-shadow: 0 10px 30px rgba(46, 55, 164, 0.05);
            transition: all 0.4s ease;
            text-align: center;
            border: none;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }

        .doctor-card-modern:hover {
            transform: translateY(-8px);
            box-shadow: 0 20px 40px rgba(46, 55, 164, 0.1);
        }

        .doc-img-modern {
            width: 120px; /* Smaller image */
            height: 120px;
            border-radius: 50%;
            object-fit: cover;
            margin: 0 auto 15px;
            border: 3px solid var(--primary);
            padding: 4px;
            background: #fff;
        }

        .doctor-name {
            font-size: 19px; /* Smaller font */
            font-weight: 700;
            color: var(--primary);
            margin-bottom: 5px;
            letter-spacing: -0.5px;
        }

        .doctor-spec {
            color: #6b7280;
            font-size: 13px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin-bottom: 20px;
        }

        .book-appointment-btn {
            background: var(--primary);
            color: #ffffff;
            border: none;
            padding: 10px 20px;
            border-radius: 50px;
            font-weight: 600;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: 1px;
            cursor: pointer;
            transition: all 0.3s ease;
            width: 90%;
            margin: 0 auto;
        }

        .book-appointment-btn:hover {
            background: var(--primary-hover);
            box-shadow: 0 5px 15px rgba(46, 55, 164, 0.3);
            transform: scale(1.03);
        }

        .no-slots-label {
            display: block;
            font-size: 13px;
            font-weight: 600;
            color: #dc3545;
            margin-top: 10px;
        }

        .next-available-label {
            display: block;
            font-size: 13px;
            font-weight: 600;
            color: #047857;
            margin-top: 10px;
        }

        /* Search Input */
        .controls-header {
            display: flex;
            justify-content: flex-end;
            margin-bottom: 25px;
        }

        .search-input {
            padding: 10px 20px;
            border: 1px solid #e5e7eb;
            border-radius: 30px;
            font-size: 14px;
            font-weight: 500;
            color: #1f2937;
            outline: none;
            width: 300px;
            background-color: #fff;
            box-shadow: 0 5px 15px rgba(46, 55, 164, 0.05);
            transition: all 0.3s ease;
        }

        .search-input:focus {
            border-color: var(--primary);
            box-shadow: 0 5px 20px rgba(46, 55, 164, 0.15);
        }

        .no-doctors-msg {
            display: block;
            text-align: center;
            font-size: 20px;
            font-weight: 600;
            color: #6b7280;
            padding: 50px 0;
            width: 100%;
        }

        @media (max-width: 1199px) {
            .doctors-grid { grid-template-columns: repeat(3, 1fr); }
        }
        @media (max-width: 991px) {
            .doctors-grid { grid-template-columns: repeat(2, 1fr); }
        }
        @media (max-width: 767px) {
            .doctors-grid { grid-template-columns: 1fr; }
            .doctors-wrapper { padding: 40px 20px; margin: 0 15px 40px 15px; }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- breadcrumb start -->
    <div class="breadcrumb-area text-center">
        <h2 class="page-title">All Doctors</h2>
    </div>
    <!-- breadcrumb end -->
    
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <div class="doctors-wrapper">
        <!-- Search Bar (OUTSIDE UpdatePanel to keep cursor focus while typing) -->
        <div class="controls-header">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search by name or specialty..." onkeyup="doSearch()"></asp:TextBox>
            <asp:Button ID="btnSearchHidden" runat="server" OnClick="btnSearchHidden_Click" style="display:none;" />
        </div>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnSearchHidden" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <asp:Literal ID="litNoDoctors" runat="server"></asp:Literal>
                
                <!-- Doctors List -->
                <div class="doctors-grid">
                    <asp:Repeater ID="rptDoctors" runat="server" OnItemDataBound="rptDoctors_ItemDataBound">
                        <ItemTemplate>
                            <div class="doctor-card-modern">
                                <div>
                                    <img src='<%# ResolveUrl(Eval("ImageUrl").ToString()) %>' class="doc-img-modern"
                                        alt="Doctor Image" onerror="this.src='https://ui-avatars.com/api/?name=<%# Eval("Name").ToString().Replace(" ", "+") %>&background=2E37A4&color=fff&size=140'">
                                    <h6 class="doctor-name">
                                        Dr. <%# Eval("Name") %></h6>
                                    <p class="doctor-spec">
                                        <%# Eval("Specialization") %></p>
                                </div>
                                <div>
                                    <asp:Button ID="btnBookAppointment" runat="server" Text="Book Appointment" CssClass="book-appointment-btn"
                                        CommandArgument='<%# Eval("DoctorID") %>' OnClick="BookAppointment_Click" />
                                    <asp:Label ID="lblNoSlots" runat="server" CssClass="no-slots-label"
                                        Visible="false"></asp:Label>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <script>
        let searchTimeout;
        function doSearch() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(function () {
                var btn = document.getElementById('<%= btnSearchHidden.ClientID %>');
                if (btn) btn.click();
            }, 400); // 400ms delay prevents spamming the server on fast keystrokes
        }
    </script>
</asp:Content>
