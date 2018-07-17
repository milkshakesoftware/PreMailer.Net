@echo off

echo.
echo Restoring project...

dotnet restore PreMailer.Net\PreMailer.Net.csproj

echo.
echo Creating NuGet Package...

dotnet pack PreMailer.Net\PreMailer.Net.csproj --configuration Release --include-symbols