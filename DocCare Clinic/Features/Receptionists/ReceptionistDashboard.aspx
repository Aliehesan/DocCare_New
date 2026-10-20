<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Receptionists/ReceptionistAdmin.Master" AutoEventWireup="true"
    CodeBehind="ReceptionistDashboard.aspx.cs" Inherits="DocCare_Clinic.ReceptionistDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .dash-icon { color: #2E37A4; font-size: 22px; }
        .dash-link { display: block; color: inherit; text-decoration: none; }
        .dash-link:hover .dash-widget { box-shadow: 0 6px 18px rgba(46, 55, 164, 0.12); }
        .dash-content h4 { white-space: nowrap; }
        .dash-warn { color: #b45309; font-weight: 600; }
    </style>
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
                            position: 'bottom',
                            // legend text follows light / dark mode (assets/js/doccare-theme.js)
                            labels: { color: window.DocCareTheme ? DocCareTheme.ink() : '#666' }
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
                            position: 'bottom',
                            // legend text follows light / dark mode (assets/js/doccare-theme.js)
                            labels: { color: window.DocCareTheme ? DocCareTheme.ink() : '#666' }
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
            var ReceptionistName = '<%= Session["ReceptionistName"] %>';
            if (ReceptionistName == "" || ReceptionistName == "null") {
                ReceptionistName = "Receptionist";
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
            document.getElementById("ReceptionistName").innerText = ReceptionistName;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/Features/Receptionists/ReceptionistDashboard.aspx") %>">Dashboard</a> </li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Receptionist Dashboard</li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="good-morning-blk">
                <div class="row">
                    <div class="col-md-6">
                        <div class="morning-user">
                            <h2>
                                Good <span id="greeting"></span>, <span id="ReceptionistName"></span>!
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
                <!-- Appointments Widget -->
                <%-- ── Front desk at a glance (ReceptionistDashboard.aspx.cs LoadFigures) ── --%>
                <div class="col-12 col-xl-6">
                    <div class="row">
                        <div class="col-md-6 col-sm-6"><a class="dash-link" href="<%= ResolveUrl("~/Features/Appointments/Appointments.aspx") %>"><div class="dash-widget">
                            <div class="dash-boxs comman-flex-center"><i class="fa-regular fa-calendar-check dash-icon"></i></div>
                            <div class="dash-content dash-count">
                                <h4>Today</h4>
                                <h2><asp:Literal ID="litToday" runat="server" Text="0" /></h2>
                                <p><asp:Literal ID="litTodaySub" runat="server" /></p>
                            </div>
                        </div></a></div>
                        <div class="col-md-6 col-sm-6"><div class="dash-widget">
                            <div class="dash-boxs comman-flex-center"><i class="fa-regular fa-calendar-days dash-icon"></i></div>
                            <div class="dash-content dash-count">
                                <h4>Next 7 Days</h4>
                                <h2><asp:Literal ID="litNext7" runat="server" Text="0" /></h2>
                                <p><asp:Literal ID="litNext7Sub" runat="server" /></p>
                            </div>
                        </div></div>
                        <div class="col-md-6 col-sm-6"><a class="dash-link" href="<%= ResolveUrl("~/Features/Appointments/FollowUps.aspx") %>"><div class="dash-widget">
                            <div class="dash-boxs comman-flex-center"><i class="fa-solid fa-stethoscope dash-icon"></i></div>
                            <div class="dash-content dash-count">
                                <h4>Follow-ups This Week</h4>
                                <h2><asp:Literal ID="litFollowUps" runat="server" Text="0" /></h2>
                                <p><asp:Literal ID="litFollowUpsSub" runat="server" /></p>
                            </div>
                        </div></a></div>
                        <div class="col-md-6 col-sm-6"><a class="dash-link" href="<%= ResolveUrl("~/Features/Appointments/Refunds.aspx") %>"><div class="dash-widget">
                            <div class="dash-boxs comman-flex-center"><i class="fa-solid fa-rotate-left dash-icon"></i></div>
                            <div class="dash-content dash-count">
                                <h4>Refunds Due</h4>
                                <h2><asp:Literal ID="litRefunds" runat="server" Text="0" /></h2>
                                <p><asp:Literal ID="litRefundsSub" runat="server" /></p>
                            </div>
                        </div></a></div>
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
        </div>
    </div>
</asp:Content>
