using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;

namespace Orchard.Core.Settings.Descriptor.Records.MappingOverrides {
    public class ShellParameterRecordOverride : IEntityTypeOverride<ShellParameterRecord> {
        public void Override(EntityTypeBuilder<ShellParameterRecord> mapping, ModelBuilder modelBuilder) {
            mapping.ToTable("Settings_ShellFeatureStateRecord");
            mapping.HasKey(x => x.Id);
        }
    }
}