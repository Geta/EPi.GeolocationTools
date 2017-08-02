﻿using System.Web.Mvc;
using EPiServer.ServiceLocation;
using Geta.Epi.GeolocationRedirect.Services;

namespace Geta.Epi.GeolocationRedirect.Attributes
{
    public class RedirectBasedOnGeolocationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var geolocationService = ServiceLocator.Current.GetInstance<IGeolocationService>();
            var languageBranch = geolocationService.GetLanguageBranch(filterContext.RequestContext);
            filterContext.Result = new RedirectResult($"/{languageBranch.LanguageID}");
        }
    }
}