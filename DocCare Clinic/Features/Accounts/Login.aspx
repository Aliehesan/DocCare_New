<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DocCare_Clinic.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base href="<%= ResolveUrl("~/") %>" />
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=0">
    <link rel="shortcut icon" type="image/x-icon" href="~/assets/img/favicon.png">
    <title>DocCare Login</title>
    <link rel="stylesheet" type="text/css" href="~/assets/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/assets/css/feather.css" />
    <link rel="stylesheet" href="~/assets/plugins/fontawesome/css/fontawesome.min.css" />
    <link rel="stylesheet" href="~/assets/plugins/fontawesome/css/all.min.css" />
    <link rel="stylesheet" type="text/css" href="~/assets/css/style.css" />
    <asp:PlaceHolder runat="server">
        <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
        <script src="assets/js/Admin.js"></script>
    </asp:PlaceHolder>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main-wrapper login-body">
        <div class="container-fluid px-0">
            <div class="row">
                <div class="col-lg-6 login-wrap">
                    <div class="login-sec">
                        <div class="log-img">
                            <img class="img-fluid" src="assets/img/login-02.png" alt="Logo" />
                        </div>
                    </div>
                </div>
                <div class="col-lg-6 login-wrap-bg">
                    <div class="login-wrapper">
                        <div class="loginbox">
                            <div class="login-right">
                                <div class="login-right-wrap">
                                    <div class="account-logo">
                                        <a href="<%= ResolveUrl("~/Features/Site/Default.aspx") %>">
                                            <img src="assets/img/favicon.png" alt="Logo" style="width: 50px; height: 50px;" />
                                            <b>DocCare</b> </a>
                                    </div>
                                    <h2>
                                        Login</h2>
                                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" ForeColor="Red"
                                        Visible="False" Font-Size="13px"></asp:Label>
                                    <br />
                                    <br />
                                    <asp:Panel ID="pnlLogin" runat="server">
                                        <div class="input-block">
                                            <label>
                                                Email <span class="login-danger">*</span></label>
                                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEmail"
                                                ErrorMessage="*Please enter email" ForeColor="Red" Font-Size="12px"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEmail"
                                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                        </div>
                                        <div class="input-block">
                                            <label>
                                                Password <span class="login-danger">*</span></label>
                                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control pass-input" TextMode="Password"></asp:TextBox>
                                            <span class="profile-views feather-eye-off toggle-password" style="position: absolute;
                                                right: 17px; top: 35%; transform: translateY(-50%); color: rgba(51, 52, 72, 0.2);
                                                cursor: pointer;"></span>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword"
                                                ErrorMessage="*Please enter password" ForeColor="Red" Font-Size="12px"></asp:RequiredFieldValidator>
                                        </div>
                                        <div class="form-check" style="display: flex; justify-content: space-between; align-items: center;
                                            margin-bottom: 1.125rem;">
                                            <div style="display: flex; align-items: center;">
                                                <asp:CheckBox ID="chkRememberMe" runat="server" CssClass="form-check-input" Style="margin-right: 5px;" />
                                                <label class="custom_check mb-0" style="color: #4a4a4a; font-size: 14px;">
                                                    Remember me
                                                </label>
                                            </div>
                                            <div>
                                                <a href="<%= ResolveUrl("~/Features/Accounts/Forget-Password.aspx") %>" style="text-decoration: none; color: #00c7c7; font-size: 14px;
                                                    transition: 0.3s;" onmouseover="this.style.color='#000'; this.style.textDecoration='underline';"
                                                    onmouseout="this.style.color='#00c7c7'; this.style.textDecoration='none';">Forgot
                                                    Password? </a>
                                            </div>
                                        </div>
                                        <div class="input-block login-btn">
                                            <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-primary btn-block" Text="Login"
                                                OnClick="btnLogin_Click" />
                                        </div>
                                    </asp:Panel>
                                    <div class="next-sign">
                                        <p id="signUpParagraph" runat="server" class="account-subtitle">
                                            Need an account? <a id="lnkSignUp" runat="server" href="~/Features/Accounts/Register.aspx">Sign Up</a>
                                        </p>
                                        <div class="social-login">
                                            <a href="https://www.linkedin.com/in/doccare-support-117309358?utm_source=share&utm_campaign=share_via&utm_content=profile&utm_medium=android_app">
                                                <svg fill="#007AB9" height="20" width="20" version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg"
                                                    xmlns:xlink="http://www.w3.org/1999/xlink" viewbox="0 0 310 310" xml:space="preserve"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <g id="XMLID_801_"> <path id="XMLID_802_" d="M72.16,99.73H9.927c-2.762,0-5,2.239-5,5v199.928c0,2.762,2.238,5,5,5H72.16c2.762,0,5-2.238,5-5V104.73 C77.16,101.969,74.922,99.73,72.16,99.73z"></path> <path id="XMLID_803_" d="M41.066,0.341C18.422,0.341,0,18.743,0,41.362C0,63.991,18.422,82.4,41.066,82.4 c22.626,0,41.033-18.41,41.033-41.038C82.1,18.743,63.692,0.341,41.066,0.341z"></path> <path id="XMLID_804_" d="M230.454,94.761c-24.995,0-43.472,10.745-54.679,22.954V104.73c0-2.761-2.238-5-5-5h-59.599 c-2.762,0-5,2.239-5,5v199.928c0,2.762,2.238,5,5,5h62.097c2.762,0,5-2.238,5-5v-98.918c0-33.333,9.054-46.319,32.29-46.319 c25.306,0,27.317,20.818,27.317,48.034v97.204c0,2.762,2.238,5,5,5H305c2.762,0,5-2.238,5-5V194.995 C310,145.43,300.549,94.761,230.454,94.761z"></path> </g> </g></svg>
                                            </a><a href="https://www.facebook.com/share/1BTFJdzGpV/?mibextid=qi2Omg">
                                                <svg width="20" height="20" viewbox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                                            <path d="M10 20C15.5228 20 20 15.5228 20 10C20 4.47715 15.5228 0 10 0C4.47715 0 0 4.47715 0 10C0 15.5228 4.47715 20 10 20Z" fill="#3C5A9A"/>
                                            <path d="M13.2528 3.07031H11.0376C9.723 3.07031 8.2608 3.62322 8.2608 5.52878C8.26722 6.19276 8.2608 6.82865 8.2608 7.5443H6.73999V9.96437H8.30786V16.9313H11.1889V9.91839H13.0905L13.2625 7.53751H11.1393C11.1393 7.53751 11.144 6.47839 11.1393 6.17082C11.1393 5.41778 11.9228 5.46091 11.97 5.46091C12.3428 5.46091 13.0678 5.46199 13.2539 5.46091V3.07031H13.2528Z" fill="white"/>
                                            </svg>
                                            </a><a href="https://x.com/DocCare2025?t=msVLwbeOVf5J-tSXtQd1wQ&s=08">
                                                <svg width="20" height="20" xmlns="http://www.w3.org/2000/svg" viewbox="0 0 512 512"><!--!Font Awesome Free 6.7.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2025 Fonticons, Inc.--><path d="M389.2 48h70.6L305.6 224.2 487 464H345L233.7 318.6 106.5 464H35.8L200.7 275.5 26.8 48H172.4L272.9 180.9 389.2 48zM364.4 421.8h39.1L151.1 88h-42L364.4 421.8z"/></svg>
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
    <script src="assets/js/bootstrap.bundle.min.js" type="text/javascript"></script>
    <script src="assets/js/feather.min.js" type="text/javascript"></script>
    <script src="assets/js/app.js" type="text/javascript"></script>
    <script src="assets/js/rocket-loader.min.js" defer></script>
</body>
</html>
