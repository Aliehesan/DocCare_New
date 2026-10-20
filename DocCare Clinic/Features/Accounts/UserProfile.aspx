<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Shared/DocCare.Master" AutoEventWireup="true"
    CodeBehind="UserProfile.aspx.cs" Inherits="DocCare_Clinic.UserProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* ── Page Wrapper ── */
        .profile-page {
            padding: 0 6% 60px 6%;
        }

        /* ── Top Identity Card ── */
        .identity-card {
            background: #ffffff;
            border-radius: 24px;
            padding: 30px 40px;
            display: flex;
            align-items: center;
            gap: 30px;
            box-shadow: 0 8px 30px rgba(46, 55, 164, 0.07);
            margin-bottom: 30px;
            flex-wrap: wrap;
        }

        .identity-avatar {
            width: 100px;
            height: 100px;
            border-radius: 50%;
            object-fit: cover;
            border: 4px solid var(--primary);
            padding: 4px;
            background: #fff;
            flex-shrink: 0;
        }

        .identity-info {
            flex: 1;
        }

        .identity-info h2 {
            font-size: 28px;
            font-weight: 700;
            color: var(--text-dark);
            margin: 0 0 6px 0;
        }

        .identity-info p {
            color: var(--text-light);
            font-size: 15px;
            margin: 0 0 4px 0;
        }

        .identity-info p i {
            color: var(--primary);
            margin-right: 8px;
            width: 16px;
        }

        .identity-actions {
            display: flex;
            flex-direction: column;
            gap: 12px;
            margin-left: auto;
        }

        .btn-profile {
            padding: 11px 28px;
            border-radius: 50px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            text-decoration: none !important;
            text-align: center;
            transition: all 0.3s ease;
            white-space: nowrap;
        }

        .btn-profile-primary {
            background: var(--primary);
            color: #fff;
            border: 2px solid var(--primary);
        }

        .btn-profile-primary:hover {
            background: var(--primary-hover);
            border-color: var(--primary-hover);
            color: #fff;
        }

        .btn-profile-outline {
            background: transparent;
            color: var(--primary);
            border: 2px solid var(--primary);
        }

        .btn-profile-outline:hover {
            background: var(--primary);
            color: #fff;
        }

        /* ── Two Column Grid ── */
        .profile-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 30px;
        }

        /* ── Info Card ── */
        .info-card {
            background: #ffffff;
            border-radius: 24px;
            padding: 30px;
            box-shadow: 0 8px 30px rgba(46, 55, 164, 0.07);
        }

        .info-card-title {
            font-size: 17px;
            font-weight: 700;
            color: var(--text-dark);
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 1px solid #f3f4f6;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .info-card-title i {
            color: var(--primary);
            font-size: 18px;
        }

        /* ── Detail Row ── */
        .detail-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 14px 0;
            border-bottom: 1px solid #f3f4f6;
            font-size: 14px;
        }

        .detail-row:last-child {
            border-bottom: none;
            padding-bottom: 0;
        }

        .detail-label {
            color: var(--text-light);
            font-weight: 500;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .detail-label i {
            color: var(--primary);
            width: 16px;
        }

        .detail-value {
            color: var(--text-dark);
            font-weight: 600;
            text-align: right;
            max-width: 55%;
            word-break: break-word;
        }

        /* ── Appointment Rows ── */
        .appt-row {
            display: flex;
            align-items: center;
            gap: 15px;
            padding: 14px 0;
            border-bottom: 1px solid #f3f4f6;
        }

        .appt-row:last-child {
            border-bottom: none;
            padding-bottom: 0;
        }

        .appt-num {
            width: 30px;
            height: 30px;
            border-radius: 50%;
            background: rgba(46, 55, 164, 0.08);
            color: var(--primary);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            font-weight: 700;
            flex-shrink: 0;
        }

        .appt-info {
            flex: 1;
        }

        .appt-info strong {
            display: block;
            font-size: 14px;
            color: var(--text-dark);
            margin-bottom: 2px;
        }

        .appt-info span {
            font-size: 12px;
            color: var(--text-light);
        }

        .appt-badge {
            padding: 5px 14px;
            border-radius: 30px;
            font-size: 12px;
            font-weight: 600;
        }

        .badge-confirmed {
            background: #d1fae5;
            color: #065f46;
        }

        .badge-cancelled {
            background: #fee2e2;
            color: #991b1b;
        }

        .badge-pending {
            background: #fef3c7;
            color: #92400e;
        }

        .badge-completed {
            background: #e0e7ff;
            color: #3730a3;
        }

        .badge-noshow {
            background: #f3f4f6;
            color: #4b5563;
        }

        .view-all-link {
            display: block;
            text-align: right;
            color: var(--primary);
            font-size: 13px;
            font-weight: 600;
            text-decoration: none !important;
            margin-top: 15px;
        }

        .view-all-link:hover {
            color: var(--primary-hover);
        }

        .no-appt-msg {
            text-align: center;
            color: var(--text-light);
            font-size: 14px;
            padding: 30px 0;
        }

        /* ── Family Members Section ── */
        .family-card-wrapper {
            margin-top: 30px;
        }

        .family-header-flex {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding-bottom: 16px;
            border-bottom: 1px solid #f3f4f6;
            margin-bottom: 18px;
            flex-wrap: wrap;
            gap: 15px;
        }

        .family-header-flex .info-card-title {
            margin-bottom: 0;
            padding-bottom: 0;
            border-bottom: none;
        }

        .btn-add-family {
            background: var(--primary);
            color: #ffffff;
            border: none;
            padding: 9px 22px;
            border-radius: 30px;
            font-size: 13.5px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 4px 14px rgba(46, 55, 164, 0.2);
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .btn-add-family:hover {
            background: var(--primary-hover);
            transform: translateY(-2px);
            color: #ffffff;
            box-shadow: 0 6px 18px rgba(46, 55, 164, 0.3);
        }

        .family-modal-close {
            position: relative;
            width: 40px;
            height: 40px;
            padding: 0;
            border: none;
            border-radius: 50%;
            background: #f1f3f5;
            opacity: 1;
            flex-shrink: 0;
            transition: background-color 0.2s ease, transform 0.2s ease;
        }

        .family-modal-close::before,
        .family-modal-close::after {
            content: "";
            position: absolute;
            top: 19px;
            left: 12px;
            width: 16px;
            height: 2px;
            border-radius: 2px;
            background: #6b7280;
        }

        .family-modal-close::before {
            transform: rotate(45deg);
        }

        .family-modal-close::after {
            transform: rotate(-45deg);
        }

        .family-modal-close:hover {
            background: #e5e7eb;
            opacity: 1;
            transform: scale(1.04);
        }

        .family-tiles-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
            gap: 18px;
            margin-top: 15px;
        }

        .family-member-card {
            background: #f9fafb;
            border-radius: 18px;
            padding: 18px 20px;
            border: 1px solid #f0f2f5;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 15px;
            transition: all 0.25s ease;
        }

        .family-member-card:hover {
            background: #ffffff;
            border-color: rgba(46, 55, 164, 0.25);
            box-shadow: 0 6px 20px rgba(46, 55, 164, 0.06);
            transform: translateY(-2px);
        }

        .family-member-left {
            display: flex;
            align-items: center;
            gap: 14px;
        }

        .family-avatar-circle {
            width: 44px;
            height: 44px;
            border-radius: 50%;
            background: rgba(46, 55, 164, 0.08);
            color: var(--primary);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            flex-shrink: 0;
        }

        .family-member-info strong {
            display: block;
            font-size: 15px;
            color: var(--text-dark);
            margin-bottom: 3px;
        }

        .family-member-info span {
            font-size: 12.5px;
            color: var(--text-light);
        }

        .family-actions-group {
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .btn-family-edit {
            background: #ffffff;
            color: var(--primary);
            border: 1.5px solid rgba(46, 55, 164, 0.2);
            padding: 5px 14px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .btn-family-edit:hover {
            background: var(--primary);
            border-color: var(--primary);
            color: #ffffff;
        }

        .btn-family-delete {
            background: #fff1f2;
            color: #e11d48;
            border: 1.5px solid rgba(225, 29, 72, 0.2);
            padding: 5px 14px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .btn-family-delete:hover {
            background: #e11d48;
            border-color: #e11d48;
            color: #ffffff;
        }

        .swal-modern-toast {
            border-radius: 16px !important;
            box-shadow: 0 10px 30px rgba(46, 55, 164, 0.12) !important;
            border: 1px solid #e5e7eb !important;
            padding: 12px 20px !important;
            font-family: inherit !important;
            font-size: 14px !important;
            font-weight: 600 !important;
        }

        @media (max-width: 991px) {
            .profile-grid { grid-template-columns: 1fr; }
            .identity-actions { flex-direction: row; margin-left: 0; width: 100%; }
        }

        @media (max-width: 575px) {
            .profile-page { padding: 0 15px 40px; }
            .identity-card { flex-direction: column; text-align: center; }
            .family-tiles-grid { grid-template-columns: 1fr; }
        }
    </style>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>

    <script>
        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }

        function openAddFamilyModal() {
            var modal = new bootstrap.Modal(document.getElementById('addFamilyModal'));
            modal.show();
        }

        function openEditFamilyModal() {
            var modal = new bootstrap.Modal(document.getElementById('editFamilyModal'));
            modal.show();
        }

        document.addEventListener('DOMContentLoaded', function () {
            const txtDob = document.getElementById('<%= txtFamilyDob.ClientID %>');
            const txtAge = document.getElementById('<%= txtFamilyAge.ClientID %>');
            const txtEditDob = document.getElementById('<%= txtEditFamilyDob.ClientID %>');
            const txtEditAge = document.getElementById('<%= txtEditFamilyAge.ClientID %>');

            if (txtAge) txtAge.setAttribute('readonly', 'readonly');
            if (txtEditAge) txtEditAge.setAttribute('readonly', 'readonly');

            function calculateAge(dobInput, ageInput) {
                if (!dobInput || !ageInput) return;
                function update() {
                    const dob = new Date(dobInput.value);
                    if (!isNaN(dob.getTime())) {
                        const today = new Date();
                        let age = today.getFullYear() - dob.getFullYear();
                        const monthDiff = today.getMonth() - dob.getMonth();
                        const dayDiff = today.getDate() - dob.getDate();

                        if (monthDiff < 0 || (monthDiff === 0 && dayDiff < 0)) {
                            age--;
                        }

                        ageInput.value = age >= 0 ? age : '';
                    } else {
                        ageInput.value = '';
                    }
                }
                dobInput.addEventListener('change', update);
                dobInput.addEventListener('input', update);
            }

            calculateAge(txtDob, txtAge);
            calculateAge(txtEditDob, txtEditAge);
        });

        window.onload = function () {
            var successMessage = '<%= Session["SuccessMessage"] %>';
            var errorMessage = '<%= Session["ErrorMessage"] %>';

            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 2500,
                timerProgressBar: true,
                customClass: {
                    popup: 'swal-modern-toast'
                }
            });

            if (successMessage && successMessage !== "null") {
                Toast.fire({
                    icon: 'success',
                    title: successMessage
                });
                <% Session["SuccessMessage"] = null; %>
            }

            if (errorMessage && errorMessage !== "null") {
                Toast.fire({
                    icon: 'error',
                    title: errorMessage
                });
                <% Session["ErrorMessage"] = null; %>
            }

            var swalConfirmedDelete = null;

            // SweetAlert2 — intercept Delete Family Member button (inside Repeater)
            document.addEventListener('click', function (e) {
                var btn = e.target.closest('.btn-family-delete');
                if (!btn) return;

                if (btn === swalConfirmedDelete) {
                    swalConfirmedDelete = null;
                    return; // Allow native form submit to proceed!
                }

                e.preventDefault();
                e.stopImmediatePropagation();

                Swal.fire({
                    title: 'Remove Family Member?',
                    text: 'This action cannot be undone.',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#2E37A4',
                    cancelButtonColor: '#6b7280',
                    confirmButtonText: 'Yes, Remove',
                    cancelButtonText: 'Cancel',
                    customClass: { popup: 'swal-modern-popup' }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        swalConfirmedDelete = btn;
                        if (btn.form && typeof btn.form.requestSubmit === 'function') {
                            btn.form.requestSubmit(btn);
                        } else {
                            btn.click();
                        }
                    }
                });
                return false;
            }, true);
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Page Title -->
    <div class="breadcrumb-area text-center">
        <h2 class="page-title">My Profile</h2>
    </div>

    <div class="profile-page">

        <!-- ── Top Identity Card ── -->
        <div class="identity-card">
            <img src='https://ui-avatars.com/api/?name=<%= Server.UrlEncode(Session["Name"] != null ? Session["Name"].ToString() : "User") %>&background=2E37A4&color=fff&size=200'
                 class="identity-avatar" alt="Profile Avatar" />

            <div class="identity-info">
                <h2><asp:Label ID="lblName" runat="server"></asp:Label></h2>
                <p><i class="fa-regular fa-envelope"></i><asp:Label ID="lblEmail" runat="server"></asp:Label></p>
                <p><i class="fa-solid fa-phone"></i><asp:Label ID="lblPhone" runat="server"></asp:Label></p>
            </div>

            <div class="identity-actions">
                <a href="<%= ResolveUrl("~/Features/Accounts/EditUser.aspx") %>" class="btn-profile btn-profile-primary">
                    <i class="fa-solid fa-pen-to-square"></i> &nbsp;Edit Profile
                </a>
                <a href='Features/Accounts/ChangePasswordUser.aspx?UserId=<%= Session["UserId"] %>' class="btn-profile btn-profile-outline">
                    <i class="fa-solid fa-lock"></i> &nbsp;Change Password
                </a>
            </div>
        </div>

        <!-- ── Two Column Grid ── -->
        <div class="profile-grid">

            <!-- LEFT: Personal Details -->
            <div class="info-card">
                <div class="info-card-title">
                    <i class="fa-regular fa-id-card"></i> Personal Details
                </div>

                <div class="detail-row">
                    <span class="detail-label"><i class="fa-solid fa-user"></i> Full Name</span>
                    <span class="detail-value"><asp:Label ID="lblNameDetail" runat="server"></asp:Label></span>
                </div>
                <div class="detail-row">
                    <span class="detail-label"><i class="fa-regular fa-envelope"></i> Email</span>
                    <span class="detail-value"><asp:Label ID="lblEmailDetail" runat="server"></asp:Label></span>
                </div>
                <div class="detail-row">
                    <span class="detail-label"><i class="fa-solid fa-phone"></i> Phone</span>
                    <span class="detail-value"><asp:Label ID="lblPhoneDetail" runat="server"></asp:Label></span>
                </div>
                <div class="detail-row">
                    <span class="detail-label"><i class="fa-solid fa-location-dot"></i> Address</span>
                    <span class="detail-value"><asp:Label ID="lblAddress" runat="server"></asp:Label></span>
                </div>
            </div>

            <!-- RIGHT: Recent Appointments -->
            <div class="info-card">
                <div class="info-card-title">
                    <i class="fa-regular fa-calendar-check"></i> Recent Appointments
                </div>

                <asp:Repeater ID="rptRecentAppts" runat="server">
                    <ItemTemplate>
                        <div class="appt-row">
                            <div class="appt-num"><%# Container.ItemIndex + 1 %></div>
                            <div class="appt-info">
                                <strong>Dr. <%# Eval("DoctorName") %></strong>
                                <span><%# Eval("Specialization") %> &nbsp;|&nbsp; <%# Eval("AppointDate", "{0:dd MMM yyyy}") %></span>
                            </div>
                            <span class='appt-badge <%# Eval("Status").ToString() == "Confirmed" ? "badge-confirmed" : Eval("Status").ToString() == "Cancelled" ? "badge-cancelled" : Eval("Status").ToString() == "Completed" ? "badge-completed" : Eval("Status").ToString() == "No Show" ? "badge-noshow" : "badge-pending" %>'>
                                <%# Eval("Status") %>
                            </span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoAppts" runat="server" Visible="false" CssClass="no-appt-msg"
                    Text="No appointments yet."></asp:Label>

                <a href="<%= ResolveUrl("~/Features/Appointments/MyAppointment.aspx") %>" class="view-all-link">View All Appointments <i class="fa-solid fa-arrow-right"></i></a>
            </div>

        </div>

        <!-- ── Family Members & Dependents Section ── -->
        <div class="info-card family-card-wrapper">
            <div class="family-header-flex">
                <div class="info-card-title">
                    <i class="fa-solid fa-users"></i> Family Members & Dependents
                </div>
                <button type="button" class="btn-add-family" onclick="openAddFamilyModal()">
                    <i class="fa-solid fa-user-plus"></i> Add Family Member
                </button>
            </div>

            <p style="font-size: 13.5px; color: var(--text-light); margin-bottom: 15px;">
                Manage registered family members here. Once added, you can quickly choose them when booking appointments.
            </p>

            <div class="family-tiles-grid">
                <asp:Repeater ID="rptFamilyMembers" runat="server">
                    <ItemTemplate>
                        <div class="family-member-card">
                            <div class="family-member-left">
                                <div class="family-avatar-circle">
                                    <i class="fa-solid fa-user"></i>
                                </div>
                                <div class="family-member-info">
                                    <strong><%# HttpUtility.HtmlEncode(Eval("PatientName")) %></strong>
                                    <span>Patient ID <strong><%# Eval("PatientCode") %></strong>&nbsp;&bull;&nbsp;Age: <%# Eval("Age") %> yrs&nbsp;&bull;&nbsp;<%# Eval("Gender") %></span>
                                </div>
                            </div>
                            <div class="family-actions-group">
                                <asp:Button ID="btnEditFamily" runat="server" Text="Edit" CssClass="btn-family-edit"
                                    CommandArgument='<%# Eval("PatientID") %>' OnClick="btnEditFamily_Click" />
                                <asp:Button ID="btnDeleteFamily" runat="server" Text="Delete" CssClass="btn-family-delete"
                                    CommandArgument='<%# Eval("PatientID") %>' OnClick="btnDeleteFamily_Click"
                                    CausesValidation="false" />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <asp:Label ID="lblNoFamily" runat="server" Visible="false" CssClass="no-appt-msg"
                Text="No family members registered yet. Click '+ Add Family Member' to add your dependents."></asp:Label>
        </div>

    </div>

    <!-- ── Add Family Modal ── -->
    <div class="modal fade" id="addFamilyModal" tabindex="-1" aria-labelledby="addFamilyLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content" style="border-radius: 24px; border: none; box-shadow: 0 20px 60px rgba(0,0,0,0.15);">
                <div class="modal-header" style="padding: 24px 30px; border-bottom: 1px solid #f3f4f6;">
                    <h5 class="modal-title" id="addFamilyLabel" style="font-size: 19px; font-weight: 700; color: var(--text-dark);">
                        <i class="fa-solid fa-user-plus" style="color:var(--primary); margin-right:8px;"></i> Add Family Member
                    </h5>
                    <button type="button" class="family-modal-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body" style="padding: 25px 30px;">
                    <div class="mb-3">
                        <label class="form-label" style="font-size: 13.5px; font-weight: 600;">Full Name</label>
                        <asp:TextBox ID="txtFamilyName" runat="server" CssClass="form-control" style="border-radius: 12px; padding: 10px 16px;" Placeholder="Enter full name"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label" style="font-size: 13.5px; font-weight: 600;">Date of Birth</label>
                        <asp:TextBox ID="txtFamilyDob" runat="server" CssClass="form-control" style="border-radius: 12px; padding: 10px 16px;" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label" style="font-size: 13.5px; font-weight: 600;">Age</label>
                        <asp:TextBox ID="txtFamilyAge" runat="server" CssClass="form-control" style="border-radius: 12px; padding: 10px 16px; background-color: #f9fafb;" Placeholder="Auto-calculated from DOB"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label" style="font-size: 13.5px; font-weight: 600;">Gender</label>
                        <asp:RadioButtonList ID="rblFamilyGender" runat="server" RepeatDirection="Horizontal" CellPadding="5">
                            <asp:ListItem Text="&nbsp;Male&nbsp;&nbsp;" Value="Male"></asp:ListItem>
                            <asp:ListItem Text="&nbsp;Female" Value="Female"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
                <div class="modal-footer" style="padding: 18px 30px 24px; border-top: 1px solid #f3f4f6;">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal" style="border-radius: 30px; padding: 9px 20px;">Cancel</button>
                    <asp:Button ID="btnSaveFamily" runat="server" Text="Save Member" CssClass="btn btn-primary" style="background: var(--primary); border: none; border-radius: 30px; padding: 9px 24px; font-weight: 600;" OnClick="btnSaveFamily_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- ── Edit Family Modal ── -->
    <div class="modal fade" id="editFamilyModal" tabindex="-1" aria-labelledby="editFamilyLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content" style="border-radius: 24px; border: none; box-shadow: 0 20px 60px rgba(0,0,0,0.15);">
                <div class="modal-header" style="padding: 24px 30px; border-bottom: 1px solid #f3f4f6;">
                    <h5 class="modal-title" id="editFamilyLabel" style="font-size: 19px; font-weight: 700; color: var(--text-dark);">
                        <i class="fa-solid fa-user-pen" style="color:var(--primary); margin-right:8px;"></i> Edit Family Member
                    </h5>
                    <button type="button" class="family-modal-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body" style="padding: 25px 30px;">
                    <asp:HiddenField ID="hdnEditFamilyId" runat="server" />
                    <div class="mb-3">
                        <label class="form-label" style="font-size: 13.5px; font-weight: 600;">Full Name</label>
                        <asp:TextBox ID="txtEditFamilyName" runat="server" CssClass="form-control" style="border-radius: 12px; padding: 10px 16px;"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label" style="font-size: 13.5px; font-weight: 600;">Date of Birth</label>
                        <asp:TextBox ID="txtEditFamilyDob" runat="server" CssClass="form-control" style="border-radius: 12px; padding: 10px 16px;" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label" style="font-size: 13.5px; font-weight: 600;">Age</label>
                        <asp:TextBox ID="txtEditFamilyAge" runat="server" CssClass="form-control" style="border-radius: 12px; padding: 10px 16px; background-color: #f9fafb;" Placeholder="Auto-calculated from DOB"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label" style="font-size: 13.5px; font-weight: 600;">Gender</label>
                        <asp:RadioButtonList ID="rblEditFamilyGender" runat="server" RepeatDirection="Horizontal" CellPadding="5">
                            <asp:ListItem Text="&nbsp;Male&nbsp;&nbsp;" Value="Male"></asp:ListItem>
                            <asp:ListItem Text="&nbsp;Female" Value="Female"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
                <div class="modal-footer" style="padding: 18px 30px 24px; border-top: 1px solid #f3f4f6;">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal" style="border-radius: 30px; padding: 9px 20px;">Cancel</button>
                    <asp:Button ID="btnUpdateFamily" runat="server" Text="Update Member" CssClass="btn btn-primary" style="background: var(--primary); border: none; border-radius: 30px; padding: 9px 24px; font-weight: 600;" OnClick="btnUpdateFamily_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
