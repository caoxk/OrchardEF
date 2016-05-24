using NUnit.Framework;
using Orchard.DocumentManagement;
using Orchard.DocumentManagement.Handlers;
using Orchard.Settings;

namespace Orchard.Tests.Settings.Handlers {

    [TestFixture]
    public class ContentHandlerTests {
        [Test]
        public void ModelDriverShouldUsePersistenceFilterToDelegateCreateAndLoad() {
            var modelDriver = new TestModelHandler();

            var contentItem = new DocumentItem();
            var part = new TestModelPart();
            contentItem.Weld(part);

            ((IDocumentHandler)modelDriver).Creating(new CreateDocumentContext(contentItem));
            Assert.That(part.CreatingCalled, Is.True);
        }

        [Test]
        public void PartShouldBeAddedBasedOnSimplePredicate() {
            var modelDriver = new TestModelHandler();

            var builder = new DocumentItemBuilder("testing");
            ((IDocumentHandler)modelDriver).Activating(new ActivatingDocumentContext { Builder = builder, ContentType = "testing" });
            var model = builder.Build();
            Assert.That(model.Is<TestModelPart>(), Is.True);
            Assert.That(model.As<TestModelPart>(), Is.Not.Null);
        }

        public class TestModelPart : DocumentPart {
            public bool CreatingCalled { get; set; }
        }


        public class TestModelHandler : DocumentHandler {
            public TestModelHandler() {
                Filters.Add(new ActivatingFilter<TestModelPart>(x => x == "testing"));
                Filters.Add(new TestModelStorageFilter());
            }
        }

        public class TestModelStorageFilter : StorageFilterBase<TestModelPart> {
            protected override void Creating(CreateDocumentContext context, TestModelPart instance) {
                instance.CreatingCalled = true;
            }
        }
    }
}

