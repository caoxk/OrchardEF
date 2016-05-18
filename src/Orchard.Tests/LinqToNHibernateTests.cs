using System.Data.Entity;
using System.Linq;
using NUnit.Framework;
using Orchard.Data;
using Orchard.Tests.Records;

namespace Orchard.Tests {
    [TestFixture]
    public class LinqToNHibernateTests {
        #region Setup/Teardown

        [SetUp]
        public void Init() {
            var sessionFactory = DataUtility.CreateSessionFactory(typeof (FooRecord));
            using (var session = sessionFactory.Create()) {
                session.Set<FooRecord>().Add(new FooRecord {Name = "one"});
                session.Set<FooRecord>().Add(new FooRecord {Name = "two"});
                session.Set<FooRecord>().Add(new FooRecord {Name = "three"});
                session.SaveChanges();
            }
            _session = sessionFactory.Create();
        }

        [TearDown]
        public void Term() {
            _session.Dispose();
        }

        #endregion

        private DataContext _session;

        [Test]
        public void WhereClauseShouldLimitResults() {
            var foos = from f in _session.Set<FooRecord>() where f.Name == "two" || f.Name == "one" select f;

            Assert.That(foos.Count(), Is.EqualTo(2));
            Assert.That(foos, Has.Some.Property("Name").EqualTo("one"));
            Assert.That(foos, Has.Some.Property("Name").EqualTo("two"));
            Assert.That(foos, Has.None.Property("Name").EqualTo("three"));
        }
    }
}