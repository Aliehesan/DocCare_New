<%@ Page Title="Doctor Details" Language="C#" MasterPageFile="~/Features/Shared/DocCare.Master" AutoEventWireup="true"
    CodeBehind="Doctor-details.aspx.cs" Inherits="DocCare_Clinic.Doctor_details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .doctor-page-container {
            padding: 0 6% 60px 6%;
            max-width: 1240px;
            margin: 0 auto;
            display: grid;
            grid-template-columns: 1.6fr 1fr;
            gap: 35px;
            align-items: start;
        }

        /* ── Left Column: Doctor Profile Overview ── */
        .doctor-main-card {
            background: #ffffff;
            border-radius: 24px;
            padding: 40px;
            box-shadow: 0 8px 30px rgba(46, 55, 164, 0.06);
            border: 1px solid #f3f4f6;
        }

        .doctor-header-flex {
            display: flex;
            align-items: center;
            gap: 28px;
            padding-bottom: 30px;
            border-bottom: 1px solid #f3f4f6;
        }

        .doctor-img-wrap {
            position: relative;
            flex-shrink: 0;
        }

        .doctor-avatar-modern {
            width: 135px;
            height: 135px;
            border-radius: 50%;
            object-fit: cover;
            border: 4px solid var(--primary);
            padding: 4px;
            background: #fff;
            box-shadow: 0 8px 24px rgba(46, 55, 164, 0.12);
        }

        .doctor-title-section h2 {
            font-size: 26px;
            font-weight: 700;
            color: var(--text-dark);
            margin: 0 0 10px 0;
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
        }

        .verified-badge-modern {
            color: #2563eb;
            font-size: 20px;
        }

        .doctor-badges-list {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 8px;
        }

        .doc-badge-pill {
            padding: 5px 14px;
            border-radius: 30px;
            font-size: 12.5px;
            font-weight: 600;
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        .badge-specialty {
            background: rgba(46, 55, 164, 0.09);
            color: var(--primary);
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .badge-degree {
            background: #f3f4f6;
            color: var(--text-dark);
        }

        .badge-experience {
            background: #ecfdf5;
            color: #047857;
        }

        /* ── About Section ── */
        .doctor-about-section {
            padding-top: 30px;
        }

        .section-subtitle {
            font-size: 18px;
            font-weight: 700;
            color: var(--text-dark);
            margin-bottom: 14px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

            .section-subtitle i {
                color: var(--primary);
                font-size: 18px;
            }

        .doctor-description-text {
            color: #4b5563;
            font-size: 15px;
            line-height: 1.8;
            margin: 0 0 30px 0;
        }

        /* Feature Highlights */
        .doc-features-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 15px;
            padding-top: 10px;
        }

        .feature-box {
            background: #f9fafb;
            border-radius: 16px;
            padding: 18px 15px;
            text-align: center;
            border: 1px solid #f0f2f5;
        }

            .feature-box i {
                font-size: 22px;
                color: var(--primary);
                margin-bottom: 8px;
                display: inline-block;
            }

            .feature-box span {
                display: block;
                font-size: 13px;
                font-weight: 600;
                color: var(--text-dark);
            }

        /* ── Right Column: Booking Card ── */
        .doctor-booking-card {
            background: #ffffff;
            border-radius: 24px;
            padding: 36px 30px;
            box-shadow: 0 10px 35px rgba(46, 55, 164, 0.08);
            border: 1px solid #f0f2f5;
            position: sticky;
            top: 100px;
        }

        .booking-card-title {
            font-size: 20px;
            font-weight: 700;
            color: var(--text-dark);
            margin-bottom: 20px;
            padding-bottom: 12px;
            border-bottom: 1px solid #f3f4f6;
            display: flex;
            align-items: center;
            gap: 10px;
        }

            .booking-card-title i {
                color: var(--primary);
            }

        .fee-highlight-box {
            background: linear-gradient(135deg, rgba(46, 55, 164, 0.06) 0%, rgba(46, 55, 164, 0.12) 100%);
            border-radius: 18px;
            padding: 22px;
            text-align: center;
            margin-bottom: 25px;
            border: 1px solid rgba(46, 55, 164, 0.15);
        }

        .fee-label {
            font-size: 13px;
            font-weight: 600;
            color: var(--text-light);
            text-transform: uppercase;
            letter-spacing: 1px;
            margin-bottom: 6px;
        }

        .fee-amount {
            font-size: 34px;
            font-weight: 800;
            color: var(--primary);
            line-height: 1;
        }

            .fee-amount span {
                font-size: 20px;
                font-weight: 600;
                margin-right: 2px;
            }

        .per-consultation-sub {
            font-size: 12px;
            color: var(--text-light);
            margin-top: 6px;
        }

        .booking-benefits-list {
            list-style: none;
            padding: 0;
            margin: 0 0 25px 0;
            display: flex;
            flex-direction: column;
            gap: 12px;
        }

            .booking-benefits-list li {
                font-size: 13.5px;
                color: #4b5563;
                display: flex;
                align-items: center;
                gap: 10px;
            }

                .booking-benefits-list li i {
                    color: #10b981;
                    font-size: 14px;
                }

        .book-next-btn-modern {
            width: 100%;
            background: var(--primary);
            color: #ffffff;
            border: none;
            padding: 15px 25px;
            border-radius: 50px;
            font-weight: 600;
            font-size: 15px;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 6px 20px rgba(46, 55, 164, 0.3);
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
        }

            .book-next-btn-modern:hover {
                background: var(--primary-hover);
                transform: translateY(-2px);
                box-shadow: 0 8px 25px rgba(46, 55, 164, 0.4);
                color: #ffffff;
            }

        .token-note-box {
            margin-top: 20px;
            background: #fffbeb;
            border-left: 4px solid #f59e0b;
            padding: 12px 14px;
            border-radius: 8px;
            font-size: 12.5px;
            color: #92400e;
            line-height: 1.5;
            display: flex;
            gap: 10px;
            align-items: flex-start;
        }

            .token-note-box i {
                font-size: 14px;
                margin-top: 2px;
                color: #d97706;
            }

        @media (max-width: 991px) {
            .doctor-page-container {
                grid-template-columns: 1fr;
            }

            .doctor-booking-card {
                position: static;
            }

            .doc-features-grid {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 576px) {
            .doctor-header-flex {
                flex-direction: column;
                text-align: center;
            }

            .doctor-title-section h2 {
                justify-content: center;
            }

            .doctor-badges-list {
                justify-content: center;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Breadcrumb Area -->
    <div class="breadcrumb-area text-center">
        <h2 class="page-title">Doctor Profile & Booking</h2>
    </div>

    <div class="doctor-page-container">
        <!-- ── Left Column: Main Doctor Information ── -->
        <div class="doctor-main-card">
            <!-- Doctor Header Info -->
            <div class="doctor-header-flex">
                <div class="doctor-img-wrap">
                    <asp:Image ID="imgDoctor" runat="server" CssClass="doctor-avatar-modern"
                        onerror="this.src='https://ui-avatars.com/api/?name=Doctor&background=2E37A4&color=fff&size=160'" />
                </div>

                <div class="doctor-title-section">
                    <h2>
                        Dr. <asp:Label ID="lblName" runat="server"></asp:Label>
                        <i class="fa-solid fa-circle-check verified-badge-modern" title="Verified Practitioner"></i>
                    </h2>

                    <div class="doctor-badges-list">
                        <span class="doc-badge-pill badge-specialty">
                            <i class="fa-solid fa-stethoscope"></i>
                            <asp:Label ID="lblSpeclist" runat="server"></asp:Label>
                        </span>

                        <span class="doc-badge-pill badge-degree">
                            <i class="fa-solid fa-graduation-cap"></i>
                            <asp:Label ID="lblDegree" runat="server"></asp:Label>
                        </span>

                        <span class="doc-badge-pill badge-experience">
                            <i class="fa-solid fa-award"></i>
                            <asp:Label ID="lblExperience" runat="server"></asp:Label>
                        </span>
                    </div>
                </div>
            </div>

            <!-- Doctor About Section -->
            <div class="doctor-about-section">
                <h3 class="section-subtitle">
                    <i class="fa-solid fa-user-doctor"></i>About Dr.
                    <asp:Label ID="lblAbout" runat="server"></asp:Label>
                </h3>

                <p class="doctor-description-text">
                    Dedicated healthcare professional with a strong commitment to delivering comprehensive medical care. Focuses on preventive medicine, early clinical diagnosis, and evidence-based personalized treatment strategies to ensure optimal patient outcomes and well-being.
                </p>

                <!-- Highlights Box -->
                <div class="doc-features-grid">
                    <div class="feature-box">
                        <i class="fa-solid fa-hospital-user"></i>
                        <span>Patient Centric</span>
                    </div>
                    <div class="feature-box">
                        <i class="fa-solid fa-shield-halved"></i>
                        <span>Verified Specialist</span>
                    </div>
                    <div class="feature-box">
                        <i class="fa-solid fa-clock-rotate-left"></i>
                        <span>Prompt Consultation</span>
                    </div>
                </div>
            </div>
        </div>

        <!-- ── Right Column: Booking & Fee Card ── -->
        <div class="doctor-booking-card">
            <h3 class="booking-card-title">
                <i class="fa-regular fa-calendar-check"></i>Book Consultation
            </h3>

            <!-- Fee Highlight -->
            <div class="fee-highlight-box">
                <div class="fee-label">Consultation Fee</div>
                <div class="fee-amount">
                    <span>&#8377;</span><asp:Label ID="lblCharges" runat="server"></asp:Label>
                </div>
                <div class="per-consultation-sub">Per Visit / Appointment Slot</div>
            </div>

            <!-- Benefit points -->
            <ul class="booking-benefits-list">
                <li>
                    <i class="fa-solid fa-circle-check"></i>
                    <span>Direct 1-on-1 Consultation</span>
                </li>
                <li>
                    <i class="fa-solid fa-circle-check"></i>
                    <span>Official Token & Queue Reservation</span>
                </li>
                <li>
                    <i class="fa-solid fa-circle-check"></i>
                    <span>Digital Health Record Included</span>
                </li>
            </ul>

            <!-- Book / Proceed Action Button -->
            <asp:Button ID="btnNext" runat="server" CssClass="book-next-btn-modern"
                Text="Proceed to Book Token" OnClick="btnNext_Click" />

            <!-- Booking Notice -->
            <div class="token-note-box">
                <i class="fa-solid fa-circle-exclamation"></i>
                <div>
                    <strong>Note:</strong> Once your token is booked, the token sequence number is confirmed and cannot be altered.
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfDoctorId" runat="server" />
</asp:Content>
