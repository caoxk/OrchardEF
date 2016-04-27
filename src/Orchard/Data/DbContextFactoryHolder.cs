using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Conventions.Internal;
using Microsoft.Data.Entity.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Orchard.Data.Alterations;
using Orchard.Data.Providers;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.FileSystems.AppData;
using Orchard.Localization;
using Orchard.Logging;

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
        private readonly IEntityTypeOverrideHandler _entityTypeOverrideHandler;

        private readonly ConcurrentDictionary<string, IModel> _models = new ConcurrentDictionary<string, IModel>();

        public SessionFactoryHolder(
            ShellSettings shellSettings,
            ShellBlueprint shellBlueprint,
            IAppDataFolder appDataFolder, 
            IDataServicesProviderFactory dataServicesProviderFactory, 
            IEntityTypeOverrideHandler entityTypeOverrideHandler)
        {
            _shellSettings = shellSettings;
            _shellBlueprint = shellBlueprint;
            _appDataFolder = appDataFolder;
            _dataServicesProviderFactory = dataServicesProviderFactory;
            _entityTypeOverrideHandler = entityTypeOverrideHandler;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public DbContext Create()
        {
            var contextTypeName = typeof (DbContext).GetTypeInfo().FullName;
            var compiledModel = _models.GetOrAdd(contextTypeName, t => GetCompiled());

            var optionsBuilder = GetContextOptionsBuilder();

            optionsBuilder.UseModel(compiledModel);
            var options = optionsBuilder.Options;

            return new DbContext(options);
        }

        private DbContextOptionsBuilder<DbContext> GetContextOptionsBuilder() {
            var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

            var parameters = GetSessionFactoryParameters();

            _dataServicesProviderFactory
                .CreateProvider(parameters)
                .ConfigureContextOptions(optionsBuilder, parameters.DataFolder, parameters.ConnectionString);
            return optionsBuilder;
        }

        private IModel GetCompiled()
        {
            var serviceCollection = new ServiceCollection();

            var entityFrameworkBuilder = serviceCollection.AddEntityFramework();
            switch (_shellSettings.DataProvider)
            {
                case "SqlServer":
                    entityFrameworkBuilder.AddSqlServer();
                    break;
                case "SqlServerCe":
                    entityFrameworkBuilder.AddSqlCe();
                    break;
                case "InMemory":
                    entityFrameworkBuilder.AddInMemoryDatabase();
                    break;
            }
            serviceCollection.AddSingleton(_ => GetContextOptionsBuilder());
            serviceCollection.AddSingleton<DbContextOptions>(p => p.GetRequiredService<DbContextOptions<DbContext>>());
            serviceCollection.AddScoped(typeof(DbContext), DbContextActivator.CreateInstance<DbContext>);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var coreConventionSetBuilder = new CoreConventionSetBuilder();
            var conventionSetBuilder = new SqlServerConventionSetBuilder(new SqlServerTypeMapper());
            //var sqlConventionSetBuilder = new SqlServerConventionSetBuilder(new SqlServerTypeMapper());
            var contextServices = serviceProvider.GetService<IDbContextServices>();
            var databaseProviderServices = contextServices.DatabaseProviderServices;
            //var conventionSetBuilder = databaseProviderServices.ConventionSetBuilder;
            var conventionSet = conventionSetBuilder.AddConventions(coreConventionSetBuilder.CreateConventionSet());

            var modelBuilder = new ModelBuilder(conventionSet);

            var entityMethod = modelBuilder.GetType().GetRuntimeMethod("Entity", new Type[0]);

            foreach (var recordDescriptor in _shellBlueprint.Records)
            {
                Logger.Debug("Mapping record {0}", recordDescriptor.Type.FullName);

                entityMethod.MakeGenericMethod(recordDescriptor.Type)
                    .Invoke(modelBuilder, new object[0]);
            }
            _entityTypeOverrideHandler.Alter(modelBuilder);

            return modelBuilder.Model;
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
