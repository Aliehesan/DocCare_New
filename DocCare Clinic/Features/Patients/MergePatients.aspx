<%@ Page Title="Merge Duplicate Patients" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="MergePatients.aspx.cs" Inherits="DocCare_Clinic.MergePatients" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .mg-note { font-size: 13px; color: #6b7280; margin: -4px 0 16px; }
        .mg-pick { display: flex; flex-wrap: wrap; gap: 14px; align-items: flex-end; margin-bottom: 18px; }
        .mg-pick .field label { display: block; font-size: 13px; font-weight: 600; color: #374151; margin-bottom: 6px; }
        .mg-pick .form-control { min-height: 42px; width: 230px; }
        .mg-section { font-size: 15px; font-weight: 700; color: #111827; margin: 6px 0 10px; }
        .mg-suggest { display: flex; flex-wrap: wrap; align-items: center; gap: 10px; padding: 10px 14px; border: 1px solid #e8eaf3; border-radius: 12px; margin-bottom: 8px; }
        .mg-suggest .why { font-size: 12px; color: #6b7280; }
        .mg-suggest a.btn { margin-left: auto; }
        .mg-none { font-size: 13.5px; color: #6b7280; padding: 4px 0 14px; }
        .mg-cards { display: grid; grid-template-columns: 1fr auto 1fr; gap: 16px; align-items: stretch; margin: 6px 0 16px; }
        .mg-card { border: 1.5px solid #e8eaf3; border-radius: 14px; padding: 16px 18px; background: #fff; }
        .mg-card.keep { border-color: #a7f3d0; background: #f6fffb; }
        .mg-card.dup { border-color: #fde68a; background: #fffdf5; }
        .mg-card h5 { font-size: 12px; text-transform: uppercase; letter-spacing: .5px; margin: 0 0 8px; }
        .mg-card.keep h5 { color: #047857; }
        .mg-card.dup h5 { color: #b45309; }
        .mg-card .name { font-size: 17px; font-weight: 700; color: #111827; }
        .mg-card dl { display: grid; grid-template-columns: 110px 1fr; gap: 4px 10px; margin: 10px 0 0; font-size: 13.5px; }
        .mg-card dt { color: #6b7280; font-weight: 500; }
        .mg-card dd { margin: 0; color: #111827; }
        .mg-arrow { align-self: center; font-size: 22px; color: #9ca3af; }
        .mg-problem { background: #fef2f2; border: 1px solid #fecaca; color: #991b1b; border-radius: 12px; padding: 12px 16px; margin-bottom: 14px; font-size: 14px; }
        .mg-explain { background: #f5f7ff; border: 1px solid #dfe3fb; border-radius: 12px; padding: 12px 16px; margin-bottom: 14px; font-size: 13.5px; color: #374151; }
        .mg-explain ul { margin: 6px 0 0 18px; }
        .mg-actions { display: flex; gap: 10px; flex-wrap: wrap; justify-content: flex-end; }
        @media (max-width: 767px) { .mg-cards { grid-template-columns: 1fr; } .mg-arrow { transform: rotate(90deg); justify-self: center; } }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/Features/Patients/Patients.aspx") %>">Patients</a></li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Merge Duplicates</li>
                        </ul>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-body">
                    <div class="form-heading"><h4>Merge Duplicate Patients</h4></div>
                    <p class="mg-note">When the same person has two records (for example from walk-in visits booked before returning patients
                        could be found), merge them so their whole history is in one place under one Patient ID.</p>

                    <div class="mg-section">Possible duplicates</div>
                    <asp:Repeater ID="rptSuggest" runat="server" OnItemCommand="rptSuggest_ItemCommand">
                        <ItemTemplate>
                            <div class="mg-suggest">
                                <strong><%# HttpUtility.HtmlEncode(Eval("AName")) %></strong>
                                <span class="patient-code" style="display:inline;"><%# Eval("ACode") %></span> &amp;
                                <span class="patient-code" style="display:inline;"><%# Eval("BCode") %></span>
                                <span class="why">&middot; <%# Eval("Reason") %> &middot; <%# Eval("AVisits") %> and <%# Eval("BVisits") %> appointments</span>
                                <asp:LinkButton ID="lnkReview" runat="server" CssClass="btn btn-outline-primary btn-sm" CommandName="Review"
                                    CommandArgument='<%# KeepFirst(Container.DataItem) %>'>Review</asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlNoSuggest" runat="server" CssClass="mg-none" Visible="false">
                        No likely duplicates found (same name with the same mobile or date of birth). You can still compare any two records below.
                    </asp:Panel>

                    <div class="mg-section">Compare two records</div>
                    <asp:Panel ID="pnlPick" runat="server" CssClass="mg-pick" DefaultButton="btnPreview">
                        <div class="field"><label for="<%= txtKeep.ClientID %>">Keep this record (Patient ID)</label>
                            <asp:TextBox ID="txtKeep" runat="server" CssClass="form-control" MaxLength="12" placeholder="e.g. DC-00019" autocomplete="off" /></div>
                        <div class="field"><label for="<%= txtDup.ClientID %>">Merge this record into it</label>
                            <asp:TextBox ID="txtDup" runat="server" CssClass="form-control" MaxLength="12" placeholder="e.g. DC-00021" autocomplete="off" /></div>
                        <asp:Button ID="btnPreview" runat="server" CssClass="btn btn-primary" Text="Compare" OnClick="btnPreview_Click" />
                    </asp:Panel>

                    <asp:Panel ID="pnlPreview" runat="server" Visible="false">
                        <div class="mg-cards">
                            <div class="mg-card keep"><h5>Keep</h5><asp:Literal ID="litKeep" runat="server" /></div>
                            <div class="mg-arrow"><i class="fa-solid fa-arrow-left"></i></div>
                            <div class="mg-card dup"><h5>Merge into the record on the left</h5><asp:Literal ID="litDup" runat="server" /></div>
                        </div>
                        <asp:Panel ID="pnlProblem" runat="server" CssClass="mg-problem" Visible="false"><asp:Literal ID="litProblem" runat="server" /></asp:Panel>
                        <asp:Panel ID="pnlExplain" runat="server" CssClass="mg-explain">
                            <strong>What merging does</strong>
                            <ul>
                                <li><asp:Literal ID="litMoveCount" runat="server" /> move to <asp:Literal ID="litKeepCode" runat="server" />, with their visit notes and prescriptions.</li>
                                <li>Mobile, email, address and online account are copied only where <asp:Literal ID="litKeepCode2" runat="server" /> has none; the health summaries are combined.</li>
                                <li><asp:Literal ID="litDupCode" runat="server" /> is hidden and remembers that it was merged. This cannot be undone from the website.</li>
                            </ul>
                        </asp:Panel>
                        <div class="mg-actions">
                            <asp:Button ID="btnSwap" runat="server" CssClass="btn btn-outline-secondary" Text="Swap (keep the other one)" OnClick="btnSwap_Click" />
                            <asp:Button ID="btnMerge" runat="server" CssClass="btn btn-danger" Text="Merge Records" OnClick="btnMerge_Click"
                                OnClientClick="return confirm('Merge these two records into one? This cannot be undone from the website.');" />
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
