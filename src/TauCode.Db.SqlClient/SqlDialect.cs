namespace TauCode.Db.SqlClient;

public class SqlDialect : Dialect
{
    public override IUtilityFactory Factory => SqlUtilityFactory.Instance;
    public override string Name => "SQL Server";

    public override string Undelimit(string identifier)
    {
        // todo temp!

        return identifier.Replace("[", "").Replace("]", "");
    }
}
