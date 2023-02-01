using Microsoft.Data.SqlClient;
using NUnit.Framework;
using TauCode.Db.Data;
using TauCode.Db.Extensions;
using TauCode.Extensions;

namespace TauCode.Db.SqlClient.LocalTests.EconeraTesting
{
    [TestFixture]
    public class EconeraTests
    {
        private IDbSerializer _serializer;
        private IDbCruder _cruder;
        private SqlConnection _connection;
        private SqlSchemaExplorer _schemaExplorer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _connection = new SqlConnection(TestHelper.ConnectionString);
            _connection.Open();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _connection.Dispose();
            _connection = null;
        }

        [SetUp]
        public void SetUp()
        {
            _schemaExplorer = new SqlSchemaExplorer(_connection);

            if (_schemaExplorer.SchemaExists(TestHelper.SchemaName))
            {
                _schemaExplorer.DropAllTables(TestHelper.SchemaName);
            }
            else
            {
                _schemaExplorer.CreateSchema(TestHelper.SchemaName);
            }

            var metadataJson = this.GetType().Assembly.GetResourceText("quick-and-dirty-metadata.json", true);

            IDbMigrator migrator = new SqlJsonMigrator(
                _connection,
                TestHelper.SchemaName,
                () => metadataJson,
                () => "{}");
            migrator.Migrate();


            _serializer = new SqlSerializer(_connection, TestHelper.SchemaName);
            _cruder = new SqlCruder(_connection, TestHelper.SchemaName);
            
        }

        [Test]
        public void DeserializeDbData_EconeraQuickAndDirty_Deserializes()
        {
            // Arrange
            var json = this.GetType().Assembly.GetResourceText("quick-and-dirty-data.json", true);

            _serializer.BeforeDeserializeTableData = (table, list) =>
            {
                var identity = table.TryGetPrimaryKeySingleColumn()?.Identity;
                if (identity != null)
                {
                    _connection.SetIdentityInsert(TestHelper.SchemaName, table.Name, true);
                }

                return list;
            };

            _serializer.AfterDeserializeTableData = (table, list) =>
            {
                var identity = table.TryGetPrimaryKeySingleColumn()?.Identity;
                if (identity != null)
                {
                    _connection.SetIdentityInsert(TestHelper.SchemaName, table.Name, false);
                }
            };

            // Act
            _serializer.DeserializeDbData(json);

            // Assert
            var rows = _cruder.GetAllRows("currency");
            dynamic uah = rows[2];
            Assert.That(uah.id, Is.EqualTo(9003L));
            Assert.That(uah.code, Is.EqualTo("UAH"));
            Assert.That(uah.is_available_to_watchers, Is.True);
        }

        [Test]
        public void DeserializeDbData_EconeraTestDb_Deserializes()
        {
            // Arrange
            this.DeserializeDbData_EconeraQuickAndDirty_Deserializes(); // use another ut to setup 'quick and dirty data'

            var json = this.GetType().Assembly.GetResourceText("testdb.json", true);

            _serializer.BeforeDeserializeTableData = (table, list) =>
            {
                var identity = table.GetPrimaryKeySingleColumn().Identity;
                if (identity != null)
                {
                    _connection.SetIdentityInsert(TestHelper.SchemaName, table.Name, true);
                }

                return list;
            };

            _serializer.AfterDeserializeTableData = (table, list) =>
            {
                var identity = table.GetPrimaryKeySingleColumn().Identity;
                if (identity != null)
                {
                    _connection.SetIdentityInsert(TestHelper.SchemaName, table.Name, false);
                }
            };

            // Act
            _serializer.DeserializeDbData(json);

            // Assert
            var quote = _cruder.GetRow("quote", 300101L);
            Assert.That(quote.date, Is.EqualTo("1900-01-01Z".ToUtcDateOffset()));
        }

        [Test]
        public void DeserializeDbData_CruderIsCustomized_Deserializes()
        {
            // Arrange
            this.DeserializeDbData_EconeraQuickAndDirty_Deserializes(); // use another ut to setup 'quick and dirty data'

            var json = this.GetType().Assembly.GetResourceText("new-system-watcher.json", true);

            // Act
            _serializer.Cruder.BeforeInsertRow = (table, rowParam, index) =>
            {
                if (table.Name == "system_watcher")
                {
                    // insert 'watcher' first
                    // NB: will do some recursion

                    var watcher = new DynamicRow(new
                    {
                        guid = ((dynamic)rowParam).guidush,
                    });

                    _serializer.Cruder.InsertRow("watcher", watcher);
                    var systemWatcher = (DynamicRow)rowParam;

                    systemWatcher.SetProperty("watcher_id", watcher.GetProperty("id"));
                }

                return rowParam;
            };

            _serializer.Cruder.AfterInsertRow = (table, rowParam, index) =>
            {
                if (table.Name == "watcher")
                {
                    var lastId = (long)_connection.GetLastIdentity();
                    var dynamicRow = (DynamicRow)rowParam;
                    dynamicRow.SetProperty("id", lastId);
                }
            };


            _serializer.DeserializeDbData(json);

            // Assert
            using var command = _connection.CreateCommand();
            command.CommandText = $@"
SELECT
    W.[id] id,
    W.[guid] guid,
    SW.[code] code
FROM
    [{TestHelper.SchemaName}].[watcher] W
INNER JOIN
    [{TestHelper.SchemaName}].[system_watcher] SW
ON
    W.[id] = SW.[watcher_id]
WHERE
    SW.[code] = @p_code
";
            command.Parameters.AddWithValue("p_code", "panaev");
            var row = command.GetCommandRows().Single();

            Assert.That(row.guid, Is.EqualTo(new Guid("6a31ff49-c618-4293-97b2-8b52a1d3d192")));
        }

        [Test]
        public void DeserializeDbData_TableWithoutPk_Deserializes()
        {
            // Arrange
            this.DeserializeDbData_EconeraQuickAndDirty_Deserializes(); // use another ut to setup 'quick and dirty data'

            using var command = _connection.CreateCommand();
            command.CommandText = $@"
CREATE TABLE [{TestHelper.SchemaName}].[no_pk_table](
    id int,
    name nvarchar(100)
)
";
            command.ExecuteNonQuery();

            // Act
            var json = this.GetType().Assembly.GetResourceText("no_pk_table.json", true);
            _serializer.DeserializeDbData(json);
            
            // Assert
            var rows = _cruder.GetAllRows("no_pk_table");
            var dict = rows.ToDictionary(
                x => (int) x.id,
                x => (string) x.name);

            Assert.That(dict, Contains.Key(1).WithValue("Ket"));
            Assert.That(dict, Contains.Key(2).WithValue("Lassie"));
        }
    }
}
