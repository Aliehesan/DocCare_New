<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="DocCare_Clinic.Register" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base href="<%= ResolveUrl("~/") %>" />
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=0">
    <link rel="shortcut icon" type="image/x-icon" href="~/assets/img/favicon.png">
    <title>DocCare - Register</title>
    <link rel="stylesheet" type="text/css" href="~/assets/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/assets/css/feather.css" />
    <link rel="stylesheet" href="~/assets/plugins/fontawesome/css/fontawesome.min.css" />
    <link rel="stylesheet" href="~/assets/plugins/fontawesome/css/all.min.css" />
    <link rel="stylesheet" type="text/css" href="~/assets/css/style.css" />
    <!-- SweetAlert2: loaded in the page head so alerts registered from code-behind
         (rendered inside the form) can call these functions -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        function swalAlert(icon, title, text) {
            Swal.fire({
                icon: icon || 'error',
                title: title,
                text: text,
                confirmButtonColor: '#2E37A4',
                customClass: { popup: 'swal-modern-popup' }
            });
        }

        // Success message that moves on to the next page after a short countdown (or on OK)
        function swalSuccessRedirect(title, text, url) {
            Swal.fire({
                icon: 'success',
                title: title,
                text: text,
                timer: 2500,
                timerProgressBar: true,
                confirmButtonText: 'Go to Login',
                confirmButtonColor: '#2E37A4',
                allowOutsideClick: false,
                customClass: { popup: 'swal-modern-popup' }
            }).then(function () {
                window.location.href = url;
            });
        }
    </script>
    <style>
        .swal-modern-popup { border-radius: 20px !important; font-family: inherit !important; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main-wrapper login-body">
        <div class="container-fluid px-0">
            <div class="row">
                <!-- Left Section with Image -->
                <div class="col-lg-6 login-wrap">
                    <div class="login-sec">
                        <div class="log-img">
                            <img class="img-fluid" src="assets/img/login-02.png" alt="Logo" />
                        </div>
                    </div>
                </div>
                <!-- Right Section with Registration Form -->
                <div class="col-lg-6 login-wrap-bg">
                    <div class="login-wrapper">
                        <div class="loginbox">
                            <div class="login-right">
                                <div class="login-right-wrap">
                                    <div class="account-logo">
                                        <a href="<%= ResolveUrl("~/Features/Accounts/Register.aspx") %>">
                                            <img src="assets/img/favicon.png" alt="" />
                                            <b>DocCare</b> </a>
                                    </div>
                                    <h2>
                                        SignUp</h2>
                                    <!-- Registration Form -->
                                    <asp:Panel ID="RegisterPanel" runat="server">
                                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" Placeholder="Name"></asp:TextBox>
                                        <br />
                                        <asp:TextBox ID="txtMobile" CssClass="form-control" runat="server" Placeholder="Enter Mobile Number"></asp:TextBox>
                                        <br />
                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Placeholder="Email"></asp:TextBox>
                                        <br />
                                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control pass-input" TextMode="Password"
                                            Placeholder="Password"></asp:TextBox>
                                        <%--<span class="profile-views feather-eye-off toggle-password" style="position: absolute;
                                            right: 23%; top: 95.5%; transform: translateY(-50%); color: rgba(51, 52, 72, 0.2);
                                            cursor: pointer;"></span>--%>
                                        <br />
                                        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control pass-input-confirm"
                                            TextMode="Password" Placeholder="Confirm Password"></asp:TextBox>
                                        <br />
                                        <%--<asp:RadioButtonList ID="rblGender" runat="server" RepeatDirection="Horizontal" CssClass="form-check-label">
                                            <asp:ListItem Text="Male" Value="Male" class="form-check-inline"></asp:ListItem>
                                            <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                                        </asp:RadioButtonList>
                                        <br />--%>
                                        <asp:TextBox ID="txtAddress" CssClass="form-control" runat="server" TextMode="MultiLine"
                                            Rows="3" Placeholder="Address"></asp:TextBox><br />
                                        <div class="form-check">
                                            <asp:CheckBox ID="TermsCheckBox" runat="server" CssClass="form-check-input" />
                                            <label class="form-check-label">
                                                I agree to the <a href="javascript:;">terms of service</a> and <a href="javascript:;">
                                                    privacy policy</a>.
                                            </label>
                                        </div>
                                        <br />
                                        <asp:Button ID="RegisterButton" runat="server" Text="Sign up" CssClass="btn btn-primary btn-block"
                                            OnClick="RegisterButton_Click" />
                                    </asp:Panel>
                                    <div class="next-sign">
                                        <p class="account-subtitle">
                                            Already have an account? <a href="<%= ResolveUrl("~/Features/Accounts/Login.aspx") %>">Login</a></p>
                                        <div class="social-login">
                                            <a href="javascript:;">
                                                <svg width="20" height="20" viewbox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                <path d="M19.9987 10.2231C20.0108 9.53538 19.9382 8.84765 19.785 8.17969H10.2031V11.891H15.8288C15.6151 13.2072 14.861 14.3771 13.7439 15.136L13.7237 15.2585L16.7523 17.5589L16.962 17.5786C18.8857 15.8356 19.9987 13.2704 19.9987 10.2231Z" fill="#4285F4"/>
                                                <path d="M10.2029 19.9962C12.9573 19.9962 15.2721 19.1069 16.9618 17.5733L13.7437 15.1268C12.6992 15.8145 11.4611 16.1623 10.2069 16.1267C7.56145 16.1109 5.22245 14.4351 4.39976 11.9688L4.27878 11.9806L1.12919 14.3679L1.08887 14.4786C2.81489 17.8658 6.34355 20.0001 10.2029 19.9962Z" fill="#34A853"/>
                                                <path d="M4.39571 11.9785C4.17391 11.3422 4.05696 10.6742 4.05696 9.99835C4.06099 9.32643 4.17391 8.65847 4.38765 8.01817L4.38361 7.88774L1.1937 5.46094L1.08885 5.50836C-0.362949 8.33437 -0.362949 11.6623 1.08885 14.4883L4.39571 11.9785Z" fill="#FBBC05"/>
                                                <path d="M10.2029 3.86609C11.6668 3.84238 13.0783 4.37596 14.147 5.35617L17.0264 2.60131C15.1793 0.901758 12.7315 -0.0270664 10.1989 0.00060078C6.33953 0.00060078 2.81087 2.13097 1.08081 5.51427L4.37961 8.02407C5.21036 5.55774 7.55339 3.8819 10.2029 3.86609Z" fill="#EB4335"/>
                                                </svg>
                                            </a><a href="javascript:;">
                                                <svg width="20" height="20" viewbox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                <path d="M10 20C15.5228 20 20 15.5228 20 10C20 4.47715 15.5228 0 10 0C4.47715 0 0 4.47715 0 10C0 15.5228 4.47715 20 10 20Z" fill="#3C5A9A"/>
                                                <path d="M13.2528 3.07031H11.0376C9.723 3.07031 8.2608 3.62322 8.2608 5.52878C8.26722 6.19276 8.2608 6.82865 8.2608 7.5443H6.73999V9.96437H8.30786V16.9313H11.1889V9.91839H13.0905L13.2625 7.53751H11.1393C11.1393 7.53751 11.144 6.47839 11.1393 6.17082C11.1393 5.41778 11.9228 5.46091 11.97 5.46091C12.3428 5.46091 13.0678 5.46199 13.2539 5.46091V3.07031H13.2528Z" fill="white"/>
                                                </svg>
                                            </a><a href="javascript:;">
                                                <svg width="20" height="17" viewbox="0 0 20 17" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                <path d="M20 1.9246C19.2645 2.25049 18.4744 2.47069 17.6438 2.57022C18.4911 2.06289 19.1412 1.25779 19.4477 0.299448C18.655 0.769831 17.7768 1.11157 16.8423 1.29478C16.0935 0.498549 15.0277 0 13.8474 0C11.582 0 9.74457 1.83739 9.74457 4.10463C9.74468 4.41884 9.78013 4.73204 9.85026 5.03833C6.43884 4.86654 3.41671 3.2326 1.39259 0.75045C1.04029 1.35822 0.837662 2.06289 0.837662 2.81334C0.837662 4.23672 1.56262 5.49368 2.66273 6.22919C2.01117 6.20844 1.37398 6.03237 0.804198 5.71565V5.7685C0.804198 7.75649 2.21793 9.4151 4.09584 9.79212C3.74346 9.88719 3.38004 9.93517 3.01506 9.9348C2.75588 9.93501 2.49726 9.91083 2.24261 9.86257C2.76493 11.4921 4.27995 12.6794 6.07595 12.7111C4.67101 13.8122 2.90145 14.4684 0.979483 14.4684C0.648294 14.4684 0.321498 14.449 0 14.4111C1.81628 15.5765 3.97254 16.2547 6.28909 16.2547C13.8377 16.2547 17.9644 10.0026 17.9644 4.58027C17.9644 4.40412 17.96 4.22617 17.9521 4.05002C18.7536 3.47042 19.4495 2.74904 19.9982 1.92723L20 1.9246Z" fill="#1DA1F2"/>
                                                </svg>
                                            </a>
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
    </form>
    <script src="assets/js/jquery-3.7.1.min.js" type="text/javascript"></script>
    <script src="assets/js/bootstrap.bundle.min.js" type="text/javascript"></script>
    <script src="assets/js/feather.min.js" type="text/javascript"></script>
    <script src="assets/js/app.js" type="text/javascript"></script>
    <script src="assets/js/rocket-loader.min.js" defer></script>
</body>
</html>
