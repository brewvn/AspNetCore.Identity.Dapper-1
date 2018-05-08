dotnet build AspNetCore.Identity.Dapper.sln -c Release
cd %cd%\src\AspNetCore.Identity.Dapper\bin\Release
for %%i in (*.nupkg) do dotnet nuget push %%i -s https://www.nuget.org/api/v2/package
cd ..\..\..\..