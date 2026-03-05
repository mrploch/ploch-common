dotnet restore Ploch.Common.sln
dotnet sonarscanner begin /k:"mrploch_ploch-common" /o:"mrploch" /d:sonar.login="$evn:SONAR_TOKEN" /d:sonar.cs.vscoveragexml.reportsPaths=coverage.xml /d:sonar.host.url="https://sonarcloud.io"
dotnet build Ploch.Common.sln --no-incremental
# dotnet-coverage collect 'dotnet test Ploch.Common.sln' -f xml  -o 'coverage.xml'
# dotnet test .\Ploch.Common.sln -p:CollectCoverage=true -p:CoverletOutput=TestResults/ -p:CoverletOutputFormat=opencover --verbosity normal --logger "trx;LogFileName=TestOutputResults.xml"
# reportgenerator -reports:**/TestResults/coverage.opencover.xml -targetdir:./CoverageReport -reporttypes:OpenCover
dotnet-coverage collect "dotnet test Ploch.Common.sln" -f xml -o "coverage.xml"
dotnet sonarscanner end /d:sonar.login="$evn:SONAR_TOKEN"
