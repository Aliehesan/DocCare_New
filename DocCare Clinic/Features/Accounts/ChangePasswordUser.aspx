<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Shared/DocCare.Master" AutoEventWireup="true"
    CodeBehind="ChangePasswordUser.aspx.cs" Inherits="DocCare_Clinic.ChangePasswordUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-page-wrapper {
            padding: 0 6% 60px 6%;
            display: flex;
            justify-content: center;
        }

        .form-card {
            background: #ffffff;
            border-radius: 24px;
            padding: 40px;
            box-shadow: 0 8px 30px rgba(46, 55, 164, 0.07);
            width: 100%;
            max-width: 500px;
        }

        .form-card-title {
            font-size: 20px;
            font-weight: 700;
            color: var(--text-dark);
            margin-bottom: 25px;
            padding-bottom: 15px;
            border-bottom: 1px solid #f3f4f6;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .form-card-title i {
            color: var(--primary);
            font-size: 20px;
        }

        .form-group-modern {
            margin-bottom: 20px;
        }

        .form-group-modern label {
            display: block;
            font-size: 14px;
            font-weight: 600;
            color: var(--text-dark);
            margin-bottom: 8px;
        }

        .form-control-modern {
            width: 100%;
            padding: 12px 20px;
            border: 1px solid #e5e7eb;
            border-radius: 30px;
            font-size: 14px;
            color: var(--text-dark);
            background: #f9fafb;
            transition: all 0.3s ease;
            box-sizing: border-box;
            outline: none;
        }

        .form-control-modern:focus {
            background: #ffffff;
            border-color: var(--primary);
            box-shadow: 0 0 0 4px rgba(46, 55, 164, 0.1);
        }

        .form-actions {
            display: flex;
            gap: 15px;
            margin-top: 35px;
            justify-content: flex-end;
        }

        .btn-form {
            padding: 12px 30px;
            border-radius: 50px;
            font-size: 15px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            border: none;
            outline: none;
        }

        .btn-save {
            background: var(--primary);
            color: #ffffff;
            box-shadow: 0 4px 15px rgba(46, 55, 164, 0.2);
            width: 100%;
        }

        .btn-save:hover {
            background: var(--primary-hover);
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(46, 55, 164, 0.3);
        }

        /* Message styling */
        .msg-alert {
            padding: 12px 15px;
            border-radius: 12px;
            font-size: 14px;
            font-weight: 500;
            margin-bottom: 20px;
            display: block;
        }
        .msg-success {
            background: #d1fae5;
            color: #065f46;
        }
        .msg-error {
            background: #fee2e2;
            color: #991b1b;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Page Title -->
    <div class="breadcrumb-area text-center">
        <h2 class="page-title">Change Password</h2>
    </div>

    <div class="form-page-wrapper">
        <div class="form-card">
            <div class="form-card-title">
                <i class="fa-solid fa-lock"></i> Security Settings
            </div>

            <!-- Messages -->
            <asp:Label ID="lblMessage" runat="server" CssClass="msg-alert msg-success" Visible="false"></asp:Label>
            <asp:Label ID="lblError" runat="server" CssClass="msg-alert msg-error" Visible="false"></asp:Label>

            <div class="form-group-modern">
                <label for="txtCurrentPassword">Current Password</label>
                <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control-modern" TextMode="Password" placeholder="Enter current password"></asp:TextBox>
            </div>

            <div class="form-group-modern">
                <label for="txtNewPassword">New Password</label>
                <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control-modern" TextMode="Password" placeholder="Enter new password"></asp:TextBox>
            </div>

            <div class="form-group-modern">
                <label for="txtConfirmPassword">Confirm New Password</label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control-modern" TextMode="Password" placeholder="Confirm new password"></asp:TextBox>
            </div>

            <div class="form-actions">
                <asp:Button ID="btnChangePassword" runat="server" CssClass="btn-form btn-save" Text="Update Password" OnClick="btnChangePassword_Click" />
            </div>
        </div>
    </div>
</asp:Content>
