using Orchard.Settings.Handlers;

namespace Orchard.Settings.FieldStorage.InfosetStorage {
    public class InfosetHandler : ContentHandlerBase {
        public override void Activating(ActivatingContentContext context) {
            context.Builder.Weld<InfosetPart>();
        }

        public override void Creating(CreateContentContext context) {
            var infosetPart = context.ContentItem.As<InfosetPart>();
            if (infosetPart != null) {
                context.ContentItemRecord.Data = infosetPart.Infoset.Data;

                infosetPart.Infoset = context.ContentItemRecord.Infoset;
            }
        }
        public override void Loading(LoadContentContext context) {
            var infosetPart = context.ContentItem.As<InfosetPart>();
            if (infosetPart != null) {
                infosetPart.Infoset = context.ContentItemRecord.Infoset;
            }
        }
    }
}