using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace Orchard.Core.Settings.Descriptor.Records {
    public class ShellDescriptorRecord {
        public ShellDescriptorRecord() {
            Features=new List<ShellFeatureRecord>();
            Parameters=new List<ShellParameterRecord>();
        }

        public virtual int Id { get; set; }
        public virtual int SerialNumber { get; set; }
        
        public virtual IList<ShellFeatureRecord> Features { get; set; }
        
        public virtual IList<ShellParameterRecord> Parameters { get; set; }
    }

    public class ShellDescriptorRecordConfiguration : EntityTypeConfiguration<ShellDescriptorRecord> {
        public ShellDescriptorRecordConfiguration()
        {
            HasMany(x => x.Features).WithRequired(x=>x.ShellDescriptorRecord).WillCascadeOnDelete();
            HasMany(x => x.Parameters).WithRequired(x=>x.ShellDescriptorRecord).WillCascadeOnDelete();
        }
    }
}
