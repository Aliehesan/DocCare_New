using System.Text.RegularExpressions;

namespace DocCare_Clinic.Config
{
    /// <summary>
    /// Doctor names are stored without a title; pages and emails add "Dr. " when showing them.
    /// </summary>
    public static class DoctorName
    {
        static readonly Regex Title = new Regex(@"^\s*(dr\.?|doctor)(\s+|\.\s*)", RegexOptions.IgnoreCase);

        /// <summary>"Dr. Sara Khan", "dr sara khan", "Doctor Sara Khan" -> "Sara Khan".</summary>
        public static string WithoutTitle(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            string clean = name.Trim();
            while (Title.IsMatch(clean))
                clean = Title.Replace(clean, "", 1).Trim();
            return clean == "" ? name.Trim() : clean;
        }
    }
}
