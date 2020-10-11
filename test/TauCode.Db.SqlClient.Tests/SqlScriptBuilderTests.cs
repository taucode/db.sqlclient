using NUnit.Framework;

namespace TauCode.Db.SqlClient.Tests
{
    [TestFixture]
    public class SqlScriptBuilderTests : TestBase
    {
        private IDbScriptBuilder _scriptBuilder;

        [SetUp]
        public void SetUp()
        {
            _scriptBuilder = this.DbInspector.Factory.CreateScriptBuilder(null);
        }

        [Test]
        public void BuildCreateTableScript_ValidArgument_CreatesScript()
        {
            // Arrange
            var table = this.DbInspector
                .Factory
                .CreateTableInspector(this.Connection, null, "fragment")
                .GetTable();
            
            // Act
            var sql = _scriptBuilder.BuildCreateTableScript(table, true);

            // Assert
            var expectedSql = @"CREATE TABLE [dbo].[fragment](
    [id] [uniqueidentifier] NOT NULL,
    [note_translation_id] [uniqueidentifier] NOT NULL,
    [sub_type_id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NULL,
    [order] [int] NOT NULL,
    [content] [ntext] NOT NULL,
    CONSTRAINT [PK_fragment] PRIMARY KEY([id] ASC),
    CONSTRAINT [FK_fragment_noteTranslation] FOREIGN KEY([note_translation_id]) REFERENCES [note_translation]([id]),
    CONSTRAINT [FK_fragment_subType] FOREIGN KEY([sub_type_id]) REFERENCES [fragment_sub_type]([id]))";

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        protected override void ExecuteDbCreationScript()
        {
            var script = TestHelper.GetResourceText("rho.script-create-tables.sql");
            this.Connection.ExecuteCommentedScript(script);

        }
    }
}
