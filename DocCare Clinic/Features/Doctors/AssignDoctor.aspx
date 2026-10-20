<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="AssignDoctor.aspx.cs" Inherits="DocCare_Clinic.AssignDoctor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .lv-doctors-list
        {
            list-style-type: none;
            padding: 0;
            margin: 0;
        }
        
        .lv-doctors-item
        {
            margin-bottom: 10px;
            display: flex;
            align-items: center;
        }
        
        .lv-doctors-item input
        {
            margin-right: 8px;
        }
    </style>
    <div class="page-wrapper">
        <div class="content">
            <!-- Breadcrumb -->
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item">
                                <asp:HyperLink ID="lnkDoctors" NavigateUrl="~/Features/Doctors/Doctors.aspx" runat="server">Receptionists</asp:HyperLink>
                            </li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Assign Doctor</li>
                        </ul>
                    </div>
                </div>
            </div>
            <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
            </asp:ScriptManagerProxy>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <!-- Doctor Form -->
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="card">
                                <div class="card-body">
                                    <asp:Panel ID="DoctorFormPanel" runat="server">
                                        <div class="row">
                                            <div class="col-12">
                                                <div class="form-heading">
                                                    <h4>
                                                        Assign Doctor</h4>
                                                </div>
                                            </div>
                                            <%--<asp:Label ID="lblErrorMessage" runat="server" Visible="false"></asp:Label>
                                    <asp:Label ID="lblSuccessMessage" runat="server" Visible="false"></asp:Label>
                                    <br />--%>
                                            <!-- Receptionists -->
                                            <div class="col-12 col-md-6 col-xl-4">
                                                <!-- Receptionists Dropdown -->
                                                <div class="input-block local-forms">
                                                    <asp:Label ID="lblReceptionists" AssociatedControlID="ddlReceptionists" runat="server"
                                                        Text="Receptionists *"></asp:Label>
                                                    <asp:DropDownList ID="ddlReceptionists" CssClass="form-control select" runat="server"
                                                        Enabled="false">
                                                        <asp:ListItem Value="" Text="Select Receptionist" Selected="True" />
                                                    </asp:DropDownList>
                                                </div>
                                                <br />
                                                <!-- Assign Doctor ListView -->
                                                <div class="input-block">
                                                    <!-- Label for the entire ListView -->
                                                    <asp:Label ID="AsgDoctor" CssClass="assign-doctor" AssociatedControlID="lvDoctors"
                                                        runat="server" Text="Assign Doctors *"></asp:Label>
                                                    <!-- Header for ListView -->
                                                    <div style="display: flex; font-weight: 500; padding: 13px; background-color: #f0f0f0;
                                                        border: 2px solid #dfd6d6; border-radius: 10px 10px 0 0; width: 150%;">
                                                        <div style="flex: 0 0 50px; text-align: center;">
                                                            Select</div>
                                                        <div style="flex: 0 0 60px; text-align: center;">
                                                        </div>
                                                        <div style="flex: 1;">
                                                            Doctor Name</div>
                                                    </div>
                                                    <!-- ListView to display the doctors -->
                                                    <asp:ListView ID="lvDoctors" runat="server">
                                                        <LayoutTemplate>
                                                            <ul style="list-style-type: none; padding: 0; margin: 0; font-size: 12px; border: 2px solid #dfd6d6;
                                                                border-radius: 0 0 10px 10px; width: 150%">
                                                                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                                                            </ul>
                                                        </LayoutTemplate>
                                                        <ItemTemplate>
                                                            <li style="display: flex; align-items: center; padding: 10px; border-bottom: 1px solid #ccc;">
                                                                <div style="flex: 0 0 50px; text-align: center;">
                                                                    <asp:CheckBox ID="chkSelect" runat="server" Checked='<%# IsDoctorAssigned(Eval("DoctorID").ToString()) %>'
                                                                        Enabled='<%# !IsDoctorAssignedToAnotherReceptionist(Eval("DoctorID").ToString()) %>'
                                                                        AutoPostBack="true" OnCheckedChanged="chkSelect_CheckedChanged" ToolTip='<%# Eval("DoctorID") %>' />
                                                                </div>
                                                                <div style="flex: 0 0 60px; text-align: center;">
                                                                    <img src='<%# ResolveClientUrl(Eval("ImageUrl").ToString()) %>' alt='<%# Eval("Name") %>'
                                                                        style="width: 50px; height: 50px; border-radius: 50%;" />
                                                                </div>
                                                                <div style="flex: 1; font-weight: bold;">
                                                                    <%# Eval("Name") %>
                                                                    <div style="font-weight: normal; color: #666; font-size: 12px;">
                                                                        <%# Eval("Degree") %>
                                                                        |
                                                                        <%# Eval("Specialization") %>
                                                                    </div>
                                                                </div>
                                                                <asp:HiddenField ID="hfDoctorId" Value='<%# Eval("DoctorID") %>' runat="server" />
                                                            </li>
                                                        </ItemTemplate>
                                                    </asp:ListView>
                                                </div>
                                                <br />
                                                <!-- Submit and Cancel Buttons -->
                                                <div class="col-12">
                                                    <div class="doctor-submit ">
                                                        <asp:Button ID="btnAssign" CssClass="btn btn-primary submit-form me-2" Text="Assign"
                                                            runat="server" OnClick="btnAssign_Click" />
                                                        <asp:Button ID="btnCancel" CssClass="btn btn-primary cancel-form" Text="Cancel" runat="server"
                                                            OnClick="btnCancel_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
