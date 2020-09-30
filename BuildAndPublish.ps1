#restore .net core packages
dotnet restore .\ThirdPartyIntegrationSample\ThirdPartyIntegrationSample.sln --force
#publish .net core  app
dotnet publish .\ThirdPartyIntegrationSample\Web\Web.csproj -o=publish\Web -c=Release