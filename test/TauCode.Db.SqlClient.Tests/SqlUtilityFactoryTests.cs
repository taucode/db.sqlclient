using System.Data;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace TauCode.Db.SqlClient.Tests
{
    [TestFixture]
    public class SqlUtilityFactoryTests
    {
        [Test]
        public void Members_DifferentArguments_HaveExpectedProps()
        {
            // Arrange
            IDbUtilityFactory utilityFactory = SqlUtilityFactory.Instance;

            // Act
            IDbConnection connection = new SqlConnection();
            connection.ConnectionString = TestHelper.ConnectionString;
            connection.Open();

            IDbDialect dialect = utilityFactory.GetDialect();

            IDbScriptBuilder scriptBuilder = utilityFactory.CreateScriptBuilder(null);

            IDbInspector dbInspector = utilityFactory.CreateInspector(connection, null);

            IDbTableInspector tableInspector = utilityFactory.CreateTableInspector(connection, null, "language");

            IDbCruder cruder = utilityFactory.CreateCruder(connection, null);

            IDbSerializer dbSerializer = utilityFactory.CreateSerializer(connection, null);

            // Assert
            Assert.That(dialect.Name, Is.EqualTo("SQLServer"));
            Assert.That(connection, Is.TypeOf<SqlConnection>());
            Assert.That(dialect, Is.SameAs(SqlDialect.Instance));

            Assert.That(scriptBuilder, Is.TypeOf<SqlScriptBuilder>());
            Assert.That(scriptBuilder.CurrentOpeningIdentifierDelimiter, Is.EqualTo('['));

            Assert.That(dbInspector, Is.TypeOf<SqlInspector>());
            Assert.That(tableInspector, Is.TypeOf<SqlTableInspector>());
            Assert.That(cruder, Is.TypeOf<SqlCruder>());
            Assert.That(dbSerializer, Is.TypeOf<SqlSerializer>());
        }
    }
}
