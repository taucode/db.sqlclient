using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Extensions;

namespace TauCode.Db.SqlClient
{
    public class SqlSchemaExplorer : DbSchemaExplorerBase
    {
        public SqlSchemaExplorer(SqlConnection connection)
            : base(connection, "[]")
        {
        }

        protected int GetTableObjectId(string schemaName, string tableName)
        {
            using var command = this.Connection.CreateCommand();
            command.CommandText =
                @"
SELECT
    T.object_id
FROM
    sys.tables T
INNER JOIN
    sys.schemas S
ON
    T.schema_id = S.schema_id
WHERE
    T.name = @p_tableName AND
    S.name = @p_schemaName
";
            command.AddParameterWithValue("p_tableName", tableName);
            command.AddParameterWithValue("p_schemaName", schemaName);

            var objectResult = command.ExecuteScalar();

            if (objectResult == null)
            {
                // should not happen, we are checking table existence.
                throw DbTools.CreateTableDoesNotExistException(schemaName, tableName);
            }

            var objectId = (int)objectResult;
            return objectId;
        }

        protected override ColumnMold ColumnInfoToColumn(ColumnInfo columnInfo)
        {
            var column = new ColumnMold
            {
                Name = columnInfo.Name,
                Type = new DbTypeMold
                {
                    Name = columnInfo.TypeName,
                    Size = columnInfo.Size,
                    Precision = columnInfo.Precision,
                    Scale = columnInfo.Scale,
                },
                IsNullable = columnInfo.IsNullable,
            };

            if (column.Type.Name.ToLowerInvariant().IsIn("text", "ntext"))
            {
                column.Type.Size = null;
            }
            else if (column.Type.Name.ToLowerInvariant().IsIn(
                "tinyint",
                "smallint",
                "int",
                "bigint",
                "smallmoney",
                "money",
                "float",
                "real"))
            {
                column.Type.Precision = null;
                column.Type.Scale = null;
            }

            return column;
        }

        protected override void ResolveIdentities(string schemaName, string tableName, IList<ColumnInfo> columnInfos)
        {
            var objectId = this.GetTableObjectId(schemaName, tableName);

            using var command = this.Connection.CreateCommand();
            command.CommandText =
                @"
SELECT
    IC.name             Name,
    IC.seed_value       SeedValue,
    IC.increment_value  IncrementValue
FROM
    sys.identity_columns IC
WHERE
    IC.object_id = @p_objectId
";
            command.AddParameterWithValue("p_objectId", objectId);

            var dictionary = command
                .GetCommandRows()
                .ToDictionary(
                    x => (string)x.Name,
                    x => new ColumnIdentityMold
                    {
                        Seed = ((object)x.SeedValue).ToString(),
                        Increment = ((object)x.IncrementValue).ToString(),
                    });

            foreach (var identityColumnName in dictionary.Keys)
            {
                var columnInfo = columnInfos.SingleOrDefault(x => x.Name == identityColumnName);
                if (columnInfo == null)
                {
                    // should not happen.
                    throw DbTools.CreateInternalErrorException();
                }

                var identity = dictionary[identityColumnName];

                columnInfo.Additional["#identity_seed"] = identity.Seed;
                columnInfo.Additional["#identity_increment"] = identity.Increment;
            }
        }

        public override IReadOnlyList<string> GetSystemSchemaNames() => new[]
        {
            "guest",
            "INFORMATION_SCHEMA",
            "sys",
            "db_owner",
            "db_accessadmin",
            "db_securityadmin",
            "db_ddladmin",
            "db_backupoperator",
            "db_datareader",
            "db_datawriter",
            "db_denydatareader",
            "db_denydatawriter",
        };

        public override string DefaultSchemaName => "dbo";

        protected override IReadOnlyList<IndexMold> GetTableIndexesImpl(string schemaName, string tableName)
        {
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    I.[index_id]            IndexId,
    I.[name]                IndexName,
    I.[is_unique]           IndexIsUnique,
    IC.[key_ordinal]        KeyOrdinal,
    C.[name]                ColumnName,
    IC.[is_descending_key]  IsDescendingKey
FROM
    sys.indexes I
INNER JOIN
    sys.index_columns IC
ON
    IC.[index_id] = I.[index_id]
    AND
    IC.[object_id] = I.[object_id]
INNER JOIN
    sys.columns C
ON
    C.[column_id] = IC.[column_id]
    AND
    C.[object_id] = IC.[object_id]
INNER JOIN
    sys.tables T
ON
    T.[object_id] = C.[object_id]
INNER JOIN
    sys.schemas S
ON
    T.[schema_id] = S.[schema_id]
WHERE
    T.[name] = @p_tableName and
    S.[name] = @p_schemaName
ORDER BY
    I.[name],
    IC.[key_ordinal]
";

            command.AddParameterWithValue("p_tableName", tableName);
            command.AddParameterWithValue("p_schemaName", schemaName);

            return command
                .GetCommandRows()
                .GroupBy(x => (int)x.IndexId)
                .Select(g => new IndexMold
                {
                    Name = (string)g.First().IndexName,
                    TableName = tableName,
                    Columns = g
                        .OrderBy(x => (int)x.KeyOrdinal)
                        .Select(x => new IndexColumnMold
                        {
                            Name = (string)x.ColumnName,
                            SortDirection = (bool)x.IsDescendingKey
                                ? SortDirection.Descending
                                : SortDirection.Ascending,
                        })
                        .ToList(),
                    IsUnique = (bool)g.First().IndexIsUnique,
                })
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}
