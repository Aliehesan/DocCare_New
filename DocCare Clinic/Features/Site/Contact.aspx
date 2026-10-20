<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Shared/DocCare.Master" AutoEventWireup="true"
    CodeBehind="Contact.aspx.cs" Inherits="DocCare_Clinic.Contact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .contact-section {
            padding: 40px 8%;
            margin-bottom: 60px;
            display: flex;
            gap: 50px;
            align-items: stretch;
        }

        .contact-image {
            flex: 1;
            position: relative;
        }

        .contact-image img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            border-radius: 30px;
            box-shadow: 0 20px 40px rgba(46, 55, 164, 0.1);
        }

        .contact-info-wrapper {
            flex: 1;
            background: #ffffff;
            padding: 40px;
            border-radius: 30px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.02);
            border: 1px solid #f3f4f6;
        }

        .contact-info-wrapper h6 {
            color: var(--primary);
            font-weight: 600;
            font-size: 16px;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin-bottom: 10px;
        }

        .contact-info-wrapper h2 {
            font-size: 36px;
            font-weight: 700;
            color: #1f2937;
            margin-bottom: 30px;
        }

        .info-card {
            display: flex;
            align-items: flex-start;
            gap: 20px;
            margin-bottom: 25px;
            padding-bottom: 25px;
            border-bottom: 1px solid #f3f4f6;
        }

        .info-card:last-child {
            border-bottom: none;
            margin-bottom: 0;
            padding-bottom: 0;
        }

        .info-icon {
            width: 50px;
            height: 50px;
            background: rgba(46, 55, 164, 0.1);
            color: var(--primary);
            border-radius: 15px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            flex-shrink: 0;
        }

        .info-details h4 {
            font-size: 18px;
            font-weight: 600;
            color: #1f2937;
            margin-bottom: 5px;
        }

        .info-details p {
            color: #6b7280;
            font-size: 15px;
            margin: 0;
            line-height: 1.6;
        }

        .social-links {
            display: flex;
            gap: 15px;
            margin-top: 30px;
        }

        .social-links a {
            width: 45px;
            height: 45px;
            background: var(--bg-light);
            color: var(--primary);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            transition: all 0.3s ease;
            text-decoration: none;
        }

        .social-links a:hover {
            background: var(--primary);
            color: #ffffff;
            transform: translateY(-3px);
        }

        @media (max-width: 991px) {
            .contact-section {
                flex-direction: column;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- breadcrumb start -->
    <div class="breadcrumb-area text-center">
        <h2 class="page-title">Contact Us</h2>
    </div>
    <!-- breadcrumb end -->

    <div class="contact-section">
        <div class="contact-image">
            <img src="assets_web/img/about/9.png" alt="Contact DocCare" onerror="this.src='assets_web/img/banner/1.png'">
        </div>
        
        <div class="contact-info-wrapper">
            <h6>Get In Touch</h6>
            <h2>Talk With Our Expert Doctors</h2>
            
            <div class="info-card">
                <div class="info-icon"><i class="fa-solid fa-phone"></i></div>
                <div class="info-details">
                    <h4>Call Us</h4>
                    <p>+91 95585 22473</p>
                    <p>+91 95585 22473</p>
                </div>
            </div>

            <div class="info-card">
                <div class="info-icon"><i class="fa-solid fa-envelope"></i></div>
                <div class="info-details">
                    <h4>Mail Us</h4>
                    <p>support@doccare.com</p>
                </div>
            </div>

            <div class="info-card">
                <div class="info-icon"><i class="fa-solid fa-location-dot"></i></div>
                <div class="info-details">
                    <h4>Location</h4>
                    <p>Ahmedabad, Gujarat, India</p>
                </div>
            </div>

            <div class="social-links">
                <a href="https://www.facebook.com/share/1BTFJdzGpV/?mibextid=qi2Omg"><i class="fa-brands fa-facebook-f"></i></a>
                <a href="https://x.com/DocCare2025?t=msVLwbeOVf5J-tSXtQd1wQ&s=08"><i class="fa-brands fa-x-twitter"></i></a>
                <a href="https://www.linkedin.com/in/doccare-support-117309358"><i class="fa-brands fa-linkedin-in"></i></a>
            </div>
        </div>
    </div>
</asp:Content>
