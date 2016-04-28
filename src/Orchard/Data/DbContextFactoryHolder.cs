﻿using System;
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

        private DbCompiledModel _compiledModel;

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
            var connectionString = _shellSettings.DataConnectionString;
            if (_compiledModel == null) {
                var parameters = GetSessionFactoryParameters();
                var provider = _dataServicesProviderFactory.CreateProvider(parameters);
                DbConfiguration configuration = provider.BuildConfiguration();
                DbConfiguration.SetConfiguration(configuration);
                _compiledModel = GetCompiled(parameters.ConnectionString);
                var options = provider.GetContextOptions(parameters);
                connectionString = options.ConnectionString;
            }
            return new DbContext(connectionString, _compiledModel);
        }

        private DbCompiledModel GetCompiled(string dataConnectionString) {

            var connection = DbConfiguration.DependencyResolver.GetService<IDbConnectionFactory>()
                .CreateConnection(dataConnectionString);

            DbModelBuilder builder = new DbModelBuilder();
            builder.Types().Having()

            var model = builder.Build(connection);
            return model.Compile();

            //var serviceCollection = new ServiceCollection();

            //var entityFrameworkBuilder = serviceCollection.AddEntityFramework();
            //switch (_shellSettings.DataProvider) {
            //    case "SqlServer":
            //        entityFrameworkBuilder.AddSqlServer();
            //        break;
            //    case "SqlServerCe":
            //        entityFrameworkBuilder.AddSqlCe();
            //        break;
            //    case "InMemory":
            //        entityFrameworkBuilder.AddInMemoryDatabase();
            //        break;
            //}
            //serviceCollection.AddSingleton(_ => optionsBuilder.Options);
            //serviceCollection.AddSingleton<DbContextOptions>(p => p.GetRequiredService<DbContextOptions<DbContext>>());
            //serviceCollection.AddScoped(typeof(DbContext), p => DbContextActivator.CreateInstance<DbContext>(p, optionsBuilder.Options));
            //var serviceProvider = serviceCollection.BuildServiceProvider();
            //ConventionSet conventions;
            //using (var dbContext = serviceProvider.GetService<DbContext>()) {
            //    var coreConventionSetBuilder = new CoreConventionSetBuilder();
            //    var contextServices = dbContext.GetService<IDbContextServices>();
            //    var databaseProviderServices = contextServices.DatabaseProviderServices;
            //    var conventionSetBuilder = databaseProviderServices.ConventionSetBuilder;
            //    conventions = conventionSetBuilder.AddConventions(coreConventionSetBuilder.CreateConventionSet());
            //}

            //var modelBuilder = new ModelBuilder(conventions);

            //var entityMethod = modelBuilder.GetType().GetRuntimeMethod("Entity", new Type[0]);

            //foreach (var recordDescriptor in _shellBlueprint.Records) {
            //    Logger.Debug("Mapping record {0}", recordDescriptor.Type.FullName);

            //    entityMethod.MakeGenericMethod(recordDescriptor.Type)
            //        .Invoke(modelBuilder, new object[0]);
            //}
            //_entityTypeOverrideHandler.Alter(modelBuilder);

            //return modelBuilder.Model;

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