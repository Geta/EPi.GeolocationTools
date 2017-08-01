using System.Collections.Specialized;
using System.Net;
using System.Web;
using System.Web.Http;

namespace Geta.Epi.GeolocationRedirect
{
    [RoutePrefix("api/redirect")]
    public class RedirectApiController : ApiController
    {
        private readonly IGeolocationService _geolocationService;

        public RedirectApiController(IGeolocationService geolocationService)
        {
            _geolocationService = geolocationService;
        }

        [HttpGet]
        [Route("")]
        public string Get()
        {
            var ipAddress = _geolocationService.GetRequestIpAddress(new HttpRequestWrapper(HttpContext.Current.Request));
            if (ipAddress.Equals(IPAddress.None))
            {
                return "No ip";
            }

            var requestLanguage = _geolocationService.GetLanguageBranchByIpAddress(ipAddress);
            return requestLanguage?.LanguageID ?? "No GetLanguageBranchByIpAddress";
        }

        [HttpGet]
        [Route("{ip}")]
        public string GetByIp(string ip)
        {
            IPAddress ipAddress;
            if (IPAddress.TryParse(ip, out ipAddress))
            {
                var requestLanguage = _geolocationService.GetLanguageBranchByIpAddress(ipAddress);
                return requestLanguage?.LanguageID ?? "No GetLanguageBranchByIpAddress";
            }

            return $"Can't parse ip address {ipAddress}";
        }
    }
}