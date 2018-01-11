# Geta Episerver geolocation tools

## Description
This library can be used to retrieve the languagebranch which matches the given request best. It provides methods to retrieve a preferred languagebranch by a users' geolocation, [browser language preference](https://www.w3.org/International/questions/qa-lang-priorities)  or both.
The commerce library can be used to find the right market and corresponding language based on the same parameters.
Useful to prompt the user that a different language might suit him/her better.
Useful for setting the right market for a user or for suggesting a specific market and language.
Builds on top of Episervers' built in support for [geolocation](https://world.episerver.com/documentation/developer-guides/CMS/personalization/Configuring-personalization/)

## Features
* Get languagebranch by geolocation, preferred browser languages or both
* [Commerce] Get market by geolocation and preferred browser languages
* Override ip address for local development testing (by setting a cookie)

## How to get started?
* ``install-package Geta.EPi.GeolocationTools``
* ``install-package Geta.EPi.GeolocationTools.Commerce``

## Details

### Local development
For local development add a cookie to override the ip adress to an ip you want to test.
Either in code:
```csharp
// This will be gone next request
Request.Cookies.Add(new HttpCookie(Geta.EPi.GeolocationTools.Constants.IPAddressOverride)
{
	Value = "59.107.128.65", // Chinese ip address
	Expires = DateTime.Now.AddYears(1)
});
var result = _commerceGeolocationService.GetMarket(Request);

// This will be there upon the next request
HttpContext.Response.SetCookie(new HttpCookie(Geta.EPi.GeolocationTools.Constants.IPAddressOverride)
{
    Value = "59.107.128.65",
    Expires = DateTime.Now.AddYears(1)
});
// Chinese market (if available)
var result = _commerceGeolocationService.GetMarket(Request);
```
Or add a cookie "geolocation_ip_override" in your browser dev tools.
![Dev tools cookie](/docs/images/cookie-dev-tools.png)

### Code example for Geta.EPi.GeolocationTools
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
### Code example for Geta.EPi.GeolocationTools.Commerce
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
        // Get current market based on geolocation and browser preferences, market, language, location can be null
        var result = _commerceGeolocationService.GetMarket(Request);
        
        // This one will be cached by storing the result in a cookie
        // Will fall back to first enabled market or the default market
        var sameMarket = _currentMarket.GetCurrentMarket();
    }
}
```

## Package maintainer
https://github.com/brianweet

## Changelog
* 11 Jan 2018 - 1.0
  * Added support for Episerver 11
* 20 Dec 2017 - 0.2
  * Remove tuple from public api
  * Add null checks
* 11 Aug 2017 - 0.1 
  * Initial release
