using Orchard.Data.Migration;

namespace Orchard.DocumentManagement.DataMigrations {
    public class DocumentDataMigration : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.Create.Table("Orchard_Framework_DocumentItemRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Data").AsString(int.MaxValue).Nullable()
                .WithColumn("ContentType").AsString(255);

            return 1;
        }
    }
}