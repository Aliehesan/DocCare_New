<%@ Page Title="" Language="C#" MasterPageFile="~/Features/Shared/DocCare.Master" AutoEventWireup="true"
    CodeBehind="Service.aspx.cs" Inherits="DocCare_Clinic.Service" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .services-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 30px;
            padding: 40px 8%;
            margin-bottom: 60px;
        }

        .service-card-modern {
            background: #ffffff;
            padding: 40px 35px;
            border-radius: 20px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.04);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            text-align: center;
            border: 1px solid #f3f4f6;
        }

        .service-card-modern:hover {
            transform: translateY(-10px);
            box-shadow: 0 20px 40px rgba(46, 55, 164, 0.1);
            border-color: rgba(46, 55, 164, 0.3);
        }

        .service-icon-modern {
            width: 70px;
            height: 70px;
            background: rgba(46, 55, 164, 0.1);
            color: #2E37A4;
            border-radius: 20px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 32px;
            margin: 0 auto 25px auto;
        }

        .service-card-modern h3 {
            font-size: 22px;
            font-weight: 600;
            margin-bottom: 15px;
            color: #1f2937;
        }

        .service-card-modern p {
            color: #6b7280;
            font-size: 15px;
            line-height: 1.6;
        }

        @media (max-width: 991px) {
            .services-grid {
                grid-template-columns: repeat(2, 1fr);
            }
        }

        @media (max-width: 767px) {
            .services-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- breadcrumb start -->
    <div class="breadcrumb-area text-center">
        <h2 class="page-title">Our Services</h2>
    </div>
    <!-- breadcrumb end -->
    
    <!-- service area start -->
    <div class="services-grid">
        <div class="service-card-modern">
            <div class="service-icon-modern"><i class="fa-solid fa-tooth"></i></div>
            <h3>Dental Surgery</h3>
            <p>Comprehensive dental care, oral surgeries, and cosmetic dentistry tailored to your specific needs.</p>
        </div>
        
        <div class="service-card-modern">
            <div class="service-icon-modern"><i class="fa-solid fa-bone"></i></div>
            <h3>Orthopedics</h3>
            <p>Advanced treatment for bone, joint, and muscle conditions with state-of-the-art rehabilitation.</p>
        </div>

        <div class="service-card-modern">
            <div class="service-icon-modern"><i class="fa-solid fa-heart-pulse"></i></div>
            <h3>Cardiology</h3>
            <p>Expert heart health monitoring, cardiovascular surgeries, and comprehensive heart care.</p>
        </div>

        <div class="service-card-modern">
            <div class="service-icon-modern"><i class="fa-solid fa-brain"></i></div>
            <h3>Neurology</h3>
            <p>Specialized care for neurological disorders, brain health, and nervous system treatments.</p>
        </div>

        <div class="service-card-modern">
            <div class="service-icon-modern"><i class="fa-solid fa-child"></i></div>
            <h3>Pediatrics</h3>
            <p>Compassionate healthcare for infants, children, and adolescents focusing on their well-being.</p>
        </div>

        <div class="service-card-modern">
            <div class="service-icon-modern"><i class="fa-solid fa-eye"></i></div>
            <h3>Ophthalmology</h3>
            <p>Complete eye care services, vision correction surgeries, and preventative optical care.</p>
        </div>
    </div>
    <!-- service area end -->
</asp:Content>
