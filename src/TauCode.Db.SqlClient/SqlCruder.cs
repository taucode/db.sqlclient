using Microsoft.Data.SqlClient;
using System.Data;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;

namespace TauCode.Db.SqlClient
{
    public class SqlCruder : DbCruderBase
    {
        private const int TimeTypeColumnSize = 4;
        private const int DateTime2TypeColumnSize = 8;
        private const int DateTimeOffsetTypeColumnSize = 10;

        public SqlCruder(SqlConnection connection, string schemaName)
            : base(connection, schemaName ?? SqlTools.DefaultSchemaName)
        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;

        protected override IDbValueConverter CreateDbValueConverter(string tableName, ColumnMold column)
        {
            switch (column.Type.Name)
            {
                case "uniqueidentifier":
                    return new GuidValueConverter();

                case "bit":
                    return new BooleanValueConverter();

                case "tinyint":
                    return new ByteValueConverter();

                case "smallint":
                    return new Int16ValueConverter();

                case "int":
                    return new Int32ValueConverter();

                case "bigint":
                    return new Int64ValueConverter();

                case "decimal":
                case "numeric":

                case "money":
                case "smallmoney":
                    return new DecimalValueConverter();

                case "real":
                    return new SingleValueConverter();

                case "float":
                    return new DoubleValueConverter();

                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return new DateTimeValueConverter();

                case "datetimeoffset":
                    return new DateTimeOffsetValueConverter();

                case "time":
                    return new TimeSpanValueConverter();

                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                    return new StringValueConverter();

                case "binary":
                case "varbinary":
                    return new ByteArrayValueConverter();

                default:
                    throw this.CreateColumnTypeNotSupportedException(tableName, column.Name, column.Type.Name);
            }
        }

        protected override IDbDataParameter CreateParameter(string tableName, ColumnMold column)
        {
            const string parameterName = "parameter_name_placeholder";

            switch (column.Type.Name)
            {
                case "uniqueidentifier":
                    return new SqlParameter(parameterName, SqlDbType.UniqueIdentifier);

                case "bit":
                    return new SqlParameter(parameterName, SqlDbType.Bit);

                case "tinyint":
                    return new SqlParameter(parameterName, SqlDbType.TinyInt);

                case "smallint":
                    return new SqlParameter(parameterName, SqlDbType.SmallInt);

                case "int":
                    return new SqlParameter(parameterName, SqlDbType.Int);

                case "bigint":
                    return new SqlParameter(parameterName, SqlDbType.BigInt);

                case "decimal":
                case "numeric":
                    var parameter = new SqlParameter(parameterName, SqlDbType.Decimal);
                    parameter.Scale = (byte)(column.Type.Scale ?? 0);
                    parameter.Precision = (byte)(column.Type.Precision ?? 0);
                    return parameter;

                case "smallmoney":
                    return new SqlParameter(parameterName, SqlDbType.SmallMoney);

                case "money":
                    return new SqlParameter(parameterName, SqlDbType.Money);

                case "real":
                    return new SqlParameter(parameterName, SqlDbType.Real);

                case "float":
                    return new SqlParameter(parameterName, SqlDbType.Float);

                case "date":
                    return new SqlParameter(parameterName, SqlDbType.Date);

                case "datetime":
                    return new SqlParameter(parameterName, SqlDbType.DateTime);

                case "datetime2":
                    return new SqlParameter(parameterName, SqlDbType.DateTime2, DateTime2TypeColumnSize);

                case "datetimeoffset":
                    return new SqlParameter(parameterName, SqlDbType.DateTimeOffset, DateTimeOffsetTypeColumnSize);

                case "smalldatetime":
                    return new SqlParameter(parameterName, SqlDbType.SmallDateTime);

                case "time":
                    return new SqlParameter(parameterName, SqlDbType.Time, TimeTypeColumnSize);

                case "char":
                    return new SqlParameter(parameterName, SqlDbType.Char, column.Type.Size ?? throw this.CreateColumnSizeMustBeProvidedException(tableName, column));

                case "varchar":
                    return new SqlParameter(parameterName, SqlDbType.VarChar, column.Type.Size ?? throw this.CreateColumnSizeMustBeProvidedException(tableName, column));

                case "nchar":
                    return new SqlParameter(parameterName, SqlDbType.NChar, column.Type.Size ?? throw this.CreateColumnSizeMustBeProvidedException(tableName, column));

                case "nvarchar":
                    return new SqlParameter(parameterName, SqlDbType.NVarChar, column.Type.Size ?? throw this.CreateColumnSizeMustBeProvidedException(tableName, column));

                case "binary":
                    return new SqlParameter(parameterName, SqlDbType.Binary, column.Type.Size ?? throw this.CreateColumnSizeMustBeProvidedException(tableName, column));

                case "varbinary":
                    return new SqlParameter(parameterName, SqlDbType.VarBinary, column.Type.Size ?? throw this.CreateColumnSizeMustBeProvidedException(tableName, column));

                default:
                    throw new TauDbException($"Type '{column.Type.Name}' not supported. Table: '{tableName}', column: '{column.Name}'.");
            }
        }

        private TauDbException CreateColumnSizeMustBeProvidedException(string tableName, ColumnMold columnMold)
        {
            var msg = $"Column '{columnMold.Name}' (type '{columnMold.Type.Name}') of table '{tableName}' must have size.";
            return new TauDbException(msg);
        }

        protected override void FitParameterValue(IDbDataParameter parameter)
        {
            var value = parameter.Value;

            int? length = null;

            if (value is string s)
            {
                length = s.Length;
            }
            else if (value is byte[] arr)
            {
                length = arr.Length;
            }

            if (length.HasValue)
            {
                parameter.Size = length.Value;
            }
        }
    }
}
