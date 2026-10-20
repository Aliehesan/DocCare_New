<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Admin/SuperAdmin.Master" AutoEventWireup="true" CodeBehind="DoctorSetting.aspx.cs" Inherits="DocCare_Clinic.DoctorSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="page-wrapper">
                    <div class="content">
                        <div class="page-header">
                            <div class="row">
                                <div class="col-sm-12">
                                    <ul class="breadcrumb">
                                        <li class="breadcrumb-item">
                                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Features/Doctors/Doctors.aspx">Doctors</asp:HyperLink>
                                        </li>
                                        <li class="breadcrumb-item">
                                            <i class="feather-chevron-right"></i>
                                        </li>
                                        <li class="breadcrumb-item active">
                                            Doctors Profile
                                        </li>
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
                                                    <h4>Doctor Profile <span><asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="javascript:;">
                                                        <i class="feather-more-vertical"></i></asp:HyperLink></span></h4>
                                                </div>
                                                <div class="doctor-profile-head">
                                                    <div class="profile-bg-img">
                                                        <img src="assets/img/profile-bg.jpg" alt="Profile" />
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-lg-4 col-md-4">
                                                            <div class="profile-user-box">
                                                                <div class="profile-user-img">
                                                                    <img src="assets/img/profile-user.jpg" alt="Profile" />
                                                                    <div class="input-block doctor-up-files profile-edit-icon mb-0">
                                                                        <div class="uplod d-flex">
                                                                            <label class="file-upload profile-upbtn mb-0">
                                                                                <svg width="25" height="25" viewBox="0 0 25 25" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                                                <path d="M5.64748 25H18.7475C22.1975 25 23.5725 22.8875 23.735 20.3125L24.385 9.9875C24.56 7.2875 22.41 5 19.6975 5C18.935 5 18.235 4.5625 17.885 3.8875L16.985 2.075C16.41 0.9375 14.91 0 13.635 0H10.7725C9.48497 0 7.98497 0.9375 7.40998 2.075L6.50997 3.8875C6.15997 4.5625 5.45998 5 4.69747 5C1.98498 5 -0.165025 7.2875 0.00997505 9.9875L0.659975 20.3125C0.809975 22.8875 2.19748 25 5.64748 25Z" fill="#333448"/>
                                                                                <path d="M14.0723 8.4375H10.3223C9.80977 8.4375 9.38477 8.0125 9.38477 7.5C9.38477 6.9875 9.80977 6.5625 10.3223 6.5625H14.0723C14.5848 6.5625 15.0098 6.9875 15.0098 7.5C15.0098 8.0125 14.5848 8.4375 14.0723 8.4375Z" fill="white"/>
                                                                                <path d="M12.2016 20.1609C14.535 20.1609 16.4266 18.2693 16.4266 15.9359C16.4266 13.6025 14.535 11.7109 12.2016 11.7109C9.86816 11.7109 7.97656 13.6025 7.97656 15.9359C7.97656 18.2693 9.86816 20.1609 12.2016 20.1609Z" fill="white"/>
                                                                                </svg>
                                                                                <asp:FileUpload ID="fileUpload" runat="server" />
                                                                            </label>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="names-profiles">
                                                                    <h4>Dr. Bruce Willis</h4>
                                                                    <h5>Senior Doctor</h5>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-4 col-md-4 d-flex align-items-center">
                                                            <div class="follow-group">
                                                                <div class="doctor-follows">
                                                                    <h5>Followers</h5>
                                                                    <h4>850</h4>
                                                                </div>
                                                                <div class="doctor-follows">
                                                                    <h5>Following</h5>
                                                                    <h4>18K</h4>
                                                                </div>
                                                                <div class="doctor-follows">
                                                                    <h5>Posts</h5>
                                                                    <h4>250</h4>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-4 col-md-4 d-flex align-items-center">
                                                            <div class="follow-btn-group">
                                                                <asp:Button ID="btnFollow" runat="server" Text="Follow" CssClass="btn btn-info follow-btns" />
                                                                <asp:Button ID="btnMessage" runat="server" Text="Message" CssClass="btn btn-info message-btns" />
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
                                                        <h4 class="mb-3">About me</h4>
                                                        <p>Hello I am Dr. Bruce Willis a Gynaecologist in Sanjivni Hospital Surat. I love to work with all my hospital staff and senior doctors.</p>
                                                    </div>
                                                    <div class="about-me-list">
                                                        <ul class="list-space">
                                                            <li>
                                                                <h4>Gender</h4>
                                                                <span>Female</span>
                                                            </li>
                                                            <li>
                                                                <h4>Operation Done</h4>
                                                                <span>30+</span>
                                                            </li>
                                                            <li>
                                                                <h4>Designation</h4>
                                                                <span>Sr. Doctor</span>
                                                            </li>
                                                        </ul>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Skills Section -->
                                        <div class="doctor-personals-grp">
                                            <div class="card">
                                                <div class="card-body">
                                                    <div class="heading-detail">
                                                        <h4>Skills:</h4>
                                                    </div>
                                                    <div class="skill-blk">
                                                        <div class="skill-statistics">
                                                            <div class="skills-head">
                                                                <h5>Operations</h5>
                                                                <p>45%</p>
                                                            </div>
                                                            <div class="progress mb-0">
                                                                <div class="progress-bar bg-operations" role="progressbar" style="width: 45%" aria-valuenow="45" aria-valuemin="0" aria-valuemax="100"></div>
                                                            </div>
                                                        </div>
                                                        <div class="skill-statistics">
                                                            <div class="skills-head">
                                                                <h5>Patient Care</h5>
                                                                <p>85%</p>
                                                            </div>
                                                            <div class="progress mb-0">
                                                                <div class="progress-bar bg-statistics" role="progressbar" style="width: 85%" aria-valuenow="85" aria-valuemin="0" aria-valuemax="100"></div>
                                                            </div>
                                                        </div>
                                                        <div class="skill-statistics">
                                                            <div class="skills-head">
                                                                <h5>Endoscopic</h5>
                                                                <p>65%</p>
                                                            </div>
                                                            <div class="progress mb-0">
                                                                <div class="progress-bar bg-endoscopic" role="progressbar" style="width: 65%" aria-valuenow="65" aria-valuemin="0" aria-valuemax="100"></div>
                                                            </div>
                                                        </div>
                                                        <div class="skill-statistics">
                                                            <div class="skills-head">
                                                                <h5>Patient Visit</h5>
                                                                <p>90%</p>
                                                            </div>
                                                            <div class="progress mb-0">
                                                                <div class="progress-bar bg-visit" role="progressbar" style="width: 90%" aria-valuenow="90" aria-valuemin="0" aria-valuemax="100"></div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Speciality Section -->
                                        <div class="doctor-personals-grp">
                                            <div class="card">
                                                <div class="card-body">
                                                    <div class="heading-detail">
                                                        <h4>Speciality</h4>
                                                    </div>
                                                    <div class="personal-activity">
                                                        <div class="personal-icons status-grey">
                                                            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                            <path opacity="0.4" d="M19 9C19 10.45 18.57 11.78 17.83 12.89C16.75 14.49 15.04 15.62 13.05 15.91C12.71 15.97 12.36 16 12 16C11.64 16 11.29 15.97 10.95 15.91C8.96 15.62 7.25 14.49 6.17 12.89C5.43 11.78 5 10.45 5 9C5 5.13 8.13 2 12 2C15.87 2 19 5.13 19 9Z" fill="#2E37A4"/>
                                                            <path d="M21.2501 18.4684L19.6001 18.8584C19.2301 18.9484 18.9401 19.2284 18.8601 19.5984L18.5101 21.0685C18.3201 21.8685 17.3001 22.1085 16.7701 21.4785L12.0001 15.9984L7.23008 21.4884C6.70008 22.1184 5.68008 21.8784 5.49008 21.0784L5.14008 19.6084C5.05008 19.2384 4.76008 18.9484 4.40008 18.8684L2.75008 18.4785C1.99008 18.2985 1.72008 17.3484 2.27008 16.7984L6.17008 12.8984C7.25008 14.4984 8.96008 15.6285 10.9501 15.9185C11.2901 15.9785 11.6401 16.0085 12.0001 16.0085C12.3601 16.0085 12.7101 15.9785 13.0501 15.9185C15.0401 15.6285 16.7501 14.4984 17.8301 12.8984L21.7301 16.7984C22.2801 17.3384 22.0101 18.2884 21.2501 18.4684Z" fill="#2E37A4"/>
                                                            <path d="M12.58 5.98L13.17 7.15999C13.25 7.31999 13.46 7.48 13.65 7.51L14.72 7.68999C15.4 7.79999 15.56 8.3 15.07 8.79L14.24 9.61998C14.1 9.75998 14.02 10.03 14.07 10.23L14.31 11.26C14.5 12.07 14.07 12.39 13.35 11.96L12.35 11.37C12.17 11.26 11.87 11.26 11.69 11.37L10.69 11.96C9.96997 12.38 9.53997 12.07 9.72997 11.26L9.96997 10.23C10.01 10.04 9.93997 9.75998 9.79997 9.61998L8.96997 8.79C8.47997 8.3 8.63997 7.80999 9.31997 7.68999L10.39 7.51C10.57 7.48 10.78 7.31999 10.86 7.15999L11.45 5.98C11.74 5.34 12.26 5.34 12.58 5.98Z" fill="#2E37A4"/>
                                                            </svg>

                                                        </div>
                                                        <div class="views-personal">
                                                            <h4>Professionals</h4>
                                                            <h5>Certified Skin Treatment</h5>
                                                        </div>
                                                    </div>
                                                    <div class="personal-activity">
                                                        <div class="personal-icons status-green">
                                                            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                            <path opacity="0.4" d="M19 9C19 10.45 18.57 11.78 17.83 12.89C16.75 14.49 15.04 15.62 13.05 15.91C12.71 15.97 12.36 16 12 16C11.64 16 11.29 15.97 10.95 15.91C8.96 15.62 7.25 14.49 6.17 12.89C5.43 11.78 5 10.45 5 9C5 5.13 8.13 2 12 2C15.87 2 19 5.13 19 9Z" fill="#00D3C7"/>
                                                            <path d="M21.2501 18.4684L19.6001 18.8584C19.2301 18.9484 18.9401 19.2284 18.8601 19.5984L18.5101 21.0685C18.3201 21.8685 17.3001 22.1085 16.7701 21.4785L12.0001 15.9984L7.23008 21.4884C6.70008 22.1184 5.68008 21.8784 5.49008 21.0784L5.14008 19.6084C5.05008 19.2384 4.76008 18.9484 4.40008 18.8684L2.75008 18.4785C1.99008 18.2985 1.72008 17.3484 2.27008 16.7984L6.17008 12.8984C7.25008 14.4984 8.96008 15.6285 10.9501 15.9185C11.2901 15.9785 11.6401 16.0085 12.0001 16.0085C12.3601 16.0085 12.7101 15.9785 13.0501 15.9185C15.0401 15.6285 16.7501 14.4984 17.8301 12.8984L21.7301 16.7984C22.2801 17.3384 22.0101 18.2884 21.2501 18.4684Z" fill="#00D3C7"/>
                                                            <path d="M12.58 5.98L13.17 7.15999C13.25 7.31999 13.46 7.48 13.65 7.51L14.72 7.68999C15.4 7.79999 15.56 8.3 15.07 8.79L14.24 9.61998C14.1 9.75998 14.02 10.03 14.07 10.23L14.31 11.26C14.5 12.07 14.07 12.39 13.35 11.96L12.35 11.37C12.17 11.26 11.87 11.26 11.69 11.37L10.69 11.96C9.96997 12.38 9.53997 12.07 9.72997 11.26L9.96997 10.23C10.01 10.04 9.93997 9.75998 9.79997 9.61998L8.96997 8.79C8.47997 8.3 8.63997 7.80999 9.31997 7.68999L10.39 7.51C10.57 7.48 10.78 7.31999 10.86 7.15999L11.45 5.98C11.74 5.34 12.26 5.34 12.58 5.98Z" fill="#00D3C7"/>
                                                            </svg>

                                                        </div>
                                                        <div class="views-personal">
                                                            <h4>Certified</h4>
                                                            <h5>Cold Laser Operation</h5>
                                                        </div>
                                                    </div>
                                                    <div class="personal-activity mb-0">
                                                        <div class="personal-icons status-orange">
                                                            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                            <path opacity="0.4" d="M19 9C19 10.45 18.57 11.78 17.83 12.89C16.75 14.49 15.04 15.62 13.05 15.91C12.71 15.97 12.36 16 12 16C11.64 16 11.29 15.97 10.95 15.91C8.96 15.62 7.25 14.49 6.17 12.89C5.43 11.78 5 10.45 5 9C5 5.13 8.13 2 12 2C15.87 2 19 5.13 19 9Z" fill="#FFB627"/>
                                                            <path d="M21.2501 18.4684L19.6001 18.8584C19.2301 18.9484 18.9401 19.2284 18.8601 19.5984L18.5101 21.0685C18.3201 21.8685 17.3001 22.1085 16.7701 21.4785L12.0001 15.9984L7.23008 21.4884C6.70008 22.1184 5.68008 21.8784 5.49008 21.0784L5.14008 19.6084C5.05008 19.2384 4.76008 18.9484 4.40008 18.8684L2.75008 18.4785C1.99008 18.2985 1.72008 17.3484 2.27008 16.7984L6.17008 12.8984C7.25008 14.4984 8.96008 15.6285 10.9501 15.9185C11.2901 15.9785 11.6401 16.0085 12.0001 16.0085C12.3601 16.0085 12.7101 15.9785 13.0501 15.9185C15.0401 15.6285 16.7501 14.4984 17.8301 12.8984L21.7301 16.7984C22.2801 17.3384 22.0101 18.2884 21.2501 18.4684Z" fill="#FFB627"/>
                                                            <path d="M12.58 5.98L13.17 7.15999C13.25 7.31999 13.46 7.48 13.65 7.51L14.72 7.68999C15.4 7.79999 15.56 8.3 15.07 8.79L14.24 9.61998C14.1 9.75998 14.02 10.03 14.07 10.23L14.31 11.26C14.5 12.07 14.07 12.39 13.35 11.96L12.35 11.37C12.17 11.26 11.87 11.26 11.69 11.37L10.69 11.96C9.96997 12.38 9.53997 12.07 9.72997 11.26L9.96997 10.23C10.01 10.04 9.93997 9.75998 9.79997 9.61998L8.96997 8.79C8.47997 8.3 8.63997 7.80999 9.31997 7.68999L10.39 7.51C10.57 7.48 10.78 7.31999 10.86 7.15999L11.45 5.98C11.74 5.34 12.26 5.34 12.58 5.98Z" fill="#FFB627"/>
                                                            </svg>

                                                        </div>
                                                        <div class="views-personal">
                                                            <h4>Medication Laser</h4>
                                                            <h5>Hair Loss Product</h5>
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
                                                            <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/Features/Doctors/DoctorProfile.aspx">
                                                                    <span class="set-about-icon me-2">
                                                                       <svg width="25" height="24" viewBox="0 0 25 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                                        <path d="M12.5667 2C9.94665 2 7.81665 4.13 7.81665 6.75C7.81665 9.32 9.82665 11.4 12.4467 11.49C12.5267 11.48 12.6067 11.48 12.6667 11.49C12.6867 11.49 12.6967 11.49 12.7167 11.49C12.7267 11.49 12.7267 11.49 12.7367 11.49C15.2967 11.4 17.3067 9.32 17.3167 6.75C17.3167 4.13 15.1867 2 12.5667 2Z" fill="#333548" fill-opacity="0.75"/>
                                                                        <path opacity="0.4" d="M17.6466 14.1528C14.8566 12.2928 10.3066 12.2928 7.49661 14.1528C6.22661 15.0028 5.52661 16.1528 5.52661 17.3828C5.52661 18.6128 6.22661 19.7528 7.48661 20.5928C8.88661 21.5328 10.7266 22.0028 12.5666 22.0028C14.4066 22.0028 16.2466 21.5328 17.6466 20.5928C18.9066 19.7428 19.6066 18.6028 19.6066 17.3628C19.5966 16.1328 18.9066 14.9928 17.6466 14.1528Z" fill="#333548" fill-opacity="0.75"/>
                                                                        <path d="M14.5667 16.6328H13.3167V15.3828C13.3167 14.9728 12.9767 14.6328 12.5667 14.6328C12.1567 14.6328 11.8167 14.9728 11.8167 15.3828V16.6328H10.5667C10.1567 16.6328 9.81665 16.9728 9.81665 17.3828C9.81665 17.7928 10.1567 18.1328 10.5667 18.1328H11.8167V19.3828C11.8167 19.7928 12.1567 20.1328 12.5667 20.1328C12.9767 20.1328 13.3167 19.7928 13.3167 19.3828V18.1328H14.5667C14.9767 18.1328 15.3167 17.7928 15.3167 17.3828C15.3167 16.9728 14.9767 16.6328 14.5667 16.6328Z" fill="#333548" fill-opacity="0.75"/>
                                                                        </svg>
                                                                    </span>About me
                                                                </asp:HyperLink>                                                               
                                                            </li>
                                                            <li>
                                                                <asp:HyperLink ID="hlSettings" runat="server" NavigateUrl="~/Features/Doctors/DoctorSetting.aspx" CssClass="active">
                                                                    <span class="set-about-icon me-2">
                                                                        <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                                        <path opacity="0.4" d="M1.6665 10.7319V9.26523C1.6665 8.39856 2.37484 7.6819 3.24984 7.6819C4.75817 7.6819 5.37484 6.61523 4.6165 5.3069C4.18317 4.5569 4.4415 3.5819 5.19984 3.14856L6.6415 2.32356C7.29984 1.9319 8.14984 2.16523 8.5415 2.82356L8.63317 2.9819C9.38317 4.29023 10.6165 4.29023 11.3748 2.9819L11.4665 2.82356C11.8582 2.16523 12.7082 1.9319 13.3665 2.32356L14.8082 3.14856C15.5665 3.5819 15.8248 4.5569 15.3915 5.3069C14.6332 6.61523 15.2498 7.6819 16.7582 7.6819C17.6248 7.6819 18.3415 8.39023 18.3415 9.26523V10.7319C18.3415 11.5986 17.6332 12.3152 16.7582 12.3152C15.2498 12.3152 14.6332 13.3819 15.3915 14.6902C15.8248 15.4486 15.5665 16.4152 14.8082 16.8486L13.3665 17.6736C12.7082 18.0652 11.8582 17.8319 11.4665 17.1736L11.3748 17.0152C10.6248 15.7069 9.3915 15.7069 8.63317 17.0152L8.5415 17.1736C8.14984 17.8319 7.29984 18.0652 6.6415 17.6736L5.19984 16.8486C4.4415 16.4152 4.18317 15.4402 4.6165 14.6902C5.37484 13.3819 4.75817 12.3152 3.24984 12.3152C2.37484 12.3152 1.6665 11.5986 1.6665 10.7319Z" fill="#333548" fill-opacity="0.75"/>
                                                                        <path d="M9.99984 12.7057C11.4956 12.7057 12.7082 11.4932 12.7082 9.9974C12.7082 8.50162 11.4956 7.28906 9.99984 7.28906C8.50407 7.28906 7.2915 8.50162 7.2915 9.9974C7.2915 11.4932 8.50407 12.7057 9.99984 12.7057Z" fill="#333548" fill-opacity="0.75"/>
                                                                        </svg>
                                                                    </span>
                                                                    Settings
                                                                </asp:HyperLink>
                                                            </li>
                                                        </ul>
                                                    </div>
                                                     <div class="setting-form-blk">
                                                        <div class="row">
                                                            <div class="col-12">
                                                                <div class="form-heading">
                                                                    <h4>Account Setting</h4>
                                                                </div>
                                                            </div>
                                                            <div class="col-12 col-sm-12">
                                                                <div class="input-block local-forms">
                                                                    <label>Username</label>
                                                                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" Text="brucewillis@29"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-12 col-sm-12">
                                                                <div class="input-block local-forms">
                                                                    <label>Mobile</label>
                                                                    <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" Text="+1 23 456890"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-12 col-sm-12">
                                                                <div class="input-block local-forms">
                                                                    <label>Email</label>
                                                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Text="brucewillis@info.com" 
                                                                        TextMode="Email"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-12 col-sm-12">
                                                                <div class="input-block local-forms">
                                                                    <label>Address <span class="login-danger">*</span></label>
                                                                    <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" Text="101, Elanxa Apartments, 340 N Madison Avenue" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
            
                                                        <!-- Security Settings -->
                                                        <div class="col-12">
                                                            <div class="form-heading">
                                                                <h4>Security Setting</h4>
                                                            </div>
                                                        </div>
                                                        <div class="col-12 col-sm-12">
                                                            <div class="input-block local-forms">
                                                                <label>Current Password</label>
                                                                <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" Text="12345" TextMode="Password"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-12 col-sm-12">
                                                            <div class="input-block local-forms">
                                                                <label>New Password</label>
                                                                <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-12 col-sm-12">
                                                            <div class="input-block local-forms">
                                                                <label>Confirm Password</label>
                                                                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                                            </div>
                                                        </div>                       
                                                        <!-- Submit and Cancel buttons -->
                                                        <div class="col-12">
                                                            <div class="doctor-submit">
                                                                <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary submit-form me-2" />
                                                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-primary cancel-form" />
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
                    </div>
                     <div class="notification-box">
                            <div class="msg-sidebar notifications msg-noti">
                                <div class="topnav-dropdown-header">
                                    <span>Messages</span>
                                </div>
                                <div class="drop-scroll msg-list-scroll" id="msg_list">
                                    <ul class="list-box">
                                        <li>
                                            <a href="javascript:void(0);">
                                                <div class="list-item">
                                                    <div class="list-left">
                                                        <span class="avatar">R</span>
                                                    </div>
                                                    <div class="list-body">
                                                        <span class="message-author">Richard Miles</span>
                                                        <span class="message-time">12:28 AM</span>
                                                        <span class="message-content">Lorem ipsum dolor sit amet, consectetur adipiscing</span>
                                                    </div>
                                                </div>
                                            </a>
                                        </li>
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
