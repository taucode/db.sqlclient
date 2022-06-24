using Microsoft.Data.SqlClient;

namespace TauCode.Db.SqlClient
{
    public static class SqlTools
    {
        public const string DefaultSchemaName = "dbo";

        public static string BuildSetIdentityInsertSql(
            string schemaName,
            string tableName,
            bool flag)
        {
            if (schemaName == null)
            {
                schemaName = SqlTools.DefaultSchemaName;
            }

            var sqlFlag = flag ? "ON" : "OFF";

            var sql = $"SET IDENTITY_INSERT [{schemaName}].[{tableName}] {sqlFlag}";
            return sql;
        }

        public static void SetIdentityInsert(
            this SqlConnection connection,
            string schemaName,
            string tableName,
            bool flag)
        {
            // justified_todo checks
            using var command = connection.CreateCommand();
            var sql = BuildSetIdentityInsertSql(schemaName, tableName, flag);

            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
    }
}
