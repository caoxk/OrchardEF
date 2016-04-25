using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Orchard.Data.Providers.SqlProvider {
    public class SqlServerDataServicesProvider : IDataServicesProvider {
        public static string ProviderName {
            get { return "SqlServer"; }
        }

        public void ConfigureContextOptions(DbContextOptionsBuilder optionsBuilders, string dataFolder, string connectionString) {
            optionsBuilders.UseSqlServer(connectionString);
        }
    }
}
