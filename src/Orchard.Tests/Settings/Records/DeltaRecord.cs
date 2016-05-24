using Orchard.DocumentManagement.Records;

namespace Orchard.Tests.Settings.Records {
    public class DeltaRecord : ContentPartRecord {
        public virtual string Quux { get; set; }
    }
}