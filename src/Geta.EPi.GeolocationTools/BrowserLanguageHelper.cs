using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geta.EPi.GeolocationTools
{
    public static class BrowserLanguageHelper
    {
        public static IEnumerable<string> GetBrowserLanguages(HttpRequestBase request)
        {
            return (request.UserLanguages ?? Enumerable.Empty<string>())
                .Select(CleanupUserLanguage);
        }

        public static string CleanupUserLanguage(string requestUserLanguage)
        {
            return requestUserLanguage?.Split(';').FirstOrDefault()?.Trim() ?? string.Empty;
        }
    }
}