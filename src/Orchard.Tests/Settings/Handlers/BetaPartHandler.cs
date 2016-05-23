using Orchard.Settings.Handlers;
using Orchard.Tests.Settings.Models;

namespace Orchard.Tests.Settings.Handlers {
    public class BetaPartHandler : ContentHandler {
        public BetaPartHandler() {
            Filters.Add(new ActivatingFilter<BetaPart>("beta"));
        }
    }
}
