using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
}
