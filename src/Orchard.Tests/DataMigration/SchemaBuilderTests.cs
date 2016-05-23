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
using System.IO;
using Orchard.Tests.Environment;
using Orchard.Tests.FileSystems.AppData;
using Orchard.Data.Providers.SqlCeProvider;
using System.Data.Entity;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Processors;
using Orchard.Data.Migration.Processors.SqlServer;
using Orchard.Tests.Settings;

namespace Orchard.Tests.DataMigration
{
	[TestFixture]
    public class SchemaBuilderTestsBase {
        private IContainer _container;
        private ISessionFactoryHolder _sessionFactory;
        private string _databaseFileName;
        private string _tempFolder;
        private IMigrationExecutor _migrationExecutor;
        private DataContext _session;

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
            builder.RegisterType<DefaultMigrationExecutor>().As<IMigrationExecutor>();
            builder.RegisterType<MigrationProcessorFactoryProvider>().As<IMigrationProcessorFactoryProvider>();
            builder.RegisterType<SqlServerCeProcessorFactory>().As<IMigrationProcessorFactory>();
            builder.RegisterInstance(new TestTransactionManager(_session)).As<ITransactionManager>();
            builder.RegisterInstance(new ShellBlueprint { Records = Enumerable.Empty<RecordBlueprint>() }).As<ShellBlueprint>();
            builder.RegisterInstance(new ShellSettings { Name = "temp", DataProvider = "SqlServerCe", DataTablePrefix = "TEST" }).As<ShellSettings>();
            builder.RegisterModule(new DataModule());
            _container = builder.Build();

            _migrationExecutor = _container.Resolve<IMigrationExecutor>();
        }

        [Test]
        public void AllMethodsShouldBeCalledSuccessfully() {
            _migrationExecutor.ExecuteMigration(builder=> {
                builder
                .Create.Table("User")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Firstname").AsString(255)
                .WithColumn("Lastname").AsDecimal(12, 1);

                builder
                   .Create.Table("Address")
                   .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                   .WithColumn("City").AsString(255)
                   .WithColumn("ZIP").AsInt32().Unique("UC_ZIP")
                   .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("User_Address", "User", "Id");

                builder.Alter.Table("User").AddColumn("Age").AsInt32().Nullable();
                builder.Delete.Column("Lastname").FromTable("User");
                builder.Create.Index("IDX_XYZ").OnTable("User").OnColumn("Firstname");
                builder.Delete.Index("IDX_XYZ").OnTable("User");
                builder.Delete.ForeignKey("User_Address").OnTable("Address");
                builder.Delete.Table("Address");
            });
        }

        [Test]
        public void CreateCommandShouldBeHandled() {
            _migrationExecutor.ExecuteMigration(builder => {
                builder
                .Create.Table("User")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Firstname").AsString(255)
                .WithColumn("Lastname").AsString(100).NotNullable()
                .WithColumn("SN").AsAnsiString(40).Unique("UC_SN")
                .WithColumn("Salary").AsDecimal(9, 2)
                .WithColumn("Gender").AsDecimal().WithDefaultValue("");
            });
        }

        [Test]
        public void DropTableCommandShouldBeHandled() {
            _migrationExecutor.ExecuteMigration(builder => {
                builder
                .Create.Table("User")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Firstname").AsString(255)
                .WithColumn("Lastname").AsString(100).NotNullable()
                .WithColumn("SN").AsAnsiString(40).Unique("UC_SN")
                .WithColumn("Salary").AsDecimal(9, 2)
                .WithColumn("Gender").AsDecimal().WithDefaultValue("");

                builder.Delete.Table("User");
            });
        }

        [Test]
        public void CustomSqlStatementsShouldBeHandled() {
            _migrationExecutor.ExecuteMigration(builder => {
                builder.Execute.Sql("select 1");
            });
        }

