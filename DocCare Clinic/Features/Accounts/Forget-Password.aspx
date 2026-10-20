<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Forget-Password.aspx.cs"
    Inherits="DocCare_Clinic.Forget_Password" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base href="<%= ResolveUrl("~/") %>" />
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=0">
    <link rel="shortcut icon" type="image/x-icon" href="~/assets/img/favicon.png">
    <title>DocCare ForgetPassword</title>
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
                <!-- Right Side Form -->
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
                                    <asp:MultiView ID="mvForget" runat="server" ActiveViewIndex="0">
                                        <!-- ========================== View 0: Enter Email ========================== -->
                                        <asp:View ID="View0" runat="server">
                                            <h2>
                                                Reset Password</h2>
                                            <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" ForeColor="Red"
                                                Font-Size="13px" Style="margin-bottom: 10px; display: block;"></asp:Label>
                                            <div class="input-block">
                                                <label>
                                                    Email <span class="login-danger">*</span></label>
                                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Placeholder="Enter your email"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                                                    ErrorMessage="*Email is required" ForeColor="Red" Font-Size="13px" />
                                            </div>
                                            <div class="input-block login-btn">
                                                <asp:Button ID="btnSendOtp" runat="server" Text="Send OTP" CssClass="btn btn-primary btn-block"
                                                    OnClick="btnSendOtp_Click" />
                                            </div>
                                        </asp:View>
                                        <!-- ========================== View 1: Verify OTP ========================== -->
                                        <asp:View ID="View1" runat="server">
                                            <h2>
                                                Verify OTP</h2>
                                            <p class="account-subtitle">
                                                We have sent a 6-digit OTP to your email.</p>
                                            <br />
                                            <asp:Label ID="lblErrorMessage1" runat="server" CssClass="error-message" ForeColor="Red"
                                                Font-Size="13px" Style="margin-bottom: 10px; display: block;"></asp:Label>
                                            <div class="input-block">
                                                <label>
                                                    OTP <span class="login-danger">*</span></label>
                                                <asp:TextBox ID="txtOtp" runat="server" CssClass="form-control" Placeholder="Enter 6-digit OTP"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvOtp" runat="server" ControlToValidate="txtOtp"
                                                    ErrorMessage="*OTP is required" ForeColor="Red" Font-Size="13px" />
                                            </div>
                                            <div class="input-block login-btn">
                                                <asp:Button ID="btnVerifyOtp" runat="server" Text="Verify OTP" CssClass="btn btn-primary btn-block"
                                                    OnClick="btnVerifyOtp_Click" />
                                            </div>
                                            <div class="next-sign">
                                                <p class="account-subtitle">
                                                    Didn't receive OTP? <a href="javascript:void(0);" onclick="resendOTP()">Resend OTP</a>
                                                </p>
                                            </div>
                                        </asp:View>
                                        <!-- ========================== View 2: Change Password ========================== -->
                                        <asp:View ID="View2" runat="server">
                                            <h2>
                                                Change Password</h2>
                                            <asp:Label ID="lblErrorMessage2" runat="server" CssClass="error-message" ForeColor="Red"
                                                Font-Size="13px" Style="margin-bottom: 10px; display: block;"></asp:Label>
                                            <div class="input-block">
                                                <label>
                                                    New Password <span class="login-danger">*</span></label>
                                                <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" CssClass="form-control"
                                                    Placeholder="Enter new password"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" ControlToValidate="txtNewPassword"
                                                    ErrorMessage="*New Password is required" ForeColor="Red" Font-Size="13px" />
                                            </div>
                                            <div class="input-block">
                                                <label>
                                                    Confirm Password <span class="login-danger">*</span></label>
                                                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control"
                                                    Placeholder="Confirm new password"></asp:TextBox>
                                                <asp:CompareValidator ID="cvPassword" runat="server" ControlToValidate="txtConfirmPassword"
                                                    ControlToCompare="txtNewPassword" ErrorMessage="*Passwords do not match" ForeColor="Red"
                                                    Font-Size="13px" />
                                            </div>
                                            <div class="input-block login-btn">
                                                <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="btn btn-primary btn-block"
                                                    OnClick="btnChangePassword_Click" />
                                            </div>
                                        </asp:View>
                                    </asp:MultiView>
                                    <asp:HiddenField ID="hfOtp" runat="server" />
                                    <!-- Back to Login -->
                                    <div class="next-sign">
                                        <p class="account-subtitle">
                                            Need an account? <a href="<%= ResolveUrl("~/Features/Accounts/Login.aspx") %>">Login</a>
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- End Right Side Form -->
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
