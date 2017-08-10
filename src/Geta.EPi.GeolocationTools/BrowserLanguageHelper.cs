using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geta.EPi.GeolocationTools
{
    internal static class BrowserLanguageHelper
    {
        /// <summary>
        /// Returns the browser locales from the request.
        /// da, en-gb;q=0.8, en;q=0.7 -> list with 'da', 'en-gb' and 'en'
        /// </summary>
        public static IEnumerable<string> GetBrowserLanguages(HttpRequestBase request)
        {
            return (request.UserLanguages ?? Enumerable.Empty<string>())
                .Select(CleanupUserLanguage)
                .Where(x => !string.IsNullOrWhiteSpace(x));
        }

        private static string CleanupUserLanguage(string requestUserLanguage)
        {
            return requestUserLanguage?.Split(';').FirstOrDefault()?.Trim() ?? string.Empty;
        }
    }
}