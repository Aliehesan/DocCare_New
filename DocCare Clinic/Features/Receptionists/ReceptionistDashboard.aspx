<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Receptionists/ReceptionistAdmin.Master" AutoEventWireup="true"
    CodeBehind="ReceptionistDashboard.aspx.cs" Inherits="DocCare_Clinic.ReceptionistDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
                <div class="col-md-6 col-sm-6 col-lg-6 col-xl-3">
                    <div class="dash-widget">
                        <div class="dash-boxs comman-flex-center">
                            <svg width="24" height="24" viewbox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                            <path d="M16.7505 3.56V2C16.7505 1.59 16.4105 1.25 16.0005 1.25C15.5905 1.25 15.2505 1.59 15.2505 2V3.5H8.75047V2C8.75047 1.59 8.41047 1.25 8.00047 1.25C7.59047 1.25 7.25047 1.59 7.25047 2V3.56C4.55047 3.81 3.24047 5.42 3.04047 7.81C3.02047 8.1 3.26047 8.34 3.54047 8.34H20.4605C20.7505 8.34 20.9905 8.09 20.9605 7.81C20.7605 5.42 19.4505 3.81 16.7505 3.56Z" fill="#2E37A4"/>
                                            <path opacity="0.4" d="M20 9.84375C20.55 9.84375 21 10.2937 21 10.8438V17.0038C21 20.0038 19.5 22.0038 16 22.0038H8C4.5 22.0038 3 20.0038 3 17.0038V10.8438C3 10.2937 3.45 9.84375 4 9.84375H20Z" fill="#2E37A4"/>
                                            <path d="M8.5 14.9989C8.24 14.9989 7.98 14.8889 7.79 14.7089C7.61 14.5189 7.5 14.2589 7.5 13.9989C7.5 13.7389 7.61 13.4789 7.79 13.2889C8.07 13.0089 8.51 12.9189 8.88 13.0789C9.01 13.1289 9.12 13.1989 9.21 13.2889C9.39 13.4789 9.5 13.7389 9.5 13.9989C9.5 14.2589 9.39 14.5189 9.21 14.7089C9.02 14.8889 8.76 14.9989 8.5 14.9989Z" fill="#2E37A4"/>
                                            <path d="M12.0002 14.9989C11.7402 14.9989 11.4802 14.8889 11.2902 14.7089C11.1102 14.5189 11.0002 14.2589 11.0002 13.9989C11.0002 13.7389 11.1102 13.4789 11.2902 13.2889C11.3802 13.1989 11.4902 13.1289 11.6202 13.0789C11.9902 12.9189 12.4302 13.0089 12.7102 13.2889C12.8902 13.4789 13.0002 13.7389 13.0002 13.9989C13.0002 14.2589 12.8902 14.5189 12.7102 14.7089C12.6602 14.7489 12.6102 14.7889 12.5602 14.8289C12.5002 14.8689 12.4402 14.8989 12.3802 14.9189C12.3202 14.9489 12.2602 14.9689 12.2002 14.9789C12.1302 14.9889 12.0702 14.9989 12.0002 14.9989Z" fill="#2E37A4"/>
                                            <path d="M15.4998 15C15.2398 15 14.9798 14.89 14.7898 14.71C14.6098 14.52 14.4998 14.26 14.4998 14C14.4998 13.74 14.6098 13.48 14.7898 13.29C14.8898 13.2 14.9898 13.13 15.1198 13.08C15.2998 13 15.4998 12.98 15.6998 13.02C15.7598 13.03 15.8198 13.05 15.8798 13.08C15.9398 13.1 15.9998 13.13 16.0598 13.17C16.1098 13.21 16.1598 13.25 16.2098 13.29C16.3898 13.48 16.4998 13.74 16.4998 14C16.4998 14.26 16.3898 14.52 16.2098 14.71C16.1598 14.75 16.1098 14.79 16.0598 14.83C15.9998 14.87 15.9398 14.9 15.8798 14.92C15.8198 14.95 15.7598 14.97 15.6998 14.98C15.6298 14.99 15.5598 15 15.4998 15Z" fill="#2E37A4"/>
                                            <path d="M8.5 18.5C8.37 18.5 8.24 18.47 8.12 18.42C7.99 18.37 7.89 18.3 7.79 18.21C7.61 18.02 7.5 17.76 7.5 17.5C7.5 17.24 7.61 16.98 7.79 16.79C7.89 16.7 7.99 16.63 8.12 16.58C8.3 16.5 8.5 16.48 8.7 16.52C8.76 16.53 8.82 16.55 8.88 16.58C8.94 16.6 9 16.63 9.06 16.67C9.11 16.71 9.16 16.75 9.21 16.79C9.39 16.98 9.5 17.24 9.5 17.5C9.5 17.76 9.39 18.02 9.21 18.21C9.16 18.25 9.11 18.3 9.06 18.33C9 18.37 8.94 18.4 8.88 18.42C8.82 18.45 8.76 18.47 8.7 18.48C8.63 18.49 8.57 18.5 8.5 18.5Z" fill="#2E37A4"/>
                                            <path d="M12.0002 18.5031C11.7402 18.5031 11.4802 18.3931 11.2902 18.2131C11.1102 18.0231 11.0002 17.7631 11.0002 17.5031C11.0002 17.2431 11.1102 16.9831 11.2902 16.7931C11.6602 16.4231 12.3402 16.4231 12.7102 16.7931C12.8902 16.9831 13.0002 17.2431 13.0002 17.5031C13.0002 17.7631 12.8902 18.0231 12.7102 18.2131C12.5202 18.3931 12.2602 18.5031 12.0002 18.5031Z" fill="#2E37A4"/>
                                            <path d="M15.4998 18.5031C15.2398 18.5031 14.9798 18.3931 14.7898 18.2131C14.6098 18.0231 14.4998 17.7631 14.4998 17.5031C14.4998 17.2431 14.6098 16.9831 14.7898 16.7931C15.1598 16.4231 15.8398 16.4231 16.2098 16.7931C16.3898 16.9831 16.4998 17.2431 16.4998 17.5031C16.4998 17.7631 16.3898 18.0231 16.2098 18.2131C16.0198 18.3931 15.7598 18.5031 15.4998 18.5031Z" fill="#2E37A4"/>
                                            </svg>
                        </div>
                        <div class="dash-content dash-count">
                            <h4>
                                Appointments</h4>
                            <h2>
                                <asp:Literal ID="AppointmentsCount" runat="server" Text="250"></asp:Literal></h2>
                            <p>
                                <span class="passive-view"><i class="feather-arrow-up-right me-1"></i>40% </span>
                                vs last month
                            </p>
                        </div>
                    </div>
                </div>
                <!-- New Patients Widget -->
                <div class="col-md-6 col-sm-6 col-lg-6 col-xl-3">
                    <div class="dash-widget">
                        <div class="dash-boxs comman-flex-center">
                            <svg width="28" height="28" viewbox="0 0 28 28" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M14.0004 2.33594C10.9437 2.33594 8.45874 4.82094 8.45874 7.8776C8.45874 10.8759 10.8037 13.3026 13.8604 13.4076C13.9537 13.3959 14.0471 13.3959 14.1171 13.4076C14.1404 13.4076 14.1521 13.4076 14.1754 13.4076C14.1871 13.4076 14.1871 13.4076 14.1987 13.4076C17.1854 13.3026 19.5304 10.8759 19.5421 7.8776C19.5421 4.82094 17.0571 2.33594 14.0004 2.33594Z" fill="#2E37A4"/>
