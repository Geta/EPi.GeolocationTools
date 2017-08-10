using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using EPiServer.DataAbstraction;
using EPiServer.Globalization;
using EPiServer.Personalization;
using EPiServer.ServiceLocation;

namespace Geta.EPi.GeolocationTools.Services
{
    [ServiceConfiguration(ServiceType = typeof(IGeolocationService))]
    public class GeolocationService : IGeolocationService
    {
        private readonly ILanguageBranchRepository _languageBranchRepository;
        private readonly List<LanguageBranch> _enabledLanguageBranches;

        public GeolocationService(ILanguageBranchRepository languageBranchRepository)
        {
            _languageBranchRepository = languageBranchRepository;
            _enabledLanguageBranches = _languageBranchRepository.ListEnabled().OrderBy(x => x.SortIndex).ToList();
        }

        /// <summary>
        /// Gets the language based on the users' location and their browser preferences, depending on what is available.
        /// 1. Language branch for both the users' country and their browser preferences
        /// 2. Language branch for users' browser preferences
        /// 3. Fallback language
        /// </summary>
        public LanguageBranch GetLanguage(HttpRequestBase requestBase)
        {
            var location = GetLocation(requestBase);
            var userBrowserLanguages = BrowserLanguageHelper.GetBrowserLanguages(requestBase).ToList();
            if (location == null)
            {
                return _enabledLanguageBranches
                           .Where(IsLanguageBranchForBrowserLanguage(userBrowserLanguages))
                           .FirstOrDefault() ?? GetFallbackLanguageBranch();
            }

            return
                // Search language branches for country and the users' browser preferences
                GetLanguageByCountryAndBrowserLanguage(location, userBrowserLanguages) ??
                // Search language branches for the users' browser preferences
                GetLanguageByBrowserPreferences(userBrowserLanguages) ??
                // Use fallback/default
                GetFallbackLanguageBranch();
        }

        /// <summary>
        /// Gets the language based on the users' location.
        /// 1. Language branch for the users country
        /// 2. null
        /// </summary>
        public LanguageBranch GetLanguageByCountry(IGeolocationResult location)
        {
            return _enabledLanguageBranches
                .Where(IsLanguageBranchForCountry(location))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the language based on the users' browser preferences.
        /// 1. Language branch for users' browser preferences
        /// 2. null
        /// </summary>
        public LanguageBranch GetLanguageByBrowserPreferences(IEnumerable<string> userBrowserLanguages)
        {
            return _enabledLanguageBranches
                .Where(IsLanguageBranchForBrowserLanguage(userBrowserLanguages))
                .FirstOrDefault();
        }

        public LanguageBranch GetLanguageByCountryAndBrowserLanguage(IGeolocationResult location, IEnumerable<string> userBrowserLanguages)
        {
            return _enabledLanguageBranches
                .Where(IsLanguageBranchForCountry(location))
                .Where(IsLanguageBranchForBrowserLanguage(userBrowserLanguages))
                .FirstOrDefault();
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
            return GetLocation(IPAddressHelper.GetRequestIpAddress(requestContext.HttpContext.Request));
        }

        public IGeolocationResult GetLocation(IPAddress ipAddress)
        {
            try
            {
                var provider = Geolocation.Provider;
                var result = provider.Lookup(ipAddress);
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
            return BrowserLanguageHelper.GetBrowserLanguages(request);
        }

        private Func<LanguageBranch, bool> IsLanguageBranchForCountry(IGeolocationResult location)
        {
            return x => x.Culture.Name.Contains('-') &&
                        x.Culture.Name.EndsWith(location.CountryCode);
        }

        private Func<LanguageBranch, bool> IsLanguageBranchForBrowserLanguage(IEnumerable<string> location)
        {
            return x => location.Any(l => l.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase));
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