<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Doctors/DoctorAdmin.Master" AutoEventWireup="true"
    CodeBehind="Schedule.aspx.cs" Inherits="DocCare_Clinic.Schedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .hours-table th { font-size: 13px; white-space: nowrap; }
        .hours-table td { vertical-align: middle; }
        .hours-table .form-control { min-height: 38px; padding: 6px 10px; min-width: 110px; }
        .hours-table .num { max-width: 110px; }
        .hours-table tr.day-off td { background: #f9fafb; color: #9ca3af; }
        .day-name { font-weight: 700; color: #111827; white-space: nowrap; }
        .slot-count { font-size: 12.5px; color: #374151; white-space: nowrap; }
        .slot-count strong { color: #2E37A4; font-size: 14px; }
        .slot-count .slot-warn { color: #b45309; }
        .hours-actions { display: flex; flex-wrap: wrap; justify-content: flex-end; gap: 10px; }
        .page-note { font-size: 13px; color: #6b7280; margin: -4px 0 16px; }
        .hours-table th .optional { font-weight: 400; color: #9ca3af; font-size: 11.5px; }
        .leave-warning {
            background: #fff7ed; border: 1px solid #fdba74; border-radius: 12px; padding: 16px 18px; margin: 4px 0 18px;
        }
        .leave-warning h5 { color: #9a3412; font-size: 15px; font-weight: 700; margin: 0 0 6px; }
        .leave-warning ul { margin: 6px 0 12px 18px; font-size: 13px; color: #7c2d12; }
        .leave-badge { display: inline-block; padding: 3px 10px; border-radius: 12px; font-size: 12px; font-weight: 600; background: #fee2e2; color: #b91c1c; }
        .for-doctor { font-size: 14px; color: #2E37A4; font-weight: 600; margin-left: 6px; }
    </style>
    <script>
        // Patients per day = whole slots that finish by the end time (same rule as SlotService.SlotTimes)
        function toMinutes(v) { var p = (v || '').split(':'); return p.length < 2 ? NaN : parseInt(p[0], 10) * 60 + parseInt(p[1], 10); }
        function fmtTime(m) {
            var h = Math.floor(m / 60), min = m % 60, ap = h >= 12 ? 'PM' : 'AM', h12 = h % 12 || 12;
            return (h12 < 10 ? '0' : '') + h12 + ':' + (min < 10 ? '0' : '') + min + ' ' + ap;
        }
        // Slots skip the break and resume when it ends
        function countSlots(start, end, slot, bFrom, bTo) {
            var hasBreak = !isNaN(bFrom) && !isNaN(bTo) && bFrom < bTo, n = 0, lastEnd = null, t = start;
            while (t + slot <= end) {
                if (hasBreak && t < bTo && t + slot > bFrom) { t = bTo; continue; }
                n++; lastEnd = t + slot; t += slot;
            }
            return { n: n, lastEnd: lastEnd };
        }
        function recalcRow(row) {
            var out = row.querySelector('.slot-count');
            var working = row.querySelector('input[type=checkbox]').checked;
            row.className = working ? '' : 'day-off';
            if (!working) { out.innerHTML = 'Day off'; return; }
            var start = toMinutes(row.querySelector('input[id*="txtStart"]').value);
            var end = toMinutes(row.querySelector('input[id*="txtEnd"]').value);
            var slot = parseInt(row.querySelector('input[id*="txtSlot"]').value, 10);
            var bFromText = row.querySelector('input[id*="txtBreakFrom"]').value, bToText = row.querySelector('input[id*="txtBreakTo"]').value;
            var bFrom = toMinutes(bFromText), bTo = toMinutes(bToText);
            if (isNaN(start) || isNaN(end) || !(slot > 0) || end <= start) { out.innerHTML = '<span class="slot-warn">Check the times</span>'; return; }
            if ((bFromText === '') !== (bToText === '')) { out.innerHTML = '<span class="slot-warn">Fill both break times (or neither)</span>'; return; }
            if (bFromText !== '' && (bFrom >= bTo || bFrom < start || bTo > end)) { out.innerHTML = '<span class="slot-warn">Break must be inside the working hours</span>'; return; }
            var c = countSlots(start, end, slot, bFrom, bTo);
            if (c.n < 1) { out.innerHTML = '<span class="slot-warn">Slot is longer than the working time</span>'; return; }
            out.innerHTML = '<strong>' + c.n + '</strong> patients &middot; ' + fmtTime(start) + ' - ' + fmtTime(c.lastEnd) +
                (bFromText !== '' ? ' &middot; break ' + fmtTime(bFrom) + ' - ' + fmtTime(bTo) : '') +
                (c.lastEnd < end ? ' <span class="slot-warn">(' + (end - c.lastEnd) + ' min unused)</span>' : '');
        }
        // Monday is the first row. Copy its times to the other days ticked as Working
        // (days off stay off); nothing is saved until "Save Working Hours" is clicked.
        function copyMondayToWorkingDays() {
            var rows = document.querySelectorAll('tr[id*="rowDay"]');
            var monday = rows[0];
            var field = function (row, name) { return row.querySelector('input[id*="' + name + '"]'); };
            var start = field(monday, 'txtStart').value, end = field(monday, 'txtEnd').value, slot = field(monday, 'txtSlot').value;
            var bFrom = field(monday, 'txtBreakFrom').value, bTo = field(monday, 'txtBreakTo').value;
            var mStart = toMinutes(start), mEnd = toMinutes(end), mSlot = parseInt(slot, 10);
            if (!monday.querySelector('input[type=checkbox]').checked || isNaN(mStart) || isNaN(mEnd) || mEnd <= mStart || !(mSlot > 0) || Math.floor((mEnd - mStart) / mSlot) < 1) {
                swalAlert('warning', 'Set Monday First', 'Tick Monday as Working and enter a valid start time, end time and slot length, then copy.');
                return;
            }
            var copied = [];
            for (var i = 1; i < rows.length; i++) {
                if (!rows[i].querySelector('input[type=checkbox]').checked) continue;
                field(rows[i], 'txtStart').value = start;
                field(rows[i], 'txtEnd').value = end;
                field(rows[i], 'txtSlot').value = slot;
                field(rows[i], 'txtBreakFrom').value = bFrom;
                field(rows[i], 'txtBreakTo').value = bTo;
                copied.push(rows[i].querySelector('.day-name').textContent.trim());
            }
            rows.forEach(recalcRow);   // refresh every row's "Patients per Day"
            swalAlert(copied.length ? 'success' : 'info',
                copied.length ? 'Copied to ' + copied.length + (copied.length === 1 ? ' day' : ' days') : 'No Other Working Days',
                copied.length
                    ? 'Monday’s hours are now on ' + copied.join(', ') + '. Click Save Working Hours to keep them.'
                    : 'Tick the days you work, then click Copy again.');
        }

        document.addEventListener('DOMContentLoaded', function () {
            document.querySelectorAll('tr[id*="rowDay"]').forEach(function (row) {
                row.querySelectorAll('input').forEach(function (i) {
                    i.addEventListener('input', function () { recalcRow(row); });
                    i.addEventListener('change', function () { recalcRow(row); });
                });
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/Features/Doctors/DoctorDashBoard.aspx">Dashboard</asp:HyperLink></li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Working Hours &amp; Leave</li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">

                    <%-- ── Weekly working hours ── --%>
                    <div class="card">
                        <div class="card-body">
                            <div class="form-heading">
                                <h4>Working Hours <asp:Label ID="lblForDoctor" runat="server" CssClass="for-doctor"></asp:Label></h4>
                            </div>
                            <p class="page-note">
                                Patients can book any working day in the next <%= DocCare_Clinic.Config.SlotService.BookingWindowDays %> days.
                                Changes apply to new bookings; existing appointments keep their time.
                                Add a break (e.g. lunch) to keep that time free of appointments.
                            </p>
                            <div class="table-responsive">
                                <table class="table hours-table">
                                    <thead>
                                        <tr>
                                            <th>Day</th>
                                            <th>Working</th>
                                            <th>Start Time</th>
                                            <th>End Time</th>
                                            <th>Slot (minutes)</th>
                                            <th>Break From <span class="optional">(optional)</span></th>
                                            <th>Break To</th>
                                            <th>Patients per Day</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptHours" runat="server" OnItemDataBound="rptHours_ItemDataBound">
                                            <ItemTemplate>
                                                <tr id="rowDay" runat="server">
                                                    <td class="day-name">
                                                        <asp:HiddenField ID="hdnDay" runat="server" Value='<%# Eval("DayOfWeek") %>' />
                                                        <%# Eval("DayName") %>
                                                    </td>
                                                    <td><asp:CheckBox ID="chkWorking" runat="server" Checked='<%# Eval("IsWorking") %>' /></td>
                                                    <td><asp:TextBox ID="txtStart" runat="server" TextMode="Time" CssClass="form-control" Text='<%# Eval("Start") %>'></asp:TextBox></td>
                                                    <td><asp:TextBox ID="txtEnd" runat="server" TextMode="Time" CssClass="form-control" Text='<%# Eval("End") %>'></asp:TextBox></td>
                                                    <td><asp:TextBox ID="txtSlot" runat="server" TextMode="Number" CssClass="form-control num" Text='<%# Eval("SlotDuration") %>'></asp:TextBox></td>
                                                    <td><asp:TextBox ID="txtBreakFrom" runat="server" TextMode="Time" CssClass="form-control" Text='<%# Eval("BreakFrom") %>'></asp:TextBox></td>
                                                    <td><asp:TextBox ID="txtBreakTo" runat="server" TextMode="Time" CssClass="form-control" Text='<%# Eval("BreakTo") %>'></asp:TextBox></td>
                                                    <%-- Worked out from the hours and slot length (updated as you type) --%>
                                                    <td class="slot-count"><%# Eval("SlotSummary") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                            <div class="hours-actions">
                                <button type="button" class="btn btn-outline-primary" onclick="copyMondayToWorkingDays()"
                                    title="Copies Monday's start, end, slot length and break to every day ticked as Working">
                                    Copy Monday to All Working Days
                                </button>
                                <asp:Button ID="btnSaveHours" runat="server" CssClass="btn btn-primary" Text="Save Working Hours" OnClick="btnSaveHours_Click" />
                            </div>

                            <%-- Shown when patients are booked at times the new hours no longer have (break, shorter day, day off...) --%>
                            <asp:Panel ID="pnlHoursWarning" runat="server" Visible="false" CssClass="leave-warning" Style="margin-top: 16px;">
                                <h5><asp:Literal ID="litHoursWarningTitle" runat="server" /></h5>
                                <div style="font-size: 13px; color: #7c2d12;">These bookings are not on one of the new appointment times:</div>
                                <ul><asp:Literal ID="litHoursWarningList" runat="server" /></ul>
                                <div style="font-size: 13px; color: #7c2d12; margin: -4px 0 12px;">
                                    Keep them to see these patients at their booked time (they stay on Doctor&rsquo;s Appointments, marked
                                    <em>Outside working hours</em>), or cancel them and email each patient to book again.
                                </div>
                                <asp:HiddenField ID="hdnHoursOffIds" runat="server" />
                                <asp:Button ID="btnHoursKeep" runat="server" CssClass="btn btn-primary me-2" Text="Save and Keep These Appointments" OnClick="btnHoursKeep_Click" />
                                <asp:Button ID="btnHoursCancel" runat="server" CssClass="btn btn-danger me-2" OnClick="btnHoursCancel_Click" />
                                <asp:Button ID="btnHoursBack" runat="server" CssClass="btn btn-outline-secondary" Text="Don&rsquo;t Save (Undo Changes)" OnClick="btnHoursBack_Click" />
                            </asp:Panel>
                        </div>
                    </div>

                    <%-- ── Leave days ── --%>
                    <div class="card">
                        <div class="card-body">
                            <div class="form-heading">
                                <h4>Days Not Available</h4>
                            </div>
                            <p class="page-note">
                                Choose one date, or a From and To date for several days in a row (up to 60 days).
                                Patients cannot book these dates. If patients are already booked, they are cancelled and emailed.
                            </p>
                            <div class="row">
                                <div class="col-12 col-md-3">
                                    <div class="input-block local-forms">
                                        <label>From Date <span class="login-danger">*</span></label>
                                        <asp:TextBox ID="txtLeaveDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-12 col-md-3">
                                    <div class="input-block local-forms">
                                        <label>To Date (optional)</label>
                                        <asp:TextBox ID="txtLeaveTo" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-12 col-md-4">
                                    <div class="input-block local-forms">
                                        <label>Reason (optional, shown to patients)</label>
                                        <asp:TextBox ID="txtLeaveReason" runat="server" CssClass="form-control" MaxLength="200"
                                            placeholder="e.g. Medical conference"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-12 col-md-2">
                                    <asp:Button ID="btnAddLeave" runat="server" CssClass="btn btn-danger w-100" Style="min-height: 45px;"
                                        Text="Mark Not Available" OnClick="btnAddLeave_Click" />
                                </div>
                            </div>

                            <%-- Shown when patients are already booked on the chosen date --%>
                            <asp:Panel ID="pnlLeaveWarning" runat="server" Visible="false" CssClass="leave-warning">
                                <h5><asp:Literal ID="litWarningTitle" runat="server" /></h5>
                                <div style="font-size: 13px; color: #7c2d12;">These appointments will be cancelled and each patient will be emailed:</div>
                                <ul><asp:Literal ID="litWarningList" runat="server" /></ul>
                                <asp:HiddenField ID="hdnPendingLeaveDate" runat="server" />
                                <asp:HiddenField ID="hdnPendingLeaveTo" runat="server" />
                                <asp:HiddenField ID="hdnPendingLeaveReason" runat="server" />
                                <asp:Button ID="btnConfirmLeave" runat="server" CssClass="btn btn-danger me-2" OnClick="btnConfirmLeave_Click" />
                                <asp:Button ID="btnCancelLeave" runat="server" CssClass="btn btn-outline-secondary" Text="Keep the Appointments" OnClick="btnCancelLeave_Click" />
                            </asp:Panel>

                            <div class="table-responsive">
                                <asp:GridView ID="gvLeave" runat="server" CssClass="table table-striped" AutoGenerateColumns="False"
                                    EmptyDataText="No upcoming days off." DataKeyNames="LeaveID">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Date">
                                            <ItemTemplate>
                                                <span class="leave-badge"><%# Convert.ToDateTime(Eval("LeaveDate")).ToString("ddd, dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) %></span>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Reason" HeaderText="Reason" NullDisplayText="-" />
                                        <asp:TemplateField HeaderText="Actions">
                                            <ItemTemplate>
                                                <asp:Button ID="btnRemoveLeave" runat="server" Text="Available Again" CssClass="btn btn-outline-primary btn-sm"
                                                    CommandArgument='<%# Eval("LeaveID") %>' OnClick="btnRemoveLeave_Click"
                                                    OnClientClick="return confirm('Make this date available for booking again?');" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
