using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Orchard.Data.Providers;
using Orchard.Data.Providers.SqlCeProvider;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.Tests.Records;

namespace Orchard.Tests {
    [TestFixture]
    public class FluentDbTests {
        public class Types {
            private readonly IEnumerable<Type> _types;

            public Types(params Type[] types) {
                _types = types;
            }

            #region ITypeSource Members

            public IEnumerable<Type> GetTypes() {
                return _types;
            }

            #endregion
        }

        [Test]
        public void CreatingSchemaForStatedClassesInTempFile() {
            var types = new Types(typeof(FooRecord), typeof(BarRecord));

            var fileName = "temp.sdf";
            var parameters = new SessionFactoryParameters {
                Provider = "SqlServerCe",
                DataFolder = Path.GetDirectoryName(fileName),
                RecordDescriptors = new List<RecordBlueprint>() {
                   new RecordBlueprint { Type =  typeof(FooRecord), TableName = "FooRecord"},
                   new RecordBlueprint { Type =  typeof(BarRecord), TableName = "BarRecord"}
                }
            };
            var provider = new SqlServerCompactDataServicesProvider(fileName);
            DbConfiguration configuration = provider.BuildConfiguration();
            DbConfiguration.SetConfiguration(configuration);
            //Database.SetInitializer<DbContext>(null);
            var contextOptions = provider.GetContextOptions(parameters);
            // Uncomment to display SQL while running tests
            // ((MsSqlCeConfiguration)persistenceConfigurer).ShowSql();

            var session = new DbContext(contextOptions.ConnectionString, contextOptions.Model);
            session.Set<FooRecord>().Add(new FooRecord { Name = "Hello" });
            session.Set<BarRecord>().Add(new BarRecord { Height = 3, Width = 4.5m });
            session.SaveChanges();
            session.Dispose();

            session = new DbContext(contextOptions.ConnectionString, contextOptions.Model);
            var foos = session.Set<FooRecord>().ToList();
            Assert.That(foos.Count, Is.EqualTo(1));
            Assert.That(foos, Has.All.Property("Name").EqualTo("Hello"));
            session.Dispose();
        }


        [Test]
        public void UsingDataUtilityToBuildSessionFactory() {
            var factory = DataUtility.CreateSessionFactory(typeof(FooRecord), typeof(BarRecord));

            var session = factory.Create();
            var foo1 = new FooRecord { Name = "world" };
            session.Set<FooRecord>().Add(foo1);
            session.SaveChanges();
            session.Dispose();

            session = factory.Create();
            var foo2 = session.Set<FooRecord>().Where(x=>x.Name == "world")
                .Single();
            session.Dispose();

            Assert.That(foo1, Is.Not.SameAs(foo2));
            Assert.That(foo1.Id, Is.EqualTo(foo2.Id));
        }
    }
}