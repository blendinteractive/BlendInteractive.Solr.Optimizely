dotnet build -c Release "./src/BlendInteractive.Solr/BlendInteractive.Solr.csproj" /p:ContinuousIntegrationBuild=true
dotnet pack  -c Release --no-build -o "./artifacts/packages" "./src/BlendInteractive.Solr/BlendInteractive.Solr.csproj"

dotnet build -c Release "./src/BlendInteractive.Solr.Optimizely/BlendInteractive.Solr.Optimizely.csproj" /p:UseLocalPackage=True /p:ContinuousIntegrationBuild=true
dotnet pack  -c Release --no-build -o "./artifacts/packages" "./src/BlendInteractive.Solr.Optimizely/BlendInteractive.Solr.Optimizely.csproj"

