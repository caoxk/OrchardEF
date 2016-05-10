using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Orchard.Data;
using Orchard.Data.Providers;
using Orchard.Data.Providers.SqlCeProvider;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.Tests.Stubs;

namespace Orchard.Tests {
    public static class DataUtility {
        public static ISessionFactoryHolder CreateSessionFactory(string fileName, params Type[] types) {
            var parameters = new SessionFactoryParameters {
                Provider = "SqlServerCe",
                DataFolder = Path.GetDirectoryName(fileName),
                RecordDescriptors = types.Select(t => new RecordBlueprint { TableName = "Test_" + t.Name, Type = t }).ToList()
            };
            var provider = new SqlServerCompactDataServicesProvider(fileName);
            DbConfiguration configuration = provider.BuildConfiguration();
            DbConfiguration.SetConfiguration(configuration);
            //Database.SetInitializer<DbContext>(null);
            var contextOptions = provider.GetContextOptions(parameters);
            // Uncomment to display SQL while running tests
            // ((MsSqlCeConfiguration)persistenceConfigurer).ShowSql();

            var sessionFactory = new Stubs.StubSessionFactoryHolder(() => {
                var session = new DbContext(contextOptions.ConnectionString, contextOptions.Model);
                return session;
            });
            return sessionFactory;
        }

        //private static void AddAlterations(AutoMappingAlterationCollection alterations, IEnumerable<Type> types) {
        //    foreach (var assembly in types.Select(t => t.Assembly).Distinct()) {
        //        alterations.AddFromAssembly(assembly);
        //    }
        //    alterations.AddFromAssemblyOf<DataModule>();
        //}

        public static ISessionFactoryHolder CreateSessionFactory(params Type[] types) {
            return CreateSessionFactory(
                string.Join(".", types.Reverse().Select(type => type.FullName)),
                types);
        }
    }
}