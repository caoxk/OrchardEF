﻿using System.ComponentModel.DataAnnotations;
using System.Linq;
using Autofac;
using Moq;
using NUnit.Framework;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Implementation;
using Orchard.DocumentManagement;
using Orchard.DocumentManagement.Drivers;
using Orchard.DocumentManagement.Drivers.Coordinators;
using Orchard.DocumentManagement.Handlers;
using Orchard.Mvc;
using Orchard.Settings;

namespace Orchard.Tests.Settings.Handlers.Coordinators {
    [TestFixture]
    public class ContentPartDriverCoordinatorTests {
        private IContainer _container;

        [SetUp]
        public void Init() {
            var builder = new ContainerBuilder();
            //builder.RegisterModule(new ImplicitCollectionSupportModule());
            builder.RegisterType<DocumentPartDriverCoordinator>().As<IDocumentHandler>();
            builder.RegisterType<DefaultShapeFactory>().As<IShapeFactory>();
            _container = builder.Build();
        }

        [Test]
        public void DriverHandlerShouldNotThrowException() {
            var contentHandler = _container.Resolve<IDocumentHandler>();
            contentHandler.BuildDisplay(null);
        }

        [Test]
        public void AllDriversShouldBeCalled() {
            var driver1 = new Mock<IDocumentPartDriver>();
            var driver2 = new Mock<IDocumentPartDriver>();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(driver1.Object);
            builder.RegisterInstance(driver2.Object);
            builder.Update(_container);
            var contentHandler = _container.Resolve<IDocumentHandler>();

            var contentItem = new DocumentItem();
            var context = new BuildDisplayContext(null, contentItem, "", "", new Mock<IShapeFactory>().Object);

            driver1.Verify(x => x.BuildDisplay(context), Times.Never());
            driver2.Verify(x => x.BuildDisplay(context), Times.Never());
            contentHandler.BuildDisplay(context);
            driver1.Verify(x => x.BuildDisplay(context));
            driver2.Verify(x => x.BuildDisplay(context));
        }

        [Test, Ignore("no implementation for IZoneCollection")]
        public void TestDriverCanAddDisplay() {
            var driver = new StubPartDriver();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(driver).As<IDocumentPartDriver>();
            builder.Update(_container);
            var contentHandler = _container.Resolve<IDocumentHandler>();
            dynamic shapeFactory = _container.Resolve<IShapeFactory>();

            var contentItem = new DocumentItem();
            contentItem.Weld(new StubPart { Foo = new[] { "a", "b", "c" } });

            var ctx = new BuildDisplayContext(null, null, "", "", null);
            var context = shapeFactory.Context(ctx);
            Assert.That(context.TopMeta, Is.Null);
            contentHandler.BuildDisplay(ctx);
            Assert.That(context.TopMeta, Is.Not.Null);
            Assert.That(context.TopMeta.Count == 1);
        }

        public class StubPartDriver : DocumentPartDriver<StubPart> {
            protected override string Prefix {
                get { return "Stub"; }
            }

            protected override DriverResult Display(StubPart part, string displayType, dynamic shapeHelper) {
                var stub = shapeHelper.Stub(Foo: string.Join(",", part.Foo));
                if (!string.IsNullOrWhiteSpace(displayType))
                    stub.Metadata.Type = string.Format("{0}.{1}", stub.Metadata.Type, displayType);
                return ContentShape(stub).Location("TopMeta");
                
                //var viewModel = new StubViewModel { Foo = string.Join(",", part.Foo) };
                //if (displayType.StartsWith("Summary"))
                //    return ContentPartTemplate(viewModel, "StubViewModelTerse").Location("TopMeta");

                //return ContentPartTemplate(viewModel).Location("TopMeta");
            }

            protected override DriverResult Editor(StubPart part, dynamic shapeHelper) {
                var viewModel = new StubViewModel { Foo = string.Join(",", part.Foo) };
                return new DocumentTemplateResult(viewModel, null, Prefix).Location("last", "10");
            }

            protected override DriverResult Editor(StubPart part, IUpdateModel updater, dynamic shapeHelper) {
                var viewModel = new StubViewModel { Foo = string.Join(",", part.Foo) };
                updater.TryUpdateModel(viewModel, Prefix, null, null);
                part.Foo = viewModel.Foo.Split(new[] { ',' }).Select(x => x.Trim()).ToArray();
                return new DocumentTemplateResult(viewModel, null, Prefix).Location("last", "10");
            }
        }

        public class StubPart : DocumentPart {
            public string[] Foo { get; set; }
        }

        public class StubViewModel {
            [Required]
            public string Foo { get; set; }
        }
    }
}
