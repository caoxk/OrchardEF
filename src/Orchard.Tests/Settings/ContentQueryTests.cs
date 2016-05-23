using System.Linq;
using Autofac;
using Moq;
using NUnit.Framework;
using Orchard.Caching;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.Tests.ContentManagement.Handlers;
using Orchard.Tests.ContentManagement.Records;
using Orchard.Tests.ContentManagement.Models;
using Orchard.DisplayManagement.Implementation;
using Orchard.Tests.Stubs;
using Orchard.UI.PageClass;
using System.Collections.Generic;
using Orchard.Settings;
using Orchard.Settings.Handlers;
using Orchard.Settings.Records;

namespace Orchard.Tests.ContentManagement {
    [TestFixture]
    public class ContentQueryTests {
        private IContainer _container;
        private IContentManager _manager;
        private ISessionFactoryHolder _sessionFactory;
        private DataContext _session;
        private ITransactionManager _transactionManager;

        [TestFixtureSetUp]
        public void InitFixture() {
            var databaseFileName = System.IO.Path.GetTempFileName();
            _sessionFactory = DataUtility.CreateSessionFactory(
                databaseFileName,
                typeof(GammaRecord),
                typeof(DeltaRecord),
                typeof(ContentItemRecord));
        }

        [SetUp]
        public void Init() {
            var builder = new ContainerBuilder();

            builder.RegisterType<DefaultContentManager>().As<IContentManager>().SingleInstance();
            builder.RegisterType<Signals>().As<ISignals>();
            builder.RegisterType<StubCacheManager>().As<ICacheManager>();
            builder.RegisterInstance(new Mock<IContentDisplay>().Object);
            builder.RegisterInstance(new ShellSettings { Name = ShellSettings.DefaultName, DataProvider = "SqlCe" });

            builder.RegisterType<AlphaPartHandler>().As<IContentHandler>();
            builder.RegisterType<BetaPartHandler>().As<IContentHandler>();
            builder.RegisterType<GammaPartHandler>().As<IContentHandler>();
            builder.RegisterType<DeltaPartHandler>().As<IContentHandler>();
            builder.RegisterType<FlavoredPartHandler>().As<IContentHandler>();
            builder.RegisterType<StyledHandler>().As<IContentHandler>();
            builder.RegisterType<DefaultShapeTableManager>().As<IShapeTableManager>();
            builder.RegisterType<ShapeTableLocator>().As<IShapeTableLocator>();
            builder.RegisterType<DefaultShapeFactory>().As<IShapeFactory>();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

            builder.RegisterType<StubExtensionManager>().As<IExtensionManager>();
            builder.RegisterInstance(new Mock<IPageClassBuilder>().Object);
            builder.RegisterType<DefaultContentDisplay>().As<IContentDisplay>();

            _session = _sessionFactory.Create();
            builder.RegisterInstance(_transactionManager = new TestTransactionManager(_session)).As<ITransactionManager>();

            //_session.Delete(string.Format("from {0}", typeof(GammaRecord).FullName));
            //_session.Delete(string.Format("from {0}", typeof(DeltaRecord).FullName));
            //_session.Delete(string.Format("from {0}", typeof(EpsilonRecord).FullName));
            //_session.Delete(string.Format("from {0}", typeof(ContentItemVersionRecord).FullName));
            //_session.Delete(string.Format("from {0}", typeof(ContentItemRecord).FullName));
            //_session.Delete(string.Format("from {0}", typeof(ContentTypeRecord).FullName));
            _transactionManager.RequireNew();

            _container = builder.Build();
            _manager = _container.Resolve<IContentManager>();

        }

        [TearDown]
        public void Cleanup() {
            if (_container != null)
                _container.Dispose();
        }

        private List<IContent> AddSampleData() {
            var items = new List<IContent> {
                _manager.Create<AlphaPart>("alpha", init => { }),
                _manager.Create<BetaPart>("beta", init => { }),
                _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "the frap value"; }),
                _manager.Create<DeltaPart>("delta", init => { init.Record.Quux = "the quux value"; })
            };

            _transactionManager.RequireNew();

            return items;
        }

        [Test]
        public void AllItemsAreReturnedByDefault() {
            AddSampleData();

            var allItems = _manager.Query().List();

            Assert.That(allItems.Count(), Is.EqualTo(4));
            Assert.That(allItems.Count(x => x.Has<AlphaPart>()), Is.EqualTo(1));
            Assert.That(allItems.Count(x => x.Has<BetaPart>()), Is.EqualTo(1));
            Assert.That(allItems.Count(x => x.Has<GammaPart>()), Is.EqualTo(1));
            Assert.That(allItems.Count(x => x.Has<DeltaPart>()), Is.EqualTo(1));
        }

        [Test]
        public void SpecificTypeIsReturnedWhenSpecified() {
            AddSampleData();

            var alphaBeta = _manager.Query().ForType("alpha", "beta").List();

            Assert.That(alphaBeta.Count(), Is.EqualTo(2));
            Assert.That(alphaBeta.Count(x => x.Has<AlphaPart>()), Is.EqualTo(1));
            Assert.That(alphaBeta.Count(x => x.Has<BetaPart>()), Is.EqualTo(1));
            Assert.That(alphaBeta.Count(x => x.Has<GammaPart>()), Is.EqualTo(0));
            Assert.That(alphaBeta.Count(x => x.Has<DeltaPart>()), Is.EqualTo(0));

            var gammaDelta = _manager.Query().ForType("gamma", "delta").List();

            Assert.That(gammaDelta.Count(), Is.EqualTo(2));
            Assert.That(gammaDelta.Count(x => x.Has<AlphaPart>()), Is.EqualTo(0));
            Assert.That(gammaDelta.Count(x => x.Has<BetaPart>()), Is.EqualTo(0));
            Assert.That(gammaDelta.Count(x => x.Has<GammaPart>()), Is.EqualTo(1));
            Assert.That(gammaDelta.Count(x => x.Has<DeltaPart>()), Is.EqualTo(1));
        }

        [Test]
        public void NoItemIsReturnedIfNoItemsSpecified() {
            AddSampleData();

            var items = _manager.Query().ForContentItems(new int[] { }).List();

            Assert.That(items.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ItemsSpecifiedAreReturnedWhenSpecified() {
            var samples = AddSampleData();


            var betaGamma = _manager.Query().ForContentItems(new int[] { samples[1].ContentItem.Id, samples[2].ContentItem.Id }).List();

            Assert.That(betaGamma.Count(), Is.EqualTo(2));
            Assert.That(betaGamma.Count(x => x.Has<AlphaPart>()), Is.EqualTo(0));
            Assert.That(betaGamma.Count(x => x.Has<BetaPart>()), Is.EqualTo(1));
            Assert.That(betaGamma.Count(x => x.Has<GammaPart>()), Is.EqualTo(1));
            Assert.That(betaGamma.Count(x => x.Has<DeltaPart>()), Is.EqualTo(0));

            var alphaDelta = _manager.Query()
                .ForContentItems(new int[] { samples[0].ContentItem.Id, samples[3].ContentItem.Id })
                .List();

            Assert.That(alphaDelta.Count(), Is.EqualTo(2));
            Assert.That(alphaDelta.Count(x => x.Has<AlphaPart>()), Is.EqualTo(1));
            Assert.That(alphaDelta.Count(x => x.Has<BetaPart>()), Is.EqualTo(0));
            Assert.That(alphaDelta.Count(x => x.Has<GammaPart>()), Is.EqualTo(0));
            Assert.That(alphaDelta.Count(x => x.Has<DeltaPart>()), Is.EqualTo(1));
        }

        [Test]
        public void ItemsSpecifiedCanBeFiltered() {
            AddSampleData();
            var oneId = _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "one"; }).ContentItem.Id;
            var twoId = _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "two"; }).ContentItem.Id;
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "three"; });
            var fourId = _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "four"; }).ContentItem.Id;
            _transactionManager.RequireNew();


            var two = _manager.Query()
                .ForContentItems(new int[] { oneId, twoId, fourId })
                .Where<GammaRecord>(x => x.Frap == "two")
                .List();

            Assert.That(two.Count(), Is.EqualTo(1));
            Assert.That(two.Count(x => x.Has<GammaPart>()), Is.EqualTo(1));
            Assert.That(two.Count(x => x.Get<GammaPart>().Record.Frap == "two"), Is.EqualTo(1));

            var none = _manager.Query()
                .ForContentItems(new int[] { oneId, twoId, fourId })
                .Where<GammaRecord>(x => x.Frap == "three")
                .List();

            Assert.That(none.Count(), Is.EqualTo(0));
        }

        [Test]
        public void WherePredicateRestrictsResults() {
            AddSampleData();
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "one"; });
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "two"; });
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "three"; });
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "four"; });
            _transactionManager.RequireNew();

            var twoOrFour = _manager.Query<GammaPart, GammaRecord>()
                .Where(x => x.Frap == "one" || x.Frap == "four")
                .List();

            Assert.That(twoOrFour.Count(), Is.EqualTo(2));
            Assert.That(twoOrFour.Count(x => x.Has<GammaPart>()), Is.EqualTo(2));
            Assert.That(twoOrFour.Count(x => x.Get<GammaPart>().Record.Frap == "one"), Is.EqualTo(1));
            Assert.That(twoOrFour.Count(x => x.Get<GammaPart>().Record.Frap == "four"), Is.EqualTo(1));
        }


        [Test]
        public void EmptyWherePredicateRequiresRecord() {
            AddSampleData();
            var gammas = _manager.Query().Join<GammaRecord>().List();
            var deltas = _manager.Query().Join<DeltaRecord>().List();

            Assert.That(gammas.Count(), Is.EqualTo(1));
            Assert.That(deltas.Count(), Is.EqualTo(1));
            Assert.That(gammas.AsPart<GammaPart>().Single().Record.Frap, Is.EqualTo("the frap value"));
            Assert.That(deltas.AsPart<DeltaPart>().Single().Record.Quux, Is.EqualTo("the quux value"));
        }

        [Test]
        public void OrderMaySortOnJoinedRecord() {
            AddSampleData();
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "one"; });
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "two"; });
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "three"; });
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "four"; });
            _transactionManager.RequireNew();

            var ascending = _manager.Query("gamma")
                .OrderBy<GammaRecord>(x => x.Frap)
                .List<GammaPart>().ToList();

            Assert.That(ascending.Count(), Is.EqualTo(5));
            Assert.That(ascending.First().Record.Frap, Is.EqualTo("four"));
            Assert.That(ascending.Last().Record.Frap, Is.EqualTo("two"));

            _transactionManager.RequireNew();

            var descending = _manager.Query<GammaPart, GammaRecord>()
                .OrderByDescending(x => x.Frap)
                .List().ToList();

            Assert.That(descending.Count(), Is.EqualTo(5));
            Assert.That(descending.First().Record.Frap, Is.EqualTo("two"));
            Assert.That(descending.Last().Record.Frap, Is.EqualTo("four"));
        }

        [Test]
        public void SkipAndTakeProvidePagination() {
            AddSampleData();
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "one"; });
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "two"; });
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "three"; });
            _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "four"; });
            _transactionManager.RequireNew();

            var reverseById = _manager.Query()
                .OrderByDescending<GammaRecord>(x => x.Id)
                .List();

            var subset = _manager.Query()
                .OrderByDescending<GammaRecord>(x => x.Id)
                .Slice(2, 3);

            Assert.That(subset.Count(), Is.EqualTo(3));
            Assert.That(subset.First().Id, Is.EqualTo(reverseById.Skip(2).First().Id));
            Assert.That(subset.Skip(1).First().Id, Is.EqualTo(reverseById.Skip(3).First().Id));
            Assert.That(subset.Skip(2).First().Id, Is.EqualTo(reverseById.Skip(4).First().Id));

        }

        //[Test]
        //public void CountReturnsNumber() {
        //    AddSampleData();

        //    var count = _manager.Query()
        //        .Count();

        //    Assert.That(count, Is.EqualTo(4));
        //}
       

        //[Test]
        //public void StartsWithExtensionShouldBeUsed() {
        //    _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "one"; });
        //    _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "two"; });
        //    _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "three"; });
        //    _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "four"; });
        //    _transactionManager.RequireNew();

        //    var result = _manager.Query<GammaPart, GammaRecord>()
        //        .Where(x => x.Frap.StartsWith("t"))
        //        .List();

        //    Assert.That(result.Count(), Is.EqualTo(2));
        //    Assert.That(result.Count(x => x.Get<GammaPart>().Record.Frap == "two"), Is.EqualTo(1));
        //    Assert.That(result.Count(x => x.Get<GammaPart>().Record.Frap == "three"), Is.EqualTo(1));
        //}

        //[Test]
        //public void EndsWithExtensionShouldBeUsed() {
        //    _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "one"; });
        //    _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "two"; });
        //    _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "three"; });
        //    _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "four"; });
        //    _transactionManager.RequireNew();

        //    var result = _manager.Query<GammaPart, GammaRecord>()
        //        .Where(x => x.Frap.EndsWith("e"))
        //        .List();

        //    Assert.That(result.Count(), Is.EqualTo(2));
        //    Assert.That(result.Count(x => x.Get<GammaPart>().Record.Frap == "one"), Is.EqualTo(1));
        //    Assert.That(result.Count(x => x.Get<GammaPart>().Record.Frap == "three"), Is.EqualTo(1));
        //}

        //[Test]
        //public void ContainsExtensionShouldBeUsed() {
        //    _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "one"; });
        //    _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "two"; });
        //    _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "three"; });
        //    _manager.Create<GammaPart>("gamma", init => { init.Record.Frap = "four"; });
        //    _transactionManager.RequireNew();

        //    var result = _manager.Query<GammaPart, GammaRecord>()
        //        .Where(x => x.Frap.Contains("o"))
        //        .List();

        //    Assert.That(result.Count(), Is.EqualTo(3));
        //    Assert.That(result.Count(x => x.Get<GammaPart>().Record.Frap == "one"), Is.EqualTo(1));
        //    Assert.That(result.Count(x => x.Get<GammaPart>().Record.Frap == "two"), Is.EqualTo(1));
        //    Assert.That(result.Count(x => x.Get<GammaPart>().Record.Frap == "four"), Is.EqualTo(1));
        //}
    }
}