        [Test]
        public void AlterTableCommandShouldBeHandled() {
            _migrationExecutor.ExecuteMigration(builder => {
                builder
                .Create.Table("TEST_User")
                .WithColumn("Firstname").AsString(255)
                .WithColumn("Lastname").AsString(100).NotNullable();

                builder.Alter.Table("TEST_User").AddColumn("Age").AsInt32().Nullable();
                builder.Alter.Column("Lastname").OnTable("TEST_User").AsString().WithDefaultValue("Doe");

                builder.Delete.Column("Firstname").FromTable("TEST_User");

                // creating a new row should assign a default value to Firstname and Age
                builder.Execute.Sql("insert into TEST_User VALUES (DEFAULT, DEFAULT)");
            });
            // ensure we have one record with the default value
            var query = _session.Database.SqlQuery<int>("SELECT count(*) FROM TEST_User WHERE Lastname = 'Doe'");
            Assert.That(query.Single(), Is.EqualTo(1));

            // ensure this is not a false positive
            query = _session.Database.SqlQuery<int>("SELECT count(*) FROM TEST_User WHERE Lastname = 'Foo'");
            Assert.That(query.Single(), Is.EqualTo(0));
        }

        [Test]
        public void ForeignKeyShouldBeCreatedAndRemoved() {
            _migrationExecutor.ExecuteMigration(builder => {
                builder
                .Create.Table("User")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Firstname").AsString(255)
                .WithColumn("Lastname").AsDecimal(12, 1);

                builder
                   .Create.Table("Address")
               .WithColumn("Id").AsInt32().PrimaryKey().Identity()
               .WithColumn("City").AsString(255)
               .WithColumn("ZIP").AsInt32().Unique("UC_ZIP")
               .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("FK_User", "User", "Id");

                builder.Delete.ForeignKey("FK_User").OnTable("Address");
            });
        }

        [Test, ExpectedException]
        public void BiggerDataShouldNotFit() {
            _migrationExecutor.ExecuteMigration(builder => {
                builder
                .Create.Table("ContentItemRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Data").AsString(255);

                // should write successfully less than 255 chars
                builder
                    .Execute.Sql("insert into TEST_ContentItemRecord (Data) values('Hello World')");

                // should throw an exception if trying to write more data
                builder
                    .Execute.Sql(String.Format("insert into TEST_ContentItemRecord (Data) values('{0}')", new String('x', 256)));

                builder
                    .Alter.Table("ContentItemRecord").AlterColumn("Data").AsString(257);

                builder
                    .Execute.Sql(String.Format("insert into TEST_ContentItemRecord (Data) values('{0}')", new String('x', 256)));
            });
        }

        [Test]
        public void ShouldAllowFieldSizeAlteration() {
            _migrationExecutor.ExecuteMigration(builder => {
                builder
                .Create.Table("TEST_ContentItemRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Data").AsString(255);

                // should write successfully less than 255 chars
                builder
                    .Execute.Sql("insert into TEST_ContentItemRecord (Data) values('Hello World')");

                builder
                    .Alter.Table("TEST_ContentItemRecord").AlterColumn("Data").AsString(2048);

                // should write successfully a bigger value now
                builder
                    .Execute.Sql(String.Format("insert into TEST_ContentItemRecord (Data) values('{0}')", new String('x', 2048)));
            });
        }

        //[Test, ExpectedException(typeof(OrchardException))]
        //public void ChangingSizeWithoutTypeShouldNotBeAllowed() {
        //    _migrationExecutor.ExecuteMigration(builder => {
        //        builder
        //        .Create.Table("ContentItemRecord")
        //        .WithColumn("Id").AsInt32().PrimaryKey().Identity()
        //        .WithColumn("Data").AsString(255);

        //        builder
        //            .Alter.Table("ContentItemRecord").AlterColumn("Data").AsString(2048);
        //    });
        //}

        [Test]
        public void PrecisionAndScaleAreApplied() {
            _migrationExecutor.ExecuteMigration(builder => {
                builder
                .Create.Table("TEST_Product")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Price").AsDecimal(19, 9);

                builder
                    .Execute.Sql(String.Format("INSERT INTO TEST_Product (Price) VALUES ({0})", "123456.123456789"));
            });
            var query = _session.Database.SqlQuery<decimal>("SELECT MAX(Price) FROM TEST_Product");
            Assert.That(query.Single(), Is.EqualTo(123456.123456789m));
        }
    }
}
