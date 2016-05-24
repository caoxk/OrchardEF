using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure.DependencyResolution;
using Orchard.Environment.ShellBuilders.Models;
using System.Reflection;

namespace Orchard.Data.Providers {
    public abstract class AbstractDataServicesProvider : IDataServicesProvider {

        public void BuildConfiguration()
        {
            var type = "System.Data.Entity.Infrastructure.DependencyResolution.DbConfigurationManager";
            var configurationManagerType = typeof(DbContext).Assembly.GetType(type);
            var instanceProperty = configurationManagerType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
            var knownAssembliesProperty = configurationManagerType.GetField("_knownAssemblies", BindingFlags.NonPublic | BindingFlags.Instance);
            var instance = instanceProperty.GetValue(null);
            if (knownAssembliesProperty != null)
            {
                var knownAssemblies = (ConcurrentDictionary<Assembly, object>)knownAssembliesProperty.GetValue(instance);
                knownAssemblies.TryAdd(typeof(DataContext).Assembly, null);
            }
            var configuration = GetConfiguration();
            DbConfiguration.SetConfiguration(configuration);
        }

        protected abstract DbConfiguration GetConfiguration();
        protected abstract string BuildConnectionString(SessionFactoryParameters parameters);

        public DbContextOptions GetContextOptions(SessionFactoryParameters parameters) {
            var options = new DbContextOptions();
            options.ConnectionString = BuildConnectionString(parameters);
            var connection = DbConfiguration.DependencyResolver.GetService<IDbConnectionFactory>()
                .CreateConnection(options.ConnectionString);

            DbModelBuilder builder = new DbModelBuilder();
            foreach (var recordAssembly in parameters.RecordDescriptors.Select(x => x.Type.Assembly).Distinct()){
                builder.Configurations.AddFromAssembly(recordAssembly);
            }
            foreach (var descriptor in parameters.RecordDescriptors) {
                builder.RegisterEntityType(descriptor.Type);
            }
            var blueprintDescriptors = parameters.RecordDescriptors.ToDictionary(d => d.Type);
            builder.Types().Configure(x => {
                RecordBlueprint blueprint;
                if (blueprintDescriptors.TryGetValue(x.ClrType, out blueprint)) {
                    x.ToTable(blueprint.TableName);
                }
            });

            var model = builder.Build(connection);
            options.Model = model.Compile();

            return options;
        }
    }
}
