using Orchard.Data.Migration;

namespace Orchard.Settings.DataMigrations {
    public class SettingsDataMigration : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.Create.Table("Orchard_Framework_ContentItemRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Data").AsString(int.MaxValue).Nullable()
                .WithColumn("ContentType").AsString(255);

            return 1;
        }
    }
}