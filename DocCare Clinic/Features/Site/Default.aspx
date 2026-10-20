<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Features/Shared/DocCare.Master" CodeBehind="Default.aspx.cs" Inherits="DocCare_Clinic.Default1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* ── Hero Section ── */
        .hero {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 80px 8%;
            background: var(--white);
            margin: 40px 8%;
            border-radius: 30px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.02);
            min-height: 60vh;
        }

        .hero-content {
            flex: 1;
            max-width: 50%;
        }

        .hero-content h1 {
            font-size: 56px;
            font-weight: 800;
            color: var(--text-dark);
            line-height: 1.15;
            margin-bottom: 20px;
        }

        .hero-content p {
            font-size: 18px;
            color: var(--text-light);
            margin-bottom: 40px;
        }

        .hero-buttons {
            display: flex;
            gap: 20px;
        }

        .hero-image {
            flex: 1;
            display: flex;
            justify-content: flex-end;
            position: relative;
        }

        .hero-image img {
            max-width: 90%;
            height: auto;
            border-radius: 20px;
        }

        /* ── Services Cards ── */
        .services {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 30px;
            padding: 0 8%;
            margin-top: -30px;
            position: relative;
            z-index: 10;
        }

        .service-card {
            background: var(--white);
            padding: 35px;
            border-radius: 20px;
            box-shadow: 0 15px 35px rgba(0,0,0,0.03);
            transition: transform 0.3s ease;
            text-align: center;
        }

        .service-card:hover {
            transform: translateY(-10px);
        }

        .service-icon {
            width: 60px;
            height: 60px;
            background: rgba(46, 55, 164, 0.1);
            color: var(--primary);
            border-radius: 15px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 28px;
            margin: 0 auto 20px auto;
        }

        .service-card h3 {
            font-size: 20px;
            margin-bottom: 12px;
            color: var(--text-dark);
        }

        .service-card p {
            color: var(--text-light);
            font-size: 14px;
        }

        /* ── Doctor Team ── */
        .team-section {
            padding: 80px 8%;
            text-align: center;
        }

        .team-section h2 {
            font-size: 32px;
            font-weight: 700;
            margin-bottom: 50px;
        }

        .team-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 30px;
        }

        .doctor-card {
            background: var(--white);
            padding: 30px 20px;
            border-radius: 20px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.02);
            transition: transform 0.3s ease;
        }

        .doctor-card:hover {
            transform: translateY(-5px);
        }

        .doc-img {
            width: 120px;
            height: 120px;
            border-radius: 50%;
            object-fit: cover;
            margin: 0 auto 20px;
            border: 4px solid rgba(46, 55, 164, 0.1);
        }

        .doctor-card h4 {
            font-size: 18px;
            margin-bottom: 5px;
        }

        .doctor-card p {
            color: var(--primary);
            font-size: 14px;
            font-weight: 500;
        }

        @media (max-width: 991px) {
            .hero {
                flex-direction: column;
                text-align: center;
            }
            .hero-content {
                max-width: 100%;
                margin-bottom: 40px;
            }
            .hero-buttons {
                justify-content: center;
            }
            .services {
                grid-template-columns: 1fr;
            }
            .team-grid {
                grid-template-columns: repeat(2, 1fr);
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- ── Hero Section ── -->
    <section class="hero">
        <div class="hero-content">
            <h1>Your Health,<br>Our Priority</h1>
            <p>Compassionate Care for Your Well-being. Simply browse through our extensive list of trusted doctors, and schedule your appointment hassle-free.</p>
            <div class="hero-buttons">
                <a href="<%= ResolveUrl("~/Features/Doctors/Doctors-Team.aspx") %>" class="btn-modern btn-primary-teal">Book Appointment</a>
                <a href="<%= ResolveUrl("~/Features/Site/About.aspx") %>" class="btn-modern btn-outline">Learn More</a>
            </div>
        </div>
        <div class="hero-image">
            <img src="assets_web/img/banner/6.png" alt="Doctor Illustration">
        </div>
    </section>

    <!-- ── Services Cards ── -->
    <section class="services">
        <div class="service-card">
            <div class="service-icon"><i class="fa-solid fa-heart-pulse"></i></div>
            <h3>Cardiology</h3>
            <p>Advanced cardiology services, heart health medical assessments, and comprehensive care.</p>
        </div>
        <div class="service-card">
            <div class="service-icon"><i class="fa-solid fa-brain"></i></div>
            <h3>Neurology</h3>
            <p>Expert neurology services, brain health presentations, and nervous system treatments.</p>
        </div>
        <div class="service-card">
            <div class="service-icon"><i class="fa-solid fa-bone"></i></div>
            <h3>Orthopedics</h3>
            <p>Top-tier orthopedics services, joint care, and comprehensive physical rehabilitation.</p>
        </div>
    </section>

    <!-- ── Doctor Team ── -->
    <section class="team-section">
        <h2>Introducing your Doctor Team</h2>
        <div class="team-grid">
            <div class="doctor-card">
                <img src="assets/img/profiles/avatar-01.jpg" onerror="this.src='https://ui-avatars.com/api/?name=Arjun+Sharma&background=2E37A4&color=fff&size=120'" alt="Doctor" class="doc-img">
                <h4>Dr. Arjun Sharma</h4>
                <p>Cardiologist</p>
            </div>
            <div class="doctor-card">
                <img src="assets/img/profiles/avatar-02.jpg" onerror="this.src='https://ui-avatars.com/api/?name=Sara+Khan&background=2E37A4&color=fff&size=120'" alt="Doctor" class="doc-img">
                <h4>Dr. Sara Khan</h4>
                <p>Neurologist</p>
            </div>
            <div class="doctor-card">
                <img src="assets/img/profiles/avatar-03.jpg" onerror="this.src='https://ui-avatars.com/api/?name=Rahul+Mehta&background=2E37A4&color=fff&size=120'" alt="Doctor" class="doc-img">
                <h4>Dr. Rahul Mehta</h4>
                <p>Orthopedics</p>
            </div>
            <div class="doctor-card">
                <img src="assets/img/profiles/avatar-04.jpg" onerror="this.src='https://ui-avatars.com/api/?name=Priya+Patel&background=2E37A4&color=fff&size=120'" alt="Doctor" class="doc-img">
                <h4>Dr. Priya Patel</h4>
                <p>Pediatrics</p>
            </div>
        </div>
    </section>
</asp:Content>
