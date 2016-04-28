using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure.DependencyResolution;
using Orchard.Environment.ShellBuilders.Models;

namespace Orchard.Data.Providers {
    public abstract class AbstractDataServicesProvider : IDataServicesProvider {

        public abstract DbConfiguration BuildConfiguration();
        protected abstract string BuildConnectionString(SessionFactoryParameters parameters);

        public DbContextOptions GetContextOptions(SessionFactoryParameters parameters) {
            var options = new DbContextOptions();
            options.ConnectionString = BuildConnectionString(parameters);
            var connection = DbConfiguration.DependencyResolver.GetService<IDbConnectionFactory>()
                .CreateConnection(options.ConnectionString);

            DbModelBuilder builder = new DbModelBuilder();
            foreach(var descriptor in parameters.RecordDescriptors) {
                builder.RegisterEntityType(descriptor.Type);
            }
            var blueprintDescriptors = parameters.RecordDescriptors.ToDictionary(d => d.Type);
            builder.Types().Configure(x => {
                RecordBlueprint blueprint;
                if (blueprintDescriptors.TryGetValue(x.ClrType, out blueprint)) {
                    x.ToTable(blueprint.TableName);
                }
            });
            foreach (var recordAssembly in parameters.RecordDescriptors.Select(x => x.Type.Assembly).Distinct()) {
                builder.Configurations.AddFromAssembly(recordAssembly);
            }

            var model = builder.Build(connection);
            options.Model = model.Compile();

            return options;
        }
    }
}
