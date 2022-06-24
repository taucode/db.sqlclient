using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace TauCode.Db.SqlClient.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected SqlConnection Connection { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            this.Connection = TestHelper.CreateConnection();
        }

        [OneTimeTearDown]
        public void OneTimeTearDownBase()
        {
            this.Connection.Dispose();
            this.Connection = null;
        }

        [SetUp]
        public void SetUpBase()
        {
            this.Connection.DropAllSchemas();
        }
    }
}
