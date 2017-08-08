@echo off

if not exist "lib" mkdir lib
nuget pack src\Geta.EPi.GeolocationRedirect\Geta.EPi.GeolocationRedirect.csproj -IncludeReferencedProjects -OutputDirectory "lib"
nuget pack src\Geta.EPi.GeolocationRedirect.Commerce\Geta.EPi.GeolocationRedirect.Commerce.csproj -IncludeReferencedProjects -OutputDirectory "lib"

@echo on