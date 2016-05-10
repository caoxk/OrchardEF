using NUnit.Framework;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.Tests.Records;

namespace Orchard.Tests.Data.Builders {
    [TestFixture]
    public class SessionFactoryBuilderTests {
        [Test]
        public void SqlCeSchemaShouldBeGeneratedAndUsable() {

            var recordDescriptors = new[] {
                                              new RecordBlueprint {TableName = "Hello", Type = typeof (FooRecord)}
                                          };

            ProviderUtilities.RunWithSqlCe(recordDescriptors,
                sessionFactory => {
                    var session = sessionFactory.Create();
                    var foo = new FooRecord { Name = "hi there" };
                    session.Set<FooRecord>().Add(foo);
                    session.SaveChanges();
                    session.Dispose();

                    Assert.That(foo, Is.Not.EqualTo(0));

                });

        }

        [Test]
        public void SqlServerSchemaShouldBeGeneratedAndUsable() {
            var recordDescriptors = new[] {
                                              new RecordBlueprint {TableName = "Hello", Type = typeof (FooRecord)}
                                          };

            ProviderUtilities.RunWithSqlServer(recordDescriptors,
                sessionFactory => {
                    var session = sessionFactory.Create();
                    var foo = new FooRecord { Name = "hi there" };
                    session.Set<FooRecord>().Add(foo);
                    session.SaveChanges();
                    session.Dispose();

                    Assert.That(foo, Is.Not.EqualTo(0));

                });
        }
    }
}