using System;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Orchard.Environment.Configuration;

namespace Orchard.Tasks.Locking.Services {
    public class DistributedLockSchemaBuilder {
        private readonly IMigrationExecutor _migrationExecutor;
        private readonly ShellSettings _shellSettings;
        private const string TableName = "Orchard_Framework_DistributedLockRecord";

        public DistributedLockSchemaBuilder(ShellSettings shellSettings, 
            IMigrationExecutor migrationExecutor) {
            _shellSettings = shellSettings;
            _migrationExecutor = migrationExecutor;
        }

        public bool EnsureSchema() {
            if (SchemaExists())
                return false;

            CreateSchema();
            return true;
        }

        public void CreateSchema() {
            _migrationExecutor.ExecuteMigration(builder=> {
                builder.Create.Table(TableName)
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("Name").AsString(512).NotNullable().Unique()
                    .WithColumn("MachineName").AsString(256)
                    .WithColumn("CreatedUtc").AsDateTime()
                    .WithColumn("ValidUntilUtc").AsDateTime().Nullable();

                builder.Create.Index("IDX_DistributedLockRecord_Name")
                    .OnTable(TableName).OnColumn("Name");
            });
        }

        public bool SchemaExists() {
            try {
                var tablePrefix = String.IsNullOrEmpty(_shellSettings.DataTablePrefix) ? "" : _shellSettings.DataTablePrefix + "_";

                return _migrationExecutor.TableExists(null, TableName);
            }
            catch {
                return false;
            }
        }
    }
}