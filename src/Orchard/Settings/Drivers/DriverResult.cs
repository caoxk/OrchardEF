using Orchard.Settings.Handlers;

namespace Orchard.Settings.Drivers {
    public class DriverResult {
        public virtual void Apply(BuildDisplayContext context) { }
        public virtual void Apply(BuildEditorContext context) { }
        
        public ContentPart ContentPart { get; set; }
    }
}
