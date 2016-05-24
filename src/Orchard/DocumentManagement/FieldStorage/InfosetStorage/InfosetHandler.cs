using Orchard.DocumentManagement.Handlers;

namespace Orchard.DocumentManagement.FieldStorage.InfosetStorage {
    public class InfosetHandler : DocumentHandlerBase {
        public override void Activating(ActivatingDocumentContext context) {
            context.Builder.Weld<InfosetPart>();
        }

        public override void Creating(CreateDocumentContext context) {
            var infosetPart = context.ContentItem.As<InfosetPart>();
            if (infosetPart != null) {
                context.ContentItemRecord.Data = infosetPart.Infoset.Data;

                infosetPart.Infoset = context.ContentItemRecord.Infoset;
            }
        }
        public override void Loading(LoadDocumentContext context) {
            var infosetPart = context.ContentItem.As<InfosetPart>();
            if (infosetPart != null) {
                infosetPart.Infoset = context.ContentItemRecord.Infoset;
            }
        }
    }
}