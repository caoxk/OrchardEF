using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Orchard.Data.Providers.InMemoryProvider {
    public class InMemoryDataServicesProvider : AbstractDataServicesProvider {
        public static string ProviderName {
            get { return "InMemory"; }
        }

        public override void ConfigureContextOptions(DbContextOptionsBuilder optionsBuilders, string connectionString) {
            optionsBuilders.UseInMemoryDatabase();
        }
    }
}

