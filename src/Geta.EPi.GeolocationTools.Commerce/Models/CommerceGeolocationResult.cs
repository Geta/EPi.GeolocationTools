using System.Globalization;
using EPiServer.Personalization;
using Mediachase.Commerce;

namespace Geta.EPi.GeolocationTools.Commerce.Models
{
    public class CommerceGeolocationResult : ICommerceGeolocationResult
    {
        public IMarket Market { get; set; }
        public CultureInfo Language { get; set; }
        public IGeolocationResult Location { get; set; }
    }
}