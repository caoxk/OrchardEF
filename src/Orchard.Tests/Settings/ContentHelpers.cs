using Orchard.DocumentManagement;
using Orchard.DocumentManagement.FieldStorage.InfosetStorage;
using Orchard.DocumentManagement.Records;
using Orchard.Settings;

namespace Orchard.Tests.Settings {
    public class ContentHelpers {
        public static DocumentItem PreparePart<TPart, TRecord>(TPart part, string contentType, int id = -1)
            where TPart : ContentPart<TRecord>
            where TRecord : new() {

            part.Record = new TRecord();
            return PreparePart(part, contentType, id);
        }

        public static DocumentItem PreparePart<TPart>(TPart part, string contentType, int id = -1)
            where TPart : DocumentPart {

            var contentItem = part.ContentItem = new DocumentItem {
                Record = new ContentItemRecord()
                //VersionRecord = new ContentItemVersionRecord {
                //    ContentItemRecord = new ContentItemRecord()
                //},
                //ContentType = contentType
            };
            contentItem.Record.Id = id;
            contentItem.Weld(part);
            contentItem.Weld(new InfosetPart());
            return contentItem;
        }
    }
}
