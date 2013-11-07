@echo off

echo.
echo Building project...

msbuild PreMailer.Net\PreMailer.Net.csproj /p:Configuration=Release /t:Clean;Rebuild

echo.
echo Creating NuGet Package...

.nuget\nuget pack PreMailer.Net\PreMailer.Net.csproj -Prop Configuration=Release