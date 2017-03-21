:: Need [.NET Core SDK 1.0 rc4](https://github.com/dotnet/core/blob/master/release-notes/rc4-download.md)
@echo off

echo.
echo Restoring project...

dotnet restore PreMailer.Net-dotnet\PreMailer.Net.csproj

echo.
echo Creating NuGet Package...

dotnet pack PreMailer.Net-dotnet\PreMailer.Net.csproj --configuration Release --include-symbols