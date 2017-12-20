using System.Globalization;
using System.Web;
using EPiServer.Personalization;
using Geta.EPi.GeolocationTools.Commerce.Models;
using Mediachase.Commerce;

namespace Geta.EPi.GeolocationTools.Commerce.Services
{
    public interface ICommerceGeolocationService
    {
        /// <summary>
        /// Gets the market based on the IP address location and browser UserLanguages.
        /// Defaults to market default language. Market, language and location can be null.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Market, language and location can be null</returns>
        ICommerceGeolocationResult GetMarket(HttpRequestBase request);

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