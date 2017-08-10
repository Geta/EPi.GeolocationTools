Geta.EPi.GeolocationTools

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