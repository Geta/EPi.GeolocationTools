using System.Collections.Generic;
using System.Net;
using System.Web;
using EPiServer.DataAbstraction;
using EPiServer.Personalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Geta.Epi.GeolocationRedirect
{
    public interface IGeolocationService
    {
        LanguageBranch FallbackLanguageBranch { get; }
        LanguageBranch GetLanguageBranch(ActionExecutingContext filterContext);
        LanguageBranch GetLanguageBranch(HttpContext context);
        LanguageBranch GetLanguageBranch(RequestContext requestContext);
        LanguageBranch GetLanguageBranchByIpAddress(IPAddress ipAddress);
        LanguageBranch GetLanguageBranchByIpAddress(IPAddress ipAddress, IEnumerable<LanguageBranch> languageBranches);
        LanguageBranch GetLanguageBranchByLocation(IGeolocationResult location);
        LanguageBranch GetLanguageBranchByLocation(IGeolocationResult location, IEnumerable<LanguageBranch> languageBranches);
        IGeolocationResult GetLocationFromIpAddress(IPAddress ipAddress);
        IPAddress GetRequestIpAddress(HttpRequestBase request);
    }
}