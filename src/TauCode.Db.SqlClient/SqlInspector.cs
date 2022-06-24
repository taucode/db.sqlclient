using Microsoft.Data.SqlClient;

namespace TauCode.Db.SqlClient
{
    public class SqlInspector : DbInspectorBase
    {
        public SqlInspector(SqlConnection connection, string schemaName)
            : base(connection, schemaName ?? SqlTools.DefaultSchemaName)
        {
        }

        protected SqlConnection SqlConnection => (SqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;
    }
}
