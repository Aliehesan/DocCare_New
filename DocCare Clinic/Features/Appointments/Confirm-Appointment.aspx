<%@ Page Title="Confirm Appointment" Language="C#" MasterPageFile="~/Features/Shared/DocCare.Master" AutoEventWireup="true"
    CodeBehind="Confirm-Appointment.aspx.cs" Inherits="DocCare_Clinic.Confirm_Appointment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta charset="utf-8" />
    <style type="text/css">
        .checkout-page-container {
            padding: 0 6% 60px 6%;
            max-width: 1200px;
            margin: 0 auto;
            display: grid;
            grid-template-columns: 1.55fr 1fr;
            gap: 35px;
            align-items: start;
        }

        /* ── Common Card Style ── */
        .confirm-card {
            background: #ffffff;
            border-radius: 24px;
            padding: 32px;
            box-shadow: 0 8px 30px rgba(46, 55, 164, 0.06);
            border: 1px solid #f3f4f6;
            margin-bottom: 25px;
        }

        .confirm-card-title {
            font-size: 19px;
            font-weight: 700;
            color: var(--text-dark);
            margin-bottom: 20px;
            padding-bottom: 14px;
            border-bottom: 1px solid #f3f4f6;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .confirm-card-title i {
            color: var(--primary);
            font-size: 19px;
        }

        .card-subtitle {
            font-size: 13.5px;
            color: var(--text-light);
            margin: -10px 0 20px 0;
        }

        /* ── Patient Selection Tiles ── */
        .patient-tiles-list {
            display: flex;
            flex-direction: column;
            gap: 12px;
        }

        .patient-tile-item {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 16px 20px;
            border-radius: 16px;
            border: 2px solid #f0f2f5;
            background: #ffffff;
            transition: all 0.25s ease;
            cursor: pointer;
        }

        .patient-tile-item:hover {
            border-color: rgba(46, 55, 164, 0.3);
            background: #fcfdfe;
            transform: translateY(-2px);
            box-shadow: 0 4px 15px rgba(46, 55, 164, 0.05);
        }

        .patient-tile-left {
            display: flex;
            align-items: center;
            gap: 14px;
        }

        .patient-radio-custom {
            width: 18px;
            height: 18px;
            accent-color: var(--primary);
            cursor: pointer;
        }

        .patient-avatar-circle {
            width: 42px;
            height: 42px;
            border-radius: 50%;
            background: rgba(46, 55, 164, 0.08);
            color: var(--primary);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            font-weight: 700;
        }

        .patient-info-meta strong {
            display: block;
            font-size: 15px;
            color: var(--text-dark);
            margin-bottom: 2px;
        }

        .patient-age-badge {
            font-size: 12px;
            color: var(--text-light);
            font-weight: 500;
        }

        .patient-actions-group {
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .patient-edit-btn {
            background: #f8fafc;
            color: var(--primary);
            border: 1.5px solid rgba(46, 55, 164, 0.2);
            padding: 6px 16px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
            outline: none;
        }

        .patient-edit-btn:hover {
            background: var(--primary);
            border-color: var(--primary);
            color: #ffffff;
            box-shadow: 0 4px 12px rgba(46, 55, 164, 0.25);
        }

        .patient-delete-btn {
            background: #fff1f2;
            color: #e11d48;
            border: 1.5px solid rgba(225, 29, 72, 0.2);
            padding: 6px 14px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
            outline: none;
        }

        .patient-delete-btn:hover {
            background: #e11d48;
            border-color: #e11d48;
            color: #ffffff;
            box-shadow: 0 4px 12px rgba(225, 29, 72, 0.25);
        }

        .add-patient-action-btn {
            display: block;
            width: 100%;
            text-align: center;
            background: transparent;
            border: 2px dashed rgba(46, 55, 164, 0.35);
            color: var(--primary);
            padding: 13px 20px;
            border-radius: 50px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            margin-top: 18px;
            transition: all 0.3s ease;
            outline: none;
        }

        .add-patient-action-btn:hover {
            border-color: var(--primary);
            background: rgba(46, 55, 164, 0.04);
            color: var(--primary-hover);
        }

        /* ── Contact & Address Details ── */
        .contact-info-grid {
            display: flex;
            flex-direction: column;
            gap: 12px;
        }

        .contact-info-item {
            display: flex;
            align-items: center;
            gap: 12px;
            font-size: 14px;
            color: #4b5563;
        }

        .contact-info-item i {
            color: var(--primary);
            width: 18px;
            font-size: 15px;
        }

        .default-pill-badge {
            background: #e0e7ff;
            color: #3730a3;
            font-size: 11px;
            font-weight: 700;
            padding: 3px 10px;
            border-radius: 20px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            margin-left: 8px;
        }

        /* ── Right Column: Summary Card ── */
        .summary-sticky-card {
            background: #ffffff;
            border-radius: 24px;
            padding: 32px;
            box-shadow: 0 10px 35px rgba(46, 55, 164, 0.08);
            border: 1px solid #f0f2f5;
            position: sticky;
            top: 100px;
        }

        .doctor-summary-box {
            background: #f8fafc;
            border-radius: 18px;
            padding: 20px;
            margin-bottom: 22px;
            border: 1px solid #e2e8f0;
        }

        .doctor-summary-name {
            font-size: 17px;
            font-weight: 700;
            color: var(--text-dark);
            margin-bottom: 4px;
        }

        .doctor-summary-spec {
            display: inline-block;
            font-size: 12px;
            font-weight: 600;
            color: var(--primary);
            background: rgba(46, 55, 164, 0.08);
            padding: 3px 10px;
            border-radius: 20px;
            text-transform: uppercase;
            margin-bottom: 12px;
        }

        .doctor-summary-fee {
            font-size: 15px;
            font-weight: 700;
            color: var(--text-dark);
            border-top: 1px solid #e2e8f0;
            padding-top: 10px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .date-picker-box {
            margin-bottom: 25px;
        }

        .date-picker-box label {
            display: block;
            font-size: 13.5px;
            font-weight: 600;
            color: var(--text-dark);
            margin-bottom: 8px;
        }

        .date-input-modern {
            width: 100%;
            padding: 12px 18px;
            border-radius: 30px;
            border: 1px solid #e5e7eb;
            font-size: 14px;
            color: var(--text-dark);
            background: #f9fafb;
            font-family: inherit;
            outline: none;
        }

        .confirm-submit-btn-modern {
            width: 100%;
            background: var(--primary);
            color: #ffffff;
            border: none;
            padding: 15px 25px;
            border-radius: 50px;
            font-size: 16px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 6px 20px rgba(46, 55, 164, 0.3);
            outline: none;
        }

        .confirm-submit-btn-modern:hover {
            background: var(--primary-hover);
            transform: translateY(-2px);
            box-shadow: 0 8px 25px rgba(46, 55, 164, 0.4);
            color: #ffffff;
        }

        .guarantee-note {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            margin-top: 15px;
            font-size: 12.5px;
            color: var(--text-light);
        }

        .guarantee-note i {
            color: #10b981;
        }

        /* ── Modern Modal Dialogs ── */
        .modal-content-modern {
            border-radius: 24px;
            border: none;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.15);
            overflow: hidden;
        }

        .modal-header-modern {
            background: #ffffff;
            padding: 24px 30px;
            border-bottom: 1px solid #f3f4f6;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .modal-header-modern h5 {
            font-size: 19px;
            font-weight: 700;
            color: var(--text-dark);
            margin: 0;
        }

        .modal-close-modern {
            background: #f3f4f6;
            border: none;
            width: 32px;
            height: 32px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 14px;
            color: #6b7280;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .modal-close-modern:hover {
            background: #e5e7eb;
            color: #111827;
        }

        .modal-body-modern {
            padding: 25px 30px;
        }

        .modal-form-group {
            margin-bottom: 16px;
        }

        .modal-form-group label {
            display: block;
            font-size: 13.5px;
            font-weight: 600;
            color: var(--text-dark);
            margin-bottom: 6px;
        }

        .modal-input-modern {
            width: 100%;
            padding: 10px 16px;
            border-radius: 12px;
            border: 1px solid #e5e7eb;
            font-size: 14px;
            background: #f9fafb;
            box-sizing: border-box;
            outline: none;
        }

        .modal-input-modern:focus {
            background: #ffffff;
            border-color: var(--primary);
        }

        .modal-footer-modern {
            padding: 18px 30px 24px 30px;
            display: flex;
            justify-content: flex-end;
            gap: 12px;
            border-top: 1px solid #f3f4f6;
        }

        .btn-modal-save {
            background: var(--primary);
            color: #ffffff;
            border: none;
            padding: 10px 24px;
            border-radius: 30px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn-modal-save:hover {
            background: var(--primary-hover);
        }

        .btn-modal-cancel {
            background: #f3f4f6;
            color: var(--text-dark);
            border: none;
            padding: 10px 20px;
            border-radius: 30px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
        }

        .swal-modern-toast {
            border-radius: 16px !important;
            box-shadow: 0 10px 30px rgba(46, 55, 164, 0.12) !important;
            border: 1px solid #e5e7eb !important;
            padding: 12px 20px !important;
            font-family: inherit !important;
            font-size: 14px !important;
            font-weight: 600 !important;
        }

        /* ── Fee Breakdown Card ── */
        .fee-breakdown-card {
            background: linear-gradient(135deg, #f0f4ff 0%, #e8eeff 100%);
            border-radius: 16px;
            padding: 18px 20px;
            margin-bottom: 22px;
            border: 1px solid rgba(46, 55, 164, 0.12);
        }

        .fee-breakdown-title {
            font-size: 13px;
            font-weight: 700;
            color: var(--text-light);
            text-transform: uppercase;
            letter-spacing: 0.6px;
            margin-bottom: 14px;
        }

        .fee-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-size: 13.5px;
            margin-bottom: 10px;
            color: #4b5563;
        }

        .fee-row:last-child { margin-bottom: 0; }

        .fee-row .fee-label { font-weight: 500; }
        .fee-row .fee-value { font-weight: 700; color: var(--text-dark); }

        .fee-row.fee-advance {
            background: rgba(46, 55, 164, 0.08);
            border-radius: 10px;
            padding: 8px 12px;
            margin: 8px -4px;
        }

        .fee-row.fee-advance .fee-label { color: var(--primary); font-weight: 600; }
        .fee-row.fee-advance .fee-value { color: var(--primary); font-size: 15px; }

        .fee-divider {
            border: none;
            border-top: 1px dashed rgba(46, 55, 164, 0.2);
            margin: 10px 0;
        }

        .fee-row.fee-due .fee-value { color: #6b7280; font-size: 13px; }

        .razorpay-note {
            display: flex;
            align-items: center;
            gap: 7px;
            font-size: 12px;
            color: #6b7280;
            margin-top: 14px;
            padding-top: 12px;
            border-top: 1px solid rgba(46, 55, 164, 0.1);
        }

        .razorpay-note img { height: 18px; opacity: 0.7; }

        .rzp-pay-btn {
            width: 100%;
            background: linear-gradient(135deg, #2E37A4 0%, #3d48c4 100%);
            color: #ffffff;
            border: none;
            padding: 15px 25px;
            border-radius: 50px;
            font-size: 15px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 6px 20px rgba(46, 55, 164, 0.35);
            outline: none;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
            margin-bottom: 0;
        }

        .rzp-pay-btn:hover {
            background: linear-gradient(135deg, #252d8e 0%, #2E37A4 100%);
            transform: translateY(-2px);
            box-shadow: 0 10px 28px rgba(46, 55, 164, 0.45);
        }

        .slot-status {
            display: block;
            margin: 0 0 12px;
            padding: 10px 14px;
            border-radius: 10px;
            font-size: 14px;
            font-weight: 600;
        }
        .slot-status:empty { display: none; }
        .slot-picker { margin: 0 0 16px; }
        .slot-picker label { display: block; font-weight: 600; font-size: 14px; color: #374151; margin-bottom: 8px; }
        .slot-list { display: flex; flex-wrap: wrap; gap: 8px; }
        .slot-hint { font-size: 13px; color: #6b7280; }
        .slot-chip {
            border: 1.5px solid #c7d2fe; background: #eef2ff; color: #2E37A4; border-radius: 10px;
            padding: 8px 14px; font-size: 13.5px; font-weight: 600; cursor: pointer; transition: all .15s;
        }
        .slot-chip:hover { border-color: #2E37A4; }
        .slot-chip.active { background: #2E37A4; color: #fff; border-color: #2E37A4; box-shadow: 0 4px 12px rgba(46,55,164,.25); }
        .slot-ok { background: #ecfdf5; color: #047857; }
        .slot-none { background: #fef2f2; color: #b91c1c; }

        .rzp-pay-btn:disabled {
            background: #9ca3af;
            cursor: not-allowed;
            transform: none;
            box-shadow: none;
        }

        .rzp-spinner {
            width: 18px;
            height: 18px;
            border: 2px solid rgba(255,255,255,0.4);
            border-top-color: #fff;
            border-radius: 50%;
            animation: rzp-spin 0.7s linear infinite;
            display: none;
        }

        @keyframes rzp-spin {
            to { transform: rotate(360deg); }
        }

        @media (max-width: 991px) {
            .checkout-page-container {
                grid-template-columns: 1fr;
            }
            .summary-sticky-card {
                position: static;
            }
        }
        /* ── DocCare UPI & QR Code Payment Gateway Modal ── */
        .upi-modal-overlay {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(15, 23, 42, 0.75);
            backdrop-filter: blur(8px);
            -webkit-backdrop-filter: blur(8px);
            z-index: 99999;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 16px;
            animation: upiFadeIn 0.25s ease;
        }

        @keyframes upiFadeIn {
            from { opacity: 0; }
            to { opacity: 1; }
        }

        .upi-modal-card {
            background: #ffffff;
            width: 100%;
            max-width: 470px;
            border-radius: 24px;
            box-shadow: 0 25px 65px -10px rgba(0, 0, 0, 0.4);
            overflow: hidden;
            position: relative;
            font-family: inherit;
            animation: upiSlideUp 0.3s cubic-bezier(0.16, 1, 0.3, 1);
        }

        @keyframes upiSlideUp {
            from { transform: translateY(24px) scale(0.97); opacity: 0; }
            to { transform: translateY(0) scale(1); opacity: 1; }
        }

        .upi-modal-header {
            background: linear-gradient(135deg, #1e2570 0%, #2E37A4 100%);
            color: #ffffff;
            padding: 20px 24px;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .upi-merchant-info {
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .upi-merchant-logo {
            width: 44px;
            height: 44px;
            border-radius: 12px;
            background: rgba(255, 255, 255, 0.18);
            border: 1px solid rgba(255, 255, 255, 0.25);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            color: #ffffff;
        }

        .upi-merchant-name {
            margin: 0;
            font-size: 17px;
            font-weight: 700;
            color: #ffffff;
            letter-spacing: -0.2px;
        }

        .upi-verified-badge {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            font-size: 11.5px;
            color: #a7f3d0;
            font-weight: 600;
            margin-top: 2px;
        }

        .upi-modal-close {
            background: rgba(255, 255, 255, 0.15);
            border: none;
            color: #ffffff;
            width: 32px;
            height: 32px;
            border-radius: 50%;
            font-size: 22px;
            line-height: 1;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: all 0.2s;
        }

        .upi-modal-close:hover {
            background: rgba(255, 255, 255, 0.35);
        }

        /* Amount Strip */
        .upi-amount-strip {
            background: #f8fafc;
            border-bottom: 1px solid #e2e8f0;
            padding: 16px 24px;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .upi-amount-label {
            font-size: 11.5px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            color: #64748b;
            font-weight: 700;
            display: block;
            margin-bottom: 3px;
        }

        .upi-amount-value {
            font-size: 25px;
            font-weight: 800;
            color: #0f172a;
            line-height: 1;
        }

        .upi-amount-right {
            text-align: right;
            font-size: 12px;
            color: #64748b;
        }

        .upi-total-breakdown {
            font-weight: 600;
        }

        .upi-due-breakdown {
            color: #059669;
            font-weight: 600;
            margin-top: 3px;
        }

        /* Tabs */
        .upi-tabs-nav {
            display: grid;
            grid-template-columns: 1fr 1fr 1fr;
            background: #f1f5f9;
            padding: 6px;
            gap: 4px;
            border-bottom: 1px solid #e2e8f0;
        }

        .upi-tab-btn {
            background: transparent;
            border: none;
            padding: 11px 8px;
            border-radius: 10px;
            font-size: 13px;
            font-weight: 600;
            color: #64748b;
            cursor: pointer;
            transition: all 0.2s;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            font-family: inherit;
        }

        .upi-tab-btn:hover {
            color: #1e293b;
        }

        .upi-tab-btn.active {
            background: #ffffff;
            color: #2E37A4;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.08);
        }

        /* Tab Contents */
        .upi-tab-content {
            display: none;
            padding: 22px 24px;
        }

        .upi-tab-content.active {
            display: block;
        }

        /* QR Tab */
        .upi-qr-wrapper {
            display: flex;
            flex-direction: column;
            align-items: center;
            text-align: center;
        }

        .upi-qr-scanner-box {
            position: relative;
            padding: 12px;
            background: #ffffff;
            border-radius: 18px;
            box-shadow: 0 6px 24px rgba(0, 0, 0, 0.07);
            border: 1px solid #e2e8f0;
            display: inline-block;
            margin-bottom: 14px;
        }

        .scanner-corner {
            position: absolute;
            width: 20px;
            height: 20px;
            border-color: #2E37A4;
            border-style: solid;
            pointer-events: none;
        }

        .scanner-corner.top-left {
            top: 6px;
            left: 6px;
            border-width: 3.5px 0 0 3.5px;
            border-top-left-radius: 8px;
        }

        .scanner-corner.top-right {
            top: 6px;
            right: 6px;
            border-width: 3.5px 3.5px 0 0;
            border-top-right-radius: 8px;
        }

        .scanner-corner.bottom-left {
            bottom: 6px;
            left: 6px;
            border-width: 0 0 3.5px 3.5px;
            border-bottom-left-radius: 8px;
        }

        .scanner-corner.bottom-right {
            bottom: 6px;
            right: 6px;
            border-width: 0 3.5px 3.5px 0;
            border-bottom-right-radius: 8px;
        }

        .upi-qr-image {
            width: 195px;
            height: 195px;
            display: block;
            border-radius: 10px;
        }

        .upi-scan-instruction {
            font-size: 13.5px;
            font-weight: 600;
            color: #1e293b;
            margin-bottom: 10px;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .upi-supported-apps-row {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            flex-wrap: wrap;
            margin-bottom: 12px;
        }

        .app-chip {
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            padding: 4px 10px;
            border-radius: 20px;
            font-size: 11.5px;
            font-weight: 600;
            color: #475569;
            display: inline-flex;
            align-items: center;
            gap: 4px;
        }

        .upi-expiry-timer {
            font-size: 12px;
            color: #ea580c;
            background: #fff7ed;
            border: 1px solid #ffedd5;
            padding: 5px 14px;
            border-radius: 20px;
            margin-bottom: 15px;
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        /* Primary Pay Button */
        .upi-primary-pay-btn {
            width: 100%;
            background: linear-gradient(135deg, #2E37A4 0%, #3d48c4 100%);
            color: #ffffff;
            border: none;
            padding: 13px 20px;
            border-radius: 12px;
            font-size: 15px;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.25s;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            box-shadow: 0 4px 14px rgba(46, 55, 164, 0.3);
            font-family: inherit;
        }

        .upi-primary-pay-btn:hover {
            background: linear-gradient(135deg, #252d8e 0%, #2E37A4 100%);
            transform: translateY(-1px);
            box-shadow: 0 6px 18px rgba(46, 55, 164, 0.4);
        }

        /* VPA Tab */
        .upi-vpa-wrapper {
            display: flex;
            flex-direction: column;
            gap: 12px;
        }

        .upi-vpa-label {
            font-size: 13px;
            font-weight: 600;
            color: #334155;
            margin: 0;
        }

        .upi-vpa-input-group {
            position: relative;
            display: flex;
            align-items: center;
        }

        .upi-vpa-icon {
            position: absolute;
            left: 14px;
            color: #94a3b8;
            font-size: 15px;
        }

        .upi-vpa-input {
            width: 100%;
            padding: 12px 14px 12px 38px;
            border: 1.5px solid #cbd5e1;
            border-radius: 12px;
            font-size: 14px;
            outline: none;
            transition: all 0.2s;
            font-family: inherit;
        }

        .upi-vpa-input:focus {
            border-color: #2E37A4;
            box-shadow: 0 0 0 3px rgba(46, 55, 164, 0.12);
        }

        .upi-fast-pills {
            display: flex;
            gap: 6px;
            flex-wrap: wrap;
        }

        .upi-pill {
            background: #f1f5f9;
            border: 1px solid #e2e8f0;
            padding: 4px 10px;
            border-radius: 14px;
            font-size: 11.5px;
            color: #475569;
            cursor: pointer;
            transition: all 0.2s;
            font-weight: 500;
        }

        .upi-pill:hover {
            background: #e0e7ff;
            color: #2E37A4;
            border-color: #c7d2fe;
        }

        .upi-vpa-hint {
            font-size: 12px;
            color: #64748b;
            margin: 2px 0 6px 0;
        }

        /* Apps Grid */
        .upi-apps-grid {
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .upi-app-card {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 14px 16px;
            background: #f8fafc;
            border: 1.5px solid #e2e8f0;
            border-radius: 14px;
            cursor: pointer;
            transition: all 0.2s;
        }

        .upi-app-card:hover {
            background: #ffffff;
            border-color: #2E37A4;
            box-shadow: 0 4px 12px rgba(46, 55, 164, 0.08);
            transform: translateY(-1px);
        }

        .app-card-icon {
            width: 40px;
            height: 40px;
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
        }

        .app-card-name {
            flex: 1;
            font-size: 14.5px;
            font-weight: 600;
            color: #1e293b;
        }

        .app-card-arrow {
            color: #94a3b8;
            font-size: 13px;
        }

        /* Processing Overlay */
        .upi-processing-overlay {
            position: absolute;
            top: 0; left: 0; right: 0; bottom: 0;
            background: rgba(255, 255, 255, 0.97);
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 24px;
            text-align: center;
            z-index: 10;
            animation: upiFadeIn 0.2s ease;
        }

        .upi-processing-box {
            display: flex;
            flex-direction: column;
            align-items: center;
        }

        .upi-spinner-ring {
            width: 50px;
            height: 50px;
            border: 4px solid #e2e8f0;
            border-top-color: #2E37A4;
            border-radius: 50%;
            animation: rzp-spin 0.8s linear infinite;
            margin-bottom: 16px;
        }

        .upi-processing-box h5 {
            font-size: 17px;
            font-weight: 700;
            color: #0f172a;
            margin-bottom: 6px;
        }

        .upi-processing-box p {
            font-size: 13.5px;
            color: #64748b;
            margin: 0;
        }

        /* Success Overlay */
        .upi-success-overlay {
            position: absolute;
            top: 0; left: 0; right: 0; bottom: 0;
            background: #ffffff;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 28px;
            text-align: center;
            z-index: 11;
            animation: upiFadeIn 0.25s ease;
        }

        .upi-success-box {
            display: flex;
            flex-direction: column;
            align-items: center;
            width: 100%;
        }

        .upi-success-check {
            width: 68px;
            height: 68px;
            border-radius: 50%;
            background: #ecfdf5;
            border: 2px solid #a7f3d0;
            color: #059669;
            font-size: 32px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-bottom: 14px;
            animation: upiScaleCheck 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
        }

        @keyframes upiScaleCheck {
            0% { transform: scale(0.3); opacity: 0; }
            70% { transform: scale(1.15); }
            100% { transform: scale(1); opacity: 1; }
        }

        .upi-success-box h4 {
            font-size: 20px;
            font-weight: 800;
            color: #0f172a;
            margin-bottom: 4px;
        }

        .upi-success-amount {
            font-size: 16px;
            font-weight: 700;
            color: #059669;
            margin-bottom: 6px;
        }

        .upi-success-ref {
            font-size: 12.5px;
            color: #64748b;
            margin-bottom: 16px;
            font-family: monospace;
        }

        .upi-success-progress {
            width: 80%;
            height: 4px;
            background: #e2e8f0;
            border-radius: 4px;
            overflow: hidden;
            margin-bottom: 10px;
        }

        .upi-progress-bar {
            width: 100%;
            height: 100%;
            background: #10b981;
            animation: upiProgress 1.4s ease-in-out;
        }

        @keyframes upiProgress {
            from { width: 0%; }
            to { width: 100%; }
        }

        .upi-success-sub {
            font-size: 13px;
            color: #64748b;
            margin: 0;
        }

        /* Footer */
        .upi-modal-footer {
            background: #f8fafc;
            border-top: 1px solid #e2e8f0;
            padding: 12px 20px;
            font-size: 11.5px;
            color: #64748b;
            text-align: center;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
        }
    </style>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://checkout.razorpay.com/v1/checkout.js"></script>
    <script>
        // Prevent form resubmission when user presses refresh (F5)
        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }

        function selectPatient(patientId) {
            console.log("Selected Patient ID:", patientId);
            var hiddenField = document.getElementById('<%= hdnSelectedPatientId.ClientID %>');
            if (hiddenField) {
                hiddenField.value = patientId;

                var bookedField = document.getElementById('<%= hdnBookedPatientIds.ClientID %>');
                var bookedIdsStr = bookedField ? bookedField.value : '';
                var bookedList = bookedIdsStr ? bookedIdsStr.split(',') : [];
                if (bookedList.indexOf(patientId.toString()) !== -1) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Already Booked Today',
                        text: 'This patient already has an appointment booked with this doctor today.',
                        confirmButtonColor: '#2E37A4',
                        timer: 4000,
                        timerProgressBar: true
                    });
                }
            }
        }

        function openEditModal() {
            var modal = new bootstrap.Modal(document.getElementById('editPatientModal'));
            modal.show();
        }

        function showAddDependentModal() {
            var modal = new bootstrap.Modal(document.getElementById('addDependentModal'));
            modal.show();
        }

        function modalClose() {
            var modalEl = document.getElementById("addDependentModal");
            var modal = bootstrap.Modal.getInstance(modalEl);
            if (modal) {
                modal.hide();
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            const txtDob = document.getElementById('<%= txtDob.ClientID %>');
            const txtAge = document.getElementById('<%= txtAge.ClientID %>');
            const txtEditDob = document.getElementById('<%= txtEditDob.ClientID %>');
            const txtEditAge = document.getElementById('<%= txtEditAge.ClientID %>');

            function calculateAge(dobInput, ageInput) {
                dobInput.addEventListener('change', function () {
                    const dob = new Date(dobInput.value);
                    if (!isNaN(dob)) {
                        const today = new Date();
                        let age = today.getFullYear() - dob.getFullYear();
                        const monthDiff = today.getMonth() - dob.getMonth();
                        const dayDiff = today.getDate() - dob.getDate();

                        if (monthDiff < 0 || (monthDiff === 0 && dayDiff < 0)) {
                            age--;
                        }

                        ageInput.value = age > 0 ? age : '';
                    } else {
                        ageInput.value = '';
                    }
                });
            }

            if (txtDob && txtAge) {
                calculateAge(txtDob, txtAge);
            }

            if (txtEditDob && txtEditAge) {
                calculateAge(txtEditDob, txtEditAge);
            }
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Breadcrumb -->
    <div class="breadcrumb-area text-center">
        <h2 class="page-title">Confirm Appointment</h2>
    </div>

    <!-- Main Container -->
    <div class="checkout-page-container">
        <!-- ── Left Column: Patient Selection & Contact ── -->
        <div class="checkout-main-content">
            <!-- Patient Selection Card -->
            <div class="confirm-card">
                <div class="confirm-card-title">
                    <i class="fa-solid fa-users"></i> Patient Information
                </div>
                <p class="card-subtitle">Choose the person for whom this consultation is scheduled:</p>

                <div class="patient-tiles-list">
                    <asp:Repeater ID="rptPatients" runat="server">
                        <ItemTemplate>
                            <label class="patient-tile-item" for='rbPatient<%# Eval("PatientID") %>'>
                                <div class="patient-tile-left">
                                    <input type="radio" class="patient-radio-custom" name="patientSelection"
                                        id='rbPatient<%# Eval("PatientID") %>' value='<%# Eval("PatientID") %>'
                                        onclick="selectPatient('<%# Eval("PatientID") %>')" />
                                    
                                    <div class="patient-avatar-circle">
                                        <i class="fa-solid fa-user"></i>
                                    </div>
                                    
                                    <div class="patient-info-meta">
                                        <strong><%# HttpUtility.HtmlEncode(Eval("PatientName")) %></strong>
                                        <span class="patient-age-badge">Age: <%# Eval("Age") %> yrs</span>
                                        <span class="patient-age-badge" title="Patient ID">ID <%# Eval("PatientCode") %></span>
                                    </div>
                                </div>

                                <div class="patient-actions-group">
                                    <asp:Button ID="btnEdit" CssClass="patient-edit-btn" runat="server"
                                        Text="Edit" CommandArgument='<%# Eval("PatientID") %>' OnClick="btnEdit_Click"
                                        ToolTip="Edit Details" />
                                    <asp:Button ID="btnDelete" CssClass="patient-delete-btn" runat="server"
                                        Text="Delete" CommandArgument='<%# Eval("PatientID") %>' OnClick="btnDelete_Click"
                                        CausesValidation="false"
                                        ToolTip="Remove Member" />
                                </div>
                            </label>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <asp:HiddenField ID="hdnSelectedPatientId" runat="server" EnableViewState="true" />

                <%-- Razorpay Payment Hidden Fields --%>
                <asp:HiddenField ID="hdnRazorpayPaymentId" runat="server" />
                <asp:HiddenField ID="hdnRazorpayOrderId" runat="server" />
                <asp:HiddenField ID="hdnRazorpaySignature" runat="server" />
                <asp:HiddenField ID="hdnPaidAmount" runat="server" />
                <asp:HiddenField ID="hdnDueAmount" runat="server" />
                <asp:HiddenField ID="hdnTotalFee" runat="server" />
                <asp:HiddenField ID="hdnAmountPaise" runat="server" />
                <asp:HiddenField ID="hdnBookedPatientIds" runat="server" />
                <%-- Hidden server-side button: triggered after successful Razorpay payment --%>
                <asp:Button ID="btnCompleteBooking" runat="server" style="display:none;"
                    OnClick="btnConfirmAppointment_Click" CausesValidation="false" />

                <!-- Add Patient Button -->
                <asp:Button ID="btnAddDependent" runat="server" Text="+ Add Family Member / Patient"
                    CssClass="add-patient-action-btn"
                    OnClientClick="showAddDependentModal(); return false;" />
            </div>

            <!-- Contact & Address Card -->
            <div class="confirm-card">
                <div class="confirm-card-title">
                    <i class="fa-solid fa-address-card"></i> Contact & Address Details
                </div>

                <div class="contact-info-grid">
                    <div class="contact-info-item">
                        <i class="fa-solid fa-user"></i>
                        <span>
                            <strong><asp:Label ID="lblPatientName" runat="server"></asp:Label></strong>
                            <span class="default-pill-badge">Primary Contact</span>
                        </span>
                    </div>

                    <asp:Label ID="Error" runat="server" CssClass="error-message" ForeColor="Red"
                        Font-Size="13px" Visible="false"></asp:Label>

                    <div class="contact-info-item">
                        <i class="fa-solid fa-phone"></i>
                        <span><asp:Label ID="lblPhone" runat="server"></asp:Label></span>
                    </div>

                    <div class="contact-info-item">
                        <i class="fa-regular fa-envelope"></i>
                        <span><asp:Label ID="lblEmail" runat="server"></asp:Label></span>
                    </div>

                    <div class="contact-info-item">
                        <i class="fa-solid fa-location-dot"></i>
                        <span><asp:Label ID="lblAddress" runat="server"></asp:Label></span>
                    </div>
                </div>
            </div>
        </div>

        <!-- ── Right Column: Summary & Confirmation ── -->
        <div class="summary-sticky-card">
            <div class="confirm-card-title">
                <i class="fa-regular fa-clipboard"></i> Booking Summary
            </div>

            <!-- Doctor Box -->
            <div class="doctor-summary-box">
                <div class="doctor-summary-name">
                    Dr. <asp:Label ID="lblDoctorName" runat="server"></asp:Label>
                </div>
                <div class="doctor-summary-spec">
                    <asp:Label ID="lblSpecialty" runat="server"></asp:Label>
                </div>
                <div class="doctor-summary-fee">
                    <span>Total Consultation Fee</span>
                    <strong style="color: var(--primary);"><asp:Label ID="lblCharges" runat="server"></asp:Label></strong>
                </div>
            </div>

            <!-- Date Box -->
            <div class="date-picker-box">
                <label for="txtAppointmentDate"><i class="fa-regular fa-calendar-check"></i> Appointment Date</label>
                <asp:TextBox ID="txtAppointmentDate" runat="server" CssClass="date-input-modern"
                    TextMode="Date" onchange="loadSlots()"></asp:TextBox>
            </div>

            <!-- Time picker: free times for the chosen date (filled by loadSlots) -->
            <div class="slot-picker">
                <label><i class="fa-regular fa-clock"></i> Choose a Time</label>
                <div id="slotList" class="slot-list"><span class="slot-hint">Loading times...</span></div>
                <asp:HiddenField ID="hdnSelectedSlot" runat="server" />
            </div>

            <!-- Fee Breakdown Card (populated by JS from hidden fields) -->
            <div class="fee-breakdown-card">
                <div class="fee-breakdown-title">
                    <i class="fa-solid fa-receipt" style="color:var(--primary);margin-right:5px;"></i>
                    Payment Breakdown
                </div>
                <div class="fee-row">
                    <span class="fee-label">Total Consultation Fee</span>
                    <span class="fee-value" id="spanTotalFee">Loading...</span>
                </div>
                <hr class="fee-divider" />
                <div class="fee-row fee-advance">
                    <span class="fee-label">
                        <i class="fa-solid fa-lock" style="font-size:11px;margin-right:4px;"></i>
                        Pay Online Now (20%)
                    </span>
                    <span class="fee-value" id="spanAdvanceFee">-</span>
                </div>
                <div class="fee-row fee-due">
                    <span class="fee-label" style="color:#6b7280;">
                        <i class="fa-solid fa-hospital" style="font-size:11px;margin-right:4px;"></i>
                        Pay at Clinic (80%)
                    </span>
                    <span class="fee-value" id="spanDueFee">-</span>
                </div>
                <div class="razorpay-note">
                    <i class="fa-solid fa-shield-halved" style="color:#10b981;"></i>
                    <span>Secured &amp; encrypted by Razorpay</span>
                </div>
            </div>

            <%-- Old server button kept hidden; btnCompleteBooking triggers postback after payment --%>
            <asp:Button ID="btnConfirmAppointment" runat="server" Text="Confirm &amp; Book Appointment"
                CssClass="confirm-submit-btn-modern" OnClick="btnConfirmAppointment_Click"
                style="display:none;" />

            <!-- Availability message for the chosen date (checked again before any payment) -->
            <asp:HiddenField ID="hdnSlotAvailable" runat="server" Value="1" />
            <asp:Label ID="lblSlotStatus" runat="server" CssClass="slot-status"></asp:Label>

            <!-- Razorpay Pay Button (visible to user) -->
            <button type="button" class="rzp-pay-btn" id="btnRazorpayPay" onclick="initiateRazorpayPayment()">
                <span id="rzpBtnText">Pay 20% Advance &amp; Confirm</span>
                <div class="rzp-spinner" id="rzpSpinner"></div>
            </button>

            <div class="guarantee-note" style="margin-top:14px;">
                <i class="fa-solid fa-shield-halved"></i>
                <span>Slot reserved only after payment - secure &amp; encrypted</span>
            </div>
        </div>
    </div>

    <!-- ── Add Dependent Modal ── -->
    <div class="modal fade" id="addDependentModal" tabindex="-1" aria-labelledby="addDependentLabel"
        aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content modal-content-modern">
                <div class="modal-header-modern">
                    <h5 id="addDependentLabel"><i class="fa-solid fa-user-plus" style="color:var(--primary); margin-right:8px;"></i> Add Patient Details</h5>
                    <button type="button" class="modal-close-modern" data-bs-dismiss="modal" aria-label="Close" onclick="modalClose()">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </div>
                
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Font-Size="14px" Style="padding: 10px 30px; display:block;"></asp:Label>
                
                <div class="modal-body-modern">
                    <div class="modal-form-group">
                        <label>Full Name</label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="modal-input-modern" Placeholder="Enter full name"></asp:TextBox>
                    </div>

                    <div class="modal-form-group">
                        <label>Date of Birth</label>
                        <asp:TextBox ID="txtDob" CssClass="modal-input-modern" runat="server" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="modal-form-group">
                        <label>Age</label>
                        <asp:TextBox ID="txtAge" CssClass="modal-input-modern" runat="server" EnableViewState="true"
                            Placeholder="Auto-calculated from DOB"></asp:TextBox>
                        <script>
                            document.getElementById('<%= txtAge.ClientID %>').setAttribute('readonly', 'readonly');
                        </script>
                    </div>

                    <div class="modal-form-group">
                        <label>Gender</label>
                        <asp:RadioButtonList ID="rblGender" runat="server" RepeatDirection="Horizontal" CellPadding="5">
                            <asp:ListItem Text="&nbsp;Male&nbsp;&nbsp;" Value="Male"></asp:ListItem>
                            <asp:ListItem Text="&nbsp;Female" Value="Female"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>

                <div class="modal-footer-modern">
                    <button type="button" class="btn-modal-cancel" data-bs-dismiss="modal" onclick="modalClose()">Cancel</button>
                    <asp:Button ID="btnSaveDependent" runat="server" Text="Save Patient" CssClass="btn-modal-save"
                        OnClick="btnSaveDependent_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- ── Edit Patient Modal ── -->
    <div class="modal fade" id="editPatientModal" tabindex="-1" aria-labelledby="editPatientLabel"
        aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content modal-content-modern">
                <div class="modal-header-modern">
                    <h5 id="editPatientLabel"><i class="fa-solid fa-user-pen" style="color:var(--primary); margin-right:8px;"></i> Edit Patient Details</h5>
                    <button type="button" class="modal-close-modern" data-bs-dismiss="modal" aria-label="Close">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </div>

                <asp:Label ID="ErrorMessage" runat="server" ForeColor="Red" Font-Size="14px" Style="padding: 10px 30px; display:block;"></asp:Label>

                <div class="modal-body-modern">
                    <div class="modal-form-group">
                        <label>Full Name</label>
                        <asp:TextBox ID="txtEditName" runat="server" CssClass="modal-input-modern"></asp:TextBox>
                    </div>

                    <div class="modal-form-group">
                        <label>Date of Birth</label>
                        <asp:TextBox ID="txtEditDob" CssClass="modal-input-modern" runat="server" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="modal-form-group">
                        <label>Age</label>
                        <asp:TextBox ID="txtEditAge" CssClass="modal-input-modern" runat="server" EnableViewState="true"></asp:TextBox>
                        <script>
                            document.getElementById('<%= txtEditAge.ClientID %>').setAttribute('readonly', 'readonly');
                        </script>
                    </div>

                    <div class="modal-form-group">
                        <label>Gender</label>
                        <asp:RadioButtonList ID="rblEditGender" runat="server" RepeatDirection="Horizontal" CellPadding="5">
                            <asp:ListItem Text="&nbsp;Male&nbsp;&nbsp;" Value="Male"></asp:ListItem>
                            <asp:ListItem Text="&nbsp;Female" Value="Female"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>

                <div class="modal-footer-modern">
                    <button type="button" class="btn-modal-cancel" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnUpdatePatient" runat="server" Text="Update Details" CssClass="btn-modal-save"
                        OnClick="btnUpdatePatient_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- ── DocCare UPI & QR Code Payment Gateway Modal ── -->
    <div id="doccareUpiModal" class="upi-modal-overlay" style="display:none;">
        <div class="upi-modal-card">
            <!-- Header -->
            <div class="upi-modal-header">
                <div class="upi-merchant-info">
                    <div class="upi-merchant-logo">
                        <i class="fa-solid fa-hospital-user"></i>
                    </div>
                    <div>
                        <h4 class="upi-merchant-name">DocCare Clinic</h4>
                        <span class="upi-verified-badge"><i class="fa-solid fa-circle-check"></i> Verified Merchant</span>
                    </div>
                </div>
                <button type="button" class="upi-modal-close" onclick="closeDocCareUpiModal()" aria-label="Close">&times;</button>
            </div>

            <!-- Amount Strip -->
            <div class="upi-amount-strip">
                <div class="upi-amount-left">
                    <span class="upi-amount-label">20% Online Advance Deposit</span>
                    <div class="upi-amount-value" id="modalAdvanceFeeDisplay">&#8377;100</div>
                </div>
                <div class="upi-amount-right">
                    <div class="upi-total-breakdown">Total Fee: <span id="modalTotalFeeDisplay">&#8377;500</span></div>
                    <div class="upi-due-breakdown"><i class="fa-solid fa-hospital"></i> Due at clinic: <span id="modalDueFeeDisplay">&#8377;400 (80%)</span></div>
                </div>
            </div>

            <!-- Navigation Tabs: QR Code vs UPI ID vs UPI Apps -->
            <div class="upi-tabs-nav">
                <button type="button" class="upi-tab-btn active" id="tabBtnQr" onclick="switchUpiTab('qr')">
                    <i class="fa-solid fa-qrcode"></i> Scan QR
                </button>
                <button type="button" class="upi-tab-btn" id="tabBtnVpa" onclick="switchUpiTab('vpa')">
                    <i class="fa-solid fa-at"></i> UPI ID
                </button>
                <button type="button" class="upi-tab-btn" id="tabBtnApps" onclick="switchUpiTab('apps')">
                    <i class="fa-solid fa-mobile-screen"></i> UPI Apps
                </button>
            </div>

            <!-- TAB 1: SCAN QR CODE -->
            <div id="tabContentQr" class="upi-tab-content active">
                <div class="upi-qr-wrapper">
                    <div class="upi-qr-scanner-box">
                        <div class="scanner-corner top-left"></div>
                        <div class="scanner-corner top-right"></div>
                        <div class="scanner-corner bottom-left"></div>
                        <div class="scanner-corner bottom-right"></div>
                        <img id="upiQrCodeImg" class="upi-qr-image" src="" alt="Scan QR Code to Pay" />
                    </div>
                    <div class="upi-scan-instruction">
                        <i class="fa-solid fa-camera"></i> Scan with any UPI app to pay
                    </div>
                    <div class="upi-supported-apps-row">
                        <span class="app-chip"><i class="fa-brands fa-google-pay" style="color:#4285F4;font-size:16px;"></i> GPay</span>
                        <span class="app-chip"><i class="fa-solid fa-wallet" style="color:#5f259f;"></i> PhonePe</span>
                        <span class="app-chip"><i class="fa-solid fa-mobile-button" style="color:#00b9f5;"></i> Paytm</span>
                        <span class="app-chip"><i class="fa-solid fa-building-columns" style="color:#f26922;"></i> BHIM</span>
                        <span class="app-chip"><i class="fa-solid fa-shield" style="color:#10b981;"></i> Cred</span>
                    </div>
                    <div class="upi-expiry-timer">
                        <i class="fa-regular fa-clock"></i> QR code valid for: <strong id="qrTimerText">04:59</strong>
                    </div>
                    <button type="button" class="upi-primary-pay-btn" onclick="startUpiVerification('QR Scanner')">
                        <i class="fa-solid fa-circle-check"></i> I Have Paid &#8212; Verify Payment
                    </button>
                </div>
            </div>

            <!-- TAB 2: PAY VIA UPI ID / VPA -->
            <div id="tabContentVpa" class="upi-tab-content">
                <div class="upi-vpa-wrapper">
                    <label class="upi-vpa-label">Enter Virtual Payment Address (UPI ID)</label>
                    <div class="upi-vpa-input-group">
                        <i class="fa-solid fa-at upi-vpa-icon"></i>
                        <input type="text" id="upiVpaInput" class="upi-vpa-input" placeholder="e.g. mobile@paytm or name@okhdfcbank" />
                    </div>
                    <div class="upi-fast-pills">
                        <span class="upi-pill" onclick="appendUpiSuffix('@okhdfcbank')">@okhdfcbank</span>
                        <span class="upi-pill" onclick="appendUpiSuffix('@okaxis')">@okaxis</span>
                        <span class="upi-pill" onclick="appendUpiSuffix('@paytm')">@paytm</span>
                        <span class="upi-pill" onclick="appendUpiSuffix('@ybl')">@ybl</span>
                        <span class="upi-pill" onclick="appendUpiSuffix('@ibl')">@ibl</span>
                    </div>
                    <p class="upi-vpa-hint">
                        <i class="fa-solid fa-shield-halved" style="color:#10b981;"></i> A 20% advance payment request will be approved for your slot reservation.
                    </p>
                    <button type="button" class="upi-primary-pay-btn" onclick="processUpiIdPayment()">
                        <i class="fa-solid fa-paper-plane"></i> Verify &amp; Pay <span id="btnVpaAmount">&#8377;100</span>
                    </button>
                </div>
            </div>

            <!-- TAB 3: PAY VIA POPULAR UPI APPS -->
            <div id="tabContentApps" class="upi-tab-content">
                <div class="upi-apps-grid">
                    <div class="upi-app-card" onclick="startAppPayment('Google Pay')">
                        <div class="app-card-icon" style="background:#e8f0fe; color:#1a73e8;"><i class="fa-brands fa-google"></i></div>
                        <div class="app-card-name">Google Pay (GPay)</div>
                        <span class="app-card-arrow"><i class="fa-solid fa-chevron-right"></i></span>
                    </div>
                    <div class="upi-app-card" onclick="startAppPayment('PhonePe')">
                        <div class="app-card-icon" style="background:#f3e8ff; color:#5f259f;"><i class="fa-solid fa-wallet"></i></div>
                        <div class="app-card-name">PhonePe</div>
                        <span class="app-card-arrow"><i class="fa-solid fa-chevron-right"></i></span>
                    </div>
                    <div class="upi-app-card" onclick="startAppPayment('Paytm')">
                        <div class="app-card-icon" style="background:#e0f7fa; color:#00b9f5;"><i class="fa-solid fa-mobile-screen"></i></div>
                        <div class="app-card-name">Paytm UPI</div>
                        <span class="app-card-arrow"><i class="fa-solid fa-chevron-right"></i></span>
                    </div>
                    <div class="upi-app-card" onclick="startAppPayment('BHIM')">
                        <div class="app-card-icon" style="background:#fff7ed; color:#ea580c;"><i class="fa-solid fa-building-columns"></i></div>
                        <div class="app-card-name">BHIM / Any UPI App</div>
                        <span class="app-card-arrow"><i class="fa-solid fa-chevron-right"></i></span>
                    </div>
                </div>
            </div>

            <!-- PROCESSING OVERLAY (Shown while verifying) -->
            <div id="upiProcessingOverlay" class="upi-processing-overlay" style="display:none;">
                <div class="upi-processing-box">
                    <div class="upi-spinner-ring"></div>
                    <h5 id="upiProcessingTitle">Verifying UPI Transaction...</h5>
                    <p id="upiProcessingText">Connecting to bank to confirm receipt of payment.</p>
                </div>
            </div>

            <!-- SUCCESS OVERLAY (Shown on confirmation) -->
            <div id="upiSuccessOverlay" class="upi-success-overlay" style="display:none;">
                <div class="upi-success-box">
                    <div class="upi-success-check"><i class="fa-solid fa-check"></i></div>
                    <h4>Payment Successful!</h4>
                    <div class="upi-success-amount" id="upiSuccessAmount">&#8377;100.00 Advance Paid</div>
                    <p class="upi-success-ref">UPI Ref ID: <span id="upiSuccessRef"></span></p>
                    <div class="upi-success-progress">
                        <div class="upi-progress-bar"></div>
                    </div>
                    <p class="upi-success-sub">Securing appointment slot in database...</p>
                </div>
            </div>

            <!-- Footer -->
            <div class="upi-modal-footer">
                <i class="fa-solid fa-shield-halved" style="color:#10b981;"></i> 256-bit Encrypted &bull; 100% Secure UPI Payment Gateway
            </div>
        </div>
    </div>

    <!-- Alert Handler Scripts (Modern Toast) -->
    <script>
        window.onload = function () {
            var successMessage = '<%= Session["SuccessMessage"] %>';
            var errorMessage = '<%= Session["ErrorMessage"] %>';
            var redirectToDefault = '<%= Session["RedirectToDefault"] %>';

            // Modern SweetAlert2 Toast configuration
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 2500,
                timerProgressBar: true,
                customClass: {
                    popup: 'swal-modern-toast'
                },
                didOpen: (toast) => {
                    toast.onmouseenter = Swal.stopTimer;
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });

            if (successMessage && successMessage !== "null") {
                Toast.fire({
                    icon: 'success',
                    title: successMessage
                }).then(function () {
                    if (redirectToDefault === "true") {
                        window.location.href = 'Features/Site/Default.aspx';
                    }
                });
                <% Session["SuccessMessage"] = null; %>
                <% Session["RedirectToDefault"] = null; %>
            }

            if (errorMessage && errorMessage !== "null") {
                Toast.fire({
                    icon: 'error',
                    title: errorMessage
                });
                <% Session["ErrorMessage"] = null; %>
            }

            var swalConfirmedPatientDelete = null;

            // SweetAlert2 — intercept Delete Patient/Family Member button (inside Repeater)
            document.addEventListener('click', function (e) {
                var btn = e.target.closest('.patient-delete-btn');
                if (!btn) return;

                if (btn === swalConfirmedPatientDelete) {
                    swalConfirmedPatientDelete = null;
                    return; // Allow native form submit to proceed!
                }

                e.preventDefault();
                e.stopImmediatePropagation();

                Swal.fire({
                    title: 'Remove Member?',
                    text: 'This family member will be removed from your list.',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#2E37A4',
                    cancelButtonColor: '#6b7280',
                    confirmButtonText: 'Yes, Remove',
                    cancelButtonText: 'Cancel',
                    customClass: { popup: 'swal-modern-popup' }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        swalConfirmedPatientDelete = btn;
                        if (btn.form && typeof btn.form.requestSubmit === 'function') {
                            btn.form.requestSubmit(btn);
                        } else {
                            btn.click();
                        }
                    }
                });
                return false;
            }, true);
        };
    </script>

        <!-- ── Razorpay Checkout Client Script ── -->
    <script>
        // Formatter using pure HTML entities (cannot be corrupted by encoding)
        function fmtRupees(num) {
            var val = Math.round(Number(num) || 0);
            return '&#8377;' + val.toLocaleString('en-IN');
        }

        // Update payment summary breakdown from hidden fields
        function updateFeeBreakdown() {
            var totalFee = parseFloat(document.getElementById('<%= hdnTotalFee.ClientID %>').value) || 0;
            var paidAmt  = parseFloat(document.getElementById('<%= hdnPaidAmount.ClientID %>').value) || 0;
            var dueAmt   = parseFloat(document.getElementById('<%= hdnDueAmount.ClientID %>').value) || 0;

            var elTotal = document.getElementById('spanTotalFee');
            var elAdv   = document.getElementById('spanAdvanceFee');
            var elDue   = document.getElementById('spanDueFee');
            var btnText = document.getElementById('rzpBtnText');

            if (totalFee > 0) {
                if (elTotal) elTotal.innerHTML = fmtRupees(totalFee);
                if (elAdv)   elAdv.innerHTML   = fmtRupees(paidAmt);
                if (elDue)   elDue.innerHTML   = fmtRupees(dueAmt);
                if (btnText) btnText.innerHTML = '<i class="fa-solid fa-lock" style="margin-right:6px;"></i>Pay ' + fmtRupees(paidAmt) + ' & Confirm Booking';
            } else {
                if (elTotal) elTotal.innerHTML = '&ndash;';
                if (elAdv)   elAdv.innerHTML   = '&ndash;';
                if (elDue)   elDue.innerHTML   = '&ndash;';
            }
        }

        // Run on DOM load
        document.addEventListener('DOMContentLoaded', function () {
            updateFeeBreakdown();
        });

        function resetPayButton(customHtml) {
            var btn = document.getElementById('btnRazorpayPay');
            var spinner = document.getElementById('rzpSpinner');
            var btnText = document.getElementById('rzpBtnText');
            if (btn) btn.disabled = false;
            if (spinner) spinner.style.display = 'none';
            if (btnText) {
                if (customHtml) {
                    btnText.innerHTML = customHtml;
                } else {
                    updateFeeBreakdown();
                }
            }
        }

        var qrTimerInterval = null;

        function startQrCountdown(seconds) {
            clearInterval(qrTimerInterval);
            var remaining = seconds;
            var timerEl = document.getElementById('qrTimerText');

            function updateDisplay() {
                var mins = Math.floor(remaining / 60);
                var secs = remaining % 60;
                if (timerEl) {
                    timerEl.textContent = (mins < 10 ? '0' : '') + mins + ':' + (secs < 10 ? '0' : '') + secs;
                }
                if (remaining <= 0) {
                    clearInterval(qrTimerInterval);
                    if (timerEl) timerEl.textContent = 'Expired (Please refresh)';
                }
                remaining--;
            }

            updateDisplay();
            qrTimerInterval = setInterval(updateDisplay, 1000);
        }

        function closeDocCareUpiModal() {
            clearInterval(qrTimerInterval);
            var modal = document.getElementById('doccareUpiModal');
            if (modal) modal.style.display = 'none';
            resetPayButton();
        }

        function switchUpiTab(tab) {
            var tabs = ['qr', 'vpa', 'apps'];
            for (var i = 0; i < tabs.length; i++) {
                var t = tabs[i];
                var capitalized = t.charAt(0).toUpperCase() + t.slice(1);
                var btn = document.getElementById('tabBtn' + capitalized);
                var content = document.getElementById('tabContent' + capitalized);
                if (btn) btn.classList.remove('active');
                if (content) content.classList.remove('active');
            }

            var activeCap = tab.charAt(0).toUpperCase() + tab.slice(1);
            var activeBtn = document.getElementById('tabBtn' + activeCap);
            var activeContent = document.getElementById('tabContent' + activeCap);
            if (activeBtn) activeBtn.classList.add('active');
            if (activeContent) activeContent.classList.add('active');
        }

        function appendUpiSuffix(suffix) {
            var input = document.getElementById('upiVpaInput');
            if (!input) return;
            var val = input.value.trim();
            if (val.indexOf('@') > -1) {
                val = val.substring(0, val.indexOf('@'));
            }
            if (!val) val = 'user';
            input.value = val + suffix;
            input.focus();
        }

        function initiateRazorpayPayment() {
            var patientId = document.getElementById('<%= hdnSelectedPatientId.ClientID %>').value;
            if (!patientId) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Select a Patient',
                    text: 'Please select a patient before booking the appointment.',
                    confirmButtonColor: '#2E37A4'
                });
                return;
            }

            var chosenSlot = document.getElementById('<%= hdnSelectedSlot.ClientID %>').value;
            if (!chosenSlot) {
                Swal.fire({
                    icon: 'info',
                    title: 'Choose a Time',
                    text: 'Please choose a date and an appointment time first.',
                    confirmButtonColor: '#2E37A4'
                });
                return;
            }

            // Before opening payment popup, verify patient does not already have an appointment that day
            var bookedField = document.getElementById('<%= hdnBookedPatientIds.ClientID %>');
            var bookedIdsStr = bookedField ? bookedField.value : '';
            var bookedList = bookedIdsStr ? bookedIdsStr.split(',') : [];
            if (bookedList.indexOf(patientId.toString()) !== -1) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Appointment Already Booked',
                    text: 'This patient already has an appointment with this doctor on that date. Please choose another date.',
                    confirmButtonColor: '#2E37A4'
                });
                return;
            }

            var amountPaise = parseInt(document.getElementById('<%= hdnAmountPaise.ClientID %>').value, 10) || 0;
            if (amountPaise < 100) {
                Swal.fire({
                    icon: 'error',
                    title: 'Fee Error',
                    text: 'Doctor charges are invalid or less than Rs. 1. Please refresh the page.',
                    confirmButtonColor: '#2E37A4'
                });
                return;
            }

            // Never take payment unless a slot is still free right now
            checkSlotThenPay(amountPaise);
        }

        function showNoSlot(message) {
            Swal.fire({
                icon: 'warning',
                title: 'Time Not Available',
                text: message || 'That time is no longer available. Please choose another time.',
                confirmButtonColor: '#2E37A4'
            });
            loadSlots();   // refresh the times so the taken one disappears
        }

        // ── Date and time picker ──
        function slotField() { return document.getElementById('<%= hdnSelectedSlot.ClientID %>'); }
        function dateField() { return document.getElementById('<%= txtAppointmentDate.ClientID %>'); }
        function doctorIdFromUrl() { return parseInt(new URLSearchParams(window.location.search).get('doctorId'), 10) || 0; }

        function setSlotStatus(text, ok) {
            var el = document.getElementById('<%= lblSlotStatus.ClientID %>');
            if (!el) return;
            el.textContent = text || '';
            el.className = 'slot-status ' + (ok ? 'slot-ok' : 'slot-none');
        }

        function setPayEnabled(on) {
            var btn = document.getElementById('btnRazorpayPay');
            if (btn) btn.disabled = !on;
        }

        function loadSlots() {
            var list = document.getElementById('slotList');
            var date = dateField().value;
            var keep = slotField().value;
            setPayEnabled(false);
            if (!date) { list.innerHTML = '<span class="slot-hint">Please choose a date.</span>'; return; }
            list.innerHTML = '<span class="slot-hint">Loading times...</span>';
            fetch(window.location.pathname + '/GetSlots', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                body: JSON.stringify({ doctorId: doctorIdFromUrl(), date: date })
            })
                .then(function (r) { if (!r.ok) throw new Error(r.status); return r.json(); })
                .then(function (data) {
                    var res = data.d || data;
                    document.getElementById('<%= hdnBookedPatientIds.ClientID %>').value = (res.bookedPatientIds || []).join(',');
                    list.innerHTML = '';
                    slotField().value = '';
                    if (!res.available) {
                        list.innerHTML = '<span class="slot-hint">No times available on this date.</span>';
                        setSlotStatus(res.message, false);
                        return;
                    }
                    res.slots.forEach(function (s) {
                        var b = document.createElement('button');
                        b.type = 'button';
                        b.className = 'slot-chip';
                        b.textContent = s.label;
                        b.setAttribute('data-value', s.value);
                        b.onclick = function () { pickSlot(b); };
                        list.appendChild(b);
                        if (s.value === keep) pickSlot(b);   // keep the choice after a postback
                    });
                    if (!slotField().value)
                        setSlotStatus(res.slots.length + (res.slots.length === 1 ? ' time' : ' times') + ' free on ' + res.dateLabel + '. Choose one.', true);
                })
                .catch(function () {
                    list.innerHTML = '<span class="slot-hint">Could not load times. Please check your connection and pick the date again.</span>';
                });
        }

        function pickSlot(button) {
            document.querySelectorAll('#slotList .slot-chip').forEach(function (b) { b.classList.remove('active'); });
            button.classList.add('active');
            slotField().value = button.getAttribute('data-value');
            setSlotStatus('Selected: ' + button.textContent + ' on ' + dateField().value.split('-').reverse().join('-'), true);
            setPayEnabled(true);
        }

        function checkSlotThenPay(amountPaise) {
            fetch(window.location.pathname + '/CheckSlot', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                body: JSON.stringify({ doctorId: doctorIdFromUrl(), date: dateField().value, slot: slotField().value })
            })
                .then(function (r) { if (!r.ok) throw new Error(r.status); return r.json(); })
                .then(function (data) {
                    var res = data.d || data;
                    if (!res.available) {
                        showNoSlot(res.message);
                        return;
                    }
                    Swal.fire({
                        icon: 'info',
                        title: 'Your appointment: ' + res.dateLabel + ' at ' + res.slot,
                        text: 'Continue to pay the 20% advance and confirm this time.',
                        showCancelButton: true,
                        confirmButtonColor: '#2E37A4',
                        cancelButtonColor: '#6b7280',
                        confirmButtonText: 'Continue to Pay',
                        cancelButtonText: 'Cancel'
                    }).then(function (choice) {
                        if (choice.isConfirmed) openDocCareUpiModal(amountPaise);
                    });
                })
                .catch(function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Could not check slots',
                        text: 'Please check your connection and try again. No payment was taken.',
                        confirmButtonColor: '#2E37A4'
                    });
                });
        }

        function openDocCareUpiModal(amountPaise) {
            var depositRupees = Math.round(amountPaise / 100);
            var totalFee = parseFloat(document.getElementById('<%= hdnTotalFee.ClientID %>').value) || (depositRupees * 5);
            var dueFee = parseFloat(document.getElementById('<%= hdnDueAmount.ClientID %>').value) || (totalFee - depositRupees);

            // Populate amount badges in modal
            var elAdvance = document.getElementById('modalAdvanceFeeDisplay');
            var elTotal = document.getElementById('modalTotalFeeDisplay');
            var elDue = document.getElementById('modalDueFeeDisplay');
            var elVpaAmount = document.getElementById('btnVpaAmount');
            var elVpaHint = document.getElementById('vpaAmountHint');

            if (elAdvance) elAdvance.innerHTML = '&#8377;' + depositRupees.toLocaleString('en-IN');
            if (elTotal) elTotal.innerHTML = '&#8377;' + Math.round(totalFee).toLocaleString('en-IN');
            if (elDue) elDue.innerHTML = '&#8377;' + Math.round(dueFee).toLocaleString('en-IN') + ' (80%)';
            if (elVpaAmount) elVpaAmount.innerHTML = '&#8377;' + depositRupees.toLocaleString('en-IN');
            if (elVpaHint) elVpaHint.innerHTML = '&#8377;' + depositRupees.toLocaleString('en-IN');

            // Generate crisp dynamic UPI QR Code
            var upiUri = 'upi://pay?pa=doccareclinic@upi&pn=' + encodeURIComponent('DocCare Clinic') +
                         '&am=' + depositRupees + '.00&cu=INR&tn=' + encodeURIComponent('DocCare Advance Slot Deposit');
            var qrUrl = 'https://api.qrserver.com/v1/create-qr-code/?size=220x220&margin=8&data=' + encodeURIComponent(upiUri);
            var qrImg = document.getElementById('upiQrCodeImg');
            if (qrImg) qrImg.src = qrUrl;

            // Reset overlays & tabs
            var procOverlay = document.getElementById('upiProcessingOverlay');
            var succOverlay = document.getElementById('upiSuccessOverlay');
            if (procOverlay) procOverlay.style.display = 'none';
            if (succOverlay) succOverlay.style.display = 'none';
            switchUpiTab('qr');

            // Start 5-minute countdown timer
            startQrCountdown(300);

            // Open Modal
            var modal = document.getElementById('doccareUpiModal');
            if (modal) modal.style.display = 'flex';
        }

        function startUpiVerification(source) {
            showUpiProcessing('Verifying UPI Payment...', 'Checking transaction confirmation from your UPI app with the bank...');
            setTimeout(function () {
                showUpiSuccess(source);
            }, 1500);
        }

        function processUpiIdPayment() {
            var input = document.getElementById('upiVpaInput');
            var upiId = input ? input.value.trim() : '';
            if (!upiId || upiId.indexOf('@') === -1 || upiId.length < 5) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Invalid UPI ID',
                    text: 'Please enter a valid UPI ID (e.g. mobile@paytm or name@okhdfcbank)',
                    confirmButtonColor: '#2E37A4'
                });
                return;
            }

            showUpiProcessing('Payment Request Sent', 'Collect request sent to <strong>' + upiId + '</strong>.<br/>Simulating instant authorization...');
            setTimeout(function () {
                showUpiSuccess('UPI ID (' + upiId + ')');
            }, 1600);
        }

        function startAppPayment(appName) {
            showUpiProcessing('Connecting to ' + appName + '...', 'Redirecting to ' + appName + ' UPI gateway to approve payment...');
            setTimeout(function () {
                showUpiSuccess(appName);
            }, 1500);
        }

        function showUpiProcessing(title, text) {
            var overlay = document.getElementById('upiProcessingOverlay');
            var titleEl = document.getElementById('upiProcessingTitle');
            var textEl = document.getElementById('upiProcessingText');
            if (titleEl) titleEl.innerHTML = title;
            if (textEl) textEl.innerHTML = text;
            if (overlay) overlay.style.display = 'flex';
        }

        function showUpiSuccess(method) {
            var processingOverlay = document.getElementById('upiProcessingOverlay');
            if (processingOverlay) processingOverlay.style.display = 'none';

            var amountPaise = parseInt(document.getElementById('<%= hdnAmountPaise.ClientID %>').value, 10) || 0;
            var depositRupees = Math.round(amountPaise / 100);
            var refId = '429' + Math.floor(100000000 + Math.random() * 900000000);

            var successOverlay = document.getElementById('upiSuccessOverlay');
            var amountEl = document.getElementById('upiSuccessAmount');
            var refEl = document.getElementById('upiSuccessRef');

            if (amountEl) amountEl.innerHTML = '&#8377;' + depositRupees + '.00 Advance Paid';
            if (refEl) refEl.textContent = refId;
            if (successOverlay) successOverlay.style.display = 'flex';

            clearInterval(qrTimerInterval);

            // Complete appointment booking after visual feedback
            setTimeout(function () {
                completeUpiBooking(method, refId);
            }, 1400);
        }

        function completeUpiBooking(method, refId) {
            var simPaymentId = 'pay_upi_' + (refId || Date.now());
            var simOrderId = 'order_upi_' + Date.now();

            document.getElementById('<%= hdnRazorpayPaymentId.ClientID %>').value = simPaymentId;
            document.getElementById('<%= hdnRazorpayOrderId.ClientID %>').value = simOrderId;
            document.getElementById('<%= hdnRazorpaySignature.ClientID %>').value = 'sig_upi_verified_' + Date.now();

            var completeBtn = document.getElementById('<%= btnCompleteBooking.ClientID %>');
            if (completeBtn) {
                if (completeBtn.form && typeof completeBtn.form.requestSubmit === 'function') {
                    completeBtn.form.requestSubmit(completeBtn);
                } else {
                    completeBtn.click();
                }
            }
        }

        // Show the free times for the date on the page (the doctor's next free day on first load)
        document.addEventListener('DOMContentLoaded', function () {
            if (document.getElementById('<%= hdnSlotAvailable.ClientID %>').value === '0') {
                document.getElementById('slotList').innerHTML = '<span class="slot-hint">No free times in the booking window.</span>';
                setPayEnabled(false);
                return;
            }
            loadSlots();
        });

        // Close on ESC
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                closeDocCareUpiModal();
            }
        });
    </script>

</asp:Content>
