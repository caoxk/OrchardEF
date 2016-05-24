using Orchard.DocumentManagement.Records;

namespace Orchard.Tests.Settings.Records {
    public class DeltaRecord : DocumentPartRecord {
        public virtual string Quux { get; set; }
    }
}