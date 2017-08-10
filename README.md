# Geta Episerver geolocation tools

## What is Geta.EPi.GeolocationTools?
This library can be used to retrieve the languagebranch which matches the given request best. It provides methods to retrieve a preferred languagebranch by a users' geolocation, browser language preference or both.
The commerce library can be used to find the right market and corresponding language based on the same parameters.

## Features
* Get languagebranch by geolocation, preferred browser languages or both
* [Commerce] Get market by geolocation and preferred browser languages
* Override ip address for local development testing (by setting a cookie)

## How to get started?
* ``install-package Geta.EPi.GeolocationTools``
* ``install-package Geta.EPi.GeolocationTools.Commerce``

For local development add a cookie to override the ip adress to an ip you want to test.
Either in code:
```csharp
// This will be gone next request
Request.Cookies.Add(new HttpCookie(Geta.EPi.GeolocationTools.Constants.IPAddressOverride)
{
	Value = "59.107.128.65", // Chinese ip address
	Expires = DateTime.Now.AddYears(1)
});
var (market, language, geolocation) = _commerceGeolocationService.GetMarket(Request);

// This will be there upon the next request
HttpContext.Response.SetCookie(new HttpCookie(Geta.EPi.GeolocationTools.Constants.IPAddressOverride)
{
    Value = "59.107.128.65",
    Expires = DateTime.Now.AddYears(1)
});
// Chinese market (if available)
var (market, language, geolocation) = _commerceGeolocationService.GetMarket(Request);
```
Or add a cookie "geolocation_ip_override" in your browser dev tools.
![Dev tools cookie](/docs/images/cookie-dev-tools.png)

## Details
Code example for Geta.EPi.GeolocationTools
```csharp
public class LanguageBranchExample : Controller
{
    private readonly IGeolocationService _geolocationService;

    public LanguageBranchExample(
        IGeolocationService geolocationService)
    {
        _geolocationService = geolocationService;
    }

    public void Index()
    {
        // Gets the language based on the users' location and their browser preferences, depending on what is available.
        // 1. Language branch for both the users' country and their browser preferences
        // 2. Language branch for users' browser preferences
        // 3. Fallback language
        var languageBranch = _geolocationService.GetLanguage(Request);
    }
}
```
Code example for Geta.EPi.GeolocationTools.Commerce
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

### Changelog
