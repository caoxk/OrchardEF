using Orchard.DocumentManagement.Handlers;

namespace Orchard.DocumentManagement.Drivers {
    public class DriverResult {
        public virtual void Apply(BuildDisplayContext context) { }
        public virtual void Apply(BuildEditorContext context) { }
        
        public DocumentPart ContentPart { get; set; }
    }
}
