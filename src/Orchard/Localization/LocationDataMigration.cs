using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Data.Migration;

namespace Orchard.Localization
{
    public class LocationDataMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("CultureRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Culture")
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder
                .AlterTable("CultureRecord", table =>
                    table.AddUniqueConstraint("UC_CR_Name", "Culture"));

            return 2;
        }
    }
}
