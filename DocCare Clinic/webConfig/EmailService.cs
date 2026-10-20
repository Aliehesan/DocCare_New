using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace TaskManagement.config
{
    public class EmailService
    {
        // ── SMTP Configuration ────────────────────────────────────────────────────
        private string SmtpServer    = "smtp.gmail.com";
        private int    SmtpPort      = 587;
        private string SenderEmail   = "doccare.it.2024.25@gmail.com";   // DocCare official sender
        private string SenderName    = "DocCare Clinic";
        // Gmail app password, from Secrets.config (never written in code: the code is on GitHub)
        private string SenderPassword = System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"] ?? "";

        /// <summary>
        /// Sends a simple email (plain or HTML).
        /// Returns "1" on success, or an error message on failure.
        /// </summary>
        public string SendEmail(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From    = new MailAddress(SenderEmail, SenderName);
                mail.To.Add(toEmail);
                mail.Subject    = subject;
                mail.Body       = body;
                mail.IsBodyHtml = isHtml;

                SmtpClient smtp = new SmtpClient(SmtpServer, SmtpPort)
                {
                    Credentials = new NetworkCredential(SenderEmail, SenderPassword),
                    EnableSsl   = true
                };

                smtp.Send(mail);
                return "1";
            }
            catch (Exception ex)
            {
                return "Error sending email: " + ex.Message;
            }
        }

        /// <summary>
        /// Sends an email with an attachment.
        /// </summary>
        public string SendEmailWithAttachment(string toEmail, string subject, string body,
                                              string attachmentPath, bool isHtml = true)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From    = new MailAddress(SenderEmail, SenderName);
                mail.To.Add(toEmail);
                mail.Subject    = subject;
                mail.Body       = body;
                mail.IsBodyHtml = isHtml;

                if (File.Exists(attachmentPath))
                    mail.Attachments.Add(new Attachment(attachmentPath));
                else
                    return "Error: Attachment file not found.";

                SmtpClient smtp = new SmtpClient(SmtpServer, SmtpPort)
                {
                    Credentials = new NetworkCredential(SenderEmail, SenderPassword),
                    EnableSsl   = true
                };

                smtp.Send(mail);
                return "1";
            }
            catch (Exception ex)
            {
                return "Error sending email with attachment: " + ex.Message;
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  EMAIL TEMPLATE BUILDERS
        //  Call these static methods to get a beautifully formatted HTML body.
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Generates a Welcome / Credentials email for a new Doctor.
        /// </summary>
        public static string BuildDoctorWelcomeEmail(string doctorName, string email,
                                                     string password, string specialization,
                                                     string department, string loginUrl)
        {
            return @"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'/>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
</head>
<body style='margin:0;padding:0;background-color:#f4f7f9;font-family:Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f4f7f9;padding:30px 0;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);'>

          <!-- Header -->
          <tr>
            <td style='background:linear-gradient(135deg,#00c7c7,#0099a8);padding:35px 40px;text-align:center;'>
              <h1 style='color:#ffffff;margin:0;font-size:28px;letter-spacing:1px;'>🏥 DocCare Clinic</h1>
              <p style='color:#e0f9f9;margin:6px 0 0;font-size:14px;'>Your Health, Our Priority</p>
            </td>
          </tr>

          <!-- Welcome Banner -->
          <tr>
            <td style='background:#e8fafa;padding:20px 40px;text-align:center;border-bottom:2px solid #00c7c7;'>
              <h2 style='color:#0099a8;margin:0;font-size:20px;'>Welcome to DocCare, Dr. " + doctorName + @"! 🎉</h2>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style='padding:35px 40px;'>
              <p style='color:#444;font-size:15px;margin:0 0 20px;'>
                Dear <strong>Dr. " + doctorName + @"</strong>,<br/><br/>
                We are delighted to welcome you to the <strong>DocCare Clinic</strong> family.
                Your account has been created successfully. Below are your login credentials
                to access your Doctor Dashboard.
              </p>

              <!-- Info Card -->
              <table width='100%' cellpadding='0' cellspacing='0'
                     style='background:#f0fafa;border:1px solid #b2e8e8;border-radius:10px;margin:0 0 25px;'>
                <tr>
                  <td style='padding:20px 25px;'>
                    <p style='margin:0 0 8px;font-size:13px;color:#888;text-transform:uppercase;letter-spacing:1px;'>Your Details</p>
                    <table width='100%' cellpadding='6' cellspacing='0' style='font-size:14px;color:#333;'>
                      <tr>
                        <td style='width:40%;color:#666;'>👤 Name</td>
                        <td><strong>Dr. " + doctorName + @"</strong></td>
                      </tr>
                      <tr style='background:#e0f5f5;border-radius:4px;'>
                        <td style='color:#666;'>🏬 Department</td>
                        <td><strong>" + department + @"</strong></td>
                      </tr>
                      <tr>
                        <td style='color:#666;'>🩺 Specialization</td>
                        <td><strong>" + specialization + @"</strong></td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>

              <!-- Credentials Card -->
              <table width='100%' cellpadding='0' cellspacing='0'
                     style='background:#fff8e1;border:1px solid #ffe082;border-radius:10px;margin:0 0 25px;'>
                <tr>
                  <td style='padding:20px 25px;'>
                    <p style='margin:0 0 12px;font-size:13px;color:#888;text-transform:uppercase;letter-spacing:1px;'>🔐 Login Credentials</p>
                    <table width='100%' cellpadding='8' cellspacing='0' style='font-size:15px;color:#333;'>
                      <tr>
                        <td style='width:35%;color:#666;'>📧 Email</td>
                        <td><strong>" + email + @"</strong></td>
                      </tr>
                      <tr style='background:#fff3cd;border-radius:4px;'>
                        <td style='color:#666;'>🔑 Password</td>
                        <td><strong style='font-size:17px;letter-spacing:2px;'>" + password + @"</strong></td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>

              <!-- CTA Button -->
              <table width='100%' cellpadding='0' cellspacing='0'>
                <tr>
                  <td align='center' style='padding:10px 0 25px;'>
                    <a href='" + loginUrl + @"'
                       style='background:linear-gradient(135deg,#00c7c7,#0099a8);color:#ffffff;
                              text-decoration:none;padding:14px 40px;border-radius:50px;
                              font-size:15px;font-weight:bold;display:inline-block;letter-spacing:0.5px;'>
                      🚀 Login to Your Dashboard
                    </a>
                  </td>
                </tr>
              </table>

              <p style='color:#e53935;font-size:13px;background:#fff3f3;padding:12px 16px;border-radius:8px;border-left:4px solid #e53935;margin:0;'>
                ⚠️ <strong>Important:</strong> Please change your password after your first login.
                Do not share your credentials with anyone.
              </p>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style='background:#f0fafa;padding:20px 40px;text-align:center;border-top:1px solid #e0e0e0;'>
              <p style='color:#aaa;font-size:12px;margin:0;'>
                This is an automated message from <strong>DocCare Clinic</strong>.<br/>
                Please do not reply to this email.<br/><br/>
                &copy; " + DateTime.Now.Year.ToString() + @" DocCare Clinic. All rights reserved.
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }

        /// <summary>
        /// Generates an appointment email for a patient. kind:
        ///   "details"   - booking details sent by the front desk / after online payment (default)
        ///   "cancelled" - appointment cancelled (cancelledByPatient picks the wording)
        ///   "completed" - visit completed, thank-you note
        /// Fee rows are shown only for "details" when totalFee > 0; location only when known.
        /// </summary>
        public static string BuildAppointmentEmail(string patientName, string doctorName, string specialization,
                                                   DateTime appointDate, string timeSlot, string tokenNo,
                                                   decimal totalFee, decimal paidAmount, decimal dueAmount,
                                                   string location, string status,
                                                   string kind = "details", bool cancelledByPatient = false, string patientCode = null)
        {
            Func<string, string> enc = HttpUtility.HtmlEncode;

            // Heading, opening text and closing note per kind
            string banner = "Your Appointment Details 📋", bannerColor = "#0099a8", bannerBg = "#e8fafa", bannerLine = "#00c7c7";
            string intro = "Thank you for booking with <strong>DocCare Clinic</strong>. Here are the details of your appointment.";
            string noteStyle = "color:#0b6b74;background:#e8fafa;border-left:4px solid #00c7c7;";
            string note = "⏱️ <strong>Please arrive 10 minutes before your time slot</strong> and show your token number at the reception desk. Bring any previous prescriptions or reports.";
            bool showFees = true;

            if (kind == "cancelled" || kind == "doctorunavailable")
            {
                banner = "Appointment Cancelled ❌"; bannerColor = "#c62828"; bannerBg = "#fdecea"; bannerLine = "#e53935";
                intro = kind == "doctorunavailable"
                    ? "We are sorry to let you know that <strong>Dr. " + enc(doctorName) + "</strong> is not available for your appointment on " +
                      appointDate.ToString("dddd, dd MMM yyyy") + ", so it has been cancelled. Please book another time on our website."
                    : cancelledByPatient
                    ? "As you requested, your appointment at <strong>DocCare Clinic</strong> has been cancelled."
                    : "We are sorry to let you know that your appointment at <strong>DocCare Clinic</strong> has been cancelled by the clinic.";
                noteStyle = "color:#8a1c1c;background:#fff3f3;border-left:4px solid #e53935;";
                note = paidAmount > 0
                    ? "💳 You paid an advance of <strong>&#8377;" + paidAmount.ToString("N0") + "</strong> online for this appointment. Please contact the clinic reception about your refund."
                    : "📅 If you would still like to see a doctor, you can book a new appointment on our website at any time.";
                showFees = false;
            }
            else if (kind == "completed")
            {
                banner = "Thank You for Visiting 💙";
                intro = "Your appointment with <strong>Dr. " + enc(doctorName) + "</strong> has been completed. We hope you are feeling better.";
                note = "🩺 If your doctor asked for a follow-up visit, you can book your next appointment on our website. Wishing you good health!";
                showFees = false;
            }
            else if (kind == "reminder")
            {
                banner = "Appointment Reminder ⏰";
                intro = "This is a reminder of your appointment <strong>tomorrow</strong> with <strong>Dr. " + enc(doctorName) + "</strong>.";
                note = "⏱️ <strong>Please arrive 10 minutes before your time slot</strong> and show your token number at the reception desk. " +
                       "Need to change it? You can reschedule or cancel on our website up to 2 hours before the time.";
            }
            string row = @"
                      <tr{0}>
                        <td style='width:40%;color:#666;'>{1}</td>
                        <td><strong>{2}</strong></td>
                      </tr>";
            string shade = " style='background:#e0f5f5;'";

            string details =
                string.Format(row, "", "👤 Patient", enc(patientName) + (string.IsNullOrEmpty(patientCode) ? "" : " &nbsp;<span style=\"color:#6b7280;\">(Patient ID " + enc(patientCode) + ")</span>")) +
                string.Format(row, shade, "👨‍⚕️ Doctor", "Dr. " + enc(doctorName)) +
                string.Format(row, "", "🩺 Specialization", enc(specialization)) +
                string.Format(row, shade, "📅 Date", appointDate.ToString("dddd, dd MMM yyyy")) +
                string.Format(row, "", "⏰ Time Slot", enc(timeSlot)) +
                string.Format(row, shade, "🎫 Token No.", enc(tokenNo)) +
                string.Format(row, "", (status == "Cancelled" ? "❌" : status == "Pending" ? "⏳" : "✅") + " Status", enc(status)) +
                (string.IsNullOrEmpty(location) ? "" : string.Format(row, shade, "📍 Location", enc(location)));

            string fees = !showFees || totalFee <= 0 ? "" : @"
              <table width='100%' cellpadding='0' cellspacing='0'
                     style='background:#fff8e1;border:1px solid #ffe082;border-radius:10px;margin:0 0 25px;'>
                <tr>
                  <td style='padding:20px 25px;'>
                    <p style='margin:0 0 12px;font-size:13px;color:#888;text-transform:uppercase;letter-spacing:1px;'>💳 Payment</p>
                    <table width='100%' cellpadding='6' cellspacing='0' style='font-size:14px;color:#333;'>" +
                      string.Format(row, "", "Consultation Fee", "&#8377;" + totalFee.ToString("N0")) +
                      string.Format(row, " style='background:#fff3cd;'", "Paid Online (Advance)", "&#8377;" + paidAmount.ToString("N0")) +
                      string.Format(row, "", "Pay at Clinic", "&#8377;" + dueAmount.ToString("N0")) + @"
                    </table>
                  </td>
                </tr>
              </table>";

            return @"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'/>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
</head>
<body style='margin:0;padding:0;background-color:#f4f7f9;font-family:Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f4f7f9;padding:30px 0;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);'>

          <!-- Header -->
          <tr>
            <td style='background:linear-gradient(135deg,#00c7c7,#0099a8);padding:35px 40px;text-align:center;'>
              <h1 style='color:#ffffff;margin:0;font-size:28px;letter-spacing:1px;'>🏥 DocCare Clinic</h1>
              <p style='color:#e0f9f9;margin:6px 0 0;font-size:14px;'>Your Health, Our Priority</p>
            </td>
          </tr>

          <!-- Banner -->
          <tr>
            <td style='background:" + bannerBg + @";padding:20px 40px;text-align:center;border-bottom:2px solid " + bannerLine + @";'>
              <h2 style='color:" + bannerColor + @";margin:0;font-size:20px;'>" + banner + @"</h2>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style='padding:35px 40px;'>
              <p style='color:#444;font-size:15px;margin:0 0 20px;'>
                Dear <strong>" + enc(patientName) + @"</strong>,<br/><br/>
                " + intro + @"
              </p>

              <!-- Appointment Card -->
              <table width='100%' cellpadding='0' cellspacing='0'
                     style='background:#f0fafa;border:1px solid #b2e8e8;border-radius:10px;margin:0 0 25px;'>
                <tr>
                  <td style='padding:20px 25px;'>
                    <p style='margin:0 0 8px;font-size:13px;color:#888;text-transform:uppercase;letter-spacing:1px;'>Appointment</p>
                    <table width='100%' cellpadding='6' cellspacing='0' style='font-size:14px;color:#333;'>" + details + @"
                    </table>
                  </td>
                </tr>
              </table>
" + fees + @"
              <p style='" + noteStyle + @"font-size:13px;padding:12px 16px;border-radius:8px;margin:0;'>
                " + note + @"
              </p>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style='background:#f0fafa;padding:20px 40px;text-align:center;border-top:1px solid #e0e0e0;'>
              <p style='color:#aaa;font-size:12px;margin:0;'>
                This message was sent by the <strong>DocCare Clinic</strong> reception desk.<br/>
                Please do not reply to this email.<br/><br/>
                &copy; " + DateTime.Now.Year.ToString() + @" DocCare Clinic. All rights reserved.
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }

        /// <summary>
        /// Generates a Welcome / Credentials email for a new Receptionist.
        /// </summary>
        public static string BuildReceptionistWelcomeEmail(string receptionistName, string email,
                                                           string password, string loginUrl)
        {
            return @"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'/>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
</head>
<body style='margin:0;padding:0;background-color:#f4f7f9;font-family:Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f4f7f9;padding:30px 0;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);'>

          <!-- Header -->
          <tr>
            <td style='background:linear-gradient(135deg,#5c6bc0,#3949ab);padding:35px 40px;text-align:center;'>
              <h1 style='color:#ffffff;margin:0;font-size:28px;letter-spacing:1px;'>🏥 DocCare Clinic</h1>
              <p style='color:#c5cae9;margin:6px 0 0;font-size:14px;'>Your Health, Our Priority</p>
            </td>
          </tr>

          <!-- Welcome Banner -->
          <tr>
            <td style='background:#ede7f6;padding:20px 40px;text-align:center;border-bottom:2px solid #5c6bc0;'>
              <h2 style='color:#3949ab;margin:0;font-size:20px;'>Welcome to DocCare, " + receptionistName + @"! 🎉</h2>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style='padding:35px 40px;'>
              <p style='color:#444;font-size:15px;margin:0 0 25px;'>
                Dear <strong>" + receptionistName + @"</strong>,<br/><br/>
                Congratulations! You have been added as a <strong>Receptionist</strong> at
                <strong>DocCare Clinic</strong>. Your account is ready.
                Please use the credentials below to log in to your dashboard.
              </p>

              <!-- Credentials Card -->
              <table width='100%' cellpadding='0' cellspacing='0'
                     style='background:#fff8e1;border:1px solid #ffe082;border-radius:10px;margin:0 0 25px;'>
                <tr>
                  <td style='padding:20px 25px;'>
                    <p style='margin:0 0 12px;font-size:13px;color:#888;text-transform:uppercase;letter-spacing:1px;'>🔐 Login Credentials</p>
                    <table width='100%' cellpadding='8' cellspacing='0' style='font-size:15px;color:#333;'>
                      <tr>
                        <td style='width:35%;color:#666;'>👤 Name</td>
                        <td><strong>" + receptionistName + @"</strong></td>
                      </tr>
                      <tr style='background:#fff3cd;'>
                        <td style='color:#666;'>📧 Email</td>
                        <td><strong>" + email + @"</strong></td>
                      </tr>
                      <tr>
                        <td style='color:#666;'>🔑 Password</td>
                        <td><strong style='font-size:17px;letter-spacing:2px;'>" + password + @"</strong></td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>

              <!-- CTA Button -->
              <table width='100%' cellpadding='0' cellspacing='0'>
                <tr>
                  <td align='center' style='padding:10px 0 25px;'>
                    <a href='" + loginUrl + @"'
                       style='background:linear-gradient(135deg,#5c6bc0,#3949ab);color:#ffffff;
                              text-decoration:none;padding:14px 40px;border-radius:50px;
                              font-size:15px;font-weight:bold;display:inline-block;letter-spacing:0.5px;'>
                      🚀 Login to Your Dashboard
                    </a>
                  </td>
                </tr>
              </table>

              <p style='color:#e53935;font-size:13px;background:#fff3f3;padding:12px 16px;border-radius:8px;border-left:4px solid #e53935;margin:0;'>
                ⚠️ <strong>Important:</strong> Please change your password after your first login.
                Do not share your credentials with anyone.
              </p>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style='background:#ede7f6;padding:20px 40px;text-align:center;border-top:1px solid #e0e0e0;'>
              <p style='color:#aaa;font-size:12px;margin:0;'>
                This is an automated message from <strong>DocCare Clinic</strong>.<br/>
                Please do not reply to this email.<br/><br/>
                &copy; " + DateTime.Now.Year.ToString() + @" DocCare Clinic. All rights reserved.
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }

        /// <summary>
        /// Generates an OTP email for password reset.
        /// </summary>
        public static string BuildOtpEmail(string otp)
        {
            return @"
<!DOCTYPE html>
<html>
<body style='margin:0;padding:0;background:#f4f7f9;font-family:Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f4f7f9;padding:30px 0;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0' style='background:#fff;border-radius:12px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);'>
          <tr>
            <td style='background:linear-gradient(135deg,#00c7c7,#0099a8);padding:30px 40px;text-align:center;'>
              <h1 style='color:#fff;margin:0;font-size:26px;'>🏥 DocCare Clinic</h1>
            </td>
          </tr>
          <tr>
            <td style='padding:40px;text-align:center;'>
              <h2 style='color:#333;margin:0 0 10px;'>Password Reset OTP</h2>
              <p style='color:#666;font-size:15px;margin:0 0 30px;'>Use the OTP below to reset your password. Valid for <strong>5 minutes</strong>.</p>
              <div style='background:#f0fafa;border:2px dashed #00c7c7;border-radius:12px;padding:20px 40px;display:inline-block;'>
                <span style='font-size:40px;font-weight:bold;letter-spacing:12px;color:#0099a8;'>" + otp + @"</span>
              </div>
              <p style='color:#e53935;font-size:13px;margin:25px 0 0;'>If you did not request this, please ignore this email.</p>
            </td>
          </tr>
          <tr>
            <td style='background:#f0fafa;padding:20px 40px;text-align:center;border-top:1px solid #e0e0e0;'>
              <p style='color:#aaa;font-size:12px;margin:0;'>
                &copy; " + DateTime.Now.Year.ToString() + @" DocCare Clinic. All rights reserved.
              </p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }
    }
}