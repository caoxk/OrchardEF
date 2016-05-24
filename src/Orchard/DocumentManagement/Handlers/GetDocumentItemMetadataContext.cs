using Orchard.Logging;

namespace Orchard.DocumentManagement.Handlers {
    public class GetDocumentItemMetadataContext {
        public DocumentItem ContentItem { get; set; }
        public DocumentItemMetadata Metadata { get; set; }
        public ILogger Logger { get; set; }
    }
}