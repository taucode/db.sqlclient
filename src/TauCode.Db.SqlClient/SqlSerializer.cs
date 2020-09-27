using System.Data;

namespace TauCode.Db.SqlClient
{
    public class SqlSerializer : DbSerializerBase
    {
        public SqlSerializer(IDbConnection connection, string schema)
            : base(connection, schema)

        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;
    }
}
