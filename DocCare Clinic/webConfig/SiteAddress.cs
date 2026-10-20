using System;
using System.Configuration;
using System.Web;

namespace DocCare_Clinic.Config
{
    /// <summary>
    /// The website's own address (e.g. https://clinic.example.com), for links in emails sent from the
    /// background reminder timer, where there is no web request to read it from. Set "SiteUrl" in
    /// Web.config appSettings to fix it; otherwise it is remembered from the first page request.
    /// </summary>
    public static class SiteAddress
    {
        static string remembered;

        public static void Remember(HttpRequest request)
        {
            if (remembered != null || request == null) return;
            string app = request.ApplicationPath == "/" ? "" : request.ApplicationPath;
            remembered = request.Url.GetLeftPart(UriPartial.Authority) + app;
        }

        /// <summary>Root address without a trailing slash, or null when not known yet.</summary>
        public static string Root
        {
            get
            {
                string configured = ConfigurationManager.AppSettings["SiteUrl"];
                return !string.IsNullOrWhiteSpace(configured) ? configured.Trim().TrimEnd('/') : remembered;
            }
        }

        /// <summary>"~/Features/..." as a full address, or null when the address is not known.</summary>
        public static string Absolute(string appRelative)
        {
            string root = Root;
            return root == null ? null : root + "/" + appRelative.TrimStart('~', '/');
        }
    }
}
