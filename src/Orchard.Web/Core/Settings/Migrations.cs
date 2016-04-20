using Orchard.Data.Migration;

namespace Orchard.Core.Settings {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            
            SchemaBuilder.CreateTable("ShellDescriptorRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("SerialNumber")
                );

            SchemaBuilder.CreateTable("ShellFeatureRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name")
                    .Column<int>("ShellDescriptorRecord_id"));

            SchemaBuilder.CreateTable("ShellFeatureStateRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name")
                    .Column<string>("InstallState")
                    .Column<string>("EnableState")
                    .Column<int>("ShellStateRecord_Id")
                );

            SchemaBuilder.CreateTable("ShellParameterRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Component")
                    .Column<string>("Name")
                    .Column<string>("Value")
                    .Column<int>("ShellDescriptorRecord_id")
                );

            SchemaBuilder.CreateTable("ShellStateRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Unused")
                );

            return 4;
        }

        public int UpdateFrom1() {
            SchemaBuilder.CreateTable("SiteSettingsPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("BaseUrl", c => c.Unlimited())
                );

            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder.AlterTable("SiteSettingsPartRecord",
                table => table
                    .AddColumn<string>("SiteTimeZone")
                );

            return 3;
        }

        public int UpdateFrom3() {

            return 4;
        }

        public int UpdateFrom4() {
            // TODO: Orchard creates dupes in this table so no can do for now.
            //SchemaBuilder.AlterTable("ShellFeatureRecord",
            //    table => table.AddUniqueConstraint("UC_SFR_SDRId_Name", "ShellDescriptorRecord_id", "Name"));
            SchemaBuilder.AlterTable("ShellFeatureStateRecord",
                table => table.AddUniqueConstraint("UC_SFSR_SSRId_Name", "ShellStateRecord_Id", "Name"));
            SchemaBuilder.AlterTable("ShellParameterRecord",
                table => table.AddUniqueConstraint("UC_SPR_SDRId_Component_Name", "ShellDescriptorRecord_id", "Component", "Name"));

            return 5;
        }
    }
}