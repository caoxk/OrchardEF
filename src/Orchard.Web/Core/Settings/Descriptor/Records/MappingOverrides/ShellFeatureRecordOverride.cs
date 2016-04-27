using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;

namespace Orchard.Core.Settings.Descriptor.Records.MappingOverrides {
    public class ShellFeatureRecordOverride : IEntityTypeOverride<ShellFeatureRecord> {
        public void Override(EntityTypeBuilder<ShellFeatureRecord> mapping, ModelBuilder modelBuilder) {
            mapping.ToTable("Settings_ShellFeatureRecord");
            mapping.HasKey(x => x.Id);
        }
    }
}