using System.Globalization;

namespace VetAmb.Middleware
{
    /// <summary>
    /// Runs after UseRequestLocalization and injects Croatian month names into the
    /// active CultureInfo, regardless of the browser's Accept-Language setting.
    /// Structural formatting rules (separators, order) still come from the
    /// resolved culture, but every textual month name is always Croatian.
    /// </summary>
    public class CroatianMonthsMiddleware
    {
        // Nominative  — used by MMMM in most contexts
        private static readonly string[] FullMonths =
        {
            "Siječanj", "Veljača", "Ožujak", "Travanj", "Svibanj", "Lipanj",
            "Srpanj",   "Kolovoz", "Rujan",  "Listopad","Studeni", "Prosinac", ""
        };

        // Abbreviated — used by MMM
        private static readonly string[] AbbrMonths =
        {
            "Sij", "Velj", "Ožu", "Tra", "Svi", "Lip",
            "Srp", "Kol", "Ruj", "Lis", "Stu", "Pro", ""
        };

        // Genitive    — used by MMMM inside "dd. MMMM yyyy." (Croatian grammar rule)
        private static readonly string[] GenitiveMonths =
        {
            "siječnja",  "veljače",  "ožujka",    "travnja",  "svibnja",   "lipnja",
            "srpnja",    "kolovoza", "rujna",      "listopada","studenog",  "prosinca", ""
        };

        private static readonly string[] AbbrGenitiveMonths =
        {
            "sij", "velj", "ožu", "tra", "svi", "lip",
            "srp", "kol", "ruj", "lis", "stu", "pro", ""
        };

        private readonly RequestDelegate _next;

        public CroatianMonthsMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            // Clone current culture (already resolved by UseRequestLocalization)
            // and replace only the month name arrays.
            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            var dtf = culture.DateTimeFormat;
            dtf.MonthNames                  = FullMonths;
            dtf.AbbreviatedMonthNames       = AbbrMonths;
            dtf.MonthGenitiveNames          = GenitiveMonths;
            dtf.AbbreviatedMonthGenitiveNames = AbbrGenitiveMonths;

            CultureInfo.CurrentCulture      = culture;
            CultureInfo.CurrentUICulture    = culture;

            await _next(context);
        }
    }
}
