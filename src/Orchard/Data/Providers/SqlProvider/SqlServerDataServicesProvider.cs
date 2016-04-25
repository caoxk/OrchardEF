using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Orchard.Data.Providers.SqlProvider {
    public class SqlServerDataServicesProvider : AbstractDataServicesProvider {
        public static string ProviderName {
            get { return "SqlServer"; }
        }

        public override void ConfigureContextOptions(DbContextOptionsBuilder optionsBuilders, string connectionString) {
            optionsBuilders.UseSqlServer(connectionString);
        }
    }
}
