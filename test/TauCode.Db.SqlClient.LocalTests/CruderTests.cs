using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace TauCode.Db.SqlClient.LocalTests;

[TestFixture]
public class CruderTests : TestBase
{
    private ICruder _cruder;
    private SqlExplorer _explorer;

    [SetUp]
    public void SetUp()
    {
        _cruder = new SqlCruder(this.SqlConnection);
        _explorer = new SqlExplorer(this.SqlConnection);

        this.DropTestSchemas();
    }

    [Test]
    public void GetAllRows_ValidArguments_ReturnsRows()
    {
        // Arrange

        // Act
        var rows = _cruder.GetAllRows("zeta", "tab1", x => x != "birth_date");

        // Assert
        throw new NotImplementedException();
    }

    private void DropTestSchemas()
    {
        try
        {
            _explorer.DropSchema("zeta", true);
        }
        catch (SqlException)
        {
            // dismiss
        }
    }
}