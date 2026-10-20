<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Doctors/DoctorAdmin.Master" AutoEventWireup="true"
    CodeBehind="DoctorDashBoard.aspx.cs" Inherits="DocCare_Clinic.DoctorDashBoard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
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
    </script>--%>
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
            var DoctorName = '<%= Session["DoctorName"] %>';
            if (DoctorName == "" || DoctorName == "null") {
                DoctorName = "Doctor";
            }

            var today = new Date();
            var hour = today.getHours();
            var greeting = "";

            if (hour >= 5 && hour < 12) {
                greeting = "Morning";
                document.getElementById("greetingImage").src = "assets/img/morning-img-02.png";
            }
            else if (hour >= 12 && hour < 17) {
                greeting = "Afternoon";
                document.getElementById("greetingImage").src = "assets/img/morning-img-02.png";
            }
            else if (hour >= 17 && hour < 21) {
                greeting = "Evening";
                document.getElementById("greetingImage").src = "assets/img/morning-img-02.png";
            }
            else {
                greeting = "Night";
                document.getElementById("greetingImage").src = "assets/img/morning-img-02.png";
            }

            // ✅ Display Greeting + Admin Name
            document.getElementById("greeting").innerText = greeting;
            document.getElementById("DoctorName").innerText = DoctorName;
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
                            <li class="breadcrumb-item"><a href="<%= ResolveUrl("~/Features/Doctors/DoctorDashBoard.aspx") %>">Dashboard</a> </li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">Doctor Dashboard</li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="good-morning-blk">
                <div class="row">
                    <div class="col-md-6">
                        <div class="morning-user">
                            <h2>
                                Good <span id="greeting" style="color: #333;"></span>, <span>Dr.</span><span id="DoctorName"></span>
                            </h2>
                            <p id="dateTime">
                            </p>
                        </div>
                    </div>
                    <div class="col-md-6 position-blk">
                        <div class="morning-img">
                            <img id="greetingImage" src="assets/img/morning-img-02.png" alt="Greeting Image" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="doctor-list-blk">
                <div class="row">
                    <div class="col-xl-3 col-md-6">
                        <div class="doctor-widget border-right-bg">
                            <div class="doctor-box-icon flex-shrink-0">
                                <svg width="32" height="32" viewbox="0 0 32 32" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M22.2706 4.85388V2.79469C22.2706 2.25349 21.8218 1.80469 21.2806 1.80469C20.7394 1.80469 20.2906 2.25349 20.2906 2.79469V4.77469H11.7106V2.79469C11.7106 2.25349 11.2618 1.80469 10.7206 1.80469C10.1794 1.80469 9.73061 2.25349 9.73061 2.79469V4.85388C6.16661 5.18388 4.43742 7.30908 4.17342 10.4639C4.14702 10.8467 4.46382 11.1635 4.83342 11.1635H27.1678C27.5506 11.1635 27.8674 10.8335 27.8278 10.4639C27.5638 7.30908 25.8346 5.18388 22.2706 4.85388Z" fill="white"/>
