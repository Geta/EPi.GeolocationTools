using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using EPiServer.DataAbstraction;
using EPiServer.Personalization;
using EPiServer.ServiceLocation;
using EPiServer.Globalization;

namespace Geta.EPi.GeolocationTools.Services
{
    [ServiceConfiguration(ServiceType = typeof(IGeolocationService))]
    public class GeolocationService : IGeolocationService
    {
        private readonly ILanguageBranchRepository _languageBranchRepository;
        private readonly List<LanguageBranch> _enabledLanguageBranches;
        private readonly IGeolocationProvider _geolocationProvider;

        public GeolocationService(ILanguageBranchRepository languageBranchRepository, IGeolocationProvider geolocationProvider)
        {
            _languageBranchRepository = languageBranchRepository;
            _enabledLanguageBranches = _languageBranchRepository.ListEnabled().OrderBy(x => x.SortIndex).ToList();
            _geolocationProvider = geolocationProvider;
        }

        /// <summary>
        /// Gets the language based on the users' location and their browser preferences, depending on what is available.
        /// 1. Language branch for both the users' country and their browser preferences
        /// 2. Language branch for users' browser preferences
        /// 3. Fallback language
        /// </summary>
        public LanguageBranch GetLanguage(HttpRequestBase requestBase)
        {
            if (requestBase == null)
            {
                throw new ArgumentNullException(nameof(requestBase));
            }

            return
                // Search language branches for country and the users' browser preferences
                GetLanguageByCountryAndBrowserLanguage(requestBase)
                // Search language branches for the users' browser preferences
                ?? GetLanguageByBrowserPreferences(requestBase)
                // Use fallback/default
                ?? GetFallbackLanguageBranch();
        }

        /// <summary>
        /// Gets the language based on the users' location.
        /// 1. Language branch for the users country
        /// 2. null
        /// </summary>
        public LanguageBranch GetLanguageByCountry(IGeolocationResult location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            return _enabledLanguageBranches
                .Where(IsLanguageBranchForCountry(location))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the language based on the users' location.
        /// 1. Language branch for the users country
        /// 2. null
        /// </summary>
        public LanguageBranch GetLanguageByCountry(HttpRequestBase requestBase)
        {
            var location = GetLocation(requestBase);
            return location != null ? GetLanguageByCountry(location) : null;
        }

        /// <summary>
        /// Gets the language based on the users' browser preferences.
        /// 1. Language branch for users' browser preferences
        /// 2. null
        /// </summary>
        public LanguageBranch GetLanguageByBrowserPreferences(IEnumerable<string> userBrowserLanguages)
        {
            if (userBrowserLanguages == null)
            {
                throw new ArgumentNullException(nameof(userBrowserLanguages));
            }

            return _enabledLanguageBranches
                .Where(IsLanguageBranchForBrowserLanguage(userBrowserLanguages))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the language based on the users' browser preferences.
        /// 1. Language branch for users' browser preferences
        /// 2. null
        /// </summary>
        public LanguageBranch GetLanguageByBrowserPreferences(HttpRequestBase requestBase)
        {
            var browserLanguages = GetBrowserLanguages(requestBase);
            return GetLanguageByBrowserPreferences(browserLanguages);
        }

        /// <summary>
        /// Gets the language based on the users' location AND browser preferences.
        /// 1. Language branch for users' location AND browser preferences
        /// 2. null
        /// </summary>
        public LanguageBranch GetLanguageByCountryAndBrowserLanguage(IGeolocationResult location, IEnumerable<string> userBrowserLanguages)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (userBrowserLanguages == null)
            {
                throw new ArgumentNullException(nameof(userBrowserLanguages));
            }

            return _enabledLanguageBranches
                .Where(IsLanguageBranchForCountry(location))
                .Where(IsLanguageBranchForBrowserLanguage(userBrowserLanguages))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the language based on the users' location AND browser preferences.
        /// 1. Language branch for users' location AND browser preferences
        /// 2. null
        /// </summary>
        public LanguageBranch GetLanguageByCountryAndBrowserLanguage(HttpRequestBase requestBase)
        {
            var location = GetLocation(requestBase);
            var browserLanguages = GetBrowserLanguages(requestBase);
            return location != null ? GetLanguageByCountryAndBrowserLanguage(location, browserLanguages) : null;
        }

        public IGeolocationResult GetLocation(HttpRequestBase requestContext)
        {
            if (requestContext == null)
            {
                return null;
            }
            return GetLocation(requestContext.RequestContext);
        }

        public IGeolocationResult GetLocation(RequestContext requestContext)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException(nameof(requestContext));
            }

            return GetLocation(IPAddressHelper.GetRequestIpAddress(requestContext.HttpContext.Request));
        }

        public IGeolocationResult GetLocation(IPAddress ipAddress)
        {
            try
            {
                var result = _geolocationProvider.Lookup(ipAddress);
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the browser locales from the request.
        /// da, en-gb;q=0.8, en;q=0.7 -> list with 'da', 'en-gb' and 'en'
        /// </summary>
        public IEnumerable<string> GetBrowserLanguages(HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return BrowserLanguageHelper.GetBrowserLanguages(request);
        }

        private Func<LanguageBranch, bool> IsLanguageBranchForCountry(IGeolocationResult location)
        {
            return x => x.Culture.Name.Contains('-') &&
                        x.Culture.Name.EndsWith(location.CountryCode);
        }

        private Func<LanguageBranch, bool> IsLanguageBranchForBrowserLanguage(IEnumerable<string> location)
        {
            return x => location.Any(l => l.Equals(x.Culture.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        private LanguageBranch GetFallbackLanguageBranch()
        {
            return GetFallbackLanguageBranch(ContentLanguage.PreferredCulture);
        }

        private LanguageBranch GetFallbackLanguageBranch(CultureInfo cultureInfo)
        {
            return _languageBranchRepository.ListEnabled().FirstOrDefault(l => l.LanguageID.Equals(cultureInfo.Name)) ??
                   _languageBranchRepository.LoadFirstEnabledBranch();
        }
    }
}