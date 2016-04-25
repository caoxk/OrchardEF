using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Orchard.Data.Providers.InMemoryProvider {
    public class InMemoryDataServicesProvider : IDataServicesProvider {
        public static string ProviderName {
            get { return "InMemory"; }
        }

        public void ConfigureContextOptions(DbContextOptionsBuilder optionsBuilders, string dataFolder, string connectionString) {
            optionsBuilders.UseInMemoryDatabase();
        }
    }
}

