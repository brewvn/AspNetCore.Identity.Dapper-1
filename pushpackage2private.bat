dotnet build AspNetCore.Identity.Dapper.sln -c Release
cd %cd%\src\AspNetCore.Identity.Dapper\bin\Release
for %%i in (*.nupkg) do dotnet nuget push %%i -s http://zlzforever.6655.la:40001/nuget
cd ..\..\..\..