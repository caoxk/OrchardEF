using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;

namespace Orchard.Roles.Models.MappingOverrides {
    public class RolesPermissionsRecordOverride : IEntityTypeOverride<RolesPermissionsRecord> {
        public void Override(EntityTypeBuilder<RolesPermissionsRecord> mapping, ModelBuilder modelBuilder) {
            mapping.ToTable("Orchard_Roles_RolesPermissionsRecord");
            mapping.HasKey(x => x.Id);
            mapping.Property<int>("Permission_Id");
            mapping.HasOne(x => x.Permission).WithMany().HasForeignKey("Permission_Id");
        }
    }
}