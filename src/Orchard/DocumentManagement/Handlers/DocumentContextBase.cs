using Orchard.DocumentManagement.Records;
using Orchard.Logging;

namespace Orchard.DocumentManagement.Handlers {
    public class DocumentContextBase {
        protected DocumentContextBase (DocumentItem contentItem) {
            ContentItem = contentItem;
            Id = contentItem.Id;
            ContentItemRecord = contentItem.Record;
            ContentManager = contentItem.ContentManager;
        }

        public int Id { get; private set; }
        public DocumentItem ContentItem { get; private set; }
        public ContentItemRecord ContentItemRecord { get; private set; }
        public IDocumentManager ContentManager { get; private set; }
        public ILogger Logger { get; set; }
    }
}