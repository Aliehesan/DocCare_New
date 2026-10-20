<%@ Page Title="My Appointments" Language="C#" MasterPageFile="~/Features/Shared/DocCare.Master" AutoEventWireup="true"
    CodeBehind="MyAppointment.aspx.cs" Inherits="DocCare_Clinic.MyAppointment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .appointments-page-wrapper {
            padding: 0 8% 60px 8%;
            max-width: 1200px;
            margin: 0 auto;
        }

        .appointment-card-modern {
            background: #ffffff;
            border-radius: 24px;
            box-shadow: 0 8px 30px rgba(46, 55, 164, 0.06);
            padding: 28px 32px;
            margin-bottom: 24px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 25px;
            transition: all 0.3s ease;
            border: 1px solid #f3f4f6;
        }

        .appointment-card-modern:hover {
            transform: translateY(-4px);
            box-shadow: 0 16px 36px rgba(46, 55, 164, 0.1);
            border-color: rgba(46, 55, 164, 0.2);
        }

        .appt-left-col {
            display: flex;
            align-items: center;
            gap: 24px;
            flex: 1;
        }

        .appt-doc-img {
            width: 90px;
            height: 90px;
            border-radius: 50%;
            object-fit: cover;
            border: 3px solid var(--primary);
            padding: 3px;
            background: #fff;
            flex-shrink: 0;
            box-shadow: 0 4px 14px rgba(46, 55, 164, 0.12);
        }

        .appt-details-main {
            flex: 1;
        }

        .appt-doc-name {
            font-size: 20px;
            font-weight: 700;
            color: var(--text-dark);
            margin: 0 0 6px 0;
        }

        .appt-spec-badge {
            display: inline-block;
            padding: 4px 14px;
            background: rgba(46, 55, 164, 0.08);
            color: var(--primary);
            border-radius: 30px;
            font-size: 12px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            margin-bottom: 12px;
        }

        .appt-meta-list {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .appt-meta-item {
            font-size: 13.5px;
            color: var(--text-light);
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .appt-meta-item i {
            color: var(--primary);
            width: 16px;
            font-size: 14px;
        }

        .appt-meta-item strong {
            color: var(--text-dark);
        }

        .appt-right-col {
            display: flex;
            flex-direction: column;
            align-items: flex-end;
            gap: 15px;
            min-width: 180px;
        }

        /* Status Badges */
        .status-badge-pill {
            padding: 6px 16px;
            border-radius: 30px;
            font-size: 12.5px;
            font-weight: 600;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            text-transform: capitalize;
        }

        .status-badge-confirmed {
            background: #d1fae5;
            color: #065f46;
        }

        .status-badge-completed {
            background: #e0e7ff;
            color: #3730a3;
        }

        .status-badge-cancelled {
            background: #fee2e2;
            color: #991b1b;
        }

        .status-badge-pending {
            background: #fef3c7;
            color: #92400e;
        }

        .status-badge-noshow {
            background: #f3f4f6;
            color: #4b5563;
        }

        .reschedule-btn-modern {
            display: inline-block;
            border: 2px solid #2E37A4;
            color: #2E37A4;
            padding: 8px 22px;
            border-radius: 50px;
            font-size: 13px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.3s ease;
            margin-bottom: 8px;
        }

        .reschedule-btn-modern:hover {
            background: #2E37A4;
            color: #ffffff;
        }

        /* Buttons */
        .cancel-btn-modern {
            background: transparent;
            border: 2px solid #ef4444;
            color: #ef4444;
            padding: 8px 22px;
            border-radius: 50px;
            font-size: 13px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            outline: none;
        }

        .cancel-btn-modern:hover {
            background: #ef4444;
            color: #ffffff;
            box-shadow: 0 4px 15px rgba(239, 68, 68, 0.3);
            transform: translateY(-2px);
        }

        .cancelled-btn-modern {
            background: #f3f4f6;
            color: #9ca3af;
            border: 1px solid #e5e7eb;
            padding: 8px 22px;
            border-radius: 50px;
            font-size: 13px;
            font-weight: 600;
            cursor: not-allowed;
            pointer-events: none;
        }

        .completed-btn-modern {
            background: #ecfdf5;
            color: #059669;
            border: 1px solid #a7f3d0;
            padding: 8px 22px;
            border-radius: 50px;
            font-size: 13px;
            font-weight: 600;
            cursor: not-allowed;
            pointer-events: none;
        }

        @media (max-width: 768px) {
            .appointment-card-modern {
                flex-direction: column;
                align-items: flex-start;
            }
            .appt-right-col {
                width: 100%;
                flex-direction: row;
                justify-content: space-between;
                align-items: center;
                border-top: 1px solid #f3f4f6;
                padding-top: 15px;
                margin-top: 10px;
            }
        }

        /* Tabs: Upcoming / Past / Cancelled */
        .appt-tabs {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
            margin: 0 0 22px;
            padding: 6px;
            background: #fff;
            border: 1px solid #eef0f7;
            border-radius: 16px;
            width: fit-content;
            box-shadow: 0 4px 16px rgba(46, 55, 164, 0.05);
        }
        .appt-tab {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 9px 18px;
            border-radius: 12px;
            font-size: 14px;
            font-weight: 600;
            color: #4b5563 !important;
            text-decoration: none;
            transition: background 0.2s ease, color 0.2s ease;
        }
        .appt-tab:hover { background: #f5f7ff; color: #2E37A4 !important; }
        .appt-tab.active { background: #2E37A4; color: #fff !important; }
        .tab-count {
            min-width: 22px;
            padding: 1px 7px;
            border-radius: 10px;
            background: #eef2ff;
            color: #2E37A4;
            font-size: 12px;
            text-align: center;
        }
        .appt-tab.active .tab-count { background: rgba(255, 255, 255, 0.22); color: #fff; }

        /* Fee line */
        .appt-meta-item.fee-line { margin-top: 3px; font-size: 12.5px; }
        .fee-paid { color: #059669; font-weight: 600; }
        .fee-due { color: #d97706; font-weight: 600; }

        /* Empty tab */
        .tab-empty {
            display: flex;
            align-items: center;
            gap: 12px;
            flex-wrap: wrap;
            background: #fff;
            border: 1px dashed #c7d2fe;
            border-radius: 18px;
            padding: 22px 26px;
            color: #4b5563;
            font-size: 14px;
        }
        .tab-empty i { color: #2E37A4; font-size: 20px; }
        .tab-empty-book { margin-left: auto; font-weight: 600; color: #2E37A4 !important; }

        /* Pager */
        .appt-pager {
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 12px;
            margin-top: 6px;
        }
        .pager-info { font-size: 13.5px; color: #6b7280; }
        .pager-buttons { display: flex; gap: 6px; flex-wrap: wrap; }
        .pager-btn {
            min-width: 38px;
            padding: 7px 13px;
            border-radius: 10px;
            border: 1.5px solid #dfe3fb;
            background: #fff;
            color: #2E37A4 !important;
            font-size: 13.5px;
            font-weight: 600;
            text-align: center;
            text-decoration: none;
        }
        .pager-btn:hover { border-color: #2E37A4; }
        .pager-btn.active { background: #2E37A4; border-color: #2E37A4; color: #fff !important; }
        .pager-btn.disabled { color: #b7bdd6 !important; border-color: #eef0f7; pointer-events: none; }

        .appt-meta-item > span { flex-wrap: wrap; max-width: 100%; }
        .nowrap { white-space: nowrap; }

        @media (max-width: 576px) {
            .appointments-page-wrapper { padding: 0 16px 48px; }
            .appointment-card-modern { padding: 20px 18px; gap: 14px; }
            .appt-left-col { align-items: flex-start; gap: 14px; }
            .appt-doc-img { width: 58px; height: 58px; border-width: 2px; padding: 2px; }
            .appt-doc-name { font-size: 18px; }
            .appt-right-col { flex-wrap: wrap; justify-content: flex-start; gap: 10px; min-width: 0; }
            .appt-right-col .status-badge-pill { width: 100%; justify-content: center; }
            .reschedule-btn-modern, .cancel-btn-modern, .cancelled-btn-modern {
                flex: 1; margin-bottom: 0; padding: 8px 12px; white-space: nowrap; text-align: center;
            }
            .appt-tabs { width: 100%; }
            .appt-tab { flex: 1; justify-content: center; padding: 9px 10px; }
            .appt-pager { justify-content: center; }
        }

        /* Empty state: no appointments booked yet */
        .no-appt-card {
            background: #fff;
            border-radius: 18px;
            box-shadow: 0 4px 20px rgba(46, 55, 164, 0.08);
            padding: 48px 24px;
            text-align: center;
        }
        .no-appt-card .no-appt-icon {
            font-size: 52px;
            color: #c7d2fe;
            margin-bottom: 16px;
        }
        .no-appt-card h4 {
            font-weight: 700;
            color: #1f2937;
            margin-bottom: 8px;
        }
        .no-appt-card p {
            color: #6b7280;
            margin-bottom: 22px;
        }
        .no-appt-card .no-appt-btn {
            display: inline-block;
            background: #2E37A4;
            color: #fff;
            padding: 10px 26px;
            border-radius: 10px;
            font-weight: 600;
            text-decoration: none;
        }
        .no-appt-card .no-appt-btn:hover {
            background: #232b85;
            color: #fff;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Breadcrumb Area -->
    <div class="breadcrumb-area text-center">
        <h2 class="page-title">My Appointments</h2>
    </div>

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="appointments-page-wrapper">
                <asp:Panel ID="pnlList" runat="server">
                <%-- Upcoming (soonest first) / Past / Cancelled, with counts --%>
                <div class="appt-tabs" role="tablist">
                    <asp:LinkButton ID="tabUpcoming" runat="server" CommandArgument="upcoming" OnCommand="Tab_Command" CausesValidation="false" />
                    <asp:LinkButton ID="tabPast" runat="server" CommandArgument="past" OnCommand="Tab_Command" CausesValidation="false" />
                    <asp:LinkButton ID="tabCancelled" runat="server" CommandArgument="cancelled" OnCommand="Tab_Command" CausesValidation="false" />
                </div>

                <asp:Repeater ID="rptAppointments" runat="server">
                    <ItemTemplate>
                        <div class="appointment-card-modern">
                            <!-- Left Col: Avatar + Info -->
                            <div class="appt-left-col">
                                <img src='<%# ResolveUrl(Eval("DoctorImage").ToString()) %>' alt="Doctor Image" class="appt-doc-img"
                                    onerror="this.src='https://ui-avatars.com/api/?name=<%# Eval("DoctorName").ToString().Replace(" ", "+") %>&background=2E37A4&color=fff&size=140'" />
                                
                                <div class="appt-details-main">
                                    <h4 class="appt-doc-name">Dr. <%# Eval("DoctorName") %></h4>
                                    <span class="appt-spec-badge"><%# Eval("Specialization") %></span>
                                    
                                    <div class="appt-meta-list">
                                        <div class="appt-meta-item" style="margin-bottom:3px;">
                                            <span style="background: #eef2ff; color: #2E37A4; border: 1px solid #c7d2fe; padding: 3px 10px; border-radius: 8px; font-size: 13px; font-weight: 600; display: inline-flex; align-items: center; gap: 6px;">
                                                <i class="fa-solid fa-hospital-user" style="color: #2E37A4; width: auto; font-size: 13px;"></i>
                                                Patient: <strong><%# HttpUtility.HtmlEncode(Eval("PatientName")) %></strong><%# Convert.ToInt32(Eval("PatientAge")) > 0 ? " (" + Eval("PatientAge") + " yrs" + (string.IsNullOrEmpty(Eval("PatientGender").ToString()) ? "" : ", " + Eval("PatientGender")) + ")" : "" %><%# Eval("PatientCode").ToString() == "" ? "" : " <span title=\"Patient ID: quote it at the clinic\" style=\"font-size: 12px; font-weight: 600; color: #4b5563; background: #eef2ff; border-radius: 6px; padding: 1px 7px; margin-left: 4px; white-space: nowrap;\">ID " + Eval("PatientCode") + "</span>" %>
                                            </span>
                                        </div>
                                        <div class="appt-meta-item">
                                            <i class="fa-solid fa-location-dot"></i>
                                            <span><%# Eval("Address") %></span>
                                        </div>
                                        <div class="appt-meta-item">
                                            <i class="fa-regular fa-calendar-check"></i>
                                            <span><strong><%# Eval("AppointDate", "{0:ddd, dd MMM yyyy}") %></strong>&nbsp;&bull;&nbsp;<%# Eval("AppointTime") %><%# Eval("TokenNo").ToString() == "" ? "" : "&nbsp;&bull; <span class=\"nowrap\">Token <strong>" + HttpUtility.HtmlEncode(Eval("TokenNo")) + "</strong></span>" %></span>
                                        </div>
                                        <%# FeeLine(Container.DataItem) %>
                                    </div>
                                </div>
                            </div>

                            <!-- Right Col: Status + Action -->
                            <div class="appt-right-col">
                                <span class='status-badge-pill <%# StatusClass(Eval("Status")) %>'>
                                    <i class='fa-solid <%# StatusIcon(Eval("Status")) %>'></i>
                                    <%# Eval("Status") %>
                                </span>

                                <%-- Online changes close PatientCutoffHours (2 h) before the appointment --%>
                                <asp:HyperLink ID="lnkReschedule" runat="server" CssClass="reschedule-btn-modern"
                                    NavigateUrl='<%# "~/Features/Appointments/Reschedule.aspx?id=" + Eval("AppointmentID") %>'
                                    Visible='<%# CanChangeOnline(Eval("Status"), Eval("AppointDate"), Eval("AppointTimeRaw")) %>'>
                                    <i class="fa-regular fa-calendar"></i> Reschedule
                                </asp:HyperLink>

                                <%-- Finished appointments: the status badge says it all --%>
                                <asp:Button ID="btnCancel" runat="server" Visible='<%# IsActive(Eval("Status")) %>'
                                    Text='<%# CancelText(Eval("Status"), Eval("AppointDate"), Eval("AppointTimeRaw")) %>'
                                    CssClass='<%# CancelCss(Eval("Status"), Eval("AppointDate"), Eval("AppointTimeRaw")) %>'
                                    Enabled='<%# CanChangeOnline(Eval("Status"), Eval("AppointDate"), Eval("AppointTimeRaw")) %>'
                                    ToolTip='<%# CanChangeOnline(Eval("Status"), Eval("AppointDate"), Eval("AppointTimeRaw")) ? "" : "Online changes close 2 hours before the appointment" %>'
                                    CommandArgument='<%# Eval("AppointmentID") %>'
                                    CausesValidation="false"
                                    OnClick="CancelAppointment" />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlTabEmpty" runat="server" Visible="false" CssClass="tab-empty">
                    <i class="fa-regular fa-calendar"></i>
                    <span><asp:Literal ID="litTabEmpty" runat="server" /></span>
                    <asp:HyperLink ID="lnkTabEmptyBook" runat="server" NavigateUrl="~/Features/Doctors/Doctors-Team.aspx" CssClass="tab-empty-book">Book an appointment</asp:HyperLink>
                </asp:Panel>

                <%-- 5 per page --%>
                <asp:Panel ID="pnlPager" runat="server" CssClass="appt-pager">
                    <span class="pager-info"><asp:Literal ID="litPagerInfo" runat="server" /></span>
                    <div class="pager-buttons">
                        <asp:LinkButton ID="lnkPrev" runat="server" CommandArgument="prev" OnCommand="Page_Command" CausesValidation="false" aria-label="Previous page">&lsaquo; Previous</asp:LinkButton>
                        <asp:Repeater ID="rptPages" runat="server">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkPage" runat="server" CommandArgument='<%# Eval("Index") %>' OnCommand="Page_Command"
                                    CausesValidation="false" CssClass='<%# Eval("Css") %>' Text='<%# Eval("Label") %>' />
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:LinkButton ID="lnkNext" runat="server" CommandArgument="next" OnCommand="Page_Command" CausesValidation="false" aria-label="Next page">Next &rsaquo;</asp:LinkButton>
                    </div>
                </asp:Panel>
                </asp:Panel>

                <asp:Panel ID="pnlNoAppointments" runat="server" Visible="false" CssClass="no-appt-card">
                    <div class="no-appt-icon"><i class="fa-regular fa-calendar-xmark"></i></div>
                    <h4>No Appointments Booked Yet</h4>
                    <p>You haven't booked any appointments. Find a doctor and book your first appointment.</p>
                    <a href="<%= ResolveUrl("~/Features/Doctors/Doctors-Team.aspx") %>" class="no-appt-btn">
                        <i class="fa-solid fa-user-doctor"></i>&nbsp; Book an Appointment
                    </a>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        window.onload = function () {
            var successMessage = '<%= HttpUtility.JavaScriptStringEncode(Convert.ToString(Session["SuccessMessage"])) %>';
            var errorMessage = '<%= HttpUtility.JavaScriptStringEncode(Convert.ToString(Session["ErrorMessage"])) %>';

            var Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 4000,
                timerProgressBar: true,
                didOpen: function (toast) {
                    toast.onmouseenter = Swal.stopTimer;
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });

            if (successMessage && successMessage !== 'null' && successMessage !== '') {
                Toast.fire({ icon: 'success', title: successMessage });
                <% Session["SuccessMessage"] = null; %>
                <% Session["RedirectToDefault"] = null; %>
            }

            if (errorMessage && errorMessage !== 'null' && errorMessage !== '') {
                Toast.fire({ icon: 'error', title: errorMessage });
                <% Session["ErrorMessage"] = null; %>
            }
        };
    </script>

    <script>
        var swalConfirmedCancel = null;

        // SweetAlert2 event delegation — intercepts cancel-btn-modern clicks
        document.addEventListener('click', function (e) {
            var btn = e.target.closest('.cancel-btn-modern');
            if (!btn) return;

            if (btn === swalConfirmedCancel) {
                swalConfirmedCancel = null;
                return; // Allow native/Ajax submit to proceed!
            }

            e.preventDefault();
            e.stopImmediatePropagation();

            Swal.fire({
                title: 'Cancel Appointment?',
                text: 'This appointment will be cancelled. You can re-book later.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#2E37A4',
                cancelButtonColor: '#6b7280',
                confirmButtonText: 'Yes, Cancel It',
                cancelButtonText: 'Keep It',
                customClass: { popup: 'swal-modern-popup' }
            }).then(function (result) {
                if (result.isConfirmed) {
                    swalConfirmedCancel = btn;
                    if (btn.form && typeof btn.form.requestSubmit === 'function') {
                        btn.form.requestSubmit(btn);
                    } else {
                        btn.click();
                    }
                }
            });
            return false;
        }, true);
    </script>

</asp:Content>
