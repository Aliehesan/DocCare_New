<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Shared/DocCare.Master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="DocCare_Clinic.About" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .about-section {
            padding: 40px 8%;
            margin-bottom: 60px;
            display: flex;
            align-items: center;
            gap: 50px;
        }

        .about-image {
            flex: 1;
            position: relative;
        }

        .about-image img {
            width: 100%;
            border-radius: 30px;
            box-shadow: 0 20px 40px rgba(46, 55, 164, 0.1);
        }

        .about-content {
            flex: 1;
        }

        .about-content .sub-title {
            color: var(--primary);
            font-weight: 600;
            font-size: 16px;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin-bottom: 15px;
            display: block;
        }

        .about-content h2 {
            font-size: 42px;
            font-weight: 700;
            color: #1f2937;
            line-height: 1.2;
            margin-bottom: 25px;
        }

        .about-content p {
            font-size: 16px;
            color: #6b7280;
            margin-bottom: 30px;
            line-height: 1.8;
        }

        .feature-list {
            list-style: none;
            padding: 0;
            margin: 0;
        }

        .feature-list li {
            font-size: 16px;
            font-weight: 500;
            color: #374151;
            margin-bottom: 15px;
            display: flex;
            align-items: center;
            gap: 15px;
        }

        .feature-list li i {
            color: var(--primary);
            font-size: 20px;
            background: rgba(46, 55, 164, 0.1);
            width: 40px;
            height: 40px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
        }

        @media (max-width: 991px) {
            .about-section {
                flex-direction: column;
                text-align: center;
            }
            .feature-list li {
                justify-content: center;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- breadcrumb start -->
    <div class="breadcrumb-area text-center">
        <h2 class="page-title">About Us</h2>
    </div>
    <!-- breadcrumb end -->

    <!-- about area start -->
    <div class="about-section">
        <div class="about-image">
            <img src="assets_web/img/about/1.png" alt="DocCare About" onerror="this.src='assets_web/img/banner/1.png'">
        </div>
        
        <div class="about-content">
            <span class="sub-title">Who We Are</span>
            <h2>Making health facilities accessible in emergencies</h2>
            <p>At DocCare, we believe that everyone deserves quick, reliable, and professional medical attention. Our clinic brings together top-tier medical experts and cutting-edge technology to provide you with the best possible care, tailored exactly to your needs.</p>
            
            <ul class="feature-list">
                <li><i class="fa-solid fa-check"></i> Quality health care and diagnostics</li>
                <li><i class="fa-solid fa-user-doctor"></i> Highly Qualified & Experienced Doctors</li>
                <li><i class="fa-solid fa-microscope"></i> Modern Medical Research Facilities</li>
                <li><i class="fa-solid fa-clock"></i> 24/7 Emergency Support</li>
            </ul>
        </div>
    </div>
</asp:Content>
