using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Orchard.Data.Providers;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.FileSystems.AppData;
using Orchard.Localization;
using Orchard.Logging;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using MySql.Data.Entity;

namespace Orchard.Data
{
    public interface ISessionFactoryHolder : ISingletonDependency {
        DbContext Create();
        //ISessionFactory GetSessionFactory();
        //DbContextOptions<TContext> GetConfiguration<TContext>() where TContext:DbContext;
        //SessionFactoryParameters GetSessionFactoryParameters();
    }

    public class SessionFactoryHolder : ISessionFactoryHolder
    {
        private readonly ShellSettings _shellSettings;
        private readonly ShellBlueprint _shellBlueprint;
        private readonly IAppDataFolder _appDataFolder;
        private readonly IDataServicesProviderFactory _dataServicesProviderFactory;
        //private readonly IEntityTypeOverrideHandler _entityTypeOverrideHandler;

        private DbContextOptions _contextOptions;

        public SessionFactoryHolder(
            ShellSettings shellSettings,
            ShellBlueprint shellBlueprint,
            IAppDataFolder appDataFolder, 
            IDataServicesProviderFactory dataServicesProviderFactory)
        {
            _shellSettings = shellSettings;
            _shellBlueprint = shellBlueprint;
            _appDataFolder = appDataFolder;
            _dataServicesProviderFactory = dataServicesProviderFactory;
            //_entityTypeOverrideHandler = entityTypeOverrideHandler;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public DbContext Create() {
            if (_contextOptions == null) {
                var parameters = GetSessionFactoryParameters();
                var provider = _dataServicesProviderFactory.CreateProvider(parameters);
                DbConfiguration configuration = provider.BuildConfiguration();
                DbConfiguration.SetConfiguration(configuration);
                Database.SetInitializer<DbContext>(null);
                _contextOptions = provider.GetContextOptions(parameters);
            }
            return new DbContext(_contextOptions.ConnectionString, _contextOptions.Model);
        }

        public SessionFactoryParameters GetSessionFactoryParameters()
        {
            var shellPath = _appDataFolder.Combine("Sites", _shellSettings.Name);
            _appDataFolder.CreateDirectory(shellPath);

            var shellFolder = _appDataFolder.MapPath(shellPath);

            return new SessionFactoryParameters
            {
                Provider = _shellSettings.DataProvider,
                DataFolder = shellFolder,
                ConnectionString = _shellSettings.DataConnectionString,
                RecordDescriptors = _shellBlueprint.Records,
            };
        }
    }
}
