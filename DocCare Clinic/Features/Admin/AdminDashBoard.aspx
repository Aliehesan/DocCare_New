<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="AdminDashBoard.aspx.cs" Inherits="DocCare_Clinic.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="assets/plugins/chartjs/chart.min.js"></script>
    <script>
        var myChart = null; // ✅ Prevent Duplicate Charts

        function drawChart(malePercentage, femalePercentage) {
            var ctx = document.getElementById('patient-chart').getContext('2d');

            // ✅ Destroy Previous Chart If Exists
            if (myChart != null) {
                myChart.destroy();
            }

            // ✅ Create New Chart
            myChart = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ['Male', 'Female'],
                    datasets: [{
                        data: [malePercentage, femalePercentage],
                        backgroundColor: ['#3498db', '#e74c3c'],
                        hoverBackgroundColor: ['#2980b9', '#c0392b']
                    }]
                },
                options: {
                    responsive: false, // ✅ Prevent Chart Stretching
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'bottom'
                        }
                    },
                    animation: {
                        duration: 800,
                        easing: 'easeInOutQuart'
                    }
                }
            });
        }
    </script>
    <style>
        .dash-icon { color: #2E37A4; font-size: 22px; }
        .dash-content h4 { white-space: nowrap; }
    </style>
    <script>
        // Booked appointments per day, last 14 days: one measure, one hue, value at the end of each bar
        function drawLast14Days(labels, values) {
            var canvas = document.getElementById('chart14days');
            if (!canvas || typeof Chart === 'undefined') return;
            var ink = '#52514e';
            new Chart(canvas, {
                type: 'bar',
                data: { labels: labels, datasets: [{ data: values, backgroundColor: '#2a78d6', borderRadius: 4, maxBarThickness: 28 }] },
                options: {
                    responsive: true, maintainAspectRatio: false, animation: false,
                    plugins: { legend: { display: false },
                               tooltip: { callbacks: { label: function (c) { return ' ' + c.parsed.y + (c.parsed.y === 1 ? ' appointment' : ' appointments'); } } } },
                    scales: {
                        x: { ticks: { color: ink, maxRotation: 0, autoSkip: true }, grid: { display: false, drawBorder: false } },
                        y: { beginAtZero: true, grace: '12%', ticks: { precision: 0, color: ink }, grid: { color: '#e8e7e3', drawBorder: false } }
                    }
                },
                plugins: [{
                    id: 'values',
                    afterDatasetsDraw: function (chart) {
                        var ctx = chart.ctx; ctx.save(); ctx.fillStyle = ink; ctx.font = '600 12px Poppins, sans-serif';
                        ctx.textAlign = 'center'; ctx.textBaseline = 'bottom';
                        chart.getDatasetMeta(0).data.forEach(function (bar, i) { var v = values[i]; if (v) ctx.fillText(v, bar.x, bar.y - 4); });
                        ctx.restore();
                    }
                }]
            });
        }
    </script>
    <script>
        function drawDepartmentChart(departments, patientCounts) {
            var ctx = document.getElementById('donut-chart-dash').getContext('2d');

            new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: departments,
                    datasets: [{
                        data: patientCounts,
                        backgroundColor: [
                        '#3498DB', '#E74C3C', '#F1C40F', '#1ABC9C', '#9B59B6'
                    ],
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'bottom'
                        }
                    }
                }
            });
        }
    </script>
    <script>   
    // Dynamic Date and Time
    function updateDateTime() {
        const now = new Date();
        const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: 'numeric' };
        document.getElementById('dateTime').innerHTML = `Today is ${now.toLocaleDateString('en-US', options)}`;
    }
    setInterval(updateDateTime, 1000);
     
