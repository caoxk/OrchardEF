using Orchard.Data.Migration;

namespace Orchard.Core.Settings {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.Create
                .Table("Settings_ShellDescriptorRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("SerialNumber").AsInt32();

            SchemaBuilder.Create
                .Table("Settings_ShellFeatureRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString()
                .WithColumn("ShellDescriptorRecord_Id").AsInt32().Nullable();

            SchemaBuilder.Create
                .Table("Settings_ShellFeatureStateRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString()
                .WithColumn("InstallState").AsString()
                .WithColumn("EnableState").AsString()
                .WithColumn("ShellStateRecord_Id").AsInt32().Nullable();

            SchemaBuilder.Create
                .Table("Settings_ShellParameterRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Component").AsString()
                .WithColumn("Name").AsString()
                .WithColumn("Value").AsString().Nullable()
                .WithColumn("ShellDescriptorRecord_id").AsInt32().Nullable();

            SchemaBuilder.Create
                .Table("Settings_ShellStateRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Unused").AsString().Nullable();

            return 4;
        }

        public int UpdateFrom4() {
            // TODO: Orchard creates dupes in this table so no can do for now.
            //SchemaBuilder.AlterTable("ShellFeatureRecord",
            //    table => table.AddUniqueConstraint("UC_SFR_SDRId_Name", "ShellDescriptorRecord_id", "Name"));
            SchemaBuilder.Create.UniqueConstraint("UC_SFSR_SSRId_Name").OnTable("Settings_ShellFeatureStateRecord").Columns("ShellStateRecord_Id", "Name");
            SchemaBuilder.Create.UniqueConstraint("UC_SPR_SDRId_Component_Name")
                .OnTable("Settings_ShellParameterRecord")
                .Columns("ShellDescriptorRecord_id", "Component", "Name");
            return 5;
        }
    }
}