using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TauCode.Db.SqlClient.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected IDbInspector DbInspector;
        protected IDbConnection Connection;

        protected virtual void OneTimeSetUpImpl()
        {
            this.Connection = new SqlConnection(TestHelper.ConnectionString);
            this.Connection.Open();
            this.DbInspector = new SqlInspector(Connection);

            this.DbInspector.DropAllTables();
            this.ExecuteDbCreationScript();
        }

        protected abstract void ExecuteDbCreationScript();

        protected virtual void OneTimeTearDownImpl()
        {
            this.Connection.Dispose();
        }

        protected virtual void SetUpImpl()
        {
            this.DbInspector.DeleteDataFromAllTables();
        }

        protected virtual void TearDownImpl()
        {
        }

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            this.OneTimeSetUpImpl();
        }

        [OneTimeTearDown]
        public void OneTimeTearDownBase()
        {
            this.OneTimeTearDownImpl();
        }

        [SetUp]
        public void SetUpBase()
        {
            this.SetUpImpl();
        }

        [TearDown]
        public void TearDownBase()
        {
            this.TearDownImpl();
        }

        protected dynamic GetRow(string tableName, object id)
        {
            using var command = this.Connection.CreateCommand();

            command.CommandText = $@"SELECT * FROM [{tableName}] WHERE [id] = @p_id";
            var parameter = command.CreateParameter();
            parameter.ParameterName = "p_id";
            parameter.Value = id;
            command.Parameters.Add(parameter);
            var row = DbTools.GetCommandRows(command).SingleOrDefault();
            return row;
        }

        protected IList<dynamic> GetRows(string tableName)
        {
            using var command = this.Connection.CreateCommand();

            command.CommandText = $@"SELECT * FROM [{tableName}]";
            var rows = DbTools.GetCommandRows(command);
            return rows;
        }
    }
}
