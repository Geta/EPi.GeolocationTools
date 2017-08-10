@echo off

if not exist "lib" mkdir lib
nuget pack src\Geta.EPi.GeolocationTools\Geta.EPi.GeolocationTools.csproj -IncludeReferencedProjects -OutputDirectory "lib"
nuget pack src\Geta.EPi.GeolocationTools.Commerce\Geta.EPi.GeolocationTools.Commerce.csproj -IncludeReferencedProjects -OutputDirectory "lib"

@echo on