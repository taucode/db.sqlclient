using Microsoft.Data.SqlClient;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TauCode.Db.Data;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Db.SqlClient.Tests.DbCruder.Dto;
using TauCode.Extensions;

namespace TauCode.Db.SqlClient.Tests.DbCruder
{
    [TestFixture]
    public class SqlCruderTests : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Connection.CreateSchema(TestHelper.SchemaName);

            var sql = this.GetType().Assembly.GetResourceText("crebase.sql", true);
            this.Connection.ExecuteCommentedScript(sql);
        }

        private void CreateSuperTable()
        {
            var sql = this.GetType().Assembly.GetResourceText("SuperTable.sql", true);
            this.Connection.ExecuteSingleSql(sql);
        }

        private SuperTableRowDto CreateSuperTableRowDto()
        {
            return new SuperTableRowDto
            {
                Id = 1,

                TheGuid = new Guid("11111111-1111-1111-1111-111111111111"),

                TheBit = true,

                TheTinyInt = 1,
                TheSmallInt = 11,
                TheInt = 111,
                TheBigInt = 1111,

                TheDecimal = 11.10m,
                TheNumeric = 111.10m,

                TheSmallMoney = 1111.1100m,
                TheMoney = 11111.1100m,

                TheReal = (float)111111.0,
                TheFloat = 1111111.0,

                TheDate = DateTime.Parse("1990-01-01"),
                TheDateTime = DateTime.Parse("1991-01-01"),
                TheDateTime2 = DateTime.Parse("1992-01-01"),
                TheDateTimeOffset = DateTime.Parse("1993-01-01"),
                TheSmallDateTime = DateTime.Parse("1994-01-01"),
                TheTime = TimeSpan.Parse("01:01:01"),

                TheChar = "a",
                TheVarChar = "aa",
                TheVarCharMax = "aaa",

                TheNChar = "ц",
                TheNVarChar = "цц",
                TheNVarCharMax = "ццц",

                TheBinary = new byte[] { 1 },
                TheVarBinary = new byte[] { 10, 11, },
                TheVarBinaryMax = new byte[] { 100, 101, 102 },

                NotExisting = 777,
            };
        }

        private void InsertSuperTableRow()
        {
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
INSERT INTO [zeta].[SuperTable](
    [TheGuid],
    [TheBit],
    [TheTinyInt],
    [TheSmallInt],
    [TheInt],
    [TheBigInt],
    [TheDecimal],
    [TheNumeric],
    [TheSmallMoney],
    [TheMoney],
    [TheReal],
    [TheFloat],
    [TheDate],
    [TheDateTime],
    [TheDateTime2],
    [TheDateTimeOffset],
    [TheSmallDateTime],
    [TheTime],
    [TheChar],
    [TheVarChar],
    [TheVarCharMax],
    [TheNChar],
    [TheNVarChar],
    [TheNVarCharMax],
    [TheBinary],
    [TheVarBinary],
    [TheVarBinaryMax])
VALUES(
    @p_theGuid,
    @p_theBit,
    @p_theTinyInt,
    @p_theSmallInt,
    @p_theInt,
    @p_theBigInt,
    @p_theDecimal,
    @p_theNumeric,
    @p_theSmallMoney,
    @p_theMoney,
    @p_theReal,
    @p_theFloat,
    @p_theDate,
    @p_theDateTime,
    @p_theDateTime2,
    @p_theDateTimeOffset,
    @p_theSmallDateTime,
    @p_theTime,
    @p_theChar,
    @p_theVarChar,
    @p_theVarCharMax,
    @p_theNChar,
    @p_theNVarChar,
    @p_theNVarCharMax,
    @p_theBinary,
    @p_theVarBinary,
    @p_theVarBinaryMax)
";

            var row = this.CreateSuperTableRowDto();

            command.Parameters.AddWithValue("@p_theGuid", row.TheGuid);

            command.Parameters.AddWithValue("@p_theBit", row.TheBit);

            command.Parameters.AddWithValue("@p_theTinyInt", row.TheTinyInt);
            command.Parameters.AddWithValue("@p_theSmallInt", row.TheSmallInt);
            command.Parameters.AddWithValue("@p_theInt", row.TheInt);
            command.Parameters.AddWithValue("@p_theBigInt", row.TheBigInt);

            command.Parameters.AddWithValue("@p_theDecimal", row.TheDecimal);
            command.Parameters.AddWithValue("@p_theNumeric", row.TheNumeric);

            command.Parameters.AddWithValue("@p_theSmallMoney", row.TheSmallMoney);
            command.Parameters.AddWithValue("@p_theMoney", row.TheMoney);

            command.Parameters.AddWithValue("@p_theReal", row.TheReal);
            command.Parameters.AddWithValue("@p_theFloat", row.TheFloat);

            command.Parameters.AddWithValue("@p_theDate", row.TheDate);
            command.Parameters.AddWithValue("@p_theDateTime", row.TheDateTime);
            command.Parameters.AddWithValue("@p_theDateTime2", row.TheDateTime2);
            command.Parameters.AddWithValue("@p_theDateTimeOffset", row.TheDateTimeOffset);
            command.Parameters.AddWithValue("@p_theSmallDateTime", row.TheSmallDateTime);
            command.Parameters.AddWithValue("@p_theTime", row.TheTime);

            command.Parameters.AddWithValue("@p_theChar", row.TheChar);
            command.Parameters.AddWithValue("@p_theVarChar", row.TheVarChar);
            command.Parameters.AddWithValue("@p_theVarCharMax", row.TheVarCharMax);

            command.Parameters.AddWithValue("@p_theNChar", row.TheNChar);
            command.Parameters.AddWithValue("@p_theNVarChar", row.TheNVarChar);
            command.Parameters.AddWithValue("@p_theNVarCharMax", row.TheNVarCharMax);

            command.Parameters.AddWithValue("@p_theBinary", row.TheBinary);
            command.Parameters.AddWithValue("@p_theVarBinary", row.TheVarBinary);
            command.Parameters.AddWithValue("@p_theVarBinaryMax", row.TheVarBinaryMax);

            command.ExecuteNonQuery();
        }

        private void CreateMediumTable()
        {
            var sql = @"
CREATE TABLE [zeta].[MediumTable](
    [Id] int NOT NULL PRIMARY KEY,

    [TheInt] int NULL DEFAULT 1599,
    [TheNVarChar] nvarchar(100) NULL DEFAULT 'Semmi')
";

            this.Connection.ExecuteSingleSql(sql);
        }

        private void CreateSmallTable()
        {
            var sql = @"
CREATE TABLE [zeta].[SmallTable](
    [Id] int NOT NULL PRIMARY KEY IDENTITY(1, 1),

    [TheInt] int NULL DEFAULT 1599,
    [TheNVarChar] nvarchar(100) NULL DEFAULT 'Semmi')
";

            this.Connection.ExecuteSingleSql(sql);
        }

        #region Constructor

        [Test]
        [TestCase("dbo")]
        [TestCase(null)]
        public void Constructor_ValidArguments_RunsOk(string schemaName)
        {
            // Arrange

            // Act
            IDbCruder cruder = new SqlCruder(this.Connection, schemaName);

            // Assert
            Assert.That(cruder.Connection, Is.SameAs(this.Connection));
            Assert.That(cruder.Factory, Is.SameAs(SqlUtilityFactory.Instance));
            Assert.That(cruder.SchemaName, Is.EqualTo("dbo"));
            Assert.That(cruder.ScriptBuilder, Is.TypeOf<SqlScriptBuilder>());
            Assert.That(cruder.BeforeInsertRow, Is.Null);
            Assert.That(cruder.AfterInsertRow, Is.Null);
        }

        [Test]
        public void Constructor_ConnectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new SqlCruder(null, "dbo"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_ConnectionIsNotOpen_ThrowsArgumentException()
        {
            // Arrange
            using var connection = new SqlConnection(TestHelper.ConnectionString);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => new SqlCruder(connection, "dbo"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
            Assert.That(ex.Message, Does.StartWith("Connection should be opened."));
        }

        #endregion

        #region GetTableValuesConverter

        [Test]
        public void GetTableValuesConverter_ValidArgument_ReturnsProperConverter()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var converter = cruder.GetTableValuesConverter("PersonData");

            // Assert
            var dbValueConverter = converter.GetColumnConverter("Id");
            Assert.That(dbValueConverter, Is.TypeOf<GuidValueConverter>());
        }

        [Test]
        public void GetTableValuesConverter_ArgumentIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.GetTableValuesConverter(null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetTableValuesConverter_NotExistingSchema_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetTableValuesConverter("some_table"));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetTableValuesConverter_NotExistingTable_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetTableValuesConverter("bad_table"));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion

        #region ResetTables

        [Test]
        public void ResetTables_NoArguments_RunsOk()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);
            cruder.GetTableValuesConverter("PersonData").SetColumnConverter("Id", new StringValueConverter());
            var oldDbValueConverter = cruder.GetTableValuesConverter("PersonData").GetColumnConverter("Id");

            // Act
            cruder.ResetTables();
            var resetDbValueConverter = cruder.GetTableValuesConverter("PersonData").GetColumnConverter("Id");

            // Assert
            Assert.That(oldDbValueConverter, Is.TypeOf<StringValueConverter>());
            Assert.That(resetDbValueConverter, Is.TypeOf<GuidValueConverter>());
        }

        #endregion

        #region InsertRow

        [Test]
        public void InsertRow_ValidArguments_InsertsRow()
        {
            // Arrange
            var row1 = new Dictionary<string, object>
            {
                {"Id", new Guid("a776fd76-f2a8-4e09-9e69-b6d08e96c075")},
                {"PersonId", 101},
                {"Weight", 69.20m},
                {"PersonMetaKey", (short) 12},
                {"IQ", 101.60m},
                {"Temper", (short) 4},
                {"PersonOrdNumber", (byte) 3},
                {"MetricB", -3},
                {"MetricA", 177},
                {"NotExisting", 11},
            };

            var row2 = new DynamicRow();
            row2.SetProperty("Id", new Guid("a776fd76-f2a8-4e09-9e69-b6d08e96c075"));
            row2.SetProperty("PersonId", 101);
            row2.SetProperty("Weight", 69.20m);
            row2.SetProperty("PersonMetaKey", (short)12);
            row2.SetProperty("IQ", 101.60m);
            row2.SetProperty("Temper", (short)4);
            row2.SetProperty("PersonOrdNumber", (byte)3);
            row2.SetProperty("MetricB", -3);
            row2.SetProperty("MetricA", 177);
            row2.SetProperty("NotExisting", 11);

            var row3 = new
            {
                Id = new Guid("a776fd76-f2a8-4e09-9e69-b6d08e96c075"),
                PersonId = 101,
                Weight = 69.20m,
                PersonMetaKey = (short)12,
                IQ = 101.60m,
                Temper = (short)4,
                PersonOrdNumber = (byte)3,
                MetricB = -3,
                MetricA = 177,
                NotExisting = 11,
            };

            var row4 = new HealthInfoDto
            {
                Id = new Guid("a776fd76-f2a8-4e09-9e69-b6d08e96c075"),
                PersonId = 101,
                Weight = 69.20m,
                PersonMetaKey = 12,
                IQ = 101.60m,
                Temper = 4,
                PersonOrdNumber = 3,
                MetricB = -3,
                MetricA = 177,
                NotExisting = 11,
            };

            object[] rows =
            {
                row1,
                row2,
                row3,
                row4,
            };

            IReadOnlyDictionary<string, object>[] loadedRows = new IReadOnlyDictionary<string, object>[rows.Length];

            this.Connection.ExecuteSingleSql("ALTER TABLE [zeta].[HealthInfo] DROP CONSTRAINT [FK_healthInfo_Person]");

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                cruder.InsertRow("HealthInfo", row, x => x != "NotExisting");
                var loadedRow = TestHelper.LoadRow(
                    this.Connection,
                    TestHelper.SchemaName,
                    "HealthInfo",
                    new Guid("a776fd76-f2a8-4e09-9e69-b6d08e96c075"));

                loadedRows[i] = loadedRow;

                this.Connection.ExecuteSingleSql("DELETE FROM [zeta].[HealthInfo]");
            }

            // Assert
            for (var i = 0; i < loadedRows.Length; i++)
            {
                var originalRow = rows[i];
                var cleanOriginalRow = new DynamicRow(originalRow);
                cleanOriginalRow.RemoveProperty("NotExisting");

                var originalRowJson = JsonConvert.SerializeObject(cleanOriginalRow);
                var loadedJson = JsonConvert.SerializeObject(loadedRows[i]);

                Assert.That(loadedJson, Is.EqualTo(originalRowJson));
            }
        }

        [Test]
        public void InsertRow_AllDataTypes_RunsOk()
        {
            // Arrange
            this.CreateSuperTable();

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            dynamic row = new DynamicRow(new
            {
                TheGuid = new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0"),

                TheBit = true,

                TheTinyInt = (byte)17,
                TheSmallInt = (short)11,
                TheInt = 44,
                TheBigInt = 777L,

                TheDecimal = 11.2m,
                TheNumeric = 22.3m,

                TheSmallMoney = 123.06m,
                TheMoney = 60.77m,

                TheReal = (float)15.99,
                TheFloat = 7001.555,

                TheDate = DateTime.Parse("2010-01-02"),
                TheDateTime = DateTime.Parse("2011-11-12T10:10:10"),
                TheDateTime2 = DateTime.Parse("2015-03-07T05:06:33.777"),
                TheDateTimeOffset = DateTimeOffset.Parse("2011-11-12T10:10:10+03:00"),
                TheSmallDateTime = DateTime.Parse("1970-04-08T11:11:11"),
                TheTime = TimeSpan.Parse("03:03:03"),

                TheChar = "abc",
                TheVarChar = "Andrey Kovalenko",
                TheVarCharMax = "Rocky Marciano",

                TheNChar = "АБВ",
                TheNVarChar = "Андрей Коваленко",
                TheNVarCharMax = "Роки Марчиано",

                TheBinary = new byte[] { 0x10, 0x20, 0x33 },
                TheVarBinary = new byte[] { 0xff, 0xee, 0xbb },
                TheVarBinaryMax = new byte[] { 0x80, 0x90, 0xa0 },
            });

            // Act
            cruder.InsertRow("SuperTable", row, (Func<string, bool>)(x => true));

            // Assert
            var insertedRow = TestHelper.LoadRow(this.Connection, TestHelper.SchemaName, "SuperTable", 1);

            Assert.That(insertedRow["TheGuid"], Is.EqualTo(new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0")));

            Assert.That(insertedRow["TheBit"], Is.EqualTo(true));

            Assert.That(insertedRow["TheTinyInt"], Is.EqualTo((byte)17));
            Assert.That(insertedRow["TheSmallInt"], Is.EqualTo((short)11));
            Assert.That(insertedRow["TheInt"], Is.EqualTo(44));
            Assert.That(insertedRow["TheBigInt"], Is.EqualTo(777L));

            Assert.That(insertedRow["TheDecimal"], Is.EqualTo(11.2m));
            Assert.That(insertedRow["TheNumeric"], Is.EqualTo(22.3m));

            Assert.That(insertedRow["TheSmallMoney"], Is.EqualTo(123.06m));
            Assert.That(insertedRow["TheMoney"], Is.EqualTo(60.77m));

            Assert.That(insertedRow["TheReal"], Is.EqualTo((float)15.99));
            Assert.That(insertedRow["TheFloat"], Is.EqualTo(7001.555));

            Assert.That(insertedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-02")));
            Assert.That(insertedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
            Assert.That(insertedRow["TheDateTime2"], Is.EqualTo(DateTime.Parse("2015-03-07T05:06:33.777")));
            Assert.That(insertedRow["TheDateTimeOffset"],
                Is.EqualTo(DateTimeOffset.Parse("2011-11-12T10:10:10+03:00")));
            Assert.That(insertedRow["TheSmallDateTime"], Is.EqualTo(DateTime.Parse("1970-04-08T11:11")));
            Assert.That(insertedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("03:03:03")));

            Assert.That(insertedRow["TheChar"], Does.StartWith("abc"));
            Assert.That(insertedRow["TheVarChar"], Is.EqualTo("Andrey Kovalenko"));
            Assert.That(insertedRow["TheVarCharMax"], Is.EqualTo("Rocky Marciano"));

            Assert.That(insertedRow["TheNChar"], Does.StartWith("АБВ"));
            Assert.That(insertedRow["TheNVarChar"], Is.EqualTo("Андрей Коваленко"));
            Assert.That(insertedRow["TheNVarCharMax"], Is.EqualTo("Роки Марчиано"));

            CollectionAssert.AreEqual(new byte[] { 0x10, 0x20, 0x33 }, ((byte[])insertedRow["TheBinary"]).Take(3));
            CollectionAssert.AreEqual(new byte[] { 0xff, 0xee, 0xbb }, (byte[])insertedRow["TheVarBinary"]);
            CollectionAssert.AreEqual(new byte[] { 0x80, 0x90, 0xa0 }, (byte[])insertedRow["TheVarBinaryMax"]);
        }

        [Test]
        public void InsertRow_RowIsEmptyAndSelectorIsFalser_InsertsDefaultValues()
        {
            // Arrange
            var row1 = new Dictionary<string, object>();
            var row2 = new DynamicRow();
            var row3 = new { };

            object[] rows =
            {
                row1,
                row2,
                row3,
            };

            var insertedRows = new IReadOnlyDictionary<string, object>[rows.Length];

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            using var command = this.Connection.CreateCommand();

            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];

                var createTableSql = @"
CREATE TABLE [zeta].[MyTab](
    [Id] int NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Length] int NULL DEFAULT NULL,
    [Name] nvarchar(100) DEFAULT 'Polly')
";
                command.CommandText = createTableSql;
                command.ExecuteNonQuery();

                cruder.InsertRow("MyTab", row, x => false);
                var insertedRow = TestHelper.LoadRow(this.Connection, TestHelper.SchemaName, "MyTab", 1);
                insertedRows[i] = insertedRow;

                this.Connection.ExecuteSingleSql("DROP TABLE [zeta].[MyTab]");
            }

            // Assert
            var json = JsonConvert.SerializeObject(
                new
                {
                    Id = 1,
                    Length = (int?)null,
                    Name = "Polly",
                },
                Formatting.Indented);

            foreach (var insertedRow in insertedRows)
            {
                var insertedJson = JsonConvert.SerializeObject(insertedRow, Formatting.Indented);
                Assert.That(insertedJson, Is.EqualTo(json));
            }
        }

        [Test]
        public void InsertRow_RowHasUnknownPropertiesAndSelectorIsFalser_InsertsDefaultValues()
        {
            // Arrange
            this.CreateSmallTable();

            var row1 = new Dictionary<string, object>
            {
                {"NonExisting", 777},
            };

            var row2 = new DynamicRow();
            row2.SetProperty("NonExisting", 777);

            var row3 = new
            {
                NonExisting = 777,
            };

            var row4 = new DummyDto
            {
                NonExisting = 777,
            };

            object[] rows =
            {
                row1,
                row2,
                row3,
                row4,
            };

            IReadOnlyDictionary<string, object>[] insertedRows = new IReadOnlyDictionary<string, object>[rows.Length];
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                cruder.InsertRow("SmallTable", row, x => false);

                var lastIdentity = (int)this.Connection.GetLastIdentity();

                var insertedRow = TestHelper.LoadRow(
                    this.Connection,
                    TestHelper.SchemaName,
                    "SmallTable",
                    lastIdentity);

                insertedRows[i] = insertedRow;

                this.Connection.ExecuteSingleSql("DELETE FROM [zeta].[SmallTable]");
            }

            // Assert
            foreach (var insertedRow in insertedRows)
            {
                Assert.That(insertedRow["TheInt"], Is.EqualTo(1599));
                Assert.That(insertedRow["TheNVarChar"], Is.EqualTo("Semmi"));
            }
        }

        [Test]
        public void InsertRow_NoColumnForSelectedProperty_ThrowsTauDbException()
        {
            // Arrange
            this.CreateSmallTable();

            var row = new
            {
                TheInt = 1,
                TheNVarChar = "Polina",
                NotExisting = 100,
            };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("SmallTable", row));

            // Assert
            Assert.That(ex, Has.Message.EqualTo($"Column 'NotExisting' not found in table 'SmallTable'."));
        }

        [Test]
        public void InsertRow_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("some_table", new object()));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void InsertRow_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("bad_table", new object()));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void InsertRow_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.InsertRow(null, new object(), x => true));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void InsertRow_RowIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.InsertRow("HealthInfo", null, x => true));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("row"));
        }

        [Test]
        public void InsertRow_RowContainsDBNullValue_ThrowsTauDbException()
        {
            // Arrange
            this.CreateSuperTable();
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);
            var row = new
            {
                TheGuid = DBNull.Value,
            };

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("SuperTable", row, x => x == "TheGuid"));
            
            // Assert
            Assert.That(ex,
                Has.Message.EqualTo(
                    "Failed to apply value to DB command. See inner exception for details. Table: 'SuperTable', column: 'TheGuid', value: 'System.DBNull'."));
        }

        #endregion

        #region InsertRows

        [Test]
        public void InsertRows_ValidArguments_InsertsRows()
        {
            // Arrange
            var row1 = new Dictionary<string, object>
            {
                {"Id", new Guid("11111111-1111-1111-1111-111111111111")},
                {"PersonId", 101},
                {"Weight", 69.20m},
                {"PersonMetaKey", (short) 12},
                {"IQ", 101.60m},
                {"Temper", (short) 4},
                {"PersonOrdNumber", (byte) 3},
                {"MetricB", -3},
                {"MetricA", 177},
                {"NotExisting", 7},
            };

            var row2 = new DynamicRow();
            row2.SetProperty("Id", new Guid("22222222-2222-2222-2222-222222222222"));
            row2.SetProperty("PersonId", 101);
            row2.SetProperty("Weight", 69.20m);
            row2.SetProperty("PersonMetaKey", (short)12);
            row2.SetProperty("IQ", 101.60m);
            row2.SetProperty("Temper", (short)4);
            row2.SetProperty("PersonOrdNumber", (byte)3);
            row2.SetProperty("MetricB", -3);
            row2.SetProperty("MetricA", 177);
            row2.SetProperty("NotExisting", 7);

            var row3 = new
            {
                Id = new Guid("33333333-3333-3333-3333-333333333333"),
                PersonId = 101,
                Weight = 69.20m,
                PersonMetaKey = (short)12,
                IQ = 101.60m,
                Temper = (short)4,
                PersonOrdNumber = (byte)3,
                MetricB = -3,
                MetricA = 177,
                NotExisting = 7,
            };

            var row4 = new HealthInfoDto
            {
                Id = new Guid("44444444-4444-4444-4444-444444444444"),
                PersonId = 101,
                Weight = 69.20m,
                PersonMetaKey = 12,
                IQ = 101.60m,
                Temper = 4,
                PersonOrdNumber = 3,
                MetricB = -3,
                MetricA = 177,
                NotExisting = 7,
            };

            object[] rows =
            {
                row1,
                row2,
                row3,
                row4,
            };

            this.Connection.ExecuteSingleSql("ALTER TABLE [zeta].[HealthInfo] DROP CONSTRAINT [FK_healthInfo_Person]");

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            cruder.InsertRows("HealthInfo", rows, x => x != "NotExisting");

            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    *
FROM
    [zeta].[HealthInfo]
ORDER BY
    [Id]
";
            var loadedRows = DbTools.GetCommandRows(command);
            Assert.That(loadedRows, Has.Count.EqualTo(4));

            for (var i = 0; i < loadedRows.Count; i++)
            {
                var cleanOriginalRow = new DynamicRow(rows[i]);
                cleanOriginalRow.RemoveProperty("NotExisting");

                var json = JsonConvert.SerializeObject(cleanOriginalRow, Formatting.Indented);
                var loadedJson = JsonConvert.SerializeObject(loadedRows[i], Formatting.Indented);

                Assert.That(json, Is.EqualTo(loadedJson));
            }
        }

        [Test]
        public void InsertRows_RowsAreEmptyAndSelectorIsFalser_InsertsDefaultValues()
        {
            // Arrange
            this.CreateSmallTable();

            var row1 = new Dictionary<string, object>();
            var row2 = new DynamicRow();
            var row3 = new object();

            var rows = new[]
            {
                row1,
                row2,
                row3,
            };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            cruder.InsertRows("SmallTable", rows, x => false);

            // Assert
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    *
FROM
    [zeta].[SmallTable]
ORDER BY
    [Id]
";
            var loadedRows = DbTools.GetCommandRows(command);
            Assert.That(loadedRows, Has.Count.EqualTo(3));

            foreach (var loadedRow in loadedRows)
            {
                Assert.That(loadedRow.TheInt, Is.EqualTo(1599));
                Assert.That(loadedRow.TheNVarChar, Is.EqualTo("Semmi"));
            }
        }

        [Test]
        public void InsertRows_PropertySelectorProducesNoProperties_InsertsDefaultValues()
        {
            // Arrange
            this.CreateSmallTable();

            var row1 = new
            {
                TheInt = 77,
                TheNVarChar = "abc",
            };

            var row2 = new
            {
                TheInt = 88,
                TheNVarChar = "def",
            };

            var rows = new[] { row1, row2 };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            cruder.InsertRows("SmallTable", rows, x => false);

            // Assert
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    *
FROM
    [zeta].[SmallTable]
ORDER BY
    [Id]
";

            var loadedRows = DbTools.GetCommandRows(command);
            Assert.That(loadedRows, Has.Count.EqualTo(2));

            foreach (var loadedRow in loadedRows)
            {
                Assert.That(loadedRow.TheInt, Is.EqualTo(1599));
                Assert.That(loadedRow.TheNVarChar, Is.EqualTo("Semmi"));
            }
        }

        [Test]
        public void InsertRows_PropertySelectorIsNull_UsesAllColumns()
        {
            // Arrange
            this.CreateSmallTable();

            var row1 = new
            {
                TheInt = 77,
                TheNVarChar = "abc",
            };

            var row2 = new
            {
                TheInt = 88,
                TheNVarChar = "def",
            };

            var rows = new[] { row1, row2 };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            cruder.InsertRows("SmallTable", rows);

            // Assert
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    *
FROM
    [zeta].[SmallTable]
ORDER BY
    [Id]
";

            var loadedRows = DbTools.GetCommandRows(command);
            Assert.That(loadedRows, Has.Count.EqualTo(2));

            var loadedRow = loadedRows[0];
            Assert.That(loadedRow.TheInt, Is.EqualTo(77));
            Assert.That(loadedRow.TheNVarChar, Is.EqualTo("abc"));

            loadedRow = loadedRows[1];
            Assert.That(loadedRow.TheInt, Is.EqualTo(88));
            Assert.That(loadedRow.TheNVarChar, Is.EqualTo("def"));
        }

        [Test]
        public void InsertRows_NoColumnForSelectedProperty_ThrowsTauDbException()
        {
            // Arrange
            this.CreateSmallTable();

            var row1 = new
            {
                TheInt = 77,
                TheNVarChar = "abc",
                NotExisting = 2,
            };

            var row2 = new
            {
                TheInt = 88,
                TheNVarChar = "def",
                NotExisting = 1,
            };

            var rows = new[] { row1, row2 };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRows("SmallTable", rows, x => true));

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Column 'NotExisting' not found in table 'SmallTable'."));
        }

        [Test]
        public void InsertRows_NextRowSignatureDiffersFromPrevious_ThrowsArgumentException()
        {
            // Arrange
            this.CreateSmallTable();

            var row1 = new
            {
                TheInt = 77,
            };

            var row2 = new
            {
                TheNVarChar = "def",
            };

            var rows = new object[] { row1, row2 };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.InsertRows("SmallTable", rows, x => true));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("'values' does not contain property representing column 'TheInt' of table 'SmallTable'."));
        }

        [Test]
        public void InsertRows_SchemaDoesNotExist_TauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRows("some_table", new object[] { }));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void InsertRows_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRows("bad_table", new object[] { }));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void InsertRows_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.InsertRows(null, new object[] { }));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void InsertRows_RowsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.InsertRows("HealthInfo", null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("rows"));
        }

        [Test]
        public void InsertRows_RowsContainNull_ThrowsArgumentException()
        {
            // Arrange
            this.CreateSmallTable();

            var rows = new[]
            {
                new object(),
                null,
            };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.InsertRows("SmallTable", rows));

            // Assert
            Assert.That(ex, Has.Message.StartWith("'rows' must not contain nulls."));
            Assert.That(ex.ParamName, Is.EqualTo("rows"));
        }

        [Test]
        public void InsertRows_RowContainsDBNullValue_ThrowsTauDbException()
        {
            // Arrange
            this.CreateSmallTable();

            var rows = new object[]
            {
                new
                {
                    TheInt = 10,
                },
                new
                {
                    TheInt = DBNull.Value,
                },
            };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRows("SmallTable", rows));

            // Assert
            Assert.That(ex,
                Has.Message.StartWith(
                    "Failed to apply value to DB command. See inner exception for details. Table: 'SmallTable', column: 'TheInt', value: 'System.DBNull'."));
        }

        #endregion

        #region RowInsertedCallback

        [Test]
        public void RowInsertedCallback_SetToSomeValue_KeepsThatValue()
        {
            // Arrange
            this.CreateSmallTable();

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);
            var sb1 = new StringBuilder();

            // Act
            Func<TableMold, object, int, object> callback = (tableMold, row, index) =>
            {
                sb1.Append($"Table name: {tableMold.Name}; index: {index}");
                return row;
            };

            cruder.BeforeInsertRow = callback;

            cruder.InsertRow("SmallTable", new object());
            var callback1 = cruder.BeforeInsertRow;

            cruder.BeforeInsertRow = null;
            var callback2 = cruder.BeforeInsertRow;

            // Assert
            var s = sb1.ToString();
            Assert.That(s, Is.EqualTo("Table name: SmallTable; index: 0"));

            Assert.That(callback1, Is.SameAs(callback));
            Assert.That(callback2, Is.Null);
        }

        [Test]
        public void RowInsertedCallback_SetToNonNull_IsCalledWhenInsertRowIsCalled()
        {
            // Arrange
            this.CreateSmallTable();

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);
            var sb1 = new StringBuilder();

            // Act
            cruder.BeforeInsertRow = (table, row, index) =>
            {
                sb1.Append($"Before insertion. Table name: {table.Name}; index: {index}. ");
                return row;
            };

            cruder.AfterInsertRow = (table, row, index) =>
            {
                sb1.Append($"After insertion. Table name: {table.Name}; index: {index}.");
            };

            cruder.InsertRow("SmallTable", new object());

            // Assert
            var s = sb1.ToString();
            Assert.That(s, Is.EqualTo("Before insertion. Table name: SmallTable; index: 0. After insertion. Table name: SmallTable; index: 0."));
        }

        [Test]
        public void RowInsertedCallback_SetToNonNull_IsCalledWhenInsertRowsIsCalled()
        {
            // Arrange
            this.CreateSmallTable();

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);
            var sb1 = new StringBuilder();

            // justified_todo: name of this ut and others like this.

            // Act
            cruder.BeforeInsertRow = (table, row, index) =>
            {
                sb1.AppendLine($"Table name: {table.Name}; index: {index}; int: {((dynamic)row).TheInt}");
                return row;
            };

            cruder.InsertRows(
                "SmallTable",
                new object[]
                {
                    new
                    {
                        TheInt = 11,
                    },
                    new
                    {
                        TheInt = 22,
                    },
                });

            // Assert
            var s = sb1.ToString();
            Assert.That(s, Is.EqualTo(@"Table name: SmallTable; index: 0; int: 11
Table name: SmallTable; index: 1; int: 22
"));
        }

        #endregion

        #region GetRow

        [Test]
        public void GetRow_ValidArguments_ReturnsRow()
        {
            // Arrange
            this.CreateSuperTable();

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            dynamic row = new DynamicRow(new
            {
                TheGuid = new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0"),

                TheBit = true,

                TheTinyInt = (byte)17,
                TheSmallInt = (short)11,
                TheInt = 44,
                TheBigInt = 777L,

                TheDecimal = 11.2m,
                TheNumeric = 22.3m,

                TheSmallMoney = 123.06m,
                TheMoney = 60.77m,

                TheReal = (float)15.99,
                TheFloat = 7001.555,

                TheDate = DateTime.Parse("2010-01-02"),
                TheDateTime = DateTime.Parse("2011-11-12T10:10:10"),
                TheDateTime2 = DateTime.Parse("2015-03-07T05:06:33.777"),
                TheDateTimeOffset = DateTimeOffset.Parse("2011-11-12T10:10:10+03:00"),
                TheSmallDateTime = DateTime.Parse("1970-04-08T11:11:11"),
                TheTime = TimeSpan.Parse("03:03:03"),

                TheChar = "abc",
                TheVarChar = "Andrey Kovalenko",
                TheVarCharMax = "Rocky Marciano",

                TheNChar = "АБВ",
                TheNVarChar = "Андрей Коваленко",
                TheNVarCharMax = "Роки Марчиано",

                TheBinary = new byte[] { 0x10, 0x20, 0x33 },
                TheVarBinary = new byte[] { 0xff, 0xee, 0xbb },
                TheVarBinaryMax = new byte[] { 0x80, 0x90, 0xa0 },
            });

            cruder.InsertRow("SuperTable", row, (Func<string, bool>)(x => true)); // InsertRow is ut'ed already :)

            // Act
            var insertedRow = ((DynamicRow)cruder.GetRow("SuperTable", 1, x => x.Contains("DateTime"))).ToDictionary();

            // Assert
            Assert.That(insertedRow, Has.Count.EqualTo(4));

            Assert.That(insertedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
            Assert.That(insertedRow["TheDateTime2"], Is.EqualTo(DateTime.Parse("2015-03-07T05:06:33.777")));
            Assert.That(insertedRow["TheDateTimeOffset"],
                Is.EqualTo(DateTimeOffset.Parse("2011-11-12T10:10:10+03:00")));
            Assert.That(insertedRow["TheSmallDateTime"], Is.EqualTo(DateTime.Parse("1970-04-08T11:11")));
        }

        [Test]
        public void GetRow_AllDataTypes_RunsOk()
        {
            // Arrange
            this.CreateSuperTable();

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            dynamic row = new DynamicRow(new
            {
                TheGuid = new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0"),

                TheBit = true,

                TheTinyInt = (byte)17,
                TheSmallInt = (short)11,
                TheInt = 44,
                TheBigInt = 777L,

                TheDecimal = 11.2m,
                TheNumeric = 22.3m,

                TheSmallMoney = 123.06m,
                TheMoney = 60.77m,

                TheReal = (float)15.99,
                TheFloat = 7001.555,

                TheDate = DateTime.Parse("2010-01-02"),
                TheDateTime = DateTime.Parse("2011-11-12T10:10:10"),
                TheDateTime2 = DateTime.Parse("2015-03-07T05:06:33.777"),
                TheDateTimeOffset = DateTimeOffset.Parse("2011-11-12T10:10:10+03:00"),
                TheSmallDateTime = DateTime.Parse("1970-04-08T11:11:11"),
                TheTime = TimeSpan.Parse("03:03:03"),

                TheChar = "abc",
                TheVarChar = "Andrey Kovalenko",
                TheVarCharMax = "Rocky Marciano",

                TheNChar = "АБВ",
                TheNVarChar = "Андрей Коваленко",
                TheNVarCharMax = "Роки Марчиано",

                TheBinary = new byte[] { 0x10, 0x20, 0x33 },
                TheVarBinary = new byte[] { 0xff, 0xee, 0xbb },
                TheVarBinaryMax = new byte[] { 0x80, 0x90, 0xa0 },
            });

            cruder.InsertRow("SuperTable", row, (Func<string, bool>)(x => true)); // InsertRow is ut'ed already :)

            // Act
            var insertedRow = ((DynamicRow)cruder.GetRow("SuperTable", 1)).ToDictionary();

            // Assert
            Assert.That(insertedRow["TheGuid"], Is.EqualTo(new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0")));

            Assert.That(insertedRow["TheBit"], Is.EqualTo(true));

            Assert.That(insertedRow["TheTinyInt"], Is.EqualTo((byte)17));
            Assert.That(insertedRow["TheSmallInt"], Is.EqualTo((short)11));
            Assert.That(insertedRow["TheInt"], Is.EqualTo(44));
            Assert.That(insertedRow["TheBigInt"], Is.EqualTo(777L));

            Assert.That(insertedRow["TheDecimal"], Is.EqualTo(11.2m));
            Assert.That(insertedRow["TheNumeric"], Is.EqualTo(22.3m));

            Assert.That(insertedRow["TheSmallMoney"], Is.EqualTo(123.06m));
            Assert.That(insertedRow["TheMoney"], Is.EqualTo(60.77m));

            Assert.That(insertedRow["TheReal"], Is.EqualTo((float)15.99));
            Assert.That(insertedRow["TheFloat"], Is.EqualTo(7001.555));

            Assert.That(insertedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-02")));
            Assert.That(insertedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
            Assert.That(insertedRow["TheDateTime2"], Is.EqualTo(DateTime.Parse("2015-03-07T05:06:33.777")));
            Assert.That(insertedRow["TheDateTimeOffset"],
                Is.EqualTo(DateTimeOffset.Parse("2011-11-12T10:10:10+03:00")));
            Assert.That(insertedRow["TheSmallDateTime"], Is.EqualTo(DateTime.Parse("1970-04-08T11:11")));
            Assert.That(insertedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("03:03:03")));

            Assert.That(insertedRow["TheChar"], Does.StartWith("abc"));
            Assert.That(insertedRow["TheVarChar"], Is.EqualTo("Andrey Kovalenko"));
            Assert.That(insertedRow["TheVarCharMax"], Is.EqualTo("Rocky Marciano"));

            Assert.That(insertedRow["TheNChar"], Does.StartWith("АБВ"));
            Assert.That(insertedRow["TheNVarChar"], Is.EqualTo("Андрей Коваленко"));
            Assert.That(insertedRow["TheNVarCharMax"], Is.EqualTo("Роки Марчиано"));

            CollectionAssert.AreEqual(new byte[] { 0x10, 0x20, 0x33 }, ((byte[])insertedRow["TheBinary"]).Take(3));
            CollectionAssert.AreEqual(new byte[] { 0xff, 0xee, 0xbb }, (byte[])insertedRow["TheVarBinary"]);
            CollectionAssert.AreEqual(new byte[] { 0x80, 0x90, 0xa0 }, (byte[])insertedRow["TheVarBinaryMax"]);
        }

        [Test]
        public void GetRow_SelectorIsTruer_DeliversAllColumns()
        {
            // Arrange
            this.CreateSuperTable();

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            dynamic row = new DynamicRow(new
            {
                TheGuid = new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0"),

                TheBit = true,

                TheTinyInt = (byte)17,
                TheSmallInt = (short)11,
                TheInt = 44,
                TheBigInt = 777L,

                TheDecimal = 11.2m,
                TheNumeric = 22.3m,

                TheSmallMoney = 123.06m,
                TheMoney = 60.77m,

                TheReal = (float)15.99,
                TheFloat = 7001.555,

                TheDate = DateTime.Parse("2010-01-02"),
                TheDateTime = DateTime.Parse("2011-11-12T10:10:10"),
                TheDateTime2 = DateTime.Parse("2015-03-07T05:06:33.777"),
                TheDateTimeOffset = DateTimeOffset.Parse("2011-11-12T10:10:10+03:00"),
                TheSmallDateTime = DateTime.Parse("1970-04-08T11:11:11"),
                TheTime = TimeSpan.Parse("03:03:03"),

                TheChar = "abc",
                TheVarChar = "Andrey Kovalenko",
                TheVarCharMax = "Rocky Marciano",

                TheNChar = "АБВ",
                TheNVarChar = "Андрей Коваленко",
                TheNVarCharMax = "Роки Марчиано",

                TheBinary = new byte[] { 0x10, 0x20, 0x33 },
                TheVarBinary = new byte[] { 0xff, 0xee, 0xbb },
                TheVarBinaryMax = new byte[] { 0x80, 0x90, 0xa0 },
            });

            cruder.InsertRow("SuperTable", row, (Func<string, bool>)(x => true)); // InsertRow is ut'ed already :)

            // Act
            var insertedRow = ((DynamicRow)cruder.GetRow("SuperTable", 1, x => true)).ToDictionary();

            // Assert
            Assert.That(insertedRow["TheGuid"], Is.EqualTo(new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0")));

            Assert.That(insertedRow["TheBit"], Is.EqualTo(true));

            Assert.That(insertedRow["TheTinyInt"], Is.EqualTo((byte)17));
            Assert.That(insertedRow["TheSmallInt"], Is.EqualTo((short)11));
            Assert.That(insertedRow["TheInt"], Is.EqualTo(44));
            Assert.That(insertedRow["TheBigInt"], Is.EqualTo(777L));

            Assert.That(insertedRow["TheDecimal"], Is.EqualTo(11.2m));
            Assert.That(insertedRow["TheNumeric"], Is.EqualTo(22.3m));

            Assert.That(insertedRow["TheSmallMoney"], Is.EqualTo(123.06m));
            Assert.That(insertedRow["TheMoney"], Is.EqualTo(60.77m));

            Assert.That(insertedRow["TheReal"], Is.EqualTo((float)15.99));
            Assert.That(insertedRow["TheFloat"], Is.EqualTo(7001.555));

            Assert.That(insertedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-02")));
            Assert.That(insertedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
            Assert.That(insertedRow["TheDateTime2"], Is.EqualTo(DateTime.Parse("2015-03-07T05:06:33.777")));
            Assert.That(insertedRow["TheDateTimeOffset"],
                Is.EqualTo(DateTimeOffset.Parse("2011-11-12T10:10:10+03:00")));
            Assert.That(insertedRow["TheSmallDateTime"], Is.EqualTo(DateTime.Parse("1970-04-08T11:11")));
            Assert.That(insertedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("03:03:03")));

            Assert.That(insertedRow["TheChar"], Does.StartWith("abc"));
            Assert.That(insertedRow["TheVarChar"], Is.EqualTo("Andrey Kovalenko"));
            Assert.That(insertedRow["TheVarCharMax"], Is.EqualTo("Rocky Marciano"));

            Assert.That(insertedRow["TheNChar"], Does.StartWith("АБВ"));
            Assert.That(insertedRow["TheNVarChar"], Is.EqualTo("Андрей Коваленко"));
            Assert.That(insertedRow["TheNVarCharMax"], Is.EqualTo("Роки Марчиано"));

            CollectionAssert.AreEqual(new byte[] { 0x10, 0x20, 0x33 }, ((byte[])insertedRow["TheBinary"]).Take(3));
            CollectionAssert.AreEqual(new byte[] { 0xff, 0xee, 0xbb }, (byte[])insertedRow["TheVarBinary"]);
            CollectionAssert.AreEqual(new byte[] { 0x80, 0x90, 0xa0 }, (byte[])insertedRow["TheVarBinaryMax"]);
        }

        [Test]
        public void GetRow_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetRow("some_table", 1));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetRow_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetRow("bad_table", 1));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void GetRow_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.GetRow(null, 1));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetRow_IdIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.GetRow("some_table", null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("id"));
        }

        [Test]
        public void GetRow_TableHasNoPrimaryKey_ThrowsArgumentException()

        {
            // Arrange
            this.Connection.ExecuteSingleSql("CREATE TABLE [zeta].[dummy](Foo int)"); // no PK
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentException>((() => cruder.GetRow("dummy", 1)));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Table 'dummy' does not have a primary key."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetRow_TablePrimaryKeyIsMultiColumn_ThrowsArgumentException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentException>((() => cruder.GetRow("Person", "the_id")));

            // Assert
            Assert.That(ex,
                Has.Message.StartsWith("Failed to retrieve single primary key column name for the table 'Person'."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetRow_IdNotFound_ReturnsNull()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);
            const int nonExistingId = 133;

            // Act
            var row = cruder.GetRow("NumericData", nonExistingId);

            // Assert
            Assert.That(row, Is.Null);
        }

        [Test]
        public void GetRow_SelectorIsFalser_ThrowsArgumentException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.GetRow("NumericData", 111, x => false));

            // Assert
            Assert.That(ex, Has.Message.StartWith("No columns were selected."));
            Assert.That(ex.ParamName, Is.EqualTo("columnSelector"));
        }

        #endregion

        #region GetAllRows

        [Test]
        public void GetAllRows_ValidArguments_ReturnsRows()
        {
            // Arrange
            var insertSql = this.GetType().Assembly.GetResourceText("InsertRows.sql", true);
            this.Connection.ExecuteCommentedScript(insertSql);

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var rows = cruder.GetAllRows("DateData", x => x == "Moment");

            // Assert
            var row = (DynamicRow)rows[0];
            Assert.That(row.GetDynamicMemberNames().Count(), Is.EqualTo(1));
            Assert.That(row.GetProperty("Moment"), Is.EqualTo(DateTimeOffset.Parse("2020-01-01T05:05:05+00:00")));

            row = rows[1];
            Assert.That(row.GetDynamicMemberNames().Count(), Is.EqualTo(1));
            Assert.That(row.GetProperty("Moment"), Is.EqualTo(DateTimeOffset.Parse("2020-02-02T06:06:06+00:00")));
        }

        [Test]
        public void GetAllRows_SelectorIsTruer_DeliversAllColumns()
        {
            // Arrange
            var insertSql = this.GetType().Assembly.GetResourceText("InsertRows.sql", true);
            this.Connection.ExecuteCommentedScript(insertSql);

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var rows = cruder.GetAllRows("DateData", x => true);

            // Assert
            var row = rows[0];
            Assert.That(row.Id, Is.EqualTo(new Guid("11111111-1111-1111-1111-111111111111")));
            Assert.That(row.Moment, Is.EqualTo(DateTimeOffset.Parse("2020-01-01T05:05:05+00:00")));

            row = rows[1];
            Assert.That(row.Id, Is.EqualTo(new Guid("22222222-2222-2222-2222-222222222222")));
            Assert.That(row.Moment, Is.EqualTo(DateTimeOffset.Parse("2020-02-02T06:06:06+00:00")));
        }

        [Test]
        public void GetAllRows_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetAllRows("some_table"));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetAllRows_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetAllRows("bad_table"));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void GetAllRows_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.GetAllRows(null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetAllRows_SelectorIsFalser_ThrowsArgumentException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.GetAllRows("HealthInfo", x => false));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("No columns were selected."));
            Assert.That(ex.ParamName, Is.EqualTo("columnSelector"));
        }

        #endregion

        #region UpdateRow

        [Test]
        public void UpdateRow_ValidArguments_UpdatesRow()
        {
            // Arrange
            var id = 1; // will be inserted by IDENTITY

            var update1 = new Dictionary<string, object>
            {
                {"Id", id},
                {"TheTinyInt", (byte) 2},
                {"TheSmallInt", (short) 22},
                {"TheInt", 222},
                {"TheBigInt", 2222L},
                {"NotExisting", 777},
            };

            var update2 = new DynamicRow();
            update2.SetProperty("Id", id);
            update2.SetProperty("TheTinyInt", (byte)2);
            update2.SetProperty("TheSmallInt", (short)22);
            update2.SetProperty("TheInt", 222);
            update2.SetProperty("TheBigInt", 2222L);
            update2.SetProperty("NotExisting", 777);

            var update3 = new
            {
                Id = id,
                TheTinyInt = (byte)2,
                TheSmallInt = (short)22,
                TheInt = 222,
                TheBigInt = 2222L,
                NotExisting = 777,
            };

            var update4 = new SuperTableRowDto
            {
                Id = id,
                TheTinyInt = (byte)2,
                TheSmallInt = (short)22,
                TheInt = 222,
                TheBigInt = 2222L,
                NotExisting = 777,
            };

            var updates = new object[]
            {
                update1,
                update2,
                update3,
                update4,
            };

            var loadedRows = new IReadOnlyDictionary<string, object>[updates.Length];

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            for (var i = 0; i < updates.Length; i++)
            {
                this.CreateSuperTable();
                this.InsertSuperTableRow();

                cruder.UpdateRow(
                    "SuperTable",
                    updates[i],
                    x => x.IsIn(
                        "Id",
                        "TheTinyInt",
                        "TheSmallInt",
                        "TheInt",
                        "TheBigInt"));

                var loadedRow = TestHelper.LoadRow(this.Connection, TestHelper.SchemaName, "SuperTable", 1);
                loadedRows[i] = loadedRow;

                this.Connection.ExecuteSingleSql("DROP TABLE [zeta].[SuperTable]");
            }

            for (var i = 0; i < loadedRows.Length; i++)
            {
                var loadedRow = loadedRows[i];

                Assert.That(loadedRow["TheGuid"], Is.EqualTo(new Guid("11111111-1111-1111-1111-111111111111")));

                Assert.That(loadedRow["TheBit"], Is.EqualTo(true));

                Assert.That(loadedRow["TheTinyInt"], Is.EqualTo((byte)2));
                Assert.That(loadedRow["TheSmallInt"], Is.EqualTo((short)22));
                Assert.That(loadedRow["TheInt"], Is.EqualTo(222));
                Assert.That(loadedRow["TheBigInt"], Is.EqualTo(2222L));

                Assert.That(loadedRow["TheDecimal"], Is.EqualTo(11.10m));
                Assert.That(loadedRow["TheNumeric"], Is.EqualTo(111.10m));

                Assert.That(loadedRow["TheSmallMoney"], Is.EqualTo(1111.1100m));
                Assert.That(loadedRow["TheMoney"], Is.EqualTo(11111.1100m));

                Assert.That(loadedRow["TheReal"], Is.EqualTo((float)111111.0));
                Assert.That(loadedRow["TheFloat"], Is.EqualTo(1111111.0));

                Assert.That(loadedRow["TheDate"], Is.EqualTo(DateTime.Parse("1990-01-01")));
                Assert.That(loadedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("1991-01-01")));
                Assert.That(loadedRow["TheDateTime2"], Is.EqualTo(DateTime.Parse("1992-01-01")));
                Assert.That(loadedRow["TheDateTimeOffset"], Is.EqualTo(DateTimeOffset.Parse("1993-01-01")));
                Assert.That(loadedRow["TheSmallDateTime"], Is.EqualTo(DateTime.Parse("1994-01-01")));
                Assert.That(loadedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("01:01:01")));

                Assert.That(loadedRow["TheChar"], Is.EqualTo("a         "));
                Assert.That(loadedRow["TheVarChar"], Is.EqualTo("aa"));
                Assert.That(loadedRow["TheVarCharMax"], Is.EqualTo("aaa"));

                Assert.That(loadedRow["TheNChar"], Is.EqualTo("ц         "));
                Assert.That(loadedRow["TheNVarChar"], Is.EqualTo("цц"));
                Assert.That(loadedRow["TheNVarCharMax"], Is.EqualTo("ццц"));

                Assert.That(loadedRow["TheBinary"], Is.EqualTo(new byte[] { 1, 0, 0, 0 }));
                Assert.That(loadedRow["TheVarBinary"], Is.EqualTo(new byte[] { 10, 11, }));
                Assert.That(loadedRow["TheVarBinaryMax"], Is.EqualTo(new byte[] { 100, 101, 102 }));
            }
        }

        [Test]
        public void UpdateRow_AllDataTypes_RunsOk()
        {
            // Arrange
            this.CreateSuperTable();
            this.InsertSuperTableRow();

            var update = new SuperTableRowDto
            {
                Id = 1,

                TheGuid = new Guid("22222222-2222-2222-2222-222222222222"),

                TheBit = false,

                TheTinyInt = 2,
                TheSmallInt = 22,
                TheInt = 222,
                TheBigInt = 2222,

                TheDecimal = 22.20m,
                TheNumeric = 222.20m,

                TheSmallMoney = 2222.2200m,
                TheMoney = 22222.2200m,

                TheReal = (float)222222.0,
                TheFloat = 2222222.0,

                TheDate = DateTime.Parse("2002-02-02"),
                TheDateTime = DateTime.Parse("2003-03-03"),
                TheDateTime2 = DateTime.Parse("2004-04-04"),
                TheDateTimeOffset = DateTime.Parse("2005-05-05"),
                TheSmallDateTime = DateTime.Parse("2006-06-06"),
                TheTime = TimeSpan.Parse("02:02:02"),

                TheChar = "b",
                TheVarChar = "bb",
                TheVarCharMax = "bbb",

                TheNChar = "щ",
                TheNVarChar = "щщ",
                TheNVarCharMax = "щщщ",

                TheBinary = new byte[] { 2 },
                TheVarBinary = new byte[] { 20, 22, },
                TheVarBinaryMax = new byte[] { 17, 177, 179 },

                NotExisting = 777,
            };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            cruder.UpdateRow("SuperTable", update, x => x != "NotExisting");

            // Assert
            var loadedRow = TestHelper.LoadRow(this.Connection, TestHelper.SchemaName, "SuperTable", 1);

            Assert.That(loadedRow["TheGuid"], Is.EqualTo(new Guid("22222222-2222-2222-2222-222222222222")));

            Assert.That(loadedRow["TheBit"], Is.EqualTo(false));

            Assert.That(loadedRow["TheTinyInt"], Is.EqualTo((byte)2));
            Assert.That(loadedRow["TheSmallInt"], Is.EqualTo((short)22));
            Assert.That(loadedRow["TheInt"], Is.EqualTo(222));
            Assert.That(loadedRow["TheBigInt"], Is.EqualTo(2222L));

            Assert.That(loadedRow["TheDecimal"], Is.EqualTo(22.20m));
            Assert.That(loadedRow["TheNumeric"], Is.EqualTo(222.20m));

            Assert.That(loadedRow["TheSmallMoney"], Is.EqualTo(2222.2200m));
            Assert.That(loadedRow["TheMoney"], Is.EqualTo(22222.2200m));

            Assert.That(loadedRow["TheReal"], Is.EqualTo((float)222222.0));
            Assert.That(loadedRow["TheFloat"], Is.EqualTo(2222222.0));

            Assert.That(loadedRow["TheDate"], Is.EqualTo(DateTime.Parse("2002-02-02")));
            Assert.That(loadedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2003-03-03")));
            Assert.That(loadedRow["TheDateTime2"], Is.EqualTo(DateTime.Parse("2004-04-04")));
            Assert.That(loadedRow["TheDateTimeOffset"], Is.EqualTo(DateTimeOffset.Parse("2005-05-05")));
            Assert.That(loadedRow["TheSmallDateTime"], Is.EqualTo(DateTime.Parse("2006-06-06")));
            Assert.That(loadedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("02:02:02")));

            Assert.That(loadedRow["TheChar"], Is.EqualTo("b         "));
            Assert.That(loadedRow["TheVarChar"], Is.EqualTo("bb"));
            Assert.That(loadedRow["TheVarCharMax"], Is.EqualTo("bbb"));

            Assert.That(loadedRow["TheNChar"], Is.EqualTo("щ         "));
            Assert.That(loadedRow["TheNVarChar"], Is.EqualTo("щщ"));
            Assert.That(loadedRow["TheNVarCharMax"], Is.EqualTo("щщщ"));

            Assert.That(loadedRow["TheBinary"], Is.EqualTo(new byte[] { 2, 0, 0, 0 }));
            Assert.That(loadedRow["TheVarBinary"], Is.EqualTo(new byte[] { 20, 22, }));
            Assert.That(loadedRow["TheVarBinaryMax"], Is.EqualTo(new byte[] { 17, 177, 179 }));
        }

        [Test]
        public void UpdateRow_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.UpdateRow("some_table", new { Id = 1, Name = 2 }));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void UpdateRow_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.UpdateRow("bad_table", new { Id = 1, Name = 2 }));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void UpdateRow_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() =>
                cruder.UpdateRow(null, new { Id = 1, Name = 2 }, x => true));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void UpdateRow_RowUpdateIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() =>
                cruder.UpdateRow("SuperTable", null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("rowUpdate"));
        }

        [Test]
        public void UpdateRow_PropertySelectorIsNull_UsesAllProperties()
        {
            // Arrange
            this.CreateSuperTable();
            this.InsertSuperTableRow();

            var updateMold = new SuperTableRowDto
            {
                Id = 1,

                TheGuid = new Guid("22222222-2222-2222-2222-222222222222"),

                TheBit = false,

                TheTinyInt = 2,
                TheSmallInt = 22,
                TheInt = 222,
                TheBigInt = 2222,

                TheDecimal = 22.20m,
                TheNumeric = 222.20m,

                TheSmallMoney = 2222.2200m,
                TheMoney = 22222.2200m,

                TheReal = (float)222222.0,
                TheFloat = 2222222.0,

                TheDate = DateTime.Parse("2002-02-02"),
                TheDateTime = DateTime.Parse("2003-03-03"),
                TheDateTime2 = DateTime.Parse("2004-04-04"),
                TheDateTimeOffset = DateTime.Parse("2005-05-05"),
                TheSmallDateTime = DateTime.Parse("2006-06-06"),
                TheTime = TimeSpan.Parse("02:02:02"),

                TheChar = "b",
                TheVarChar = "bb",
                TheVarCharMax = "bbb",

                TheNChar = "щ",
                TheNVarChar = "щщ",
                TheNVarCharMax = "щщщ",

                TheBinary = new byte[] { 2 },
                TheVarBinary = new byte[] { 20, 22, },
                TheVarBinaryMax = new byte[] { 17, 177, 179 },

                NotExisting = 777,
            };

            var update = new DynamicRow(updateMold);
            update.RemoveProperty("NotExisting");

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            cruder.UpdateRow("SuperTable", update, null);

            // Assert
            var loadedRow = TestHelper.LoadRow(this.Connection, TestHelper.SchemaName, "SuperTable", 1);

            Assert.That(loadedRow["TheGuid"], Is.EqualTo(new Guid("22222222-2222-2222-2222-222222222222")));

            Assert.That(loadedRow["TheBit"], Is.EqualTo(false));

            Assert.That(loadedRow["TheTinyInt"], Is.EqualTo((byte)2));
            Assert.That(loadedRow["TheSmallInt"], Is.EqualTo((short)22));
            Assert.That(loadedRow["TheInt"], Is.EqualTo(222));
            Assert.That(loadedRow["TheBigInt"], Is.EqualTo(2222L));

            Assert.That(loadedRow["TheDecimal"], Is.EqualTo(22.20m));
            Assert.That(loadedRow["TheNumeric"], Is.EqualTo(222.20m));

            Assert.That(loadedRow["TheSmallMoney"], Is.EqualTo(2222.2200m));
            Assert.That(loadedRow["TheMoney"], Is.EqualTo(22222.2200m));

            Assert.That(loadedRow["TheReal"], Is.EqualTo((float)222222.0));
            Assert.That(loadedRow["TheFloat"], Is.EqualTo(2222222.0));

            Assert.That(loadedRow["TheDate"], Is.EqualTo(DateTime.Parse("2002-02-02")));
            Assert.That(loadedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2003-03-03")));
            Assert.That(loadedRow["TheDateTime2"], Is.EqualTo(DateTime.Parse("2004-04-04")));
            Assert.That(loadedRow["TheDateTimeOffset"], Is.EqualTo(DateTimeOffset.Parse("2005-05-05")));
            Assert.That(loadedRow["TheSmallDateTime"], Is.EqualTo(DateTime.Parse("2006-06-06")));
            Assert.That(loadedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("02:02:02")));

            Assert.That(loadedRow["TheChar"], Is.EqualTo("b         "));
            Assert.That(loadedRow["TheVarChar"], Is.EqualTo("bb"));
            Assert.That(loadedRow["TheVarCharMax"], Is.EqualTo("bbb"));

            Assert.That(loadedRow["TheNChar"], Is.EqualTo("щ         "));
            Assert.That(loadedRow["TheNVarChar"], Is.EqualTo("щщ"));
            Assert.That(loadedRow["TheNVarCharMax"], Is.EqualTo("щщщ"));

            Assert.That(loadedRow["TheBinary"], Is.EqualTo(new byte[] { 2, 0, 0, 0 }));
            Assert.That(loadedRow["TheVarBinary"], Is.EqualTo(new byte[] { 20, 22, }));
            Assert.That(loadedRow["TheVarBinaryMax"], Is.EqualTo(new byte[] { 17, 177, 179 }));
        }

        [Test]
        public void UpdateRow_PropertySelectorDoesNotContainPkColumn_ThrowsArgumentException()
        {
            // Arrange
            this.CreateSuperTable();
            this.InsertSuperTableRow();

            var update = new
            {
                TheGuid = new Guid("22222222-2222-2222-2222-222222222222"),
            };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.UpdateRow("SuperTable", update));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("'rowUpdate' does not contain primary key value."));
            Assert.That(ex.ParamName, Is.EqualTo("rowUpdate"));
        }

        [Test]
        public void UpdateRow_PropertySelectorContainsOnlyPkColumn_ThrowsArgumentException()
        {
            // Arrange
            this.CreateSuperTable();
            this.InsertSuperTableRow();

            var update = new
            {
                Id = 1,
            };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.UpdateRow("SuperTable", update));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("'rowUpdate' has no columns to update."));
            Assert.That(ex.ParamName, Is.EqualTo("rowUpdate"));
        }

        [Test]
        public void UpdateRow_IdIsNull_ThrowsArgumentException()
        {
            // Arrange
            this.CreateSuperTable();
            this.InsertSuperTableRow();

            var update = new
            {
                Id = (object)null,
                TheGuid = Guid.NewGuid(),
            };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.UpdateRow("SuperTable", update));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Primary key column value must not be null."));
            Assert.That(ex.ParamName, Is.EqualTo("rowUpdate"));
        }

        [Test]
        public void UpdateRow_NoColumnForSelectedProperty_ThrowsTauDbException()
        {
            // Arrange
            this.CreateSuperTable();
            this.InsertSuperTableRow();

            var update = new
            {
                Id = 1,
                NotExisting = 7,
            };

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.UpdateRow("SuperTable", update));

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Column 'NotExisting' not found in table 'SuperTable'."));
        }

        [Test]
        public void UpdateRow_TableHasNoPrimaryKey_ThrowsArgumentException()

        {
            // Arrange
            this.Connection.ExecuteSingleSql("CREATE TABLE [zeta].[dummy](Foo int)"); // no PK
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentException>((() => cruder.UpdateRow("dummy", new { Foo = 1 })));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Table 'dummy' does not have a primary key."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void UpdateRow_TablePrimaryKeyIsMultiColumn_ThrowsArgumentException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentException>((() => cruder.UpdateRow("Person", new { Key = 3 })));

            // Assert
            Assert.That(ex,
                Has.Message.StartsWith("Failed to retrieve single primary key column name for the table 'Person'."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        #endregion

        #region DeleteRow

        [Test]
        public void DeleteRow_ValidArguments_DeletesRowAndReturnsTrue()
        {
            // Arrange
            this.CreateMediumTable();
            const int id = 1;
            this.Connection.ExecuteSingleSql($"INSERT INTO [zeta].[MediumTable]([Id]) VALUES ({id})");

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var deleted = cruder.DeleteRow("MediumTable", id);

            // Assert
            var deletedRow = TestHelper.LoadRow(this.Connection, TestHelper.SchemaName, "MediumTable", id);

            Assert.That(deleted, Is.True);
            Assert.That(deletedRow, Is.Null);
        }

        [Test]
        public void DeleteRow_IdNotFound_ReturnsFalse()
        {
            // Arrange
            this.CreateMediumTable();
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);
            var notExistingId = 11;

            // Act
            var deleted = cruder.DeleteRow("MediumTable", notExistingId);

            // Assert
            Assert.That(deleted, Is.False);
        }

        [Test]
        public void DeleteRow_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.DeleteRow("some_table", 17));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void DeleteRow_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.DeleteRow("bad_table", 17));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void DeleteRow_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.DeleteRow(null, 11));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void DeleteRow_IdIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.DeleteRow("MediumTable", null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("id"));
        }

        [Test]
        public void DeleteRow_TableHasNoPrimaryKey_ThrowsArgumentException()
        {
            // Arrange
            this.Connection.ExecuteSingleSql("CREATE TABLE [zeta].[dummy](Foo int)"); // no PK
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act
            var ex = Assert.Throws<TauDbException>((() => cruder.DeleteRow("dummy", 1)));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Table 'dummy' does not have a primary key."));
        }

        [Test]
        public void DeleteRow_PrimaryKeyIsMultiColumn_ThrowsArgumentException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            // Act

            var ex = Assert.Throws<ArgumentException>((() => cruder.DeleteRow("Person", "the_id")));

            // Assert
            Assert.That(
                ex,
                Has.Message.StartsWith("Failed to retrieve single primary key column name for the table 'Person'."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        #endregion

        [Test]
        public void InsertRow_ProblemWithApplyingLongStringToCommand_ThrowsException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            var badConverterMock = new Mock<IDbValueConverter>();
            badConverterMock
                .Setup(x => x.ToDbValue(It.IsAny<object>()))
                .Returns(null);

            var badConverter = badConverterMock.Object;
            cruder.GetTableValuesConverter("DateData").SetColumnConverter("Moment", badConverter);

            var row1 = new DynamicRow(new
            {
                Moment = "Very, very long string; it should really be ellipsized. Test it in your good ut.",
            });

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("DateData", row1));

            // Assert
            Assert.That(ex, Has.Message.Contain("'Very, very long string;"));
            Assert.That(ex, Has.Message.Contain("...'")); // ellispized
        }


        [Test]
        [TestCase(DateTimeKind.Utc)]
        [TestCase((byte)17)]
        [TestCase((sbyte)17)]
        [TestCase((short)17)]
        [TestCase((ushort)17)]
        [TestCase((int)17)]
        [TestCase((uint)17)]
        [TestCase((long)17)]
        [TestCase((ulong)17)]
        [TestCase((double)17)]
        [TestCase((float)17)]
        [TestCase("decimal 17")]
        [TestCase("datetime 2020-01-01Z")]
        [TestCase("datetimeoffset 2020-01-01Z")]
        [TestCase("timespan 01:01:01")]
        [TestCase("guid f3a36c3a-20b8-4b54-87f2-251e35353600")]
        public void InsertRow_ProblemWithApplyingValue_ThrowsException(object val)
        {
            // Arrange
            if (val is string s)
            {
                var parts = s.Split(' ');
                switch (parts[0])
                {
                    case "decimal":
                        val = decimal.Parse(parts[1]);
                        break;

                    case "datetime":
                        val = DateTime.Parse(parts[1]);
                        break;

                    case "datetimeoffset":
                        val = DateTimeOffset.Parse(parts[1]);
                        break;

                    case "timespan":
                        val = TimeSpan.Parse(parts[1]);
                        break;

                    case "guid":
                        val = Guid.Parse(parts[1]);
                        break;

                    default:
                        throw new Exception("Unexpected test arg.");
                }
            }

            IDbCruder cruder = new SqlCruder(this.Connection, TestHelper.SchemaName);

            var badConverterMock = new Mock<IDbValueConverter>();
            badConverterMock
                .Setup(x => x.ToDbValue(It.IsAny<object>()))
                .Returns(null);

            var badConverter = badConverterMock.Object;
            cruder.GetTableValuesConverter("DateData").SetColumnConverter("Moment", badConverter);

            var row1 = new DynamicRow(new
            {
                Moment = val,
            });

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("DateData", row1));

            // Assert
            Assert.That(ex, Has.Message.Contain($"'{val}'."));

            Assert.Pass(ex.Message);
        }

    }
}