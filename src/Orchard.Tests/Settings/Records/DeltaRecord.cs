using Orchard.Settings.Records;

namespace Orchard.Tests.Settings.Records {
    public class DeltaRecord : ContentPartRecord {
        public virtual string Quux { get; set; }
    }
}