</script>
    <script type="text/javascript">
        window.onload = function () {
            // ✅ Get Admin Name from Session (Directly from ASP.NET Session)
            var adminName = '<%= Session["UserName"] %>';
            if (adminName == "" || adminName == "null") {
                adminName = "Admin";
            }

            var today = new Date();
            var hour = today.getHours();
            var greeting = "";

            if (hour >= 5 && hour < 12) {
                greeting = "Morning";
                document.getElementById("greetingImage").src = "assets/img/morning-img-01.png";
            }
            else if (hour >= 12 && hour < 17) {
                greeting = "Afternoon";
                document.getElementById("greetingImage").src = "assets/img/morning-img-01.png";
            }
            else if (hour >= 17 && hour < 21) {
                greeting = "Evening";
                document.getElementById("greetingImage").src = "assets/img/morning-img-01.png";
            }
            else {
                greeting = "Night";
                document.getElementById("greetingImage").src = "assets/img/morning-img-01.png";
            }

            // ✅ Display Greeting + Admin Name
            document.getElementById("greeting").innerText = greeting;
            document.getElementById("adminName").innerText = adminName;
        }
    </script>
    <div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/Features/Admin/AdminDashBoard.aspx") %>">Dashboard</a> </li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Admin Dashboard</li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="good-morning-blk">
                <div class="row">
                    <div class="col-md-6">
                        <div class="morning-user">
                            <h2>
                                Good <span id="greeting"></span>, <span id="adminName"></span>!
                            </h2>
                            <p id="dateTime">
                            </p>
                        </div>
                    </div>
                    <div class="col-md-6 position-blk">
                        <div class="morning-img">
                            <img id="greetingImage" src="assets/img/morning-img-01.png" alt="Greeting Image" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <%-- ── Live figures (AdminDashBoard.aspx.cs LoadFigures) ── --%>
                <div class="col-12 col-xl-6">
                    <div class="row">
                        <div class="col-md-6 col-sm-6"><div class="dash-widget">
                            <div class="dash-boxs comman-flex-center"><i class="fa-regular fa-calendar-check dash-icon"></i></div>
                            <div class="dash-content dash-count">
                                <h4>Today</h4>
                                <h2><asp:Literal ID="litToday" runat="server" Text="0" /></h2>
                                <p><asp:Literal ID="litTodaySub" runat="server" /></p>
                            </div>
                        </div></div>
                        <div class="col-md-6 col-sm-6"><div class="dash-widget">
                            <div class="dash-boxs comman-flex-center"><i class="fa-regular fa-calendar-days dash-icon"></i></div>
                            <div class="dash-content dash-count">
                                <h4>Next 7 Days</h4>
                                <h2><asp:Literal ID="litNext7" runat="server" Text="0" /></h2>
                                <p>upcoming bookings</p>
                            </div>
                        </div></div>
                        <div class="col-md-6 col-sm-6"><div class="dash-widget">
                            <div class="dash-boxs comman-flex-center"><i class="fa-solid fa-chart-column dash-icon"></i></div>
                            <div class="dash-content dash-count">
                                <h4>Appointments This Month</h4>
                                <h2><asp:Literal ID="AppointmentsCount" runat="server" Text="0" /></h2>
                                <p><asp:Literal ID="litMonthChange" runat="server" /></p>
                            </div>
                        </div></div>
                        <div class="col-md-6 col-sm-6"><div class="dash-widget">
                            <div class="dash-boxs comman-flex-center"><i class="fa-solid fa-indian-rupee-sign dash-icon"></i></div>
                            <div class="dash-content dash-count">
                                <h4>Revenue This Month</h4>
                                <h2><asp:Literal ID="litRevenue" runat="server" Text="0" /></h2>
                                <p><asp:Literal ID="litRevenueChange" runat="server" /></p>
                            </div>
                        </div></div>
                        <div class="col-md-6 col-sm-6"><div class="dash-widget">
                            <div class="dash-boxs comman-flex-center"><i class="fa-regular fa-calendar-xmark dash-icon"></i></div>
                            <div class="dash-content dash-count">
                                <h4>Cancelled &middot; No Show</h4>
                                <h2><asp:Literal ID="litCancelled" runat="server" Text="0" /></h2>
                                <p><asp:Literal ID="litCancelledSub" runat="server" /></p>
                            </div>
                        </div></div>
                        <div class="col-md-6 col-sm-6"><div class="dash-widget">
                            <div class="dash-boxs comman-flex-center"><i class="fa-solid fa-user-doctor dash-icon"></i></div>
                            <div class="dash-content dash-count">
                                <h4>Doctors &middot; Patients</h4>
                                <h2><asp:Literal ID="NewPatientsCount" runat="server" Text="0" /></h2>
                                <p><asp:Literal ID="litPeopleSub" runat="server" /></p>
                            </div>
                        </div></div>
                    </div>
                </div>
                <div class="col-12 col-md-12 col-lg-6 col-xl-6">
                    <div class="card">
                        <div class="card-body">
                            <div class="chart-title patient-visit">
                                <h4>
                                    Patient Visit by Gender</h4>
                                <div>
                                    <ul class="nav chat-user-total">
                                        <li><i class="fa fa-circle current-users" aria-hidden="true"></i>Male <span id="malePercentage">
                                            0%</span></li>
                                        <li><i class="fa fa-circle old-users" aria-hidden="true"></i>Female <span id="femalePercentage">
                                            0%</span></li>
                                    </ul>
                                </div>
                            </div>
                            <canvas id="patient-chart" style="width: 250px; height: 250px; margin-left: 125px;"></canvas>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="chart-title"><h4>Appointments, Last 14 Days</h4></div>
                            <p style="font-size: 12px; color: #52514e; margin: -6px 0 8px;">Booked appointments per day (cancelled not counted). Details: Appointment Report.</p>
                            <div style="position: relative; height: 240px;"><canvas id="chart14days" aria-label="Appointments in the last 14 days" role="img"></canvas></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
