﻿using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Data;
using Orchard.Data.Providers;
using Orchard.Data.Providers.SqlCeProvider;
using Orchard.Environment.ShellBuilders.Models;

namespace Orchard.Tests {
    public static class DataUtility {
        public static ISessionFactoryHolder CreateSessionFactory(string fileName, params Type[] types) {

            //var persistenceModel = AutoMap.Source(new Types(types))
            //    .Alterations(alt => AddAlterations(alt, types))
            //    .Conventions.AddFromAssemblyOf<DataModule>();
            var persistenceModel = AbstractDataServicesProvider.CreatePersistenceModel(types.Select(t => new RecordBlueprint { TableName = "Test_" + t.Name, Type = t }).ToList());
            var persistenceConfigurer = new SqlServerCompactDataServicesProvider(fileName).GetPersistenceConfigurer(true/*createDatabase*/);
            // Uncomment to display SQL while running tests
            // ((MsSqlCeConfiguration)persistenceConfigurer).ShowSql();

            return Fluently.Configure()
                .Database(persistenceConfigurer)
                .Mappings(m => m.AutoMappings.Add(persistenceModel))
                .ExposeConfiguration(c => {
                    // This is to work around what looks to be an issue in the NHibernate driver:
                    // When inserting a row with IDENTITY column, the "SELET @@IDENTITY" statement
                    // is issued as a separate command. By default, it is also issued in a separate
                    // connection, which is not supported (returns NULL).
                    c.SetProperty("connection.release_mode", "on_close");
                    new SchemaExport(c).Create(false /*script*/, true /*export*/);
                })
                .BuildSessionFactory();
        }

        private static void AddAlterations(AutoMappingAlterationCollection alterations, IEnumerable<Type> types) {
            foreach (var assembly in types.Select(t => t.Assembly).Distinct()) {
                alterations.Add(new AutoMappingOverrideAlteration(assembly));
                alterations.AddFromAssembly(assembly);
            }
            alterations.AddFromAssemblyOf<DataModule>();
        }

        public static ISessionFactoryHolder CreateSessionFactory(params Type[] types) {
            return CreateSessionFactory(
                string.Join(".", types.Reverse().Select(type => type.FullName)),
                types);
        }
    }
}