using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;

namespace Orchard.Core.Settings.State.Records.MappingOverrides {
    public class ShellStateRecordOverride : IEntityTypeOverride<ShellStateRecord> {
        public void Override(EntityTypeBuilder<ShellStateRecord> mapping, ModelBuilder modelBuilder) {
            mapping.ToTable("Settings_ShellStateRecord");
            mapping.HasKey(x => x.Id);
            modelBuilder.Entity<ShellFeatureStateRecord>().Property<int>("ShellStateRecord_Id");
            mapping.HasMany(x => x.Features)
                .WithOne()
                .HasForeignKey("ShellStateRecord_Id").IsRequired();
        }
    }
}