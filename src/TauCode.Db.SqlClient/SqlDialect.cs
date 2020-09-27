namespace TauCode.Db.SqlClient
{
    [DbDialect(
        typeof(SqlDialect),
        "reserved-words.txt",
        //"data-type-names.txt",
        "[],\"\"")]
    public class SqlDialect : DbDialectBase
    {
        #region Static

        public static readonly SqlDialect Instance = new SqlDialect();

        #endregion

        #region Constructor

        private SqlDialect()
            : base(DbProviderNames.SQLServer)
        {
        }

        #endregion

        #region Overridden

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;
        
        public override string UnicodeTextLiteralPrefix => "N";

        #endregion
    }
}
