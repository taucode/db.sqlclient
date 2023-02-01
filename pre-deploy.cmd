dotnet restore

dotnet build TauCode.Db.SqlClient.sln -c Debug
dotnet build TauCode.Db.SqlClient.sln -c Release

dotnet test TauCode.Db.SqlClient.sln -c Debug
dotnet test TauCode.Db.SqlClient.sln -c Release

nuget pack nuget\TauCode.Db.SqlClient.nuspec
