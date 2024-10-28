namespace TauCode.Db.SqlClient;

public class SqlScriptBuilder : ScriptBuilder
{
    #region Overridden

    public override IUtilityFactory Factory => SqlUtilityFactory.Instance;

    #endregion
}