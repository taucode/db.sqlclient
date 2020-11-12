using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Extensions;

namespace TauCode.Db.SqlClient.Tests.DbTableInspector
{
    [TestFixture]
    public class SqlTableInspectorTests : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Connection.CreateSchema("zeta");

            var sql = this.GetType().Assembly.GetResourceText("crebase.sql", true);
            this.Connection.ExecuteCommentedScript(sql);
        }

        private void AssertColumn(
            ColumnMold actualColumnMold,
            string expectedColumnName,
            DbTypeMoldInfo expectedType,
            bool expectedIsNullable,
            ColumnIdentityMoldInfo expectedIdentity,
            string expectedDefault)
        {
            Assert.That(actualColumnMold.Name, Is.EqualTo(expectedColumnName));

            Assert.That(actualColumnMold.Type.Name, Is.EqualTo(expectedType.Name));
            Assert.That(actualColumnMold.Type.Size, Is.EqualTo(expectedType.Size));
            Assert.That(actualColumnMold.Type.Precision, Is.EqualTo(expectedType.Precision));
            Assert.That(actualColumnMold.Type.Scale, Is.EqualTo(expectedType.Scale));

            Assert.That(actualColumnMold.IsNullable, Is.EqualTo(expectedIsNullable));

            if (actualColumnMold.Identity == null)
            {
                Assert.That(expectedIdentity, Is.Null);
            }
            else
            {
                Assert.That(expectedIdentity, Is.Not.Null);
                Assert.That(actualColumnMold.Identity.Seed, Is.EqualTo(expectedIdentity.Seed));
                Assert.That(actualColumnMold.Identity.Increment, Is.EqualTo(expectedIdentity.Increment));
            }

            Assert.That(actualColumnMold.Default, Is.EqualTo(expectedDefault));
        }

        #region Constructor

        [Test]
        public void Constructor_ValidArguments_RunsOk()
        {
            // Arrange

            // Act
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "dbo", "tab1");

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(SqlUtilityFactory.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("dbo"));
            Assert.That(inspector.TableName, Is.EqualTo("tab1"));
        }

        [Test]
        public void Constructor_ConnectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new SqlTableInspector(null, "dbo", "tab1"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_ConnectionIsNotOpen_ThrowsArgumentException()
        {
            // Arrange
            using var connection = new SqlConnection(TestHelper.ConnectionString);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => new SqlTableInspector(connection, "dbo", "tab1"));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Connection should be opened."));
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_SchemaIsNull_RunsOkAndSchemaIsDbo()
        {
            // Arrange

            // Act
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, null, "tab1");

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(SqlUtilityFactory.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("dbo"));
            Assert.That(inspector.TableName, Is.EqualTo("tab1"));
        }

        [Test]
        public void Constructor_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new SqlTableInspector(this.Connection, "dbo", null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        #endregion

        #region GetColumns

        [Test]
        public void GetColumns_ValidInput_ReturnsColumns()
        {
            // Arrange
            var tableNames = new[]
            {
                "Person",
                "PersonData",
                "WorkInfo",
                "TaxInfo",
                "HealthInfo",
                "NumericData",
                "DateData",
            };


            // Act
            var dictionary = tableNames
                .Select(x => new SqlTableInspector(this.Connection, "zeta", x))
                .ToDictionary(x => x.TableName, x => x.GetColumns());

            // Assert

            IReadOnlyList<ColumnMold> columns;

            #region Person

            columns = dictionary["Person"];
            Assert.That(columns, Has.Count.EqualTo(8));

            this.AssertColumn(columns[0], "MetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[1], "OrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[2], "Id", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(columns[3], "FirstName", new DbTypeMoldInfo("nvarchar", size: 100), false, null, null);
            this.AssertColumn(columns[4], "LastName", new DbTypeMoldInfo("nvarchar", size: 100), false, null, null);
            this.AssertColumn(columns[5], "Birthday", new DbTypeMoldInfo("date"), false, null, null);
            this.AssertColumn(columns[6], "Gender", new DbTypeMoldInfo("bit"), true, null, null);
            this.AssertColumn(columns[7], "Initials", new DbTypeMoldInfo("nchar", size: 2), true, null, null);

            #endregion

            #region PersonData

            columns = dictionary["PersonData"];
            Assert.That(columns, Has.Count.EqualTo(8));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[1], "Height", new DbTypeMoldInfo("int"), true, null, null);
            this.AssertColumn(columns[2], "Photo", new DbTypeMoldInfo("varbinary", size: -1), true, null, null);
            this.AssertColumn(columns[3], "EnglishDescription", new DbTypeMoldInfo("varchar", size: -1), false, null,
                null);
            this.AssertColumn(columns[4], "UnicodeDescription", new DbTypeMoldInfo("nvarchar", size: -1), false, null,
                null);
            this.AssertColumn(columns[5], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[6], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[7], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);

            #endregion

            #region WorkInfo

            columns = dictionary["WorkInfo"];
            Assert.That(columns, Has.Count.EqualTo(11));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[1], "Position", new DbTypeMoldInfo("varchar", size: 20), false, null, null);
            this.AssertColumn(columns[2], "HireDate", new DbTypeMoldInfo("smalldatetime"), false, null, null);
            this.AssertColumn(columns[3], "Code", new DbTypeMoldInfo("char", size: 3), true, null, null);
            this.AssertColumn(columns[4], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[5], "DigitalSignature", new DbTypeMoldInfo("binary", size: 16), false, null,
                null);
            this.AssertColumn(columns[6], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(columns[7], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[8], "Hash", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[9], "Salary", new DbTypeMoldInfo("smallmoney"), true, null, null);
            this.AssertColumn(columns[10], "VaryingSignature", new DbTypeMoldInfo("varbinary", size: 100), true, null,
                null);

            #endregion

            #region TaxInfo

            columns = dictionary["TaxInfo"];
            Assert.That(columns, Has.Count.EqualTo(10));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[1], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(columns[2], "Tax", new DbTypeMoldInfo("money"), false, null, null);
            this.AssertColumn(columns[3], "Ratio", new DbTypeMoldInfo("float"), true, null, null);
            this.AssertColumn(columns[4], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[5], "SmallRatio", new DbTypeMoldInfo("real"), false, null, null);
            this.AssertColumn(columns[6], "RecordDate", new DbTypeMoldInfo("datetime"), true, null, null);
            this.AssertColumn(columns[7], "CreatedAt", new DbTypeMoldInfo("datetimeoffset"), false, null, null);
            this.AssertColumn(columns[8], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[9], "DueDate", new DbTypeMoldInfo("datetime2"), true, null, null);

            #endregion

            #region HealthInfo

            columns = dictionary["HealthInfo"];
            Assert.That(columns, Has.Count.EqualTo(9));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[1], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(
                columns[2],
                "Weight",
                new DbTypeMoldInfo("decimal", precision: 8, scale: 2),
                false,
                null,
                null);
            this.AssertColumn(columns[3], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[4], "IQ", new DbTypeMoldInfo("numeric", precision: 8, scale: 2), true, null, null);
            this.AssertColumn(columns[5], "Temper", new DbTypeMoldInfo("smallint"), true, null, null);
            this.AssertColumn(columns[6], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[7], "MetricB", new DbTypeMoldInfo("int"), true, null, null);
            this.AssertColumn(columns[8], "MetricA", new DbTypeMoldInfo("int"), true, null, null);

            #endregion

            #region NumericData

            columns = dictionary["NumericData"];
            Assert.That(columns, Has.Count.EqualTo(10));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("int"), false, new ColumnIdentityMoldInfo("15", "99"), null);
            this.AssertColumn(columns[1], "BooleanData", new DbTypeMoldInfo("bit"), true, null, null);
            this.AssertColumn(columns[2], "ByteData", new DbTypeMoldInfo("tinyint"), true, null, null);
            this.AssertColumn(columns[3], "Int16", new DbTypeMoldInfo("smallint"), true, null, null);
            this.AssertColumn(columns[4], "Int32", new DbTypeMoldInfo("int"), true, null, null);
            this.AssertColumn(columns[5], "Int64", new DbTypeMoldInfo("bigint"), true, null, null);
            this.AssertColumn(columns[6], "NetDouble", new DbTypeMoldInfo("float"), true, null, null);
            this.AssertColumn(columns[7], "NetSingle", new DbTypeMoldInfo("real"), true, null, null);
            this.AssertColumn(columns[8], "NumericData", new DbTypeMoldInfo("numeric", precision: 10, scale: 6), true, null, null);
            this.AssertColumn(columns[9], "DecimalData", new DbTypeMoldInfo("decimal", precision: 11, scale: 5), true, null, null);

            #endregion

            #region DateData

            columns = dictionary["DateData"];
            Assert.That(columns, Has.Count.EqualTo(2));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[1], "Moment", new DbTypeMoldInfo("datetimeoffset"), true, null, null);

            #endregion
        }

        [Test]
        public void GetColumns_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "bad_schema", "tab1");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetColumns());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetColumns_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "zeta", "bad_table");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetColumns());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion

        #region GetPrimaryKey

        [Test]
        public void GetPrimaryKey_ValidInput_ReturnsPrimaryKey()
        {
            // Arrange
            var tableNames = this.Connection.GetTableNames("zeta", true);

            // Act
            var dictionary = tableNames
                .Select(x => new SqlTableInspector(this.Connection, "zeta", x))
                .ToDictionary(x => x.TableName, x => x.GetPrimaryKey());

            // Assert

            PrimaryKeyMold pk;

            // Person
            pk = dictionary["Person"];
            Assert.That(pk.Name, Is.EqualTo("PK_person"));
            CollectionAssert.AreEqual(
                new[]
                {
                    "Id",
                    "MetaKey",
                    "OrdNumber",
                },
                pk.Columns);

            // PersonData
            pk = dictionary["PersonData"];
            Assert.That(pk.Name, Is.EqualTo("PK_personData"));
            CollectionAssert.AreEqual(
                new[]
                {
                    "Id",
                },
                pk.Columns);

            // NumericData
            pk = dictionary["NumericData"];
            Assert.That(pk.Name, Is.EqualTo("PK_numericData"));
            CollectionAssert.AreEqual(
                new[]
                {
                    "Id",
                },
                pk.Columns);

        }

        [Test]
        public void GetPrimaryKey_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "bad_schema", "tab1");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetPrimaryKey());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetPrimaryKey_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "zeta", "bad_table");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetPrimaryKey());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion

        #region GetForeignKeys

        [Test]
        public void GetForeignKeys_ValidInput_ReturnsForeignKeys()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "zeta", "PersonData");

            // Act
            var foreignKeys = inspector.GetForeignKeys();

            // Assert
            Assert.That(foreignKeys, Has.Count.EqualTo(1));
            var fk = foreignKeys.Single();

            Assert.That(fk.Name, Is.EqualTo("FK_personData_Person"));
            CollectionAssert.AreEqual(
                new string[] { "PersonId", "PersonMetaKey", "PersonOrdNumber" },
                fk.ColumnNames);
            Assert.That(fk.ReferencedTableName, Is.EqualTo("Person"));
            CollectionAssert.AreEqual(
                new string[] { "Id", "MetaKey", "OrdNumber" },
                fk.ReferencedColumnNames);
        }

        [Test]
        public void GetForeignKeys_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "bad_schema", "tab1");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetForeignKeys());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetForeignKeys_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "zeta", "bad_table");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetForeignKeys());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion

        #region GetIndexes

        [Test]
        public void GetIndexes_ValidInput_ReturnsIndexes()
        {
            // Arrange
            IDbTableInspector inspector1 = new SqlTableInspector(this.Connection, "zeta", "Person");
            IDbTableInspector inspector2 = new SqlTableInspector(this.Connection, "zeta", "WorkInfo");
            IDbTableInspector inspector3 = new SqlTableInspector(this.Connection, "zeta", "HealthInfo");

            // Act
            var indexes1 = inspector1.GetIndexes();
            var indexes2 = inspector2.GetIndexes();
            var indexes3 = inspector3.GetIndexes();

            // Assert

            // Person
            var index = indexes1.Single();
            Assert.That(index.Name, Is.EqualTo("PK_person"));
            Assert.That(index.TableName, Is.EqualTo("Person"));
            Assert.That(index.IsUnique, Is.True);
            Assert.That(index.Columns, Has.Count.EqualTo(3));

            var column = index.Columns[0];
            Assert.That(column.Name, Is.EqualTo("Id"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            column = index.Columns[1];
            Assert.That(column.Name, Is.EqualTo("MetaKey"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            column = index.Columns[2];
            Assert.That(column.Name, Is.EqualTo("OrdNumber"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            // WorkInfo
            Assert.That(indexes2, Has.Count.EqualTo(2));

            // index: PK_workInfo
            index = indexes2[0];
            Assert.That(index.Name, Is.EqualTo("PK_workInfo"));
            Assert.That(index.TableName, Is.EqualTo("WorkInfo"));
            Assert.That(index.IsUnique, Is.True);
            Assert.That(index.Columns, Has.Count.EqualTo(1));

            column = index.Columns.Single();
            Assert.That(column.Name, Is.EqualTo("Id"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            // index: UX_workInfo_Hash
            index = indexes2[1];
            Assert.That(index.Name, Is.EqualTo("UX_workInfo_Hash"));
            Assert.That(index.TableName, Is.EqualTo("WorkInfo"));
            Assert.That(index.IsUnique, Is.True);
            Assert.That(index.Columns, Has.Count.EqualTo(1));

            column = index.Columns.Single();
            Assert.That(column.Name, Is.EqualTo("Hash"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            // HealthInfo
            Assert.That(indexes3, Has.Count.EqualTo(2));

            // index: IX_healthInfo_metricAmetricB
            index = indexes3[0];
            Assert.That(index.Name, Is.EqualTo("IX_healthInfo_metricAmetricB"));
            Assert.That(index.TableName, Is.EqualTo("HealthInfo"));
            Assert.That(index.IsUnique, Is.False);
            Assert.That(index.Columns, Has.Count.EqualTo(2));

            column = index.Columns[0];
            Assert.That(column.Name, Is.EqualTo("MetricA"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            column = index.Columns[1];
            Assert.That(column.Name, Is.EqualTo("MetricB"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Descending));

            // index: PK_healthInfo
            index = indexes3[1];
            Assert.That(index.Name, Is.EqualTo("PK_healthInfo"));
            Assert.That(index.TableName, Is.EqualTo("HealthInfo"));
            Assert.That(index.IsUnique, Is.True);
            Assert.That(index.Columns, Has.Count.EqualTo(1));

            column = index.Columns.Single();
            Assert.That(column.Name, Is.EqualTo("Id"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));
        }

        [Test]
        public void GetIndexes_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "bad_schema", "tab1");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetForeignKeys());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetIndexes_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "zeta", "bad_table");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetForeignKeys());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion

        #region GetTable

        [Test]
        public void GetTable_ValidInput_ReturnsTable()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "zeta", "HealthInfo");

            // Act
            var table = inspector.GetTable();

            // Assert

            Assert.That(table.Name, Is.EqualTo("HealthInfo"));

            #region primary keys

            var primaryKey = table.PrimaryKey;

            Assert.That(primaryKey.Name, Is.EqualTo("PK_healthInfo"));

            Assert.That(primaryKey.Columns, Has.Count.EqualTo(1));

            var column = primaryKey.Columns.Single();
            Assert.That(column, Is.EqualTo("Id"));

            #endregion

            #region columns

            var columns = table.Columns;
            Assert.That(columns, Has.Count.EqualTo(9));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[1], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(
                columns[2],
                "Weight",
                new DbTypeMoldInfo("decimal", precision: 8, scale: 2),
                false,
                null,
                null);
            this.AssertColumn(columns[3], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[4], "IQ", new DbTypeMoldInfo("numeric", precision: 8, scale: 2), true, null, null);
            this.AssertColumn(columns[5], "Temper", new DbTypeMoldInfo("smallint"), true, null, null);
            this.AssertColumn(columns[6], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[7], "MetricB", new DbTypeMoldInfo("int"), true, null, null);
            this.AssertColumn(columns[8], "MetricA", new DbTypeMoldInfo("int"), true, null, null);

            #endregion

            #region foreign keys

            var foreignKeys = table.ForeignKeys;

            Assert.That(foreignKeys, Has.Count.EqualTo(1));
            var fk = foreignKeys.Single();

            Assert.That(fk.Name, Is.EqualTo("FK_healthInfo_Person"));
            CollectionAssert.AreEqual(
                new string[]
                {
                    "PersonId",
                    "PersonMetaKey",
                    "PersonOrdNumber",
                },
                fk.ColumnNames);

            Assert.That(fk.ReferencedTableName, Is.EqualTo("Person"));
            CollectionAssert.AreEqual(
                new string[]
                {
                    "Id",
                    "MetaKey",
                    "OrdNumber",
                },
                fk.ReferencedColumnNames);

            #endregion

            #region indexes

            var indexes = table.Indexes;

            Assert.That(indexes, Has.Count.EqualTo(2));

            // index: IX_healthInfo_metricAmetricB
            var index = indexes[0];
            Assert.That(index.Name, Is.EqualTo("IX_healthInfo_metricAmetricB"));
            Assert.That(index.TableName, Is.EqualTo("HealthInfo"));
            Assert.That(index.IsUnique, Is.False);
            Assert.That(index.Columns, Has.Count.EqualTo(2));

            var indexColumn = index.Columns[0];
            Assert.That(indexColumn.Name, Is.EqualTo("MetricA"));
            Assert.That(indexColumn.SortDirection, Is.EqualTo(SortDirection.Ascending));

            indexColumn = index.Columns[1];
            Assert.That(indexColumn.Name, Is.EqualTo("MetricB"));
            Assert.That(indexColumn.SortDirection, Is.EqualTo(SortDirection.Descending));

            // index: PK_healthInfo
            index = indexes[1];
            Assert.That(index.Name, Is.EqualTo("PK_healthInfo"));
            Assert.That(index.TableName, Is.EqualTo("HealthInfo"));
            Assert.That(index.IsUnique, Is.True);
            Assert.That(index.Columns, Has.Count.EqualTo(1));

            indexColumn = index.Columns.Single();
            Assert.That(indexColumn.Name, Is.EqualTo("Id"));
            Assert.That(indexColumn.SortDirection, Is.EqualTo(SortDirection.Ascending));

            #endregion
        }

        [Test]
        public void GetTable_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "bad_schema", "tab1");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetTable());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetTable_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspector(this.Connection, "zeta", "bad_table");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetTable());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion
    }
}
