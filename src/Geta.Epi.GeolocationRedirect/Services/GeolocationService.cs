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

namespace Geta.Epi.GeolocationRedirect.Services
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

        public LanguageBranch GetLanguageByCountry(IGeolocationResult location)
        {
            return _enabledLanguageBranches
                .Where(IsLanguageBranchForCountry(location))
                .FirstOrDefault();
        }

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