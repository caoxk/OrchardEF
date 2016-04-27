using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Data.Entity;
using System.Linq;
using Orchard.Logging;
using Orchard.Environment.ShellBuilders.Models;
using Autofac;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Storage;
using Orchard.Data.Alterations;

namespace Orchard.Data {
    public interface IDataContext {
        DataContext Context { get; }
    }

    public class DataContext : DbContext, IDataContext {
        private readonly IDbContextFactoryHolder _dbContextFactoryHolder;
        private readonly ShellBlueprint _shellBlueprint;
        private readonly Guid _instanceId;
        private readonly IEntityTypeOverrideHandler _entityTypeOverrideHandler;
        private readonly IServiceProvider _serviceProvider;

        public DataContext(
            IServiceProvider serviceProvider,
            IDbContextFactoryHolder dbContextFactoryHolder,
            ShellBlueprint shellBlueprint,
            IEntityTypeOverrideHandler entityTypeOverrideHandler) :base(serviceProvider) {

            _dbContextFactoryHolder = dbContextFactoryHolder;
            _shellBlueprint = shellBlueprint;
            _instanceId = Guid.NewGuid();
            _entityTypeOverrideHandler = entityTypeOverrideHandler;
            _serviceProvider = serviceProvider;

            Logger = NullLogger.Instance;
        }



        public ILogger Logger { get; set; }

        public DataContext Context => this;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            var contextServices = (IDatabaseProvider)_serviceProvider.GetService(typeof (IDatabaseProvider));
            //var modelSource = contextServices.DatabaseProviderServices.ModelSource;
            Logger.Information("[{0}]: Mapping Records to DB Context", GetType().Name);
            var sw = Stopwatch.StartNew();

            var entityMethod = modelBuilder.GetType().GetRuntimeMethod("Entity", new Type[0]);

            foreach (var recordDescriptor in _shellBlueprint.Records) {
                Logger.Debug("Mapping record {0}", recordDescriptor.Type.FullName);

                entityMethod.MakeGenericMethod(recordDescriptor.Type)
                    .Invoke(modelBuilder, new object[0]);
            }
            _entityTypeOverrideHandler.Alter(modelBuilder);
            sw.Stop();
            Logger.Information("[{0}]: Records Mapped in {1}ms", GetType().Name, sw.ElapsedMilliseconds);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            _dbContextFactoryHolder.Configure(optionsBuilder);
        }
    }
}