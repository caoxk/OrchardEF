using Orchard.DocumentManagement.Handlers;
using Orchard.Tests.Settings.Models;

namespace Orchard.Tests.Settings.Handlers {
    public class BetaPartHandler : DocumentHandler {
        public BetaPartHandler() {
            Filters.Add(new ActivatingFilter<BetaPart>("beta"));
        }
    }
}