<path opacity="0.4" d="M19.9264 16.5103C16.6714 14.3403 11.363 14.3403 8.08471 16.5103C6.60304 17.502 5.78638 18.8436 5.78638 20.2786C5.78638 21.7136 6.60304 23.0436 8.07304 24.0236C9.70638 25.1203 11.853 25.6686 13.9997 25.6686C16.1464 25.6686 18.293 25.1203 19.9264 24.0236C21.3964 23.032 22.213 21.702 22.213 20.2553C22.2014 18.8203 21.3964 17.4903 19.9264 16.5103Z" fill="#2E37A4"/>
<path d="M16.3329 19.4036H14.8746V17.9453C14.8746 17.467 14.4779 17.0703 13.9996 17.0703C13.5213 17.0703 13.1246 17.467 13.1246 17.9453V19.4036H11.6663C11.1879 19.4036 10.7913 19.8003 10.7913 20.2786C10.7913 20.757 11.1879 21.1536 11.6663 21.1536H13.1246V22.612C13.1246 23.0903 13.5213 23.487 13.9996 23.487C14.4779 23.487 14.8746 23.0903 14.8746 22.612V21.1536H16.3329C16.8113 21.1536 17.2079 20.757 17.2079 20.2786C17.2079 19.8003 16.8113 19.4036 16.3329 19.4036Z" fill="#2E37A4"/>
</svg>
                        </div>
                        <div class="dash-content dash-count">
                            <h4>
                                New Patients</h4>
                            <h2>
                                <asp:Literal ID="NewPatientsCount" runat="server" Text="140"></asp:Literal></h2>
                            <p>
                                <span class="passive-view"><i class="feather-arrow-up-right me-1"></i>20% </span>
                                vs last month
                            </p>
                        </div>
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
