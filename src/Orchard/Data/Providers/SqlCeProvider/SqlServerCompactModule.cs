using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Orchard.Data.Providers.SqlCeProvider {
    public class SqlServerCompactModule : IDataServicesModule {
        public static string ProviderName
        {
            get { return "SqlServerCe"; }
        }
        public void Configure(IServiceCollection serviceCollection) {
            serviceCollection
                .AddEntityFramework()
                .AddSqlCe()
                .AddDbContext<DataContext>();

            serviceCollection.AddScoped<IDataServicesProvider, SqlServerCompactDataServicesProvider>();
        }
    }
}