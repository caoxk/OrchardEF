using Orchard.Data;
using Orchard.Settings.Handlers;
using Orchard.Tests.Settings.Models;
using Orchard.Tests.Settings.Records;

namespace Orchard.Tests.Settings.Handlers {
    class GammaPartHandler : ContentHandler {
        public GammaPartHandler(IRepository<GammaRecord> repository) {
            Filters.Add(new ActivatingFilter<GammaPart>("gamma"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
