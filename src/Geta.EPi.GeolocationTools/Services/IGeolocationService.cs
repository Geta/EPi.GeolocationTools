using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Routing;
using EPiServer.DataAbstraction;
using EPiServer.Personalization;

namespace Geta.EPi.GeolocationTools.Services
{
    public interface IGeolocationService
    {
        /// <summary>
        /// Gets the language based on the users' location and their browser preferences, depending on what is available.
        /// 1. Language branch for both the users' country and their browser preferences
        /// 2. Language branch for users' browser preferences
        /// 3. Fallback language (by Episerver)
        /// </summary>
        LanguageBranch GetLanguage(HttpRequestBase requestBase);
        
        /// <summary>
        /// Gets the language based on the users' location.
        /// 1. Language branch for the users country
        /// </summary>
        LanguageBranch GetLanguageByCountry(IGeolocationResult location);

        /// <summary>
        /// Gets the language based on the users' browser preferences.
        /// 1. Language branch for users' browser preferences
        /// </summary>
        LanguageBranch GetLanguageByBrowserPreferences(IEnumerable<string> userBrowserLanguages);

        /// <summary>
        /// Gets the language based on the users' location and their browser preferences, depending on what is available.
        /// 1. Language branch for both the users' country and their browser preferences
        /// </summary>
        LanguageBranch GetLanguageByCountryAndBrowserLanguage(IGeolocationResult location, IEnumerable<string> userBrowserLanguages);

        IGeolocationResult GetLocation(HttpRequestBase requestContext);
        IGeolocationResult GetLocation(RequestContext requestContext);
        IGeolocationResult GetLocation(IPAddress ipAddress);
    }
}