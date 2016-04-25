using Autofac;
using Autofac.Core;
using Microsoft.Data.Entity;

namespace Orchard.Data.Providers {
    public interface IDataServicesProvider {
        void ConfigureContextOptions(DbContextOptionsBuilder optionsBuilders, string dataFolder, string connectionString);
    }
}