<path opacity="0.4" d="M26.5599 13.1484C27.2859 13.1484 27.8799 13.7424 27.8799 14.4684V22.5996C27.8799 26.5596 25.8999 29.1996 21.2799 29.1996H10.7199C6.09987 29.1996 4.11987 26.5596 4.11987 22.5996V14.4684C4.11987 13.7424 4.71387 13.1484 5.43987 13.1484H26.5599Z" fill="white"/>
<path d="M11.3798 19.9589C11.0366 19.9589 10.6934 19.8136 10.4426 19.576C10.205 19.3252 10.0598 18.9821 10.0598 18.6389C10.0598 18.2957 10.205 17.9525 10.4426 17.7017C10.8122 17.3321 11.393 17.2132 11.8814 17.4244C12.053 17.4904 12.1982 17.5829 12.317 17.7017C12.5546 17.9525 12.6998 18.2957 12.6998 18.6389C12.6998 18.9821 12.5546 19.3252 12.317 19.576C12.0662 19.8136 11.723 19.9589 11.3798 19.9589Z" fill="white"/>
<path d="M16.0002 19.9589C15.657 19.9589 15.3138 19.8136 15.063 19.576C14.8254 19.3252 14.6802 18.9821 14.6802 18.6389C14.6802 18.2957 14.8254 17.9525 15.063 17.7017C15.1818 17.5829 15.327 17.4904 15.4986 17.4244C15.987 17.2132 16.5678 17.3321 16.9374 17.7017C17.175 17.9525 17.3202 18.2957 17.3202 18.6389C17.3202 18.9821 17.175 19.3252 16.9374 19.576C16.8714 19.6288 16.8054 19.6816 16.7394 19.7344C16.6602 19.7872 16.581 19.8269 16.5018 19.8533C16.4226 19.8929 16.3434 19.9193 16.2642 19.9325C16.1718 19.9457 16.0926 19.9589 16.0002 19.9589Z" fill="white"/>
<path d="M20.6196 19.9603C20.2764 19.9603 19.9332 19.8151 19.6824 19.5775C19.4448 19.3267 19.2996 18.9835 19.2996 18.6403C19.2996 18.2971 19.4448 17.9539 19.6824 17.7031C19.8144 17.5843 19.9464 17.4919 20.118 17.4259C20.3556 17.3203 20.6196 17.2939 20.8836 17.3467C20.9628 17.3599 21.042 17.3863 21.1212 17.4259C21.2004 17.4523 21.2796 17.4919 21.3588 17.5447C21.4248 17.5975 21.4908 17.6503 21.5568 17.7031C21.7944 17.9539 21.9396 18.2971 21.9396 18.6403C21.9396 18.9835 21.7944 19.3267 21.5568 19.5775C21.4908 19.6303 21.4248 19.6831 21.3588 19.7359C21.2796 19.7887 21.2004 19.8283 21.1212 19.8547C21.042 19.8943 20.9628 19.9207 20.8836 19.9339C20.7912 19.9471 20.6988 19.9603 20.6196 19.9603Z" fill="white"/>
<path d="M11.3798 24.5775C11.2082 24.5775 11.0366 24.5379 10.8782 24.4719C10.7066 24.4059 10.5746 24.3135 10.4426 24.1947C10.205 23.9439 10.0598 23.6007 10.0598 23.2575C10.0598 22.9143 10.205 22.5711 10.4426 22.3203C10.5746 22.2015 10.7066 22.1091 10.8782 22.0431C11.1158 21.9375 11.3798 21.9111 11.6438 21.9639C11.723 21.9771 11.8022 22.0035 11.8814 22.0431C11.9606 22.0695 12.0398 22.1091 12.119 22.1619C12.185 22.2147 12.251 22.2675 12.317 22.3203C12.5546 22.5711 12.6998 22.9143 12.6998 23.2575C12.6998 23.6007 12.5546 23.9439 12.317 24.1947C12.251 24.2475 12.185 24.3135 12.119 24.3531C12.0398 24.4059 11.9606 24.4455 11.8814 24.4719C11.8022 24.5115 11.723 24.5379 11.6438 24.5511C11.5514 24.5643 11.4722 24.5775 11.3798 24.5775Z" fill="white"/>
<path d="M16.0002 24.5766C15.657 24.5766 15.3138 24.4314 15.063 24.1938C14.8254 23.943 14.6802 23.5998 14.6802 23.2566C14.6802 22.9134 14.8254 22.5702 15.063 22.3194C15.5514 21.831 16.449 21.831 16.9374 22.3194C17.175 22.5702 17.3202 22.9134 17.3202 23.2566C17.3202 23.5998 17.175 23.943 16.9374 24.1938C16.6866 24.4314 16.3434 24.5766 16.0002 24.5766Z" fill="white"/>
<path d="M20.6196 24.5766C20.2764 24.5766 19.9332 24.4314 19.6824 24.1938C19.4448 23.943 19.2996 23.5998 19.2996 23.2566C19.2996 22.9134 19.4448 22.5702 19.6824 22.3194C20.1708 21.831 21.0684 21.831 21.5568 22.3194C21.7944 22.5702 21.9396 22.9134 21.9396 23.2566C21.9396 23.5998 21.7944 23.943 21.5568 24.1938C21.306 24.4314 20.9628 24.5766 20.6196 24.5766Z" fill="white"/>
</svg>
                            </div>
                            <div class="doctor-content dash-count flex-grow-1">
                                <h4>
                                    <asp:Label ID="lblAppointments" runat="server" CssClass="counter-up"></asp:Label>
                                    <asp:Label ID="lblTotalAppointments" runat="server"></asp:Label>
                                    <span class="status-green"><asp:Label ID="lblTodayAppointments" runat="server" /> today</span>
                                </h4>
                                <h5>
                                    Appointments</h5>
                            </div>
                        </div>
                    </div>
                    <div class="col-xl-3 col-md-6">
                        <div class="doctor-widget border-right-bg">
                            <div class="doctor-box-icon flex-shrink-0">
                                <svg width="38" height="38" viewbox="0 0 38 38" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M19.0005 3.60156C14.9657 3.60156 11.6855 6.88176 11.6855 10.9166C11.6855 14.8744 14.7809 18.0776 18.8157 18.2162C18.9389 18.2008 19.0621 18.2008 19.1545 18.2162C19.1853 18.2162 19.2007 18.2162 19.2315 18.2162C19.2469 18.2162 19.2469 18.2162 19.2623 18.2162C23.2047 18.0776 26.3001 14.8744 26.3155 10.9166C26.3155 6.88176 23.0353 3.60156 19.0005 3.60156Z" fill="white"/>
<path opacity="0.4" d="M26.823 22.3124C22.5264 19.448 15.5194 19.448 11.192 22.3124C9.2362 23.6214 8.1582 25.3924 8.1582 27.2866C8.1582 29.1808 9.2362 30.9364 11.1766 32.23C13.3326 33.6776 16.1662 34.4014 18.9998 34.4014C21.8334 34.4014 24.667 33.6776 26.823 32.23C28.7634 30.921 29.8414 29.1654 29.8414 27.2558C29.826 25.3616 28.7634 23.606 26.823 22.3124Z" fill="white"/>
<path d="M22.0796 26.1347H20.1546V24.2097C20.1546 23.5783 19.631 23.0547 18.9996 23.0547C18.3682 23.0547 17.8446 23.5783 17.8446 24.2097V26.1347H15.9196C15.2882 26.1347 14.7646 26.6583 14.7646 27.2897C14.7646 27.9211 15.2882 28.4447 15.9196 28.4447H17.8446V30.3697C17.8446 31.0011 18.3682 31.5247 18.9996 31.5247C19.631 31.5247 20.1546 31.0011 20.1546 30.3697V28.4447H22.0796C22.711 28.4447 23.2346 27.9211 23.2346 27.2897C23.2346 26.6583 22.711 26.1347 22.0796 26.1347Z" fill="white"/>
</svg>
                            </div>
                            <div class="doctor-content dash-count flex-grow-1">
                                <h4>
                                    <asp:Label ID="lblActivePatients" runat="server" CssClass="counter-up"></asp:Label>
                                    <asp:Label ID="lblTotalPatients" runat="server"></asp:Label>
                                    <span class="status-green">New this month:
                                        <asp:Label ID="lblInactivePatients" runat="server"></asp:Label></span>
                                </h4>
                                <h5>
                                    Patients seen</h5>
                            </div>
                        </div>
                    </div>
                    <div class="col-xl-3 col-md-6">
                        <div class="doctor-widget">
                            <div class="doctor-box-icon flex-shrink-0">
                                <svg width="32" height="32" viewbox="0 0 32 32" fill="none" xmlns="http://www.w3.org/2000/svg">
<path opacity="0.4" d="M23.9728 18.0388C23.4184 18.58 23.1016 19.3588 23.1808 20.1904C23.2996 21.616 24.6064 22.6588 26.032 22.6588H28.54V24.2296C28.54 26.962 26.3092 29.1928 23.5768 29.1928H8.42316C5.69076 29.1928 3.45996 26.962 3.45996 24.2296V15.346C3.45996 12.6136 5.69076 10.3828 8.42316 10.3828H23.5768C26.3092 10.3828 28.54 12.6136 28.54 15.346V17.2468H25.8736C25.1344 17.2468 24.4612 17.5372 23.9728 18.0388Z" fill="white"/>
<path d="M19.762 5.36844V10.3844H8.42316C5.69076 10.3844 3.45996 12.6152 3.45996 15.3476V10.5033C3.45996 8.93246 4.42356 7.5332 5.88876 6.9788L16.3696 3.0188C18.0064 2.4116 19.762 3.61284 19.762 5.36844Z" fill="white"/>
<path d="M29.9394 18.5963V21.3156C29.9394 22.0416 29.3586 22.6356 28.6194 22.662H26.0322C24.6066 22.662 23.2998 21.6192 23.181 20.1936C23.1018 19.362 23.4186 18.5832 23.973 18.042C24.4614 17.5404 25.1346 17.25 25.8738 17.25H28.6194C29.3586 17.2764 29.9394 17.8703 29.9394 18.5963Z" fill="white"/>
<path d="M18.6402 16.9878H9.40016C8.85896 16.9878 8.41016 16.539 8.41016 15.9978C8.41016 15.4566 8.85896 15.0078 9.40016 15.0078H18.6402C19.1814 15.0078 19.6302 15.4566 19.6302 15.9978C19.6302 16.539 19.1814 16.9878 18.6402 16.9878Z" fill="white"/>
</svg>
                            </div>
                            <div class="doctor-content dash-count flex-grow-1">
                                <h4>
                                    &#8377;<asp:Label ID="lblEarnings" runat="server" CssClass="counter-up"></asp:Label>
                                    <span class="status-green">
                                        <asp:Label ID="lblTodayEarnings" runat="server"></asp:Label>
                                        /Today</span>
                                </h4>
                                <h5>
                                    Earnings</h5>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <%-- This doctor's appointments, last 14 days (one measure, one colour, value on each bar) --%>
            <div class="card" style="margin-top: 20px;">
                <div class="card-body">
                    <div class="chart-title"><h4>Your Appointments, Last 14 Days</h4></div>
                    <p class="doc-chart-note">Booked appointments per day (cancelled not counted).</p>
                    <div style="position: relative; height: 240px;"><canvas id="doctor14days" aria-label="Your appointments in the last 14 days" role="img"></canvas></div>
                </div>
            </div>
        </div>
    </div>
    <style>.doc-chart-note { font-size: 12px; color: #52514e; margin: -6px 0 8px; }</style>
    <script src="assets/plugins/chartjs/chart.min.js"></script>
    <script>
        function drawDoctor14Days(labels, values) {
            var canvas = document.getElementById('doctor14days');
            if (!canvas || typeof Chart === 'undefined') return;
            var ink = function () { return window.DocCareTheme ? DocCareTheme.ink() : '#52514e'; };   // follows light / dark mode
            new Chart(canvas, {
                type: 'bar',
                data: { labels: labels, datasets: [{ data: values, backgroundColor: '#2a78d6', borderRadius: 4, maxBarThickness: 28 }] },
                options: {
                    responsive: true, maintainAspectRatio: false, animation: false,
                    plugins: { legend: { display: false },
                               tooltip: { callbacks: { label: function (c) { return ' ' + c.parsed.y + (c.parsed.y === 1 ? ' appointment' : ' appointments'); } } } },
                    scales: {
                        x: { ticks: { color: ink(), maxRotation: 0, autoSkip: true }, grid: { display: false, drawBorder: false } },
                        y: { beginAtZero: true, grace: '12%', ticks: { precision: 0, color: ink() }, grid: { color: window.DocCareTheme ? DocCareTheme.grid() : '#e8e7e3', drawBorder: false } }
                    }
                },
                plugins: [{
                    id: 'values',
                    afterDatasetsDraw: function (chart) {
                        var ctx = chart.ctx; ctx.save(); ctx.fillStyle = ink(); ctx.font = '600 12px Poppins, sans-serif';
                        ctx.textAlign = 'center'; ctx.textBaseline = 'bottom';
                        chart.getDatasetMeta(0).data.forEach(function (bar, i) { var v = values[i]; if (v) ctx.fillText(v, bar.x, bar.y - 4); });
                        ctx.restore();
                    }
                }]
            });
        }
    </script>
</asp:Content>
