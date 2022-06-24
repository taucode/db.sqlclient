using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Model;

namespace TauCode.Db.SqlClient
{
    [DbDialect(
        typeof(SqlDialect),
        "reserved-words.txt",
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

        public override IList<IndexMold> GetCreatableIndexes(TableMold tableMold)
        {
            var pk = tableMold.PrimaryKey;

            return base.GetCreatableIndexes(tableMold)
                .Where(x => x.Name != pk?.Name)
                .ToList();
        }

        #endregion
    }
}
