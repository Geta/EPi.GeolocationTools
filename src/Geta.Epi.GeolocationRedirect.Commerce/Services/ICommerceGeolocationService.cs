using System.Globalization;
using System.Web;
using EPiServer.Personalization;
using Mediachase.Commerce;

namespace Geta.Epi.GeolocationRedirect.Commerce.Services
{
    public interface ICommerceGeolocationService
    {
        /// <summary>
        /// Gets the market based on the IP address location and browser UserLanguages.
        /// Defaults to market default language
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Location, market and language tuple, can be null</returns>
        (IGeolocationResult location, IMarket market, CultureInfo uiLanguage) GetMarket(HttpRequestBase request);

        /// <summary>
        /// Gets the language based on the browser UserLanguages and the given market.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="market">The market.</param>
        /// <returns></returns>
        CultureInfo GetLanguage(HttpRequestBase request, IMarket market);

        /// <summary>
        /// Gets the market based on the IP address.
        /// </summary>
        /// <param name="geolocationResult">The geolocation result.</param>
        /// <returns></returns>
        IMarket GetMarket(IGeolocationResult geolocationResult);
    }
}