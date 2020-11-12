using System.Data;
using Microsoft.Data.SqlClient;

namespace TauCode.Db.SqlClient
{
    public class SqlUtilityFactory : IDbUtilityFactory
    {
        public static SqlUtilityFactory Instance { get; } = new SqlUtilityFactory();

        private SqlUtilityFactory()
        {
        }

        public IDbDialect GetDialect() => SqlDialect.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schema) => new SqlScriptBuilder(schema);

        public IDbConnection CreateConnection() => new SqlConnection();

        public IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection)
        {
            return new SqlSchemaExplorer((SqlConnection)connection);
        }

        public IDbInspector CreateInspector(IDbConnection connection, string schema) =>
            new SqlInspector((SqlConnection)connection, schema);

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schema, string tableName) =>
            new SqlTableInspector((SqlConnection)connection, schema, tableName);

        public IDbCruder CreateCruder(IDbConnection connection, string schema) =>
            new SqlCruder((SqlConnection)connection, schema);

        public IDbSerializer CreateSerializer(IDbConnection connection, string schema) =>
            new SqlSerializer((SqlConnection)connection, schema);
    }
}
