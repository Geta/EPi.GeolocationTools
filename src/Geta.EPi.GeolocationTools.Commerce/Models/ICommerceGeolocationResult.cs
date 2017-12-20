using System.Globalization;
using EPiServer.Personalization;
using Mediachase.Commerce;

namespace Geta.EPi.GeolocationTools.Commerce.Models
{
    public interface ICommerceGeolocationResult
    {
        IMarket Market { get; set; }
        CultureInfo Language { get; set; }
        IGeolocationResult Location { get; set; }
    }
}