dotnet restore

dotnet build --configuration Debug
dotnet build --configuration Release

dotnet test -c Debug .\test\TauCode.Db.SqlClient.Tests\TauCode.Db.SqlClient.Tests.csproj
dotnet test -c Release .\test\TauCode.Db.SqlClient.Tests\TauCode.Db.SqlClient.Tests.csproj

nuget pack nuget\TauCode.Db.SqlClient.nuspec