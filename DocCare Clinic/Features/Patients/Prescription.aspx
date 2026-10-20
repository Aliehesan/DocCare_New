<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Prescription.aspx.cs" Inherits="DocCare_Clinic.Prescription" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Prescription</title>
    <style>
        * { box-sizing: border-box; }
        body { margin: 0; background: #eef0f5; font-family: "Segoe UI", Arial, sans-serif; color: #1f2937; font-size: 14px; }
        .sheet { width: 210mm; min-height: 270mm; margin: 18px auto; background: #fff; padding: 16mm 15mm; box-shadow: 0 4px 24px rgba(0,0,0,.08); }
        .head { display: flex; justify-content: space-between; align-items: flex-start; border-bottom: 3px solid #2E37A4; padding-bottom: 12px; }
        .clinic { font-size: 24px; font-weight: 800; color: #2E37A4; letter-spacing: .3px; }
        .clinic small { display: block; font-size: 12px; font-weight: 500; color: #6b7280; letter-spacing: 0; }
        .doctor { text-align: right; }
        .doctor strong { display: block; font-size: 17px; }
        .doctor span { color: #6b7280; font-size: 13px; }
        .patient { display: grid; grid-template-columns: repeat(4, auto); gap: 6px 24px; justify-content: start; margin: 14px 0; padding: 10px 12px;
                   background: #f5f7ff; border-radius: 8px; font-size: 13px; }
        .patient b { color: #6b7280; font-weight: 600; margin-right: 4px; }
        .allergy { border: 1px solid #fecaca; background: #fef2f2; color: #991b1b; padding: 8px 12px; border-radius: 8px; margin-bottom: 12px; font-size: 13px; }
        h4 { font-size: 13px; text-transform: uppercase; letter-spacing: .6px; color: #2E37A4; margin: 16px 0 6px; }
        p { margin: 0; line-height: 1.5; white-space: pre-line; }
        .rx-symbol { font-size: 28px; font-weight: 700; color: #2E37A4; font-family: Georgia, serif; }
        table { width: 100%; border-collapse: collapse; margin-top: 4px; }
        th { text-align: left; font-size: 12px; color: #6b7280; font-weight: 600; border-bottom: 1px solid #e5e7eb; padding: 6px 8px; }
        td { padding: 8px; border-bottom: 1px solid #f1f2f6; vertical-align: top; }
        td.num { width: 28px; color: #6b7280; }
        .follow { margin-top: 16px; padding: 8px 12px; border-left: 4px solid #2E37A4; background: #f5f7ff; }
        .sign { margin-top: 48px; display: flex; justify-content: flex-end; }
        .sign div { border-top: 1px solid #9ca3af; padding-top: 6px; min-width: 220px; text-align: center; font-size: 13px; color: #4b5563; }
        .foot { margin-top: 28px; font-size: 11px; color: #9ca3af; text-align: center; }
        .toolbar { width: 210mm; margin: 18px auto 0; display: flex; justify-content: flex-end; gap: 10px; }
        .toolbar button { background: #2E37A4; color: #fff; border: 0; border-radius: 8px; padding: 9px 18px; font-size: 14px; font-weight: 600; cursor: pointer; }
        .empty { color: #9ca3af; font-style: italic; }
        @media print {
            body { background: #fff; }
            .toolbar { display: none; }
            .sheet { margin: 0; box-shadow: none; width: auto; min-height: 0; padding: 0; }
            @page { size: A4; margin: 14mm; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="toolbar"><button type="button" onclick="window.print()">Print</button></div>
        <div class="sheet">
            <div class="head">
                <div class="clinic">DocCare Clinic<small><asp:Literal ID="litLocation" runat="server" /></small></div>
                <div class="doctor">
                    <strong><asp:Literal ID="litDoctor" runat="server" /></strong>
                    <span><asp:Literal ID="litSpecialization" runat="server" /></span>
                </div>
            </div>

            <div class="patient">
                <span><b>Patient</b><asp:Literal ID="litPatient" runat="server" /></span>
                <span><b>Patient ID</b><asp:Literal ID="litCode" runat="server" /></span>
                <span><b>Age / Sex</b><asp:Literal ID="litAgeSex" runat="server" /></span>
                <span><b>Date</b><asp:Literal ID="litDate" runat="server" /></span>
            </div>

            <asp:Panel ID="pnlAllergy" runat="server" CssClass="allergy" Visible="false">
                <strong>Allergies:</strong> <asp:Literal ID="litAllergies" runat="server" />
            </asp:Panel>

            <asp:Panel ID="pnlVitals" runat="server"><h4>Vitals</h4><p><asp:Literal ID="litVitals" runat="server" /></p></asp:Panel>
            <asp:Panel ID="pnlComplaint" runat="server"><h4>Complaint</h4><p><asp:Literal ID="litComplaint" runat="server" /></p></asp:Panel>
            <asp:Panel ID="pnlDiagnosis" runat="server"><h4>Diagnosis</h4><p><asp:Literal ID="litDiagnosis" runat="server" /></p></asp:Panel>

            <h4><span class="rx-symbol">&#8478;</span></h4>
            <asp:Repeater ID="rptRx" runat="server">
                <HeaderTemplate><table><thead><tr><th></th><th>Medicine</th><th>Dose</th><th>How often</th><th>Days</th><th>Instructions</th></tr></thead><tbody></HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="num"><%# Container.ItemIndex + 1 %>.</td>
                        <td><strong><%# HttpUtility.HtmlEncode(Eval("Medicine")) %></strong></td>
                        <td><%# HttpUtility.HtmlEncode(Eval("Dose")) %></td>
                        <td><%# HttpUtility.HtmlEncode(Eval("Frequency")) %></td>
                        <td><%# Eval("Days") %></td>
                        <td><%# HttpUtility.HtmlEncode(Eval("Instructions")) %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate></tbody></table></FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlNoRx" runat="server" Visible="false"><p class="empty">No medicines prescribed.</p></asp:Panel>

            <asp:Panel ID="pnlAdvice" runat="server"><h4>Advice</h4><p><asp:Literal ID="litAdvice" runat="server" /></p></asp:Panel>
            <asp:Panel ID="pnlFollow" runat="server" CssClass="follow"><strong>Follow-up:</strong> <asp:Literal ID="litFollow" runat="server" /></asp:Panel>

            <div class="sign"><div><asp:Literal ID="litSign" runat="server" /></div></div>
            <div class="foot">Prescription for appointment token <asp:Literal ID="litToken" runat="server" /> &middot; printed <asp:Literal ID="litPrinted" runat="server" /></div>
        </div>
    </form>
</body>
</html>
