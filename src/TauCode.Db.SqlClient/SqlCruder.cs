using Microsoft.Data.SqlClient;
using System;
using System.Data;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Model;

namespace TauCode.Db.SqlClient
{
    public class SqlCruder : DbCruderBase
    {
        public SqlCruder(IDbConnection connection, string schema)
            : base(connection, schema)
        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;

        protected override IDbValueConverter CreateDbValueConverter(ColumnMold column)
        {
            var typeName = column.Type.Name.ToLowerInvariant();
            switch (typeName)
            {
                case "uniqueidentifier":
                    return new GuidValueConverter();

                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                    return new StringValueConverter();

                case "int":
                case "integer":
                    return new Int32ValueConverter();

                case "datetime":
                case "datetime2":
                case "date":
                    return new DateTimeValueConverter();

                case "bit":
                    return new BooleanValueConverter();

                case "binary":
                case "varbinary":
                    return new ByteArrayValueConverter();

                case "float":
                    return new DoubleValueConverter();

                case "real":
                    return new SingleValueConverter();

                case "money":
                case "decimal":
                case "numeric":
                    return new DecimalValueConverter();

                case "tinyint":
                    return new ByteValueConverter();

                case "smallint":
                    return new Int16ValueConverter();

                case "bigint":
                    return new Int64ValueConverter();

                default:
                    throw new NotImplementedException();
            }
        }

        protected override IDbDataParameter CreateParameter(string tableName, ColumnMold column, string parameterName)
        {
            SqlParameter parameter;

            switch (column.Type.Name)
            {
                case "uniqueidentifier":
                    return new SqlParameter(parameterName, SqlDbType.UniqueIdentifier);

                case "int":
                    return new SqlParameter(parameterName, SqlDbType.Int);

                case "tinyint":
                    return new SqlParameter(parameterName, SqlDbType.TinyInt);

                case "smallint":
                    return new SqlParameter(parameterName, SqlDbType.SmallInt);

                case "bigint":
                    return new SqlParameter(parameterName, SqlDbType.BigInt);

                case "float":
                    return new SqlParameter(parameterName, SqlDbType.Float);

                case "real":
                    return new SqlParameter(parameterName, SqlDbType.Real);

                case "money":
                    return new SqlParameter(parameterName, SqlDbType.Money);

                case "decimal":
                case "numeric":
                    parameter = new SqlParameter(parameterName, SqlDbType.Decimal);
                    parameter.Scale = (byte)(column.Type.Scale ?? 0);
                    parameter.Precision = (byte)(column.Type.Precision ?? 0);
                    return parameter;

                case "nchar":
                    return new SqlParameter(parameterName, SqlDbType.NChar, column.Type.Size ?? 0);

                case "nvarchar":
                    return new SqlParameter(parameterName, SqlDbType.NVarChar, column.Type.Size ?? 0);

                case "char":
                    return new SqlParameter(parameterName, SqlDbType.Char, column.Type.Size ?? 0);

                case "varchar":
                    return new SqlParameter(parameterName, SqlDbType.VarChar, column.Type.Size ?? 0);

                case "datetime":
                    return new SqlParameter(parameterName, SqlDbType.DateTime);

                case "bit":
                    return new SqlParameter(parameterName, SqlDbType.Bit);

                case "varbinary":
                    return new SqlParameter(parameterName, SqlDbType.VarBinary, column.Type.Size ?? -1);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
