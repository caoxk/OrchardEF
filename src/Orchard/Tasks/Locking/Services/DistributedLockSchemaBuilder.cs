using System;
using Orchard.Data.Migration.Schema;
using Orchard.Environment.Configuration;

namespace Orchard.Tasks.Locking.Services {
    public class DistributedLockSchemaBuilder {
        private readonly ShellSettings _shellSettings;
        private readonly SchemaBuilder _schemaBuilder;
        private const string TableName = "Orchard_Framework_DistributedLockRecord";

        public DistributedLockSchemaBuilder(ShellSettings shellSettings, SchemaBuilder schemaBuilder) {
            _shellSettings = shellSettings;
            _schemaBuilder = schemaBuilder;
        }

        public bool EnsureSchema() {
            if (SchemaExists())
                return false;

            CreateSchema();
            return true;
        }

        public void CreateSchema() {
            _schemaBuilder.Create.Table(TableName)
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString(512).NotNullable().Unique()
                .WithColumn("MachineName").AsString(256)
                .WithColumn("CreatedUtc").AsDateTime()
                .WithColumn("ValidUntilUtc").AsDateTime().Nullable();

            _schemaBuilder.Create.Index("IDX_DistributedLockRecord_Name")
                .OnTable(TableName).OnColumn("Name");
        }

        public bool SchemaExists() {
            try {
                var tablePrefix = String.IsNullOrEmpty(_shellSettings.DataTablePrefix) ? "" : _shellSettings.DataTablePrefix + "_";
                _schemaBuilder.Execute.Sql(string.Format("select * from {0}", TableName));
                return true;
            }
            catch {
                return false;
            }
        }
    }
}