using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;

namespace Orchard.Core.Settings.Descriptor.Records.MappingOverrides {
    public class ShellDescriptorRecordOverride : IEntityTypeOverride<ShellDescriptorRecord> {
        public void Override(EntityTypeBuilder<ShellDescriptorRecord> mapping) {
            mapping.HasKey(x => x.Id);
            mapping.HasMany(x => x.Features)
                .WithOne(x => x.ShellDescriptorRecord)
                .HasForeignKey("ShellDescriptorRecord_Id");

            mapping.HasMany(x=>x.Parameters)
                .WithOne(x=>x.ShellDescriptorRecord)
                .HasForeignKey("ShellDescriptorRecord_Id");
        }
    }
}