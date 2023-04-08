using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Data;

namespace TauCode.Db.SqlClient.LocalTests;

[TestFixture]
public abstract class TestBase
{
    protected string ConnectionString { get; set; }
    protected IDbConnection? Connection { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUpBase()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");
        this.Connection = new SqlConnection(connectionString);
        this.Connection.Open();
    }

    [OneTimeTearDown]
    public void OneTimeTearDownBase()
    {
        this.Connection?.Dispose();
        this.Connection = null;
    }

    protected SqlConnection? SqlConnection => (SqlConnection)this.Connection!;
}
