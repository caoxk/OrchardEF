
namespace Orchard.Settings.Handlers {
    public class ActivatingContentContext {
        public string ContentType { get; set; }
        //public ContentTypeDefinition Definition { get; set; }
        public ContentItemBuilder Builder { get; set; }
    }
}
