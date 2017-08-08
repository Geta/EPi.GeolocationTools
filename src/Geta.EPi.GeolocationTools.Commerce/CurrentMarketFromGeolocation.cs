using System;
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
        private const string MarketCookie = "MarketId";
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
            var marketName = CookieHelper.Get(MarketCookie);
            MarketId marketId;
            if (string.IsNullOrEmpty(marketName))
            {
                // TODO inject HttpContext?
                var httpContextBase = new HttpRequestWrapper(HttpContext.Current.Request);
                var (market, _, _) = CommerceGeolocationService.GetMarket(httpContextBase);
                marketId = market?.MarketId ?? DefaultMarketId;
                CookieHelper.Set(MarketCookie, marketId.Value);
            }
            else
            {
                marketId = new MarketId(marketName);
            }
            return GetMarket(marketId);
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
