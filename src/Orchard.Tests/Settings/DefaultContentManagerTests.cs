using System;
using System.Diagnostics;
using Autofac;
using Moq;
using NUnit.Framework;
using Orchard.Caching;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Implementation;
using Orchard.DocumentManagement;
using Orchard.DocumentManagement.Handlers;
using Orchard.DocumentManagement.Records;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.Settings;
using Orchard.Tests.Settings.Handlers;
using Orchard.Tests.Settings.Models;
using Orchard.Tests.Settings.Records;
using Orchard.Tests.Stubs;
using Orchard.UI.PageClass;

namespace Orchard.Tests.Settings {
    [TestFixture]
    public class DefaultContentManagerTests {
        private const string DefaultAlphaName = "alpha";
        private const string DefaultBetaName = "beta";
        private const string DefaultGammaName = "gamma";
        private const string DefaultDeltaName = "delta";

        private IContainer _container;
        private IDocumentManager _manager;
        private ISessionFactoryHolder _sessionFactory;
        private DataContext _session;

        [TestFixtureSetUp]
        public void InitFixture() {
            var databaseFileName = System.IO.Path.GetTempFileName();
            _sessionFactory = DataUtility.CreateSessionFactory(
                databaseFileName,
                typeof(DocumentItemRecord),
                typeof(GammaRecord),
                typeof(DeltaRecord),
                typeof(MegaRecord));
        }

        [SetUp]
        public void Init() {

            var builder = new ContainerBuilder();
            builder.RegisterType<DefaultDocumentManager>().As<IDocumentManager>();
            builder.RegisterType<StubCacheManager>().As<ICacheManager>();
            builder.RegisterType<Signals>().As<ISignals>();
            builder.RegisterInstance(new Mock<IDocumentDisplay>().Object);
            builder.RegisterInstance(new ShellSettings {Name = ShellSettings.DefaultName, DataProvider = "SqlCe"});

            builder.RegisterType<AlphaPartHandler>().As<IDocumentHandler>();
            builder.RegisterType<BetaPartHandler>().As<IDocumentHandler>();
            builder.RegisterType<GammaPartHandler>().As<IDocumentHandler>();
            builder.RegisterType<DeltaPartHandler>().As<IDocumentHandler>();
            //builder.RegisterType<EpsilonPartHandler>().As<IContentHandler>();
            builder.RegisterType<FlavoredPartHandler>().As<IDocumentHandler>();
            builder.RegisterType<StyledHandler>().As<IDocumentHandler>();
            builder.RegisterType<DefaultShapeTableManager>().As<IShapeTableManager>();
            builder.RegisterType<ShapeTableLocator>().As<IShapeTableLocator>();
            builder.RegisterType<DefaultShapeFactory>().As<IShapeFactory>();
            builder.RegisterInstance(new Mock<IPageClassBuilder>().Object); 
            builder.RegisterType<DefaultDocumentDisplay>().As<IDocumentDisplay>();

            builder.RegisterType<StubExtensionManager>().As<IExtensionManager>();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

            _session = _sessionFactory.Create();
            builder.RegisterInstance(new TestTransactionManager(_session)).As<ITransactionManager>();

            _container = builder.Build();
            _manager = _container.Resolve<IDocumentManager>();
        }

        [TearDown]
        public void Cleanup() {
            if (_container != null)
                _container.Dispose();
        }

        [Test]
        public void AlphaDriverShouldWeldItsPart() {
            var foo = _manager.New(DefaultAlphaName);

            Assert.That(foo.Is<AlphaPart>(), Is.True);
            Assert.That(foo.As<AlphaPart>(), Is.Not.Null);
            Assert.That(foo.Is<BetaPart>(), Is.False);
            Assert.That(foo.As<BetaPart>(), Is.Null);
        }

        [Test]
        public void StronglyTypedNewShouldTypeCast() {
            var foo = _manager.New<AlphaPart>(DefaultAlphaName);
            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.GetType(), Is.EqualTo(typeof(AlphaPart)));
        }

        [Test, ExpectedException(typeof(InvalidCastException))]
        public void StronglyTypedNewShouldThrowCastExceptionIfNull() {
            _manager.New<BetaPart>(DefaultAlphaName);
        }

        [Test]
        public void AlphaIsFlavoredAndStyledAndBetaIsFlavoredOnly() {
            var alpha = _manager.New<AlphaPart>(DefaultAlphaName);
            var beta = _manager.New<BetaPart>(DefaultBetaName);

            Assert.That(alpha.Is<FlavoredPart>(), Is.True);
            Assert.That(alpha.Is<StyledPart>(), Is.True);
            Assert.That(beta.Is<FlavoredPart>(), Is.True);
            Assert.That(beta.Is<StyledPart>(), Is.False);
        }

        [Test]
        public void GetByIdShouldDetermineTypeAndLoadParts() {
            var modelRecord = CreateModelRecord(DefaultAlphaName);

            var contentItem = _manager.Get(modelRecord.Id);
            Assert.That(contentItem.ContentType, Is.EqualTo(DefaultAlphaName));
            Assert.That(contentItem.Id, Is.EqualTo(modelRecord.Id));
        }


        [Test]
        public void ModelPartWithRecordShouldCallRepositoryToPopulate() {

            CreateModelRecord(DefaultGammaName);
            CreateModelRecord(DefaultGammaName);
            var modelRecord = CreateModelRecord(DefaultGammaName);

            var model = _manager.Get(modelRecord.Id);

            //// create a gamma record
            //var gamma = new GammaRecord {
            //    ContentItemRecord = _container.Resolve<IRepository<ContentItemRecord>>().Get(model.Id),
            //    Frap = "foo"
            //};

            //_container.Resolve<IRepository<GammaRecord>>().Create(gamma);
            //_session.Flush();
            //_session.Clear();

            // re-fetch from database
            model = _manager.Get(modelRecord.Id);

            Assert.That(model.ContentType, Is.EqualTo(DefaultGammaName));
            Assert.That(model.Id, Is.EqualTo(modelRecord.Id));
            Assert.That(model.Is<GammaPart>(), Is.True);
            Assert.That(model.As<GammaPart>().Record, Is.Not.Null);
            Assert.That(model.As<GammaPart>().Record.ContentItemRecord.Id, Is.EqualTo(model.Id));

        }

        [Test]
        public void CreateShouldMakeModelAndContentTypeRecords() {
            var beta = _manager.New(DefaultBetaName);
            _manager.Create(beta);

            var modelRecord = _container.Resolve<IRepository<DocumentItemRecord>>().Get(beta.Id);
            Assert.That(modelRecord, Is.Not.Null);
            Assert.That(modelRecord.ContentType, Is.EqualTo(DefaultBetaName));
        }


        [Test]
        public void BigStringsShouldNotBeTruncated() {
            var megaRepository = _container.Resolve<IRepository<MegaRecord>>();
            var mega = new MegaRecord() { BigStuff = new string('x', 20000) };
            megaRepository.Create(mega);
            _session.SaveChanges();
        }

        [Test]
        public void StandardStringsShouldNotHaveAStandardSize() {
            var megaRepository = _container.Resolve<IRepository<MegaRecord>>();
            var mega = new MegaRecord() { SmallStuff = new string('x', 256) };
            megaRepository.Create(mega);
            _session.SaveChanges();
        }

        private DocumentItemRecord CreateModelRecord(string contentType) {
            var contentItemRepository = _container.Resolve<IRepository<DocumentItemRecord>>();

            var modelRecord = new DocumentItemRecord { ContentType = contentType };
            contentItemRepository.Create(modelRecord);

            _session.SaveChanges();
            return modelRecord;
        }

        [Test]
        public void InitialVersionShouldBeOne() {
            var gamma1 = _manager.Create<GammaPart>(DefaultGammaName);
            Assert.That(gamma1.ContentItem.Record, Is.Not.Null);

            _session.SaveChanges();
            Trace.WriteLine("session flushed");
            _session.Clear();

            var gamma2 = _manager.Get<GammaPart>(gamma1.ContentItem.Id);
            Assert.That(gamma2.ContentItem.Record, Is.Not.Null);

            // asserts results are re-acquired from db
            Assert.That(gamma1, Is.Not.SameAs(gamma2));
            Assert.That(gamma1.Record, Is.Not.SameAs(gamma2.Record));
            Assert.That(gamma1.ContentItem, Is.Not.SameAs(gamma2.ContentItem));
            Assert.That(gamma1.ContentItem.Record, Is.Not.SameAs(gamma2.ContentItem.Record));
        }


        [Test]
        public void NonVersionedPartsAreBoundToSameRecord() {
            Trace.WriteLine("gamma1");
            var gamma1 = _manager.Create<GammaPart>(DefaultGammaName, init => init.Record.Frap = "version one");
            Trace.WriteLine("gamma2");
            var gamma2 = _manager.Get<GammaPart>(gamma1.ContentItem.Id);
            Assert.That(gamma1.Record.Frap, Is.EqualTo("version one"));
            Assert.That(gamma2.Record.Frap, Is.EqualTo("version one"));
            gamma2.Record.Frap = "version two";
            Assert.That(gamma1.Record.Frap, Is.EqualTo("version two"));
            Assert.That(gamma2.Record.Frap, Is.EqualTo("version two"));

            Trace.WriteLine("flush");
            _session.SaveChanges();
            _session.Clear();

            Trace.WriteLine("gamma1B");
            var gamma1B = _manager.Get<GammaPart>(gamma1.ContentItem.Id);
            Trace.WriteLine("gamma2B");
            var gamma2B = _manager.Get<GammaPart>(gamma1.ContentItem.Id);
            Assert.That(gamma1B.Record, Is.SameAs(gamma2B.Record));
            Assert.That(gamma1B.Record.Frap, Is.EqualTo("version two"));
            Assert.That(gamma2B.Record.Frap, Is.EqualTo("version two"));

            Assert.That(gamma1.ContentItem.Record, Is.Not.SameAs(gamma1B.ContentItem.Record));
            Assert.That(gamma2.ContentItem.Record, Is.Not.SameAs(gamma2B.ContentItem.Record));
            Assert.That(gamma1.ContentItem.Record, Is.SameAs(gamma2.ContentItem.Record));
            Assert.That(gamma1B.ContentItem.Record, Is.SameAs(gamma2B.ContentItem.Record));

            Trace.WriteLine("flush");
            _session.SaveChanges();
        }

        private void Flush() {
            Trace.WriteLine("flush");
            _session.SaveChanges();

        }
        private void FlushAndClear() {
            Trace.WriteLine("flush");
            _session.SaveChanges();
            Trace.WriteLine("clear");

        }

        [Test]
        public void EmptyTypeDefinitionShouldBeCreatedIfNotAlreadyDefined() {
            var contentItem = _manager.New("no-such-type");
            Assert.That(contentItem.ContentType, Is.EqualTo("no-such-type"));
        }
    }
}

