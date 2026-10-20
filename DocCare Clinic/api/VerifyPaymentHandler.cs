using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using DocCare_Clinic.Config;

namespace DocCare_Clinic.Api
{
    public class VerifyPaymentHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.TrySkipIisCustomErrors = true;
            context.Response.ContentType = "application/json; charset=utf-8";
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            // Only allow POST
            if (!context.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 405; // Method Not Allowed
                context.Response.Write("{\"status\":\"failure\",\"message\":\"Method not allowed. Only POST is accepted.\"}");
                return;
            }

            try
            {
                string orderId = string.Empty;
                string paymentId = string.Empty;
                string signature = string.Empty;

                // Read request body safely
                string requestBody = string.Empty;
                try
                {
                    if (context.Request.InputStream.CanSeek)
                    {
                        context.Request.InputStream.Position = 0;
                    }
                    using (StreamReader reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                    {
                        requestBody = reader.ReadToEnd();
                    }
                }
                catch { }

                if (!string.IsNullOrEmpty(requestBody))
                {
                    try
                    {
                        Dictionary<string, object> dict = serializer.Deserialize<Dictionary<string, object>>(requestBody);
                        if (dict != null)
                        {
                            foreach (KeyValuePair<string, object> kvp in dict)
                            {
                                string key = (kvp.Key ?? "").Trim().ToLowerInvariant();
                                string val = kvp.Value != null ? kvp.Value.ToString() : string.Empty;

                                if (key == "razorpay_order_id" || key == "order_id")
                                    orderId = val;
                                else if (key == "razorpay_payment_id" || key == "payment_id")
                                    paymentId = val;
                                else if (key == "razorpay_signature" || key == "signature")
                                    signature = val;
                            }
                        }
                    }
                    catch { }

                    // Regex fallback
                    if (string.IsNullOrEmpty(orderId))
                    {
                        Match m = Regex.Match(requestBody, @"(?i)""(?:razorpay_order_id|order_id)""\s*:\s*""([^""]+)""");
                        if (m.Success) orderId = m.Groups[1].Value;
                    }
                    if (string.IsNullOrEmpty(paymentId))
                    {
                        Match m = Regex.Match(requestBody, @"(?i)""(?:razorpay_payment_id|payment_id)""\s*:\s*""([^""]+)""");
                        if (m.Success) paymentId = m.Groups[1].Value;
                    }
                    if (string.IsNullOrEmpty(signature))
                    {
                        Match m = Regex.Match(requestBody, @"(?i)""(?:razorpay_signature|signature)""\s*:\s*""([^""]+)""");
                        if (m.Success) signature = m.Groups[1].Value;
                    }
                }

                // Check form or query string fallback
                if (string.IsNullOrEmpty(orderId))
                {
                    orderId = context.Request["razorpay_order_id"] ?? context.Request["order_id"] ?? string.Empty;
                }
                if (string.IsNullOrEmpty(paymentId))
                {
                    paymentId = context.Request["razorpay_payment_id"] ?? context.Request["payment_id"] ?? string.Empty;
                }
                if (string.IsNullOrEmpty(signature))
                {
                    signature = context.Request["razorpay_signature"] ?? context.Request["signature"] ?? string.Empty;
                }

                // Validate missing fields: return 400
                if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(signature))
                {
                    context.Response.StatusCode = 400; // Bad Request
                    context.Response.Write("{\"status\":\"failure\",\"message\":\"Missing required fields: razorpay_order_id, razorpay_payment_id, and razorpay_signature are required.\"}");
                    return;
                }

                // Verify signature using HMAC-SHA256(order_id + "|" + payment_id, KEY_SECRET)
                bool isValid = RazorpayConfig.VerifySignature(orderId, paymentId, signature);

                if (isValid)
                {
                    context.Response.StatusCode = 200; // OK
                    context.Response.Write("{\"status\":\"success\",\"message\":\"Payment signature verified successfully.\"}");
                }
                else
                {
                    // Signature mismatch: return 400, do NOT mark as paid
                    context.Response.StatusCode = 400; // Bad Request
                    context.Response.Write("{\"status\":\"failure\",\"message\":\"Signature verification failed. Potential payment tampering detected.\"}");
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.Write("{\"status\":\"failure\",\"message\":\"Internal error during verification: " + CleanJsonString(ex.Message) + "\"}");
            }
        }

        private static string CleanJsonString(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace("\\", "\\\\").Replace("\"", "'").Replace("\r", "").Replace("\n", " ");
        }
    }
}
