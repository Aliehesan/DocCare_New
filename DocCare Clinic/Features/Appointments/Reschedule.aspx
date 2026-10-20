<%@ Page Title="Reschedule Appointment" Language="C#" MasterPageFile="~/Features/Shared/DocCare.Master" AutoEventWireup="true"
    CodeBehind="Reschedule.aspx.cs" Inherits="DocCare_Clinic.Reschedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .reschedule-wrapper { padding: 0 8% 60px; max-width: 760px; margin: 0 auto; }
        .reschedule-card { background: #fff; border-radius: 18px; box-shadow: 0 4px 20px rgba(46, 55, 164, 0.08); padding: 28px 30px; }
        .current-appt { background: #eef2ff; border: 1px solid #c7d2fe; border-radius: 14px; padding: 16px 18px; margin-bottom: 22px; }
        .current-appt .label { font-size: 12px; text-transform: uppercase; letter-spacing: .5px; color: #6b7280; font-weight: 600; }
        .current-appt .doc { font-size: 18px; font-weight: 700; color: #111827; margin: 4px 0; }
        .current-appt .when { font-size: 14px; color: #2E37A4; font-weight: 600; }
        .field-label { display: block; font-weight: 600; font-size: 14px; color: #374151; margin: 0 0 8px; }
        .date-input { width: 100%; max-width: 260px; padding: 10px 14px; border: 1.5px solid #c7d2fe; border-radius: 10px; font-size: 15px; margin-bottom: 18px; }
        .slot-list { display: flex; flex-wrap: wrap; gap: 8px; margin-bottom: 14px; min-height: 40px; }
        .slot-hint { font-size: 13px; color: #6b7280; }
        .slot-chip {
            border: 1.5px solid #c7d2fe; background: #eef2ff; color: #2E37A4; border-radius: 10px;
            padding: 8px 14px; font-size: 13.5px; font-weight: 600; cursor: pointer;
        }
        .slot-chip:hover { border-color: #2E37A4; }
        .slot-chip.active { background: #2E37A4; color: #fff; border-color: #2E37A4; }
        .slot-status { display: block; margin: 0 0 16px; padding: 10px 14px; border-radius: 10px; font-size: 14px; font-weight: 600; }
        .slot-status:empty { display: none; }
        .slot-ok { background: #ecfdf5; color: #047857; }
        .slot-none { background: #fef2f2; color: #b91c1c; }
        .actions { display: flex; flex-wrap: wrap; gap: 10px; align-items: center; }
        .btn-confirm { background: #2E37A4; color: #fff; border: none; border-radius: 50px; padding: 11px 28px; font-weight: 600; }
        .btn-confirm:disabled { opacity: .5; cursor: not-allowed; }
        .btn-back { color: #6b7280; font-weight: 600; text-decoration: none; padding: 11px 16px; }
        .note { font-size: 13px; color: #6b7280; margin-top: 16px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="breadcrumb-area text-center">
        <h2 class="page-title">Reschedule Appointment</h2>
    </div>

    <div class="reschedule-wrapper">
        <div class="reschedule-card">
            <div class="current-appt">
                <div class="label">Current appointment</div>
                <div class="doc">Dr. <asp:Literal ID="litDoctor" runat="server" /></div>
                <div class="when"><asp:Literal ID="litCurrent" runat="server" /></div>
                <div style="font-size: 13px; color: #4b5563; margin-top: 4px;">Patient: <asp:Literal ID="litPatient" runat="server" /></div>
            </div>

            <label class="field-label" for="<%= txtNewDate.ClientID %>"><i class="fa-regular fa-calendar-check"></i> New Date</label>
            <asp:TextBox ID="txtNewDate" runat="server" TextMode="Date" CssClass="date-input" onchange="loadSlots()"></asp:TextBox>

            <label class="field-label"><i class="fa-regular fa-clock"></i> New Time</label>
            <div id="slotList" class="slot-list"><span class="slot-hint">Loading times...</span></div>
            <asp:HiddenField ID="hdnNewSlot" runat="server" />
            <span id="slotStatus" class="slot-status"></span>

            <div class="actions">
                <asp:Button ID="btnConfirm" runat="server" CssClass="btn-confirm" Text="Confirm New Time" OnClick="btnConfirm_Click" />
                <a class="btn-back" href="<%= ResolveUrl("~/Features/Appointments/MyAppointment.aspx") %>">Back to My Appointments</a>
            </div>
            <p class="note">
                Your payment carries over to the new time. You will receive an email with the new details.
                Online changes close <%= DocCare_Clinic.Config.AppointmentStatus.PatientCutoffHours %> hours before the appointment.
            </p>
        </div>
    </div>

    <script>
        function slotField() { return document.getElementById('<%= hdnNewSlot.ClientID %>'); }
        function dateField() { return document.getElementById('<%= txtNewDate.ClientID %>'); }
        function confirmButton() { return document.getElementById('<%= btnConfirm.ClientID %>'); }
        function appointmentId() { return parseInt(new URLSearchParams(window.location.search).get('id'), 10) || 0; }

        function setStatus(text, ok) {
            var el = document.getElementById('slotStatus');
            el.textContent = text || '';
            el.className = 'slot-status ' + (ok ? 'slot-ok' : 'slot-none');
        }

        function loadSlots() {
            var list = document.getElementById('slotList');
            var keep = slotField().value;
            confirmButton().disabled = true;
            slotField().value = '';
            if (!dateField().value) { list.innerHTML = '<span class="slot-hint">Please choose a date.</span>'; return; }
            list.innerHTML = '<span class="slot-hint">Loading times...</span>';
            fetch(window.location.pathname + '/GetSlots', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                body: JSON.stringify({ appointmentId: appointmentId(), date: dateField().value })
            })
                .then(function (r) { if (!r.ok) throw new Error(r.status); return r.json(); })
                .then(function (data) {
                    var res = data.d || data;
                    list.innerHTML = '';
                    if (!res.available) {
                        list.innerHTML = '<span class="slot-hint">No times available on this date.</span>';
                        setStatus(res.message, false);
                        return;
                    }
                    res.slots.forEach(function (s) {
                        var b = document.createElement('button');
                        b.type = 'button';
                        b.className = 'slot-chip';
                        b.textContent = s.label;
                        b.setAttribute('data-value', s.value);
                        b.onclick = function () { pickSlot(b); };
                        list.appendChild(b);
                        if (s.value === keep) pickSlot(b);
                    });
                    if (!slotField().value) setStatus(res.slots.length + (res.slots.length === 1 ? ' time' : ' times') + ' free on ' + res.dateLabel + '. Choose one.', true);
                })
                .catch(function () {
                    list.innerHTML = '<span class="slot-hint">Could not load times. Please check your connection and pick the date again.</span>';
                });
        }

        function pickSlot(button) {
            document.querySelectorAll('#slotList .slot-chip').forEach(function (b) { b.classList.remove('active'); });
            button.classList.add('active');
            slotField().value = button.getAttribute('data-value');
            setStatus('New time: ' + button.textContent + ' on ' + dateField().value.split('-').reverse().join('-'), true);
            confirmButton().disabled = false;
        }

        document.addEventListener('DOMContentLoaded', function () {
            if (confirmButton().getAttribute('data-locked') === '1') {
                document.getElementById('slotList').innerHTML = '<span class="slot-hint">This appointment can no longer be changed online.</span>';
                confirmButton().disabled = true;
                return;
            }
            loadSlots();
        });
    </script>
</asp:Content>
