using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TauCode.Db.SqlClient
{
    public class SqlInspector : DbInspectorBase
    {
        #region Constants

        public const string DefaultSchema = "dbo";

        private static readonly HashSet<string> SystemSchemata = new HashSet<string>(new string[]
        {
            "guest",
            "information_schema",
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
        });

        #endregion

        #region Constructor

        public SqlInspector(IDbConnection connection, string schema = null)
            : base(connection, schema ?? DefaultSchema)
        {
        }

        #endregion

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;

        protected override IReadOnlyList<string> GetTableNamesImpl(string schema)
        {
            using var command = this.Connection.CreateCommand();
            var sql =
                $@"
SELECT
    T.table_name TableName
FROM
    information_schema.tables T
WHERE
    T.table_type = 'BASE TABLE' AND
    T.table_schema = @p_schema
";

            command.AddParameterWithValue("p_schema", this.SchemaName);

            command.CommandText = sql;

            var tableNames = DbTools
                .GetCommandRows(command)
                .Select(x => (string)x.TableName)
                .ToArray();

            return tableNames;
        }

        protected override HashSet<string> GetSystemSchemata() => SystemSchemata;
    }
}
