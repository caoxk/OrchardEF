using Orchard.Settings.Handlers;
using Orchard.Tests.Settings.Models;

namespace Orchard.Tests.Settings.Handlers {
    public class FlavoredPartHandler : ContentHandler {
        public FlavoredPartHandler() {
            Filters.Add(new ActivatingFilter<FlavoredPart>("alpha"));
            Filters.Add(new ActivatingFilter<FlavoredPart>("beta"));

            OnGetDisplayShape<FlavoredPart>((ctx, part) => ctx.Shape.Zones["Main"].Add(part));
        }
    }
}
