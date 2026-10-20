<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true"
    CodeBehind="DoctorProfile.aspx.cs" Inherits="DocCare_Clinic.DoctorProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .profile-user-img
        {
            width: 140px;
            height: 140px;
            border-radius: 50%;
            overflow: hidden;
            border: 4px solid #fff;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }
        .doctor-image
        {
            width: 100% !important;
            height: 100%;
            object-fit: cover;
            border-radius: 50%;
        }
    </style>
    <div class="page-wrapper">
        <div class="content">
            <div class="page-header">
                <div class="row">
                    <div class="col-sm-12">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item">
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Features/Doctors/Doctors.aspx"><asp:Label ID="lblDashBoard" runat="server" ></asp:Label></asp:HyperLink>
                            </li>
                            <li class="breadcrumb-item"><i class="feather-chevron-right"></i></li>
                            <li class="breadcrumb-item active">
                                <asp:Label ID="lblBreadcrumb" runat="server" Text="Doctor Profile"></asp:Label></li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="about-info">
                                        <h4>
                                            Doctor Profile</h4>
                                    </div>
                                    <div class="doctor-profile-head">
                                        <div class="profile-bg-img">
                                            <img src="assets/img/profile-bg.jpg" alt="Profile" />
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-4 col-md-4">
                                                <div class="profile-user-box">
                                                    <div class="profile-user-img">
                                                        <asp:Image ID="imgDoctor" runat="server" CssClass="doctor-image" AlternateText="Doctor Profile Image" />
                                                    </div>
                                                    <div class="names-profiles">
                                                        <h4>
                                                            Dr. <asp:Label ID="lblDoctorName" runat="server"></asp:Label></h4>
                                                        <h5>
                                                            <asp:Label ID="lblDesignation1" runat="server"></asp:Label></h5>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-4">
                            <!-- About Me Section -->
                            <div class="doctor-personals-grp">
                                <div class="card">
                                    <div class="card-body">
                                        <div class="heading-detail">
                                            <h4 class="mb-3">
                                                About me</h4>
                                            <p>
                                                <asp:Label ID="lblAboutMe" runat="server"></asp:Label></p>
                                        </div>
                                        <div class="about-me-list">
                                            <ul class="list-space">
                                                <li>
                                                    <h4>
                                                        Gender</h4>
                                                    <span>
                                                        <asp:Label ID="lblGender" runat="server"></asp:Label></span> </li>
                                                <li>
                                                    <h4>
                                                        Designation</h4>
                                                    <span>
                                                        <asp:Label ID="lblDesignation" runat="server"></asp:Label></span> </li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- Speciality Section -->
                            <div class="doctor-personals-grp">
                                <div class="card">
                                    <div class="card-body">
                                        <div class="heading-detail">
                                            <h4>
                                                Speciality</h4>
                                        </div>
                                        <div class="personal-activity">
                                            <div class="personal-icons status-grey">
                                                <svg width="24" height="24" viewbox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                            <path opacity="0.4" d="M19 9C19 10.45 18.57 11.78 17.83 12.89C16.75 14.49 15.04 15.62 13.05 15.91C12.71 15.97 12.36 16 12 16C11.64 16 11.29 15.97 10.95 15.91C8.96 15.62 7.25 14.49 6.17 12.89C5.43 11.78 5 10.45 5 9C5 5.13 8.13 2 12 2C15.87 2 19 5.13 19 9Z" fill="#2E37A4"/>
                                                            <path d="M21.2501 18.4684L19.6001 18.8584C19.2301 18.9484 18.9401 19.2284 18.8601 19.5984L18.5101 21.0685C18.3201 21.8685 17.3001 22.1085 16.7701 21.4785L12.0001 15.9984L7.23008 21.4884C6.70008 22.1184 5.68008 21.8784 5.49008 21.0784L5.14008 19.6084C5.05008 19.2384 4.76008 18.9484 4.40008 18.8684L2.75008 18.4785C1.99008 18.2985 1.72008 17.3484 2.27008 16.7984L6.17008 12.8984C7.25008 14.4984 8.96008 15.6285 10.9501 15.9185C11.2901 15.9785 11.6401 16.0085 12.0001 16.0085C12.3601 16.0085 12.7101 15.9785 13.0501 15.9185C15.0401 15.6285 16.7501 14.4984 17.8301 12.8984L21.7301 16.7984C22.2801 17.3384 22.0101 18.2884 21.2501 18.4684Z" fill="#2E37A4"/>
                                                            <path d="M12.58 5.98L13.17 7.15999C13.25 7.31999 13.46 7.48 13.65 7.51L14.72 7.68999C15.4 7.79999 15.56 8.3 15.07 8.79L14.24 9.61998C14.1 9.75998 14.02 10.03 14.07 10.23L14.31 11.26C14.5 12.07 14.07 12.39 13.35 11.96L12.35 11.37C12.17 11.26 11.87 11.26 11.69 11.37L10.69 11.96C9.96997 12.38 9.53997 12.07 9.72997 11.26L9.96997 10.23C10.01 10.04 9.93997 9.75998 9.79997 9.61998L8.96997 8.79C8.47997 8.3 8.63997 7.80999 9.31997 7.68999L10.39 7.51C10.57 7.48 10.78 7.31999 10.86 7.15999L11.45 5.98C11.74 5.34 12.26 5.34 12.58 5.98Z" fill="#2E37A4"/>
                                                            </svg>
                                            </div>
                                            <div class="views-personal">
                                                <h4>
                                                    <asp:Label ID="lblSpecialization" runat="server"></asp:Label></h4>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!-- Main Profile Section -->
                        <div class="col-lg-8">
                            <div class="doctor-personals-grp">
                                <div class="card">
                                    <div class="card-body">
                                        <!-- Tabs Navigation -->
                                        <div class="tab-content-set">
                                            <ul class="nav">
                                                <li>
                                                    <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="#">
                                                                    <span class="set-about-icon me-2">
                                                                       <svg width="25" height="24" viewBox="0 0 25 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                                        <path d="M12.5667 2C9.94665 2 7.81665 4.13 7.81665 6.75C7.81665 9.32 9.82665 11.4 12.4467 11.49C12.5267 11.48 12.6067 11.48 12.6667 11.49C12.6867 11.49 12.6967 11.49 12.7167 11.49C12.7267 11.49 12.7267 11.49 12.7367 11.49C15.2967 11.4 17.3067 9.32 17.3167 6.75C17.3167 4.13 15.1867 2 12.5667 2Z" fill="#333548" fill-opacity="0.75"/>
                                                                        <path opacity="0.4" d="M17.6466 14.1528C14.8566 12.2928 10.3066 12.2928 7.49661 14.1528C6.22661 15.0028 5.52661 16.1528 5.52661 17.3828C5.52661 18.6128 6.22661 19.7528 7.48661 20.5928C8.88661 21.5328 10.7266 22.0028 12.5666 22.0028C14.4066 22.0028 16.2466 21.5328 17.6466 20.5928C18.9066 19.7428 19.6066 18.6028 19.6066 17.3628C19.5966 16.1328 18.9066 14.9928 17.6466 14.1528Z" fill="#333548" fill-opacity="0.75"/>
                                                                        <path d="M14.5667 16.6328H13.3167V15.3828C13.3167 14.9728 12.9767 14.6328 12.5667 14.6328C12.1567 14.6328 11.8167 14.9728 11.8167 15.3828V16.6328H10.5667C10.1567 16.6328 9.81665 16.9728 9.81665 17.3828C9.81665 17.7928 10.1567 18.1328 10.5667 18.1328H11.8167V19.3828C11.8167 19.7928 12.1567 20.1328 12.5667 20.1328C12.9767 20.1328 13.3167 19.7928 13.3167 19.3828V18.1328H14.5667C14.9767 18.1328 15.3167 17.7928 15.3167 17.3828C15.3167 16.9728 14.9767 16.6328 14.5667 16.6328Z" fill="#333548" fill-opacity="0.75"/>
                                                                        </svg>
                                                                    </span>About me
                                                    </asp:HyperLink>
                                                </li>
                                            </ul>
                                        </div>
                                        <!-- Profile Details -->
                                        <div class="personal-list-out">
                                            <div class="row">
                                                <div class="col-xl-3 col-md-6">
                                                    <div class="detail-personal">
                                                        <h2>
                                                            Full Name</h2>
                                                        <h3>
                                                            <asp:Label ID="lblFullName" runat="server"></asp:Label></h3>
                                                    </div>
                                                </div>
                                                <div class="col-xl-3 col-md-6">
                                                    <div class="detail-personal">
                                                        <h2>
                                                            Mobile</h2>
                                                        <h3>
                                                            <asp:Label ID="lblMobile" runat="server"></asp:Label></h3>
                                                    </div>
                                                </div>
                                                <div class="col-xl-3 col-md-6">
                                                    <div class="detail-personal">
                                                        <h2>
                                                            Email</h2>
                                                        <h3>
                                                            <a id="emailLink" runat="server" href="#">
                                                                <asp:Label ID="lblEmail" runat="server"></asp:Label>
                                                            </a>
                                                        </h3>
                                                    </div>
                                                </div>
                                                <div class="col-xl-3 col-md-6">
                                                    <div class="detail-personal">
                                                        <h2>
                                                            Address</h2>
                                                        <h3>
                                                            <asp:Label ID="lblLocation" runat="server"></asp:Label></h3>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- Bio -->
                                        <div class="hello-park">
                                            <p>
                                                <asp:Label ID="lblDoctorBio" runat="server"></asp:Label></p>
                                        </div>
                                        <!-- Education -->
                                        <div class="hello-park mb-2">
                                            <h5>
                                                Education</h5>
                                            <div class="table-responsive">
                                                <table class="table mb-0 border-0 custom-table profile-table">
                                                    <thead>
                                                        <tr>
                                                            <th class="col-xl-4">
                                                                Year Of Completion
                                                            </th>
                                                            <th class="col-xl-3">
                                                                Degree
                                                            </th>
                                                            <th>
                                                                Institute
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td>
                                                                <span>
                                                                    <asp:Label ID="lblEducation" runat="server"></asp:Label></span>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblDegree" runat="server"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <span>
                                                                    <asp:Label ID="lblInstitute" runat="server"></asp:Label></span>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <!-- Experience -->
                                        <div class="hello-park mb-2">
                                            <h5>
                                                Experience</h5>
                                            <div class="table-responsive">
                                                <table class="table mb-0 border-0 custom-table profile-table">
                                                    <thead>
                                                        <tr>
                                                            <th>
                                                                Year
                                                            </th>
                                                            <th>
                                                                Position
                                                            </th>
                                                            <th>
                                                                Hospital
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td>
                                                                <span>
                                                                    <asp:Label ID="lblExperience" runat="server"></asp:Label></span>
                                                            </td>
                                                            <td>
                                                                <span>
                                                                    <asp:Label ID="lblDesignation2" runat="server"></asp:Label></span>
                                                            </td>
                                                            <td>
                                                                <span>
                                                                    <asp:Label ID="lblHospital" runat="server"></asp:Label></span>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="notification-box">
            <div class="msg-sidebar notifications msg-noti">
                <div class="topnav-dropdown-header">
                    <span>Messages</span>
                </div>
                <div class="drop-scroll msg-list-scroll" id="msg_list">
                    <ul class="list-box">
                        <li><a href="javascript:void(0);">
                            <div class="list-item">
                                <div class="list-left">
                                    <span class="avatar">R</span>
                                </div>
                                <div class="list-body">
                                    <span class="message-author">Richard Miles</span> <span class="message-time">12:28 AM</span>
                                    <span class="message-content">Lorem ipsum dolor sit amet, consectetur adipiscing</span>
                                </div>
                            </div>
                        </a></li>
                        <!-- Add similar messages as needed -->
                    </ul>
                </div>
                <div class="topnav-dropdown-footer">
                    <a href="javascript:void(0);">See all messages</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
