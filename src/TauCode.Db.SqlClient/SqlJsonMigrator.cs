using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace TauCode.Db.SqlClient
{
    public class SqlJsonMigrator : DbJsonMigratorBase
    {
        public SqlJsonMigrator(
            SqlConnection connection,
            string schemaName,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter,
            Func<string, bool> tableNamePredicate = null)
            : base(
                connection,
                schemaName ?? SqlTools.DefaultSchemaName,
                metadataJsonGetter,
                dataJsonGetter,
                tableNamePredicate)
        {
        }

        protected SqlConnection SqlConnection => (SqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) => new SqlSchemaExplorer(this.SqlConnection);
    }
}
