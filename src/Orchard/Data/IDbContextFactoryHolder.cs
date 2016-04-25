using Microsoft.Data.Entity;
using System.Collections.Generic;
using Orchard.Environment.Configuration;
using Orchard.Data.Providers;
using Orchard.FileSystems.AppData;

namespace Orchard.Data {
    public interface IDbContextFactoryHolder: IDependency {
        void Configure(DbContextOptionsBuilder optionsBuilder);
    }

    public class DbContextFactoryHolder : IDbContextFactoryHolder {
        private readonly ShellSettings _shellSettings;
        private readonly IDataServicesProvider _dataServicesProvider;
        private readonly IAppDataFolder _appDataFolder;

        public DbContextFactoryHolder(
            ShellSettings shellSettings,
            IDataServicesProvider dataServicesProvider,
            IAppDataFolder appDataFolder) {
            _shellSettings = shellSettings;
            _dataServicesProvider = dataServicesProvider;
            _appDataFolder = appDataFolder;
        }

        public void Configure(DbContextOptionsBuilder optionsBuilders) {
            var shellPath = _appDataFolder.Combine("Sites", _shellSettings.Name);
            _appDataFolder.CreateDirectory(shellPath);

            var shellFolder = _appDataFolder.MapPath(shellPath);
            _dataServicesProvider.ConfigureContextOptions(optionsBuilders, _shellSettings.DataConnectionString);
            //foreach (var provider in _dataServicesProviders) {
            //    provider.ConfigureContextOptions(optionsBuilders, _shellSettings.DataConnectionString);
            //}
        }
    }
}