using System.Net;
using System.Web;
using System.Web.Routing;
using EPiServer.DataAbstraction;
using EPiServer.Personalization;

namespace Geta.Epi.GeolocationRedirect.Services
{
    public interface IGeolocationService
    {
        LanguageBranch GetLanguageBranch(RequestContext requestContext);
        IGeolocationResult GetLocation(HttpRequestBase request);
        IGeolocationResult GetLocation(RequestContext requestContext);
        IGeolocationResult GetLocation(IPAddress ipAddress);
        IPAddress GetRequestIpAddress(HttpRequestBase request);
    }
}