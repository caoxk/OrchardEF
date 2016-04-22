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
        public int Create() {
            SchemaBuilder.Create
                .Table("Orchard_Framework_CultureRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Culture").AsString();

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.Create
                .UniqueConstraint("UC_CR_Name").OnTable("Orchard_Framework_CultureRecord").Column("Culture");

            return 2;
        }
    }
}
