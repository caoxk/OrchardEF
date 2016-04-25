using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Orchard.Data.Providers.SqlProvider {
    public class SqlServerModule : IDataServicesModule {
        public static string ProviderName
        {
            get { return "SqlServer"; }
        }

        public void Configure(IServiceCollection serviceCollection) {
            serviceCollection = new ServiceCollection();
            serviceCollection
                .AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<DataContext>();

            serviceCollection.AddScoped<IDataServicesProvider, SqlServerDataServicesProvider>();
        }
    }
}
