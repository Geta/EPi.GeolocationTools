using System.Web.Mvc;
using EPiServer.Web.Mvc;

namespace Geta.Epi.GeolocationRedirect
{
    public class RedirectBlockController : BlockController<RedirectBlock>
    {
        private readonly IGeolocationService _geolocationService;

        public RedirectBlockController(
            IGeolocationService geolocationService)
        {
            _geolocationService = geolocationService;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public override ActionResult Index(RedirectBlock currentBlock)
        {
            var viewModel = new RedirectViewModel(currentBlock)
            {
                CurrentLanguageBasedOnIp =
                    _geolocationService.GetLanguageBranch(System.Web.HttpContext.Current).LanguageID
            };

            return PartialView(viewModel);
        }
    }
}