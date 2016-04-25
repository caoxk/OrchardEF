using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;

namespace Orchard.Users.Models.MappingOverrides {
    public class UserPartRecordOverride : IEntityTypeOverride<UserPartRecord> {
        public void Override(EntityTypeBuilder<UserPartRecord> mapping) {
            mapping.HasKey(x => x.Id);
        }
    }
}