using Orchard.DocumentManagement.Handlers;

namespace Orchard.Tests.Settings.Models {
    public class StyledHandler : DocumentHandler {
        public StyledHandler() {
            OnGetDisplayShape<StyledPart>((ctx, part) => ctx.Shape.Zones["Main"].Add(part, "10"));
        }

        protected override void Activating(ActivatingDocumentContext context) {
            if (context.ContentType == "alpha") {
                context.Builder.Weld<StyledPart>();
            }
        }
    }
}
