using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Data.Entity;
using System.Linq;
using Orchard.Logging;
using Orchard.Environment.ShellBuilders.Models;
using Autofac;

namespace Orchard.Data {
    public interface IDataContext {
        DataContext Context { get; }
    }

    public class DataContext : DbContext, IDataContext {
        private readonly IDbContextFactoryHolder _dbContextFactoryHolder;
        private readonly ShellBlueprint _shellBlueprint;
        private readonly Guid _instanceId;

        public DataContext(
            IServiceProvider serviceProvider,
            //Microsoft.Extensions.Logging.ILoggerFactory factory,
            IComponentContext _componentContext,
        //Microsoft.Extensions.Logging.ILogger<DbContext> logger,
        IDbContextFactoryHolder dbContextFactoryHolder,
            ShellBlueprint shellBlueprint):base(serviceProvider) {

            var f = _componentContext.Resolve(typeof(Microsoft.Extensions.Logging.ILoggerFactory));
            var l =_componentContext.Resolve(typeof(Microsoft.Extensions.Logging.ILogger<DataContext>));
            var loger = serviceProvider.GetService(typeof(Microsoft.Extensions.Logging.ILogger<DataContext>));
            _dbContextFactoryHolder = dbContextFactoryHolder;
            _shellBlueprint = shellBlueprint;
            _instanceId = Guid.NewGuid();

            Logger = NullLogger.Instance;
        }



        public ILogger Logger { get; set; }

        public DataContext Context => this;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            Logger.Information("[{0}]: Mapping Records to DB Context", GetType().Name);
            var sw = Stopwatch.StartNew();

            var entityMethod = modelBuilder.GetType().GetRuntimeMethod("Entity", new Type[0]);

            foreach (var recordDescriptor in _shellBlueprint.Records) {
                Logger.Debug("Mapping record {0}", recordDescriptor.Type.FullName);

                entityMethod.MakeGenericMethod(recordDescriptor.Type)
                    .Invoke(modelBuilder, new object[0]);
            }

            sw.Stop();
            Logger.Information("[{0}]: Records Mapped in {1}ms", GetType().Name, sw.ElapsedMilliseconds);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            _dbContextFactoryHolder.Configure(optionsBuilder);
        }
    }
}