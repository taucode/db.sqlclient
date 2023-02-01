namespace TauCode.Db.SqlClient.LocalTests.DbCruder.Dto
{
    public class SuperTableRowDto
    {
        public int Id { get; set; }

        public Guid? TheGuid { get; set; }

        public bool? TheBit { get; set; }

        public byte? TheTinyInt { get; set; }
        public short? TheSmallInt { get; set; }
        public int? TheInt { get; set; }
        public long? TheBigInt { get; set; }
        
        public decimal? TheDecimal { get; set; }
        public decimal? TheNumeric { get; set; }

        public decimal? TheSmallMoney { get; set; }
        public decimal? TheMoney { get; set; }

        public float? TheReal { get; set; }
        public double? TheFloat { get; set; }

        public DateTime? TheDate { get; set; }
        public DateTime? TheDateTime { get; set; }
        public DateTime? TheDateTime2 { get; set; }
        public DateTimeOffset? TheDateTimeOffset { get; set; }
        public DateTime? TheSmallDateTime { get; set; }
        public TimeSpan? TheTime { get; set; }

        public string TheChar { get; set; }
        public string TheVarChar { get; set; }
        public string TheVarCharMax { get; set; }

        public string TheNChar { get; set; }
        public string TheNVarChar { get; set; }
        public string TheNVarCharMax { get; set; }

        public byte[] TheBinary { get; set; }
        public byte[] TheVarBinary { get; set; }
        public byte[] TheVarBinaryMax { get; set; }
        
        public int? NotExisting { get; set; }
    }
}
