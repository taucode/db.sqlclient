using Microsoft.Data.SqlClient;

namespace TauCode.Db.SqlClient;

public class SqlInstructionProcessor : InstructionProcessor
{
    #region ctor

    public SqlInstructionProcessor()
    {

    }

    public SqlInstructionProcessor(SqlConnection connection)
        : base(connection)
    {

    }

    #endregion

    #region Overridden

    public override IUtilityFactory Factory => SqlUtilityFactory.Instance;

    #endregion
}