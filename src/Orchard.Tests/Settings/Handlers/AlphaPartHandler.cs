using Orchard.Settings.Handlers;
using Orchard.Tests.Settings.Models;

namespace Orchard.Tests.Settings.Handlers {
    public class AlphaPartHandler : ContentHandler {
        public AlphaPartHandler() {
            Filters.Add(new ActivatingFilter<AlphaPart>("alpha"));

            OnGetDisplayShape<AlphaPart>((ctx, part) => ctx.Shape.Zones["Main"].Add(part, "3"));
        }
    }
}
