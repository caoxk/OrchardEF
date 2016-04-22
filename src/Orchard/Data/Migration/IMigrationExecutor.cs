using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Expressions;
using Orchard.Data.Migration.Announcers;
using Orchard.Data.Migration.Processors;
using Orchard.Data.Migration.Schema;
using Orchard.Environment.Configuration;

namespace Orchard.Data.Migration
{
    public interface IMigrationExecutor: IDependency {
        void ExecuteMigration(Action<SchemaBuilder> migraitonAction);
        int ExecuteMigrationMethod(MethodInfo method, IDataMigration migration);
        bool TableExists(string schemaName, string tableName);
    }

    public class DefaultMigrationExecutor : IMigrationExecutor {
        private readonly IMigrationProcessorFactoryProvider _migrationProcessorFactoryProvider;
        private readonly ShellSettings _shellSettings;
        public DefaultMigrationExecutor(IMigrationProcessorFactoryProvider migrationProcessorFactoryProvider, 
            ShellSettings shellSettings) {
            _migrationProcessorFactoryProvider = migrationProcessorFactoryProvider;
            _shellSettings = shellSettings;
        }

        public void ExecuteMigration(Action<SchemaBuilder> migraitonAction)
        {
            var context = new MigrationContext();
            var builder = new SchemaBuilder {_context = context};
            migraitonAction(builder);

            var processorFactory = _migrationProcessorFactoryProvider.GetFactory(_shellSettings.DataProvider);
            var announcer = new TextWriterAnnouncer(s => System.Diagnostics.Debug.WriteLine(s));
            var options = new MigrationOptions { PreviewOnly = false, Timeout = 60 };
            var processor = processorFactory.Create(announcer, options);

            foreach (IMigrationExpression expression in context.Expressions)
            {
                expression.ExecuteWith(processor);
            }
        }

        public bool TableExists(string schemaName, string tableName) {
            var processorFactory = _migrationProcessorFactoryProvider.GetFactory(_shellSettings.DataProvider);
            var announcer = new TextWriterAnnouncer(s => System.Diagnostics.Debug.WriteLine(s));
            var options = new MigrationOptions { PreviewOnly = false, Timeout = 60 };
            var processor = (ProcessorBase)processorFactory.Create(announcer, options);
            return processor.TableExists(schemaName, tableName);
        }

        public int ExecuteMigrationMethod(MethodInfo method, IDataMigration migration)
        {
            var context = new MigrationContext();
            migration.SchemaBuilder._context = context;
            var current = (int)method.Invoke(migration, new object[0]);

            var processorFactory = _migrationProcessorFactoryProvider.GetFactory(_shellSettings.DataProvider);
            var announcer = new TextWriterAnnouncer(s => System.Diagnostics.Debug.WriteLine(s));
            var options = new MigrationOptions { PreviewOnly = false, Timeout = 60 };
            var processor = processorFactory.Create(announcer, options);

            foreach (IMigrationExpression expression in context.Expressions)
            {
                expression.ExecuteWith(processor);
            }
            return current;
        }

        public class MigrationOptions : IMigrationProcessorOptions
        {
            public bool PreviewOnly { get; set; }
            public string ProviderSwitches { get; set; }
            public int Timeout { get; set; }
        }
    }
}
