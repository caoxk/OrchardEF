using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;

namespace Orchard.Roles.Models.MappingOverrides
{
    public class PermissionRecordOverride : IEntityTypeOverride<PermissionRecord>
    {
        public void Override(EntityTypeBuilder<PermissionRecord> mapping, ModelBuilder modelBuilder)
        {
            mapping.ToTable("Orchard_Roles_PermissionRecord");
            mapping.HasKey(x => x.Id);
        }
    }
}