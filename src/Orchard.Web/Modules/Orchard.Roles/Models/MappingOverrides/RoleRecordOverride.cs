using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;

namespace Orchard.Roles.Models.MappingOverrides {
    public class RoleRecordOverride : IEntityTypeOverride<RoleRecord> {
        public void Override(EntityTypeBuilder<RoleRecord> mapping) {
            mapping.HasKey(x => x.Id);
            mapping.HasMany(x => x.RolesPermissions).WithOne(x => x.Role).HasForeignKey("RoleRecord_Id");
        }
    }
}