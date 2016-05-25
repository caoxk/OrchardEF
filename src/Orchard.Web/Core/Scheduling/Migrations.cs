using System;
using Orchard.Data.Migration;

namespace Orchard.Core.Scheduling {
    public class Migrations : DataMigrationImpl {

        public int Create()
        {

            SchemaBuilder.Create
                .Table("ScheduledTaskRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("TaskType").AsString()
                .WithColumn("ScheduledUtc").AsDateTime();

            return 1;
        }
    }
}