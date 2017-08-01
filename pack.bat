@echo off

if not exist "lib" mkdir lib
nuget pack src\Geta.Epi.GeolocationRedirect\Geta.Epi.GeolocationRedirect.csproj -IncludeReferencedProjects -OutputDirectory "lib"

@echo on