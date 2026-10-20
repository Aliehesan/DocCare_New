using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using DocCare_Clinic.Config;

namespace DocCare_Clinic.Api
{
    public class CreateOrderHandler : IHttpHandler
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
                context.Response.Write("{\"error\":\"Method not allowed. Only POST is accepted.\"}");
                return;
            }

            try
            {
                // Ensure TLS 1.2 is supported for Razorpay API calls in .NET 4.0
                try
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls;
                }
                catch { }

                long amountPaise = 0;
                string currency = "INR";
                string receipt = null;

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

                // 1. Try parsing JSON body
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
                                if (key == "amount" && kvp.Value != null)
                                {
                                    long.TryParse(kvp.Value.ToString(), out amountPaise);
                                }
                                else if (key == "currency" && kvp.Value != null)
                                {
                                    currency = kvp.Value.ToString();
                                }
                                else if (key == "receipt" && kvp.Value != null)
                                {
                                    receipt = kvp.Value.ToString();
                                }
                            }
                        }
                    }
                    catch { }

                    // Regex fallback if JSON serializer didn't extract amount
                    if (amountPaise <= 0)
                    {
                        Match matchAmount = Regex.Match(requestBody, @"(?i)""amount""\s*:\s*(\d+)");
                        if (matchAmount.Success)
                        {
                            long.TryParse(matchAmount.Groups[1].Value, out amountPaise);
                        }
                    }
                }

                // 2. Fallback to Form / QueryString parameters
                if (amountPaise <= 0 && !string.IsNullOrEmpty(context.Request["amount"]))
                {
                    long.TryParse(context.Request["amount"], out amountPaise);
                }
                if (!string.IsNullOrEmpty(context.Request["currency"]))
                {
                    currency = context.Request["currency"];
                }
                if (string.IsNullOrEmpty(receipt) && !string.IsNullOrEmpty(context.Request["receipt"]))
                {
                    receipt = context.Request["receipt"];
                }

                if (string.IsNullOrEmpty(receipt))
                {
                    receipt = "rcpt_" + DateTime.UtcNow.Ticks.ToString();
                }

                // Minimum amount validation: 100 paise (Rs. 1.00)
                if (amountPaise < 100)
                {
                    context.Response.StatusCode = 400; // Bad Request
                    context.Response.Write("{\"error\":\"Validation failed. Minimum amount is 100 paise (Rs. 1.00).\"}");
                    return;
                }

                string keyId = RazorpayConfig.KeyId;
                string keySecret = RazorpayConfig.KeySecret;

                if (string.IsNullOrEmpty(keyId) || string.IsNullOrEmpty(keySecret))
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    context.Response.Write("{\"error\":\"Razorpay credentials are not configured. Please check environment variables or Web.config.\"}");
                    return;
                }

                // Prepare Razorpay Order Request
                string razorpayApiUrl = "https://api.razorpay.com/v1/orders";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(razorpayApiUrl);
                request.Method = "POST";
                request.ContentType = "application/json";

                // HTTP Basic Authentication: base64(KEY_ID:KEY_SECRET)
                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes(keyId + ":" + keySecret));
                request.Headers["Authorization"] = "Basic " + authInfo;

                string postJson = serializer.Serialize(new Dictionary<string, object>
                {
                    { "amount", amountPaise },
                    { "currency", currency },
                    { "receipt", receipt }
                });

                byte[] postBytes = Encoding.UTF8.GetBytes(postJson);
                request.ContentLength = postBytes.Length;

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(postBytes, 0, postBytes.Length);
                }

                // Execute Razorpay API call
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader respReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        string responseJson = respReader.ReadToEnd();
                        Dictionary<string, object> orderData = serializer.Deserialize<Dictionary<string, object>>(responseJson);

                        string orderId = orderData.ContainsKey("id") ? orderData["id"].ToString() : string.Empty;
                        object respAmount = orderData.ContainsKey("amount") ? orderData["amount"] : amountPaise;
                        object respCurrency = orderData.ContainsKey("currency") ? orderData["currency"] : currency;

                        Dictionary<string, object> result = new Dictionary<string, object>
                        {
                            { "order_id", orderId },
                            { "amount", respAmount },
                            { "currency", respCurrency }
                        };

                        context.Response.StatusCode = 200;
                        context.Response.Write(serializer.Serialize(result));
                    }
                }
            }
            catch (WebException webEx)
            {
                HttpWebResponse errResp = webEx.Response as HttpWebResponse;
                if (errResp != null)
                {
                    string errContent = string.Empty;
                    using (StreamReader r = new StreamReader(errResp.GetResponseStream(), Encoding.UTF8))
                    {
                        errContent = r.ReadToEnd();
                    }

                    if (errResp.StatusCode == HttpStatusCode.Unauthorized ||
                        (errContent != null && errContent.IndexOf("Authentication failed", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        try
                        {
                            System.Reflection.PropertyInfo prop = context.Response.GetType().GetProperty("SuppressFormsAuthenticationRedirect");
                            if (prop != null) prop.SetValue(context.Response, true, null);
                        }
                        catch { }

                        context.Response.StatusCode = 401; // Auth failure
                        context.Response.Write("{\"error\":\"Razorpay authentication failed. Please verify your RAZORPAY_KEY_ID and RAZORPAY_KEY_SECRET in Web.config.\",\"code\":\"AUTH_FAILED\"}");
                        return;
                    }

                    context.Response.StatusCode = 500; // API error
                    context.Response.Write("{\"error\":\"Razorpay API error: " + CleanJsonString(webEx.Message) + "\"}");
                    return;
                }

                context.Response.StatusCode = 500;
                context.Response.Write("{\"error\":\"Razorpay request error: " + CleanJsonString(webEx.Message) + "\"}");
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.Write("{\"error\":\"Internal server error: " + CleanJsonString(ex.Message) + "\"}");
            }
        }

        private static string CleanJsonString(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace("\\", "\\\\").Replace("\"", "'").Replace("\r", "").Replace("\n", " ");
        }
    }
}
