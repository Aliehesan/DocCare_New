using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace TaskManagement.config
{
    public class OTPService
    {
        private const int otpLength = 6;        // Length of the OTP
        private const int validDuration = 300;  // OTP valid for 5 minutes (300 seconds)

        // Generate an OTP using Random
        public static string GenerateOTP()
        {
            Random random = new Random();
            StringBuilder otp = new StringBuilder(otpLength);
            for (int i = 0; i < otpLength; i++)
            {
                otp.Append(random.Next(0, 10));
            }
            return otp.ToString();
        }

        // Verify an OTP
        public static bool VerifyOTP(string generatedOtp, string userInputOtp, DateTime otpGeneratedTime)
        {
            // ✅ Use DateTime.Now — consistent with how OTPGeneratedAt is stored in Session
            if (DateTime.Now - otpGeneratedTime > TimeSpan.FromSeconds(validDuration))
            {
                // OTP has expired
                return false;
            }

            return generatedOtp == userInputOtp;
        }
    }
}