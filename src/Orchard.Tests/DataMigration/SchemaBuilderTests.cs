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
            _schemaBuilder.Alter.Column("Lastname").OnTable("User").AsString().WithDefaultValue("Doe");

            _schemaBuilder.Delete.Column("Firstname").FromTable("User");

            // creating a new row should assign a default value to Firstname and Age
            _schemaBuilder.Execute.Sql("insert into TEST_User VALUES (DEFAULT, DEFAULT)");

            // ensure we have one record with the default value
            var query = _session.Database.ExecuteSqlCommand("SELECT count(*) FROM TEST_User WHERE Lastname = 'Doe'");
            Assert.That(query, Is.EqualTo(1));

            // ensure this is not a false positive
            query = _session.Database.ExecuteSqlCommand("SELECT count(*) FROM TEST_User WHERE Lastname = 'Foo'");
            Assert.That(query, Is.EqualTo(0));
        }

        [Test]
        public void ForeignKeyShouldBeCreatedAndRemoved() {

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
               .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("FK_User", "User","Id");

            _schemaBuilder.Delete.ForeignKey("FK_User").OnTable("Address");
        }

        [Test, ExpectedException]
        public void BiggerDataShouldNotFit() {
            _schemaBuilder
                .Create.Table("ContentItemRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Data").AsString(255);

            // should write successfully less than 255 chars
            _schemaBuilder
                .Execute.Sql("insert into TEST_ContentItemRecord (Data) values('Hello World')");

            // should throw an exception if trying to write more data
            _schemaBuilder
                .Execute.Sql(String.Format("insert into TEST_ContentItemRecord (Data) values('{0}')", new String('x', 256)));

            _schemaBuilder
                .Alter.Table("ContentItemRecord").AlterColumn("Data").AsString(257);

            _schemaBuilder
                .Execute.Sql(String.Format("insert into TEST_ContentItemRecord (Data) values('{0}')", new String('x', 256)));
        }

        [Test]
        public void ShouldAllowFieldSizeAlteration() {
            _schemaBuilder
                .Create.Table("ContentItemRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Data").AsString(255);

            // should write successfully less than 255 chars
            _schemaBuilder
                .Execute.Sql("insert into TEST_ContentItemRecord (Data) values('Hello World')");

            _schemaBuilder
                .Alter.Table("ContentItemRecord").AlterColumn("Data").AsString(2048);

            // should write successfully a bigger value now
            _schemaBuilder
                .Execute.Sql(String.Format("insert into TEST_ContentItemRecord (Data) values('{0}')", new String('x', 2048)));
        }

        [Test, ExpectedException(typeof(OrchardException))]
        public void ChangingSizeWithoutTypeShouldNotBeAllowed() {
            _schemaBuilder
                .Create.Table("ContentItemRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Data").AsString(255);

            _schemaBuilder
                .Alter.Table("ContentItemRecord").AlterColumn("Data").AsString(2048);

        }

        [Test]
        public void PrecisionAndScaleAreApplied() {
            _schemaBuilder
                .Create.Table("Product")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Price").AsDecimal(19, 9);

            _schemaBuilder
                .Execute.Sql(String.Format("INSERT INTO TEST_Product (Price) VALUES ({0})", "123456.123456789"));

            var query = _session.Database.SqlQuery<decimal>("SELECT MAX(Price) FROM TEST_Product");
            Assert.That(query, Is.EqualTo(123456.123456789m));

        }
    }
}
