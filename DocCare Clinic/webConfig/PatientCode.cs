using System.Text.RegularExpressions;

namespace DocCare_Clinic.Config
{
    /// <summary>
    /// The permanent Patient ID shown to staff and patients, e.g. "DC-00029" for PatientID 29.
    /// The database has the same value in Patients.PatientCode (add_patient_code.sql).
    /// </summary>
    public static class PatientCode
    {
        public const string Prefix = "DC-";

        static readonly Regex Typed = new Regex(@"^\s*(?:dc)?\s*-?\s*0*(\d{1,9})\s*$", RegexOptions.IgnoreCase);

        public static string Of(int patientId)
        {
            return Prefix + (patientId < 100000 ? patientId.ToString("00000") : patientId.ToString());
        }

        public static string Of(object patientId)
        {
            int id;
            return patientId != null && int.TryParse(patientId.ToString(), out id) ? Of(id) : "";
        }

        /// <summary>
        /// The PatientID someone typed as a Patient ID: "DC-00029", "dc29", "DC 29" -> 29.
        /// A bare number only counts with the DC prefix or leading zeros ("00029"), so a search for
        /// part of a mobile number or token is not mistaken for a patient.
        /// </summary>
        public static int? Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            Match m = Typed.Match(text);
            if (!m.Success) return null;
            string t = text.Trim();
            bool looksLikeCode = t.StartsWith("dc", System.StringComparison.OrdinalIgnoreCase) || t.StartsWith("0");
            int id;
            return looksLikeCode && int.TryParse(m.Groups[1].Value, out id) ? id : (int?)null;
        }
    }
}
