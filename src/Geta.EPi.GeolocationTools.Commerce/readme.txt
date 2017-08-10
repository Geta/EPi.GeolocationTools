Geta.EPi.GeolocationTools.Commerce

Register ICurrentMarket implementation
```csharp
 public class StructureMapRegistry : Registry
{
    public StructureMapRegistry()
    {
        For<ICurrentMarket>().Use<CurrentMarketFromGeolocation>()}
    }
}
```
Get market based on geolocation and browser preferences
```csharp
public class MarketExample : Controller
{
    private readonly ICurrentMarket _currentMarket;
    private readonly ICommerceGeolocationService _commerceGeolocationService;

    public MarketExample(
        ICurrentMarket currentMarket, 
        ICommerceGeolocationService commerceGeolocationService)
    {
        _currentMarket = currentMarket;
        _commerceGeolocationService = commerceGeolocationService;
    }

    public void Index()
    {
        // Get current market based on geolocation and browser preferences, can be null
        var (market, language, location) = _commerceGeolocationService.GetMarket(Request);
        
        // This one will be cached by storing the result in a cookie
        // Will fall back to first enabled market or the default market
        var sameMarket = _currentMarket.GetCurrentMarket();
    }
}
```