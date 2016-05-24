
namespace Orchard.DocumentManagement.Handlers {
    public class ActivatingDocumentContext {
        public string ContentType { get; set; }
        //public ContentTypeDefinition Definition { get; set; }
        public DocumentItemBuilder Builder { get; set; }
    }
}
