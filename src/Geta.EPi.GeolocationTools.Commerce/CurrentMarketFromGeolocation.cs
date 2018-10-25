using System.Linq;
using System.Web;
using EPiServer.Business.Commerce;
using Geta.EPi.GeolocationTools.Commerce.Services;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;

namespace Geta.EPi.GeolocationTools.Commerce
{
    public class CurrentMarketFromGeolocation : ICurrentMarket
    {
        private const string MarketCookie = "MarketFromGeolocation";
        protected static MarketId DefaultMarketId; // TODO check if it is enabled?
        protected readonly IMarketService MarketService;
        protected readonly ICommerceGeolocationService CommerceGeolocationService;

        public CurrentMarketFromGeolocation(IMarketService marketService, ICommerceGeolocationService commerceGeolocationService)
        {
            MarketService = marketService;
            DefaultMarketId = marketService.GetAllMarkets().FirstOrDefault(x => x.IsEnabled)?.MarketId ??
                              new MarketId("DEFAULT");
            CommerceGeolocationService = commerceGeolocationService;
        }

        public virtual IMarket GetCurrentMarket()
        {
            var marketId = GetMarketId();
            return GetMarket(marketId);
        }

        private MarketId GetMarketId()
        {
            var marketName = CookieHelper.Get(MarketCookie);
            if (!string.IsNullOrEmpty(marketName))
            {
                return new MarketId(marketName);
            }

            if (HttpContext.Current != null)
            {
                // TODO inject HttpContext?
                var httpContextBase = new HttpRequestWrapper(HttpContext.Current.Request);
                var result = CommerceGeolocationService.GetMarket(httpContextBase);
                var market = result.Market;
                var marketId = market?.MarketId ?? DefaultMarketId;
                CookieHelper.Set(MarketCookie, marketId.Value);
                return marketId;
            }

            return DefaultMarketId;
        }

        public virtual void SetCurrentMarket(MarketId marketId)
        {
            CookieHelper.Set(MarketCookie, marketId.Value);
        }

        protected virtual IMarket GetMarket(MarketId marketId)
        {
            return MarketService.GetMarket(marketId) ?? MarketService.GetMarket(DefaultMarketId);
        }
    }
}
