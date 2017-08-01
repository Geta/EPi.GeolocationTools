using System.Web.Mvc;
using EPiServer.ServiceLocation;

namespace Geta.Epi.GeolocationRedirect
{
    public class RedirectBasedOnLocationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var locationLanguageService = ServiceLocator.Current.GetInstance<IGeolocationService>();
            var languageBranch = locationLanguageService.GetLanguageBranch(filterContext);
            filterContext.Result = new RedirectResult($"/{languageBranch.LanguageID}");
        }
    }
}