<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Shared/DocCare.Master" AutoEventWireup="true"
    CodeBehind="EditUser.aspx.cs" Inherits="DocCare_Clinic.EditUser" %>

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
            max-width: 600px;
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
        }

        .btn-save:hover {
            background: var(--primary-hover);
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(46, 55, 164, 0.3);
        }

        .btn-cancel {
            background: #f3f4f6;
            color: var(--text-dark);
        }

        .btn-cancel:hover {
            background: #e5e7eb;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Page Title -->
    <div class="breadcrumb-area text-center">
        <h2 class="page-title">Edit Profile</h2>
    </div>

    <div class="form-page-wrapper">
        <div class="form-card">
            <div class="form-card-title">
                <i class="fa-solid fa-user-pen"></i> Update Personal Details
            </div>
            
            <div class="form-group-modern">
                <label for="txtName">Full Name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control-modern" placeholder="Enter your full name"></asp:TextBox>
            </div>
            
            <div class="form-group-modern">
                <label for="txtEmail">Email Address</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control-modern" TextMode="Email" placeholder="Enter your email"></asp:TextBox>
            </div>
            
            <div class="form-group-modern">
                <label for="txtMobile">Phone Number</label>
                <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control-modern" placeholder="Enter your mobile number"></asp:TextBox>
            </div>

            <div class="form-group-modern">
                <label for="txtAddress">Home Address</label>
                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control-modern" placeholder="Enter your address"></asp:TextBox>
            </div>
            
            <div class="form-actions">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-form btn-cancel" OnClick="btnCancel_Click" />
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn-form btn-save" OnClick="btnSave_Click" />
            </div>
        </div>
    </div>
</asp:Content>
