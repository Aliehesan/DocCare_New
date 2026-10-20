<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="EditPatient.aspx.cs" Inherits="DocCare_Clinic.EditPatient" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/Features/Patients/Patients.aspx") %>">Patients</a> </li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Edit Patient</li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>
                            <div class="row">
                                <div class="col-12">
                                    <div class="form-heading">
                                        <h4>
                                            Patient Details</h4>
                                    </div>
                                </div>
                                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="red" Font-Size="13px" />
                                <asp:HiddenField ID="hdnPatientId" runat="server" />
                                <div class="col-12 col-md-6 col-xl-4">
                                    <div class="input-block local-forms">
                                        <label>
                                            Patient Name <span class="login-danger">*</span></label>
                                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-12 col-md-6 col-xl-4">
                                    <div class="input-block local-forms">
                                        <label>
                                            Date Of Birth <span class="login-danger">*</span>
                                        </label>
                                        <asp:TextBox ID="txtDOB" CssClass="form-control" runat="server" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-12 col-md-6 col-xl-4">
                                    <div class="input-block local-forms">
                                        <label>
                                            Age <span class="login-danger">*</span>
                                        </label>
                                        <asp:TextBox ID="txtAge" CssClass="form-control" runat="server" Placeholder="Enter Age"
                                            ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-12 col-md-6 col-xl-5">
                                    <div class="input-block local-forms">
                                        <label>
                                            Mobile <span class="login-danger">*</span></label>
                                        <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-12 col-md-6 col-xl-4">
                                    <div class="input-block local-forms">
                                        <label>
                                            Email <span class="login-danger">*</span></label>
                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-12 col-md-6 col-xl-3">
                                    <div class="input-block select-gender">
                                        <label class="gen-label">
                                            Gender<span class="login-danger">*</span></label>
                                        <asp:RadioButtonList ID="rblGender" runat="server" RepeatDirection="Horizontal" CssClass="form-check-label">
                                            <asp:ListItem Text="Male" Value="Male" class="form-check-inline"></asp:ListItem>
                                            <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                                <div class="col-12 col-sm-10">
                                    <div class="input-block local-forms">
                                        <label>
                                            Address <span class="login-danger">*</span></label>
                                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine"
                                            Rows="3"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-12 col-md-6 col-xl-6">
                                    <div class="input-block select-gender">
                                        <label class="gen-label">
                                            Status <span class="login-danger">*</span></label>
                                        <asp:RadioButtonList ID="rblStatus" runat="server" RepeatDirection="Horizontal" CssClass="form-check-label">
                                            <asp:ListItem Text="Active" Value="Active" class="form-check-inline"></asp:ListItem>
                                            <asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                                <div class="col-12">
                                    <div class="doctor-submit text-end">
                                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary submit-form me-2"
                                            Text="Submit" OnClick="btnSubmit_Click" />
                                        <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-primary cancel-form"
                                            Text="Cancel" CausesValidation="false" onclick="btnCancel_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
    document.addEventListener('DOMContentLoaded', function () {
        const dobInput = document.getElementById('<%= txtDOB.ClientID %>');
        const ageInput = document.getElementById('<%= txtAge.ClientID %>');

        dobInput.addEventListener('change', function () {
            const dob = new Date(dobInput.value);
            if (!isNaN(dob)) {
                const today = new Date();
                let age = today.getFullYear() - dob.getFullYear();
                const monthDiff = today.getMonth() - dob.getMonth();
                const dayDiff = today.getDate() - dob.getDate();

                // Adjust age if the birthday hasn't occurred yet this year
                if (monthDiff < 0 || (monthDiff === 0 && dayDiff < 0)) {
                    age--;
                }

                ageInput.value = age; // Set the calculated age
            } else {
                ageInput.value = ''; // Clear age if DOB is invalid
            }
        });
    });
</script>
</asp:Content>
