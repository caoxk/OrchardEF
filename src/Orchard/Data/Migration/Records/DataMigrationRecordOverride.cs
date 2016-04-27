using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;

namespace Orchard.Data.Migration.Records {
    public class DataMigrationRecordOverride : IEntityTypeOverride<DataMigrationRecord> {
        public void Override(EntityTypeBuilder<DataMigrationRecord> mapping, ModelBuilder modelBuilder) {
            mapping.ToTable("Orchard_Framework_DataMigrationRecord");
            mapping.HasKey(x => x.Id);
        }
    }
}