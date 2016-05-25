using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Moq;
using NUnit.Framework;
using Orchard.Caching;
using Orchard.Core.Scheduling.Models;
using Orchard.Core.Scheduling.Services;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Implementation;
using Orchard.Environment.Extensions;
using Orchard.Tasks.Scheduling;
using Orchard.Tests;
using Orchard.Tests.Settings;
using Orchard.Tests.Stubs;
using Orchard.UI.PageClass;

namespace Orchard.Core.Tests.Scheduling {
    [TestFixture]
    public class ScheduledTaskManagerTests : DatabaseEnabledTestsBase {
        private IRepository<ScheduledTaskRecord> _repository;
        private IScheduledTaskManager _scheduledTaskManager;
        private Mock<IOrchardServices> _mockServices;

        public override void Init() {
            _mockServices = new Mock<IOrchardServices>();
            base.Init();
            _repository = _container.Resolve<IRepository<ScheduledTaskRecord>>();
            _scheduledTaskManager = _container.Resolve<IScheduledTaskManager>();
        }

        public override void Register(ContainerBuilder builder) {
            builder.RegisterInstance(_mockServices.Object);
            builder.RegisterType<Signals>().As<ISignals>();
            builder.RegisterType<StubCacheManager>().As<ICacheManager>();
            builder.RegisterType<DefaultShapeTableManager>().As<IShapeTableManager>();
            builder.RegisterType<ShapeTableLocator>().As<IShapeTableLocator>();
            builder.RegisterType<DefaultShapeFactory>().As<IShapeFactory>();

            builder.RegisterType<ScheduledTaskManager>().As<IScheduledTaskManager>();

            builder.RegisterType<StubExtensionManager>().As<IExtensionManager>();
            builder.RegisterInstance(new Mock<IPageClassBuilder>().Object); 
        }

        protected override IEnumerable<Type> DatabaseTypes {
            get {
                return new[] {
                                 typeof(ScheduledTaskRecord),
                             };
            }
        }

        [Test]
        public void ShouldGetTasksByType() {
            _scheduledTaskManager.CreateTask("First", _clock.UtcNow);
            _scheduledTaskManager.CreateTask("First", _clock.UtcNow);
            _scheduledTaskManager.CreateTask("First", _clock.UtcNow);
            _scheduledTaskManager.CreateTask("Second", _clock.UtcNow);
            _scheduledTaskManager.CreateTask("Second", _clock.UtcNow);
            _scheduledTaskManager.CreateTask("Third", _clock.UtcNow);

            _session.SaveChanges();
            _session.Clear();

            Assert.That(_scheduledTaskManager.GetTasks("First").Count(), Is.EqualTo(3));
            Assert.That(_scheduledTaskManager.GetTasks("Second").Count(), Is.EqualTo(2));
            Assert.That(_scheduledTaskManager.GetTasks("Third").Count(), Is.EqualTo(1));
            Assert.That(_scheduledTaskManager.GetTasks("Fourth").Count(), Is.EqualTo(0));
        }

        [Test]
        public void ShouldGetTasksByTypeAndScheduledDate() {
            _scheduledTaskManager.CreateTask("First", _clock.UtcNow);
            _scheduledTaskManager.CreateTask("First", _clock.UtcNow.AddHours(1));
            _scheduledTaskManager.CreateTask("First", _clock.UtcNow.AddHours(2));

            _session.SaveChanges();
            _session.Clear();

            Assert.That(_scheduledTaskManager.GetTasks("Foo", _clock.UtcNow.AddHours(5)).Count(), Is.EqualTo(0));

            Assert.That(_scheduledTaskManager.GetTasks("First", _clock.UtcNow.AddMinutes(-1)).Count(), Is.EqualTo(0));
            Assert.That(_scheduledTaskManager.GetTasks("First", _clock.UtcNow.AddMinutes(1)).Count(), Is.EqualTo(1));
            Assert.That(_scheduledTaskManager.GetTasks("First", _clock.UtcNow.AddHours(1)).Count(), Is.EqualTo(2));
            Assert.That(_scheduledTaskManager.GetTasks("First", _clock.UtcNow.AddHours(2)).Count(), Is.EqualTo(3));
        }
    }
}
