using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;

namespace Orchard.Roles.Models.MappingOverrides {
    public class RolesPermissionsRecordOverride : IEntityTypeOverride<RolesPermissionsRecord> {
        public void Override(EntityTypeBuilder<RolesPermissionsRecord> mapping) {
            mapping.HasKey(x => x.Id);
            mapping.HasOne(x => x.Permission).WithMany().HasForeignKey("Permission_Id");
        }
    }
}