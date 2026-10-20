using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;

namespace DocCare_Clinic.Config
{
    public static class RazorpayConfig
    {
        private static string _keyId;
        private static string _keySecret;
        private static bool _initialized = false;
        private static readonly object _lock = new object();

        private static void Initialize()
        {
            if (_initialized) return;

            lock (_lock)
            {
                if (_initialized) return;

                // 1. Try Environment Variables
                _keyId = Environment.GetEnvironmentVariable("RAZORPAY_KEY_ID");
                _keySecret = Environment.GetEnvironmentVariable("RAZORPAY_KEY_SECRET");

                // 2. Try loading from .env file if not set in environment
                if (string.IsNullOrEmpty(_keyId) || string.IsNullOrEmpty(_keySecret))
                {
                    LoadFromDotEnv();
                }

                // 3. Fallback to Web.config AppSettings
                if (string.IsNullOrEmpty(_keyId))
                {
                    _keyId = ConfigurationManager.AppSettings["RazorpayKeyId"];
                }
                if (string.IsNullOrEmpty(_keySecret))
                {
                    _keySecret = ConfigurationManager.AppSettings["RazorpayKeySecret"];
                }

                // 4. Fallback to directly reading Web.config file if running outside ASP.NET host
                if (string.IsNullOrEmpty(_keyId) || string.IsNullOrEmpty(_keySecret))
                {
                    LoadFromWebConfigDirect();
                }

                _initialized = true;
            }
        }

        private static void LoadFromDotEnv()
        {
            try
            {
                List<string> candidateDirs = new List<string>();

                // ASP.NET AppDomain app path
                try
                {
                    if (HttpRuntime.AppDomainAppPath != null)
                        candidateDirs.Add(HttpRuntime.AppDomainAppPath);
                }
                catch { }

                // AppDomain BaseDirectory
                if (!string.IsNullOrEmpty(AppDomain.CurrentDomain.BaseDirectory))
                    candidateDirs.Add(AppDomain.CurrentDomain.BaseDirectory);

                // Current Working Directory
                try
                {
                    string curDir = Directory.GetCurrentDirectory();
                    if (!string.IsNullOrEmpty(curDir)) candidateDirs.Add(curDir);
                }
                catch { }

                // Assembly Location Directory and its parents
                try
                {
                    string asmPath = typeof(RazorpayConfig).Assembly.Location;
                    if (!string.IsNullOrEmpty(asmPath))
                    {
                        string dir = Path.GetDirectoryName(asmPath);
                        if (!string.IsNullOrEmpty(dir))
                        {
                            candidateDirs.Add(dir);
                            DirectoryInfo parent1 = Directory.GetParent(dir);
                            if (parent1 != null) candidateDirs.Add(parent1.FullName);
                            if (parent1 != null && parent1.Parent != null) candidateDirs.Add(parent1.Parent.FullName);
                        }
                    }
                }
                catch { }

                foreach (string dir in candidateDirs)
                {
                    string[] envFilePaths = new string[]
                    {
                        Path.Combine(dir, ".env"),
                        Path.Combine(dir, "..", ".env"),
                        Path.Combine(dir, "DocCare Clinic", ".env")
                    };

                    foreach (string envPath in envFilePaths)
                    {
                        try
                        {
                            string fullPath = Path.GetFullPath(envPath);
                            if (File.Exists(fullPath))
                            {
                                string[] lines = File.ReadAllLines(fullPath);
                                foreach (string line in lines)
                                {
                                    string trimmed = line.Trim();
                                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                                        continue;

                                    int idx = trimmed.IndexOf('=');
                                    if (idx > 0)
                                    {
                                        string key = trimmed.Substring(0, idx).Trim();
                                        string val = trimmed.Substring(idx + 1).Trim().Trim('"', '\'');

                                        if (key.Equals("RAZORPAY_KEY_ID", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(_keyId))
                                        {
                                            _keyId = val;
                                        }
                                        else if (key.Equals("RAZORPAY_KEY_SECRET", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(_keySecret))
                                        {
                                            _keySecret = val;
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(_keyId) && !string.IsNullOrEmpty(_keySecret))
                                    return;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        private static void LoadFromWebConfigDirect()
        {
            try
            {
                string asmPath = typeof(RazorpayConfig).Assembly.Location;
                string dir = Path.GetDirectoryName(asmPath);
                DirectoryInfo parent = Directory.GetParent(dir);
                string webConfigPath = parent != null ? Path.Combine(parent.FullName, "Web.config") : string.Empty;

                if (!File.Exists(webConfigPath))
                {
                    webConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.config");
                }

                if (File.Exists(webConfigPath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(webConfigPath);

                    XmlNode keyNode = doc.SelectSingleNode("//appSettings/add[@key='RazorpayKeyId']");
                    if (keyNode != null && keyNode.Attributes["value"] != null && string.IsNullOrEmpty(_keyId))
                    {
                        _keyId = keyNode.Attributes["value"].Value;
                    }

                    XmlNode secretNode = doc.SelectSingleNode("//appSettings/add[@key='RazorpayKeySecret']");
                    if (secretNode != null && secretNode.Attributes["value"] != null && string.IsNullOrEmpty(_keySecret))
                    {
                        _keySecret = secretNode.Attributes["value"].Value;
                    }
                }
            }
            catch { }
        }

        public static string KeyId
        {
            get
            {
                if (!_initialized) Initialize();
                return _keyId ?? string.Empty;
            }
        }

        public static string KeySecret
        {
            get
            {
                if (!_initialized) Initialize();
                return _keySecret ?? string.Empty;
            }
        }

        /// <summary>
        /// Calculates the HMAC-SHA256 hex string for verifying Razorpay payment signature.
        /// Algorithm: HMAC-SHA256(order_id + "|" + payment_id, KEY_SECRET)
        /// </summary>
        public static string ComputeHmacSha256(string data, string secret)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(secret))
                return string.Empty;

            byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
            {
                byte[] hash = hmac.ComputeHash(dataBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Verifies whether the Razorpay signature matches HMAC-SHA256(orderId + "|" + paymentId, secret)
        /// </summary>
        public static bool VerifySignature(string orderId, string paymentId, string signature)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(signature))
                return false;

            string payload = orderId + "|" + paymentId;
            string expectedSignature = ComputeHmacSha256(payload, KeySecret);

            return expectedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase);
        }
    }
}
