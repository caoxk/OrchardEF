using System;
using System.Data;
using System.Linq;
using Autofac;
using NUnit.Framework;
using Orchard.Data;
using Orchard.Data.Migration.Schema;
using Orchard.Data.Providers;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.FileSystems.AppData;
using Orchard.Tests.ContentManagement;
using System.IO;
using Orchard.Tests.Environment;
using Orchard.Tests.FileSystems.AppData;
using Orchard.Data.Providers.SqlCeProvider;
using System.Data.Entity;

namespace Orchard.Tests.DataMigration
{
	[TestFixture]
    public class SchemaBuilderTestsBase {
        private IContainer _container;
        private ISessionFactoryHolder _sessionFactory;
        private string _databaseFileName;
        private string _tempFolder;
        private SchemaBuilder _schemaBuilder;
        private DbContext _session;

        [SetUp]
        public void Setup() {
            _databaseFileName = Path.GetTempFileName();
            _sessionFactory = DataUtility.CreateSessionFactory(_databaseFileName);

            _tempFolder = Path.GetTempFileName();
            File.Delete(_tempFolder);
            var appDataFolder = AppDataFolderTests.CreateAppDataFolder(_tempFolder);

            var builder = new ContainerBuilder();

            _session = _sessionFactory.Create();
            builder.RegisterInstance(appDataFolder).As<IAppDataFolder>();
            builder.RegisterType<SqlServerCompactDataServicesProvider>().As<IDataServicesProvider>();
            builder.RegisterType<DataServicesProviderFactory>().As<IDataServicesProviderFactory>();
            builder.RegisterType<SessionFactoryHolder>().As<ISessionFactoryHolder>();
            builder.RegisterType<StubHostEnvironment>().As<IHostEnvironment>();
            builder.RegisterInstance(new TestTransactionManager(_session)).As<ITransactionManager>();
            builder.RegisterInstance(new ShellBlueprint { Records = Enumerable.Empty<RecordBlueprint>() }).As<ShellBlueprint>();
            builder.RegisterInstance(new ShellSettings { Name = "temp", DataProvider = "SqlCe", DataTablePrefix = "TEST" }).As<ShellSettings>();
            builder.RegisterModule(new DataModule());
            _container = builder.Build();

            _schemaBuilder = new SchemaBuilder();
        }

        [Test]
        public void AllMethodsShouldBeCalledSuccessfully() {

            _schemaBuilder
                .Create.Table("User")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Firstname").AsString(255)
                .WithColumn("Lastname").AsDecimal(12, 1);

            _schemaBuilder
               .Create.Table("Address")
               .WithColumn("Id").AsInt32().PrimaryKey().Identity()
               .WithColumn("City").AsString(255)
               .WithColumn("ZIP").AsInt32().Unique()
               .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("User_Address", "User");

            _schemaBuilder.Alter.Table("User").AddColumn("Age").AsInt32();
            _schemaBuilder.Delete.Column("Lastname").FromTable("User");
            _schemaBuilder.Create.Index("IDX_XYZ").OnTable("User").OnColumn("Firstname");
            _schemaBuilder.Delete.Index("IDX_XYZ").OnTable("User");
            _schemaBuilder.Delete.ForeignKey("User_Address").OnTable("Address");
            _schemaBuilder.Delete.Table("Address");
        }

        [Test]
        public void CreateCommandShouldBeHandled() {
            _schemaBuilder
                .Create.Table("User")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Firstname").AsString(255)
                .WithColumn("Lastname").AsString(100).NotNullable()
                .WithColumn("SN").AsAnsiString(40).Unique()
                .WithColumn("Salary").AsDecimal(9, 2)
                .WithColumn("Gender").AsDecimal().WithDefaultValue("");
        }

        [Test]
        public void DropTableCommandShouldBeHandled() {
            _schemaBuilder
                .Create.Table("User")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Firstname").AsString(255)
                .WithColumn("Lastname").AsString(100).NotNullable()
                .WithColumn("SN").AsAnsiString(40).Unique()
                .WithColumn("Salary").AsDecimal(9, 2)
                .WithColumn("Gender").AsDecimal().WithDefaultValue("");

            _schemaBuilder.Delete.Table("User");
        }

        [Test]
        public void CustomSqlStatementsShouldBeHandled() {
            _schemaBuilder.Execute.Sql("select 1");
        }

        [Test]
        public void AlterTableCommandShouldBeHandled() {

            _schemaBuilder
                .Create.Table("User")
                .WithColumn("Firstname").AsString(255)
                .WithColumn("Lastname").AsString(100).NotNullable();

            _schemaBuilder.Alter.Table("User").AddColumn("Age").AsInt32();
            _schemaBuilder.Alter.Column("Lastname").OnTable("User").AsString().WithDefaultValue("Doe").

            _schemaBuilder
                .CreateTable("User", table => table
                    .Column("Firstname", DbType.String, column => column.WithLength(255))
                    .Column("Lastname", DbType.String, column => column.WithLength(100).NotNull()))
                .AlterTable("User", table => table
                    .AddColumn("Age", DbType.Int32))
                .AlterTable("User", table => table
                    .AlterColumn("Lastname", column => column.WithDefault("Doe")))
                .AlterTable("User", table => table
                    .DropColumn("Firstname")
                );

            // creating a new row should assign a default value to Firstname and Age
            _schemaBuilder.ExecuteSql("insert into TEST_User VALUES (DEFAULT, DEFAULT)");

            // ensure we have one record with the default value
            var query = _session.CreateSQLQuery("SELECT count(*) FROM TEST_User WHERE Lastname = 'Doe'");
            Assert.That(query.UniqueResult<int>(), Is.EqualTo(1));

            // ensure this is not a false positive
            query = _session.CreateSQLQuery("SELECT count(*) FROM TEST_User WHERE Lastname = 'Foo'");
            Assert.That(query.UniqueResult<int>(), Is.EqualTo(0));
        }

        [Test]
        public void ForeignKeyShouldBeCreatedAndRemoved() {

            _schemaBuilder
                .CreateTable("User", table => table
                    .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
                    .Column("Firstname", DbType.String, column => column.WithLength(255))
                    .Column("Lastname", DbType.String, column => column.WithPrecision(0).WithScale(1)))
                .CreateTable("Address", table => table
                    .Column("City", DbType.String)
                    .Column("ZIP", DbType.Int32, column => column.Unique())
                    .Column("UserId", DbType.Int32, column => column.NotNull()))
                .CreateForeignKey("FK_User", "Address", new[] { "UserId" }, "User", new[] { "Id" })
                .DropForeignKey("Address", "FK_User");
        }

        [Test, ExpectedException]
        public void BiggerDataShouldNotFit() {
            _schemaBuilder
                .CreateTable("ContentItemRecord", table => table
                    .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
                    .Column("Data", DbType.String, column => column.WithLength(255)));

            // should write successfully less than 255 chars
            _schemaBuilder
                .ExecuteSql("insert into TEST_ContentItemRecord (Data) values('Hello World')");

            // should throw an exception if trying to write more data
            _schemaBuilder
                .ExecuteSql(String.Format("insert into TEST_ContentItemRecord (Data) values('{0}')", new String('x', 256)));

            _schemaBuilder
                .AlterTable("ContentItemRecord", table => table
                    .AlterColumn("Data", column => column.WithType(DbType.String).WithLength(257)));

            _schemaBuilder
                .ExecuteSql(String.Format("insert into TEST_ContentItemRecord (Data) values('{0}')", new String('x', 256)));
        }

        [Test]
        public void ShouldAllowFieldSizeAlteration() {
            _schemaBuilder
                .CreateTable("ContentItemRecord", table => table
                    .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
                    .Column("Data", DbType.String, column => column.WithLength(255)));

            // should write successfully less than 255 chars
            _schemaBuilder
                .ExecuteSql("insert into TEST_ContentItemRecord (Data) values('Hello World')");

            _schemaBuilder
                .AlterTable("ContentItemRecord", table => table
                    .AlterColumn("Data", column => column.WithType(DbType.String).WithLength(2048)));

            // should write successfully a bigger value now
            _schemaBuilder
                .ExecuteSql(String.Format("insert into TEST_ContentItemRecord (Data) values('{0}')", new String('x', 2048)));
        }

        [Test, ExpectedException(typeof(OrchardException))]
        public void ChangingSizeWithoutTypeShouldNotBeAllowed() {
            _schemaBuilder
                .CreateTable("ContentItemRecord", table => table
                    .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
                    .Column("Data", DbType.String, column => column.WithLength(255)));

            _schemaBuilder
                .AlterTable("ContentItemRecord", table => table
                    .AlterColumn("Data", column => column.WithLength(2048)));

        }

        [Test]
        public void PrecisionAndScaleAreApplied() {

            _schemaBuilder
                .CreateTable("Product", table => table
                    .Column("Price", DbType.Decimal, column => column.WithPrecision(19).WithScale(9))
                    );

            _schemaBuilder
                .ExecuteSql(String.Format("INSERT INTO TEST_Product (Price) VALUES ({0})", "123456.123456789"));

            var query = _session.CreateSQLQuery("SELECT MAX(Price) FROM TEST_Product");
            Assert.That(query.UniqueResult(), Is.EqualTo(123456.123456789m));

        }
    }
}
