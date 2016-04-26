using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;

namespace Orchard.Core.Settings.State.Records.MappingOverrides {
    public class ShellFeatureStateRecordOverride : IEntityTypeOverride<ShellFeatureStateRecord> {
        public void Override(EntityTypeBuilder<ShellFeatureStateRecord> mapping) {
            mapping.ToTable("Settings_ShellFeatureStateRecord");
            mapping.HasKey(x => x.Id);
            mapping.Property<int>("ShellStateRecord_Id");
        }
    }
}