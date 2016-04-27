using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;
using Microsoft.Data.Entity;

namespace Orchard.Users.Models.MappingOverrides {
    public class UserPartRecordOverride : IEntityTypeOverride<UserPartRecord> {
        public void Override(EntityTypeBuilder<UserPartRecord> mapping, ModelBuilder modelBuilder) {
            mapping.ToTable("Orchard_Users_UserPartRecord");
            mapping.HasKey(x => x.Id);
        }
    }
}