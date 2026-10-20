<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="AppointmentReport.aspx.cs" Inherits="DocCare_Clinic.AppointmentReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        /* Make the entire GridView text smaller */
        .small-grid
        {
            font-size: 12px !important;
        }

        /* Make specific text elements smaller */
        .small-text
        {
            font-size: 12px;
        }

        /* Adjust table header for better visibility */
        .small-grid th
        {
            font-size: 13px;
            font-weight: bold;
        }
        .pagination
        {
            text-align: center;
            margin-top: 20px;
        }
        .pagination a, .pagination span
        {
            color: #6c757d;
            background-color: #f8f9fa;
            border: 1px solid #dee2e6;
            padding: 6px 12px;
            margin: 0 2px;
            border-radius: 5px;
            text-decoration: none;
            transition: 0.3s;
        }
        .pagination a:hover
        {
            background-color: #007bff;
            color: white;
            border-color: #007bff;
        }
        .pagination .current
        {
            background-color: #007bff;
            color: white;
            border-color: #007bff;
        }
        .pagination a:disabled
        {
            background-color: #e9ecef;
            color: #6c757d;
            border-color: #dee2e6;
            cursor: not-allowed;
        }

        /* Quick date range buttons */
        .range-links { display: flex; flex-wrap: wrap; gap: 8px; align-items: center; }
        .range-links span { font-size: 13px; color: #6b7280; margin-right: 4px; }
        .range-link {
            font-size: 13px; padding: 5px 14px; border-radius: 20px; border: 1px solid #c7d2fe;
            color: #2E37A4; background: #eef2ff; text-decoration: none; font-weight: 600;
        }
        .range-link:hover, .range-link.active { background: #2E37A4; color: #fff; border-color: #2E37A4; }

        .report-toolbar {
            display: flex; flex-wrap: wrap; justify-content: space-between; align-items: center;
            gap: 14px; margin: 0 0 24px; padding: 14px 0 0; border-top: 1px solid #eef0f4;
        }
        .report-actions { display: flex; flex-wrap: wrap; gap: 8px; align-items: center; }
        .report-actions .btn { min-height: 40px; padding: 8px 18px; font-weight: 600; border-radius: 8px; }
        .toolbar-divider { width: 1px; height: 28px; background: #e5e7eb; margin: 0 4px; }

        /* Summary cards */
        .summary-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(138px, 1fr)); gap: 12px; margin: 6px 0 24px; }
        .summary-card { border-radius: 14px; padding: 16px 18px; background: #f8fafc; border: 1px solid #e5e7eb; }
        .summary-card .label { font-size: 12.5px; color: #6b7280; font-weight: 600; text-transform: uppercase; letter-spacing: .4px; }
        .summary-card .value { font-size: 24px; font-weight: 700; color: #111827; margin-top: 4px; line-height: 1.2; }
        .summary-card .sub { font-size: 12px; color: #6b7280; margin-top: 2px; }
        .summary-card.total { background: #eef2ff; border-color: #c7d2fe; } .summary-card.total .value { color: #2E37A4; }
        .summary-card.completed { background: #ecfdf5; border-color: #a7f3d0; } .summary-card.completed .value { color: #047857; }
        .summary-card.upcoming { background: #eff6ff; border-color: #bfdbfe; } .summary-card.upcoming .value { color: #1d4ed8; }
        .summary-card.cancelled { background: #fef2f2; border-color: #fecaca; } .summary-card.cancelled .value { color: #b91c1c; }
        .summary-card.money { background: #fffbeb; border-color: #fde68a; } .summary-card.money .value { color: #92400e; }

        .section-title { font-size: 16px; font-weight: 700; margin: 8px 0 12px; color: #111827; }

        /* Charts: one measure each, one hue, recessive grid, value at the end of each bar */
        .chart-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin: 0 0 24px; }
        .chart-card { border: 1px solid #e5e7eb; border-radius: 14px; padding: 14px 16px 10px; background: #fcfcfb; min-width: 0; }
        .chart-card.wide { grid-column: 1 / -1; }
        .chart-card h6 { font-size: 14px; font-weight: 700; color: #0b0b0b; margin: 0 0 2px; }
        .chart-card .chart-sub { font-size: 12px; color: #52514e; margin: 0 0 8px; }
        .chart-box { position: relative; height: 240px; }
        .chart-empty { display: none; height: 240px; align-items: center; justify-content: center; color: #52514e; font-size: 13px; }
        @media (max-width: 767px) { .chart-grid { grid-template-columns: 1fr; } }
        @media print { .chart-card { break-inside: avoid; } }
        .doctor-summary td, .doctor-summary th { font-size: 13px; }
        .doctor-summary tr:last-child td { font-weight: 700; background: #f8fafc; }

        /* Status badges in the table */
        .st-badge { display: inline-block; padding: 3px 10px; border-radius: 12px; font-size: 12px; font-weight: 600; }
        .st-Pending { background: #f3f4f6; color: #4b5563; }
        .st-Confirmed { background: #dbeafe; color: #1d4ed8; }
        .st-Completed { background: #d1fae5; color: #047857; }
        .st-Cancelled { background: #fee2e2; color: #b91c1c; }
        .st-NoShow { background: #e5e7eb; color: #374151; }
        .summary-card.noshow { background: #f3f4f6; border-color: #d1d5db; } .summary-card.noshow .value { color: #374151; }
        .money-cell { white-space: nowrap; font-size: 12.5px; }
        .estimate { color: #b45309; font-size: 11px; }
        .removed-note { color: #6b7280; font-size: 11px; font-style: italic; }

        /* Print / Save as PDF: only the report itself */
        @media print {
            .sidebar, .header, .no-print, .pagination, .breadcrumb, #loader { display: none !important; }
            .page-wrapper { margin: 0 !important; padding: 0 !important; }
            .card { box-shadow: none !important; border: none !important; }
            .summary-card { -webkit-print-color-adjust: exact; print-color-adjust: exact; }
            .print-only { display: block !important; }
        }
        .print-only { display: none; }
    </style>
    <div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/Features/Admin/AdminDashBoard.aspx") %>">Dashboard</a></li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Appointment Report</li>
                        </ul>
                    </div>
                </div>
            </div>
            <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
            </asp:ScriptManagerProxy>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <Triggers>
                    <%-- Downloads and printing need a full page request --%>
                    <asp:PostBackTrigger ControlID="btnExport" />
                    <asp:PostBackTrigger ControlID="btnPrint" />
                </Triggers>
                <ContentTemplate>
                    <div class="card">
                        <div class="card-body">
                            <div class="col-12">
                                <div class="form-heading">
                                    <h4>
                                        Appointment Report</h4>
                                </div>
                                <p class="print-only" style="margin:0 0 12px;color:#4b5563;">
                                    <asp:Literal ID="litPrintFilters" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <%-- ── Filters ── --%>
                            <div class="no-print">
                                <div class="row">
                                    <div class="col-12 col-md-6 col-xl-3">
                                        <div class="input-block local-forms">
                                            <label>From Date</label>
                                            <asp:TextBox ID="txtFromDate" CssClass="form-control" runat="server" TextMode="Date"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-12 col-md-6 col-xl-3">
                                        <div class="input-block local-forms">
                                            <label>To Date</label>
                                            <asp:TextBox ID="txtToDate" CssClass="form-control" runat="server" TextMode="Date"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-12 col-md-6 col-xl-3">
                                        <div class="input-block local-forms">
                                            <label>Status</label>
                                            <asp:DropDownList ID="ddlAppointmentStatus" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="ALL" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                                                <asp:ListItem Text="Confirmed" Value="Confirmed"></asp:ListItem>
                                                <asp:ListItem Text="Completed" Value="Completed"></asp:ListItem>
                                                <asp:ListItem Text="Cancelled" Value="Cancelled"></asp:ListItem>
                                                <asp:ListItem Text="No Show" Value="No Show"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-12 col-md-6 col-xl-3">
                                        <div class="input-block local-forms">
                                            <label>Doctor</label>
                                            <asp:DropDownList ID="ddlDoctorList" runat="server" CssClass="form-control">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-12 col-md-6 col-xl-3">
                                        <div class="input-block local-forms">
                                            <label>Booking Type</label>
                                            <asp:DropDownList ID="ddlBookingType" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="ALL" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Online" Value="Online"></asp:ListItem>
                                                <asp:ListItem Text="Walk-in" Value="Walk-in"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-12 col-md-6 col-xl-3">
                                        <div class="input-block local-forms">
                                            <label>Walk-in Booked By</label>
                                            <asp:DropDownList ID="ddlBookedBy" runat="server" CssClass="form-control">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-12 col-xl-6">
                                        <div class="input-block local-forms">
                                            <label>Search</label>
                                            <asp:TextBox ID="txtSearch" CssClass="form-control" runat="server"
                                                placeholder="Patient name, Patient ID, mobile or token"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <%-- Quick ranges on the left, all buttons on one line on the right --%>
                                <div class="report-toolbar">
                                    <div class="range-links">
                                        <span>Quick range:</span>
                                        <asp:LinkButton ID="lnkToday" runat="server" CssClass="range-link" CommandArgument="today" OnCommand="QuickRange_Command">Today</asp:LinkButton>
                                        <asp:LinkButton ID="lnkWeek" runat="server" CssClass="range-link" CommandArgument="week" OnCommand="QuickRange_Command">This Week</asp:LinkButton>
                                        <asp:LinkButton ID="lnkMonth" runat="server" CssClass="range-link" CommandArgument="month" OnCommand="QuickRange_Command">This Month</asp:LinkButton>
                                        <asp:LinkButton ID="lnkAll" runat="server" CssClass="range-link" CommandArgument="all" OnCommand="QuickRange_Command">All Dates</asp:LinkButton>
                                    </div>
                                    <div class="report-actions">
                                        <asp:Button ID="btnFilter" runat="server" CssClass="btn btn-primary"
                                            Text="Filter" OnClick="btnFilter_Click" />
                                        <asp:Button ID="btnReset" runat="server" CssClass="btn btn-outline-secondary"
                                            Text="Reset" OnClick="btnReset_Click" />
                                        <span class="toolbar-divider"></span>
                                        <asp:Button ID="btnExport" runat="server" CssClass="btn btn-success"
                                            Text="Export to Excel" OnClick="btnExport_Click" />
                                        <asp:Button ID="btnPrint" runat="server" CssClass="btn btn-outline-primary"
                                            Text="Print / Save as PDF" OnClick="btnPrint_Click" />
                                    </div>
                                </div>
                            </div>

                            <%-- ── Summary cards (for the filtered appointments) ── --%>
                            <div class="summary-grid">
                                <div class="summary-card total">
                                    <div class="label">Total</div>
                                    <div class="value"><asp:Literal ID="litTotal" runat="server" /></div>
                                    <div class="sub"><asp:Literal ID="litTotalSub" runat="server" /></div>
                                </div>
                                <div class="summary-card completed">
                                    <div class="label">Completed</div>
                                    <div class="value"><asp:Literal ID="litCompleted" runat="server" /></div>
                                </div>
                                <div class="summary-card upcoming">
                                    <div class="label">Upcoming</div>
                                    <div class="value"><asp:Literal ID="litUpcoming" runat="server" /></div>
                                    <div class="sub">Pending + Confirmed</div>
                                </div>
                                <div class="summary-card cancelled">
                                    <div class="label">Cancelled</div>
                                    <div class="value"><asp:Literal ID="litCancelled" runat="server" /></div>
                                    <div class="sub"><asp:Literal ID="litCancelRate" runat="server" /></div>
                                </div>
                                <div class="summary-card noshow">
                                    <div class="label">No Show</div>
                                    <div class="value"><asp:Literal ID="litNoShow" runat="server" /></div>
                                    <div class="sub">patient did not come</div>
                                </div>
                                <div class="summary-card money">
                                    <div class="label">Revenue</div>
                                    <div class="value"><asp:Literal ID="litRevenue" runat="server" /></div>
                                    <div class="sub">from completed visits</div>
                                </div>
                                <div class="summary-card money">
                                    <div class="label">Advance Online</div>
                                    <div class="value"><asp:Literal ID="litAdvance" runat="server" /></div>
                                    <div class="sub"><asp:Literal ID="litAdvanceSub" runat="server" /></div>
                                </div>
                            </div>

                            <%-- ── Charts (same filtered appointments as the cards and tables) ── --%>
                            <div class="chart-grid">
                                <div class="chart-card wide">
                                    <h6>Appointments per <span id="chartDaysUnit">day</span></h6>
                                    <div class="chart-sub">All statuses, for the filtered dates</div>
                                    <div class="chart-box"><canvas id="chartDays" aria-label="Appointments per day" role="img"></canvas></div>
                                    <div class="chart-empty">No appointments for these filters.</div>
                                </div>
                                <div class="chart-card">
                                    <h6>Appointments by doctor</h6>
                                    <div class="chart-sub">Table: Summary by Doctor below</div>
                                    <div class="chart-box"><canvas id="chartDoctors" aria-label="Appointments by doctor" role="img"></canvas></div>
                                    <div class="chart-empty">No appointments for these filters.</div>
                                </div>
                                <div class="chart-card">
                                    <h6>Appointments by status</h6>
                                    <div class="chart-sub">Pending, Confirmed, Completed, Cancelled, No Show</div>
                                    <div class="chart-box"><canvas id="chartStatus" aria-label="Appointments by status" role="img"></canvas></div>
                                    <div class="chart-empty">No appointments for these filters.</div>
                                </div>
                            </div>

                            <%-- ── Per-doctor summary ── --%>
                            <div class="section-title">Summary by Doctor</div>
                            <div class="table-responsive">
                            <asp:GridView ID="gvDoctorSummary" runat="server" CssClass="table table-bordered doctor-summary mb-4"
                                AutoGenerateColumns="False" EmptyDataText="No appointments for these filters.">
                                <Columns>
                                    <asp:BoundField DataField="Doctor" HeaderText="Doctor" />
                                    <asp:BoundField DataField="Total" HeaderText="Total" />
                                    <asp:BoundField DataField="Completed" HeaderText="Completed" />
                                    <asp:BoundField DataField="Upcoming" HeaderText="Upcoming" />
                                    <asp:BoundField DataField="Cancelled" HeaderText="Cancelled" />
                                    <asp:BoundField DataField="NoShow" HeaderText="No Show" />
                                    <asp:BoundField DataField="Revenue" HeaderText="Revenue (completed)" HtmlEncode="false" />
                                </Columns>
                            </asp:GridView>
                            </div>

                            <%-- ── Appointments ── --%>
                            <div class="section-title">Appointments</div>
                            <div class="table-responsive">
                            <asp:GridView ID="gvAppointmentReport" runat="server" CssClass="table table-striped small-grid"
                                AutoGenerateColumns="False" ShowHeader="true" EmptyDataText="No Report Found."
                                AllowPaging="true" PageSize="10" OnPageIndexChanging="gvAppointmentReport_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="AppointmentID" HeaderText="ID" />
                                    <asp:TemplateField HeaderText="User & Patient Details">
                                        <ItemTemplate>
                                            <div class="small-text" style="border-bottom: 1px solid #ddd; padding-bottom: 10px;">
                                                <strong style="font-weight: bold;">User Details:</strong>
                                                <span class='custom-badge <%# Eval("BookingType").ToString() == "Walk-in" ? "status-orange" : "status-blue" %>'>
                                                    <%# Eval("BookingType").ToString() == "Walk-in" ? "Walk-in" : "Online" %>
                                                </span><br />
                                                <asp:PlaceHolder runat="server" Visible='<%# Eval("BookingType").ToString() == "Walk-in" %>'>
                                                    Booked at front desk<br />
                                                    <asp:PlaceHolder runat="server" Visible='<%# Eval("BookedByName").ToString() != "" %>'>
                                                        by <strong><%# Eval("BookedByName") %></strong> (<%# Eval("BookedByRole") %>)<br />
                                                    </asp:PlaceHolder>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%# Eval("BookingType").ToString() != "Walk-in" %>'>
                                                    <%# Eval("UserName")%><br />
                                                    <%# Eval("UserMobile") %><br />
                                                    <%# Eval("UserEmail") %><br />
                                                </asp:PlaceHolder>
                                            </div>
                                            <div class="small-text" style="margin-top: 10px;">
                                                <strong style="font-weight: bold;">Patient Details:</strong>
                                                <%# HttpUtility.HtmlEncode(Eval("PatientName")) %> <span class="patient-code" style="display:inline;"><%# Eval("PatientCode") %></span><br />
                                                <strong style="font-weight: bold;">Contact No.:</strong>
                                                <%# Eval("PatientMobile") %><br />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Appointment">
                                        <ItemTemplate>
                                            <span class="small-text">
                                                <%# Eval("AppointDate", "{0:dd MMM yyyy}") %><br />
                                                <%# FormatTime(Eval("AppointTime")) %><br />
                                                Token: <strong><%# Eval("TokenNo") %></strong>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Doctor">
                                        <ItemTemplate>
                                            <span class="small-text"><strong style="font-weight: bold;">Doctor Details:</strong>
                                                <%# Eval("DoctorName")%><br />
                                                <strong style="font-weight: bold;">Speciality:</strong>
                                                <%# Eval("Specialization") %><br />
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status">
                                        <ItemTemplate>
                                            <span class='st-badge st-<%# Eval("Status").ToString().Replace(" ", "") %>'><%# Eval("Status") %></span>
                                            <asp:PlaceHolder runat="server" Visible='<%# Eval("DeletedAt") != DBNull.Value %>'>
                                                <br /><span class="removed-note">Removed from list<br /><%# Eval("DeletedAt", "{0:dd MMM yyyy}") %></span>
                                            </asp:PlaceHolder>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fee">
                                        <ItemTemplate>
                                            <span class="money-cell"><%# Money(Eval("Fee")) %></span>
                                            <asp:PlaceHolder runat="server" Visible='<%# Convert.ToInt32(Eval("FeeEstimated")) == 1 %>'>
                                                <br /><span class="estimate" title="Booked before fees were saved with walk-in bookings; this is the doctor's current fee">estimate</span>
                                            </asp:PlaceHolder>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Paid Online">
                                        <ItemTemplate><span class="money-cell"><%# Money(Eval("PaidAmount")) %></span></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Due at Clinic">
                                        <ItemTemplate><span class="money-cell"><%# Money(Eval("DueAmount")) %></span></ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pagination" />
                            </asp:GridView>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <script src="assets/plugins/chartjs/chart.min.js"></script>
    <script>
        // Called by the server after every bind (first load and each filter), with the filtered counts
        var reportCharts = {};
        // ink and grid follow light / dark mode (assets/js/doccare-theme.js)
        var CHART_BAR = '#2a78d6';
        function chartInk() { return window.DocCareTheme ? DocCareTheme.ink() : '#52514e'; }
        function chartGrid() { return window.DocCareTheme ? DocCareTheme.grid() : '#e8e7e3'; }

        // Value at the end of each bar (one series, few bars, so every bar is labelled)
        var barValueLabels = {
            id: 'barValueLabels',
            afterDatasetsDraw: function (chart) {
                var ctx = chart.ctx, horizontal = chart.options.indexAxis === 'y';
                ctx.save();
                ctx.fillStyle = chartInk();
                ctx.font = '600 12px Poppins, sans-serif';
                chart.getDatasetMeta(0).data.forEach(function (bar, i) {
                    var v = chart.data.datasets[0].data[i];
                    if (!v) return;
                    if (horizontal) { ctx.textAlign = 'left'; ctx.textBaseline = 'middle'; ctx.fillText(v, bar.x + 6, bar.y); }
                    else { ctx.textAlign = 'center'; ctx.textBaseline = 'bottom'; ctx.fillText(v, bar.x, bar.y - 4); }
                });
                ctx.restore();
            }
        };

        function drawBarChart(id, labels, values, horizontal) {
            var canvas = document.getElementById(id);
            if (!canvas || typeof Chart === 'undefined') return;
            var card = canvas.closest('.chart-card');
            if (reportCharts[id]) { reportCharts[id].destroy(); delete reportCharts[id]; }
            var empty = values.every(function (v) { return !v; });
            card.querySelector('.chart-box').style.display = empty ? 'none' : '';
            card.querySelector('.chart-empty').style.display = empty ? 'flex' : 'none';
            if (empty) return;

            var valueAxis = { beginAtZero: true, grace: '12%', ticks: { precision: 0, color: chartInk() }, grid: { color: chartGrid(), drawBorder: false } };
            var labelAxis = { ticks: { color: chartInk(), autoSkip: true, maxRotation: 0 }, grid: { display: false, drawBorder: false } };
            reportCharts[id] = new Chart(canvas, {
                type: 'bar',
                data: { labels: labels, datasets: [{ data: values, backgroundColor: CHART_BAR, borderRadius: 4, maxBarThickness: 28 }] },
                options: {
                    indexAxis: horizontal ? 'y' : 'x',
                    responsive: true, maintainAspectRatio: false, animation: false,
                    plugins: {
                        legend: { display: false },
                        tooltip: { callbacks: { label: function (c) { return ' ' + c.parsed[horizontal ? 'x' : 'y'] + (c.parsed[horizontal ? 'x' : 'y'] === 1 ? ' appointment' : ' appointments'); } } }
                    },
                    scales: horizontal ? { x: valueAxis, y: labelAxis } : { x: labelAxis, y: valueAxis }
                },
                plugins: [barValueLabels]
            });
        }

        function renderReportCharts(d) {
            document.getElementById('chartDaysUnit').textContent = d.byMonth ? 'month' : 'day';
            drawBarChart('chartDays', d.days.labels, d.days.values, false);
            drawBarChart('chartDoctors', d.doctors.labels, d.doctors.values, true);
            drawBarChart('chartStatus', d.statuses.labels, d.statuses.values, true);
        }
    </script>
</asp:Content>
