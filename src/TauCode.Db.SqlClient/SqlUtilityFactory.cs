namespace TauCode.Db.SqlClient;

// todo regions

public class SqlUtilityFactory : IUtilityFactory
{
    public static SqlUtilityFactory Instance { get; } = new();

    private SqlUtilityFactory()
    {
    }

    public IDialect Dialect { get; } = new SqlDialect();

    public IScriptBuilder CreateScriptBuilder() => new SqlScriptBuilder();

    public IExplorer CreateExplorer() => new SqlExplorer();

    public ICruder CreateCruder() => new SqlCruder();
}