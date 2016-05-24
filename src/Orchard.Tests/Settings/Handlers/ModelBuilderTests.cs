﻿using NUnit.Framework;
using Orchard.DocumentManagement;
using Orchard.DocumentManagement.Handlers;
using Orchard.Settings;
using Orchard.Tests.Settings.Models;

namespace Orchard.Tests.Settings.Handlers {
    [TestFixture]
    public class ModelBuilderTests {
        [Test]
        public void BuilderShouldReturnWorkingModelWithTypeAndId() {
            var builder = new DocumentItemBuilder("foo");
            var model = builder.Build();
            Assert.That(model.ContentType, Is.EqualTo("foo"));
        }

        [Test]
        public void IdShouldDefaultToZero() {
            var builder = new DocumentItemBuilder("foo");
            var model = builder.Build();
            Assert.That(model.Id, Is.EqualTo(0));
        }

        [Test]
        public void WeldShouldAddPartToModel() {
            var builder = new DocumentItemBuilder("foo");
            builder.Weld<AlphaPart>();
            var model = builder.Build();

            Assert.That(model.Is<AlphaPart>(), Is.True);
            Assert.That(model.As<AlphaPart>(), Is.Not.Null);
            Assert.That(model.Is<BetaPart>(), Is.False);
            Assert.That(model.As<BetaPart>(), Is.Null);
        }
    }
}

