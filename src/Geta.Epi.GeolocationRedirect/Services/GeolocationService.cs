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
using EPiServer.Web.Routing;

namespace Geta.Epi.GeolocationRedirect.Services
{
    [ServiceConfiguration(ServiceType = typeof(IGeolocationService))]
    public class GeolocationService : IGeolocationService
    {
        private readonly ILanguageBranchRepository _languageBranchRepository;

        public LanguageBranch GetFallbackLanguageBranch()
        {
            return GetFallbackLanguageBranch(ContentLanguage.PreferredCulture);
        }

        public LanguageBranch GetFallbackLanguageBranch(CultureInfo cultureInfo)
        {
            return _languageBranchRepository.ListEnabled().FirstOrDefault(l => l.LanguageID.Equals(cultureInfo.Name)) ??
                   _languageBranchRepository.LoadFirstEnabledBranch();
        }

        public GeolocationService(ILanguageBranchRepository languageBranchRepository)
        {
            _languageBranchRepository = languageBranchRepository;
        }

        public LanguageBranch GetLanguageBranch(RequestContext requestContext)
        {
            // Use Epi extension method to get language from route
            var routeLanguage = requestContext.GetLanguage();
            if (!string.IsNullOrWhiteSpace(routeLanguage))
            {
                return GetFallbackLanguageBranch();
            }

            return GetLanguageBranchByIpAddress(requestContext);
        }

        public LanguageBranch GetLanguageBranchByIpAddress(RequestContext requestContext)
        {
            var ipAddress = GetRequestIpAddress(requestContext.HttpContext.Request);
            if (ipAddress.Equals(IPAddress.None))
            {
                return GetFallbackLanguageBranch();
            }
            var requestLanguage = GetLanguageBranchByIpAddress(ipAddress);

            return requestLanguage;
        }

        public LanguageBranch GetLanguageBranchByIpAddress(IPAddress ipAddress)
        {
            var languageBranches = _languageBranchRepository.ListEnabled().OrderBy(x => x.SortIndex);
            return GetLanguageBranchByIpAddress(ipAddress, languageBranches);
        }

        public LanguageBranch GetLanguageBranchByIpAddress(IPAddress ipAddress, IEnumerable<LanguageBranch> languageBranches)
        {
            var location = GetLocation(ipAddress);
            var requestLanguage = GetLanguageBranchByLocation(location, languageBranches);
            return requestLanguage;
        }

        public LanguageBranch GetLanguageBranchByLocation(IGeolocationResult location)
        {
            var languageBranches = _languageBranchRepository.ListEnabled().OrderBy(x => x.SortIndex);
            return GetLanguageBranchByLocation(location, languageBranches);
        }

        public LanguageBranch GetLanguageBranchByLocation(IGeolocationResult location, IEnumerable<LanguageBranch> languageBranches)
        {
            if (location == null || languageBranches == null)
            {
                return GetFallbackLanguageBranch();
            }

            var allLanguagesForCountry =
                CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .Where(c => c.Name.EndsWith($"-{location.CountryCode}"))
                    .Select(x => x.TwoLetterISOLanguageName)
                    .Distinct()
                    .ToList();

            foreach (var languageBranch in languageBranches)
            {
                if (allLanguagesForCountry.Any(x => x.Equals(languageBranch.LanguageID)))
                {
                    return languageBranch;
                }
            }

            return GetFallbackLanguageBranch();
        }

        public IGeolocationResult GetLocation(HttpRequestBase requestContext)
        {
            return GetLocation(requestContext.RequestContext);
        }

        public IGeolocationResult GetLocation(RequestContext requestContext)
        {
            return GetLocation(GetRequestIpAddress(requestContext.HttpContext.Request));
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

        public IPAddress GetRequestIpAddress(HttpRequestBase request)
        {
            var address = request.ServerVariables["True-Client-IP"];

            if (string.IsNullOrWhiteSpace(address))
            {
                var forwardedForAddresses = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(forwardedForAddresses))
                {
                    var addresses = forwardedForAddresses.Split(',');
                    if (addresses.Length > 0)
                    {
                        address = addresses.FirstOrDefault();
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                address = request.ServerVariables["REMOTE_ADDR"];
            }

            //Port number is included, discard it
            address = address.Contains(":") ? address.Substring(0, address.IndexOf(':')) : address;

            return ParseIpAddress(address);
        }

        private IPAddress ParseIpAddress(string address)
        {
            IPAddress ipAddress;
            if (!IPAddress.TryParse(address, out ipAddress))
            {
                return IPAddress.None;
            }

            return ipAddress;
        }
    }
}