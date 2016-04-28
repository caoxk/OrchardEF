using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Builders;
using Orchard.Data.Alterations;

namespace Orchard.Localization.Records
{
    public class CultureRecordOverride : IEntityTypeOverride<CultureRecord> {
        public void Override(EntityTypeBuilder<CultureRecord> mapping, ModelBuilder modelBuilder) {
            mapping.ToTable("Orchard_Framework_CultureRecord");
            mapping.HasKey(x => x.Id);
        }
    }
}