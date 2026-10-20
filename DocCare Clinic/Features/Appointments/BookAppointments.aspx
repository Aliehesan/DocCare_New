<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Shared/DocCare.Master" AutoEventWireup="true"
    CodeBehind="BookAppointments.aspx.cs" Inherits="DocCare_Clinic.BookAppointments"
    EnableViewState="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .main
        {
            max-width: 800px;
            margin: auto;
        }
        .img-fluid
        {
            width: 100%;
            max-width: 200px;
            height: 200px;
            object-fit: cover;
            border-radius: 5px;
        }
        .btn-selected
        {
            background-color: #007bff; /* Blue background */
            color: white; /* White text color */
            border-color: #007bff; /* Blue border */
        }
    </style>
    <script type="text/javascript">
        function highlightToken(button) {            
            var buttons = document.querySelectorAll('.btn-outline-primary');
            buttons.forEach(function (btn) {
                btn.classList.remove('btn-selected');
            });

            // Add 'btn-selected' class to the clicked button
            button.classList.add('btn-selected');

            // Store the selected token in the hidden field
            var tokenValue = button.value.trim(); // Use the value attribute
            document.getElementById('<%= hfSelectedToken.ClientID %>').value = tokenValue;

            // Debugging: Log the selected token to the console
            console.log("Selected Token: " + tokenValue);

            // Prevent the default postback behavior of the button
            return false;
        }

        function validateTokenSelection() {
            var selectedToken = document.getElementById('<%= hfSelectedToken.ClientID %>').value;
            if (!selectedToken) {
                swalAlert('warning', 'No Token Selected', 'Please select a token before continuing.');
                return false; // Prevent postback
            }
            return true; // Allow postback
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main py-4">
        <div class="card mb-4 shadow-sm">
            <div class="row g-0">
                <div class="col-md-3 text-center">
                    <asp:Image ID="imgDoctor" runat="server" CssClass="img-fluid rounded-start p-3" />
                </div>
                <asp:HiddenField ID="hfSelectedToken" runat="server" />
                <!-- HiddenField to store doctorId -->
                <asp:HiddenField ID="hfDoctorId" runat="server" />
                <div class="col-md-9 align-self-center">
                    <div class="card-body">
                        <asp:Label ID="lblDoctorName" runat="server" CssClass="card-title" style="color:Red;"></asp:Label>
                        <p class="card-text">
                            <asp:Label ID="lblSpecialization" runat="server"></asp:Label>
                        </p>
                        <p class="card-text">
                            <asp:Label ID="lblDepartment" runat="server"></asp:Label>
                        </p>
                    </div>
                </div>
            </div>
        </div>
        <h5 class="mb-3">
            <i class="fas fa-hospital-symbol"></i>Registration Tokens</h5>
        <div class="card p-3 shadow-sm">
            <div class="row text-center">
                <asp:Repeater ID="rptTokens" runat="server" OnItemCommand="rptTokens_ItemCommand">
                    <ItemTemplate>
                        <div class="col-2 mb-3">
                            <asp:Button ID="btnToken" runat="server" CssClass='<%# (bool)Eval("IsAvailable") ? "btn btn-outline-primary" : "btn btn-secondary disabled" %>'
                                Text='<%# Eval("TokenName") %>' Enabled='<%# (bool)Eval("IsAvailable") %>' CommandName="SelectToken"
                                CommandArgument='<%# Eval("TokenName") %>' OnClientClick="return highlightToken(this);" />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <div class="text-end mt-4">
            <asp:Button ID="btnContinue" runat="server" Text="Continue" CssClass="btn btn-primary px-4"
                OnClick="btnContinue_Click" OnClientClick="return validateTokenSelection();" />
        </div>
    </div>
</asp:Content>
