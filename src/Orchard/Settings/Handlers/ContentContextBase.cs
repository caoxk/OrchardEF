using Orchard.ContentManagement.Records;
using Orchard.Logging;

namespace Orchard.ContentManagement.Handlers {
    public class ContentContextBase {
        protected ContentContextBase (ContentItem contentItem) {
            ContentItem = contentItem;
            Id = contentItem.Id;
            ContentItemRecord = contentItem.Record;
            ContentManager = contentItem.ContentManager;
        }

        public int Id { get; private set; }
        public ContentItem ContentItem { get; private set; }
        public ContentItemRecord ContentItemRecord { get; private set; }
        public IContentManager ContentManager { get; private set; }
        public ILogger Logger { get; set; }
    }
}