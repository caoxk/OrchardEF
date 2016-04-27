using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;
using Microsoft.Extensions.DependencyInjection;
using Orchard.Data.Providers;
using Orchard.Data.Providers.InMemoryProvider;
using Orchard.Data.Providers.SqlCeProvider;
using Orchard.Data.Providers.SqlProvider;
using Orchard.Environment.Configuration;

namespace Orchard.Data {
    public interface IDataModuleRegistrations {
        void Registrations(ContainerBuilder builder, ShellSettings shellSettings);
    }

    public class DefaultDataModuleRegistrations : IDataModuleRegistrations {
        public void Registrations(ContainerBuilder builder, ShellSettings shellSettings) {
            if (string.IsNullOrEmpty(shellSettings.DataProvider)) {
                return;
            }
            IServiceCollection serviceCollection = new ServiceCollection();
            var entityFrameworkBuilder = serviceCollection.AddEntityFramework();
            switch (shellSettings.DataProvider) {
                case "SqlServer":
                    entityFrameworkBuilder.AddSqlServer();
                    serviceCollection.AddScoped<IDataServicesProvider, SqlServerDataServicesProvider>();
                    break;
                case "SqlServerCe":
                    entityFrameworkBuilder.AddSqlCe();
                    serviceCollection.AddScoped<IDataServicesProvider, SqlServerCompactDataServicesProvider>();
                    break;
                case "InMemory":
                    entityFrameworkBuilder.AddInMemoryDatabase();
                    serviceCollection.AddScoped<IDataServicesProvider, InMemoryDataServicesProvider>();
                    break;
            }
            //entityFrameworkBuilder.AddDbContext<DataContext>();
            RemoveServiceType(serviceCollection, typeof(Microsoft.Extensions.Logging.ILoggerFactory));
            RemoveServiceType(serviceCollection, typeof(Microsoft.Extensions.Logging.ILogger<>));
            //RemoveServiceType(serviceCollection, typeof(DataContext));

            builder.RegisterType<DataContext>().AsSelf().InstancePerDependency();

            //serviceCollection.AddLogging();
            //Microsoft.Extensions.Logging.LoggerFactory logFactory = new Microsoft.Extensions.Logging.LoggerFactory();
            //logFactory.AddProvider(new Orchard.Logging.Log4NetProvider(Orchard.Logging.NullLogger.Instance));
            //serviceCollection.AddInstance<Microsoft.Extensions.Logging.ILoggerFactory>(logFactory);
            //serviceCollection.Add(ServiceDescriptor.Singleton<Microsoft.Extensions.Logging.ILoggerFactory, Microsoft.Extensions.Logging.LoggerFactory>());
            //serviceCollection.Add(ServiceDescriptor.Singleton(typeof(Microsoft.Extensions.Logging.ILogger<>), typeof(Microsoft.Extensions.Logging.Logger<>)));

            builder.Populate(serviceCollection);
            
        }

        private void RemoveServiceType(IServiceCollection serviceCollection, Type servicewType) {
            var descriptor = serviceCollection.FirstOrDefault(x => x.ServiceType == servicewType);
            if(descriptor != null) {
                serviceCollection.Remove(descriptor);
            }
        }
    }

}
