using Orchard.Data.Migration;

namespace Orchard.Settings.DataMigrations {
    public class SettingsDataMigration : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.Create.Table("ContentItemRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Data").AsString(int.MaxValue)
                .WithColumn("ContentType").AsString(255);

            return 1;
        }
    }
}