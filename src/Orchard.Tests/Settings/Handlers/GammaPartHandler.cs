using Orchard.Data;
using Orchard.DocumentManagement.Handlers;
using Orchard.Tests.Settings.Models;
using Orchard.Tests.Settings.Records;

namespace Orchard.Tests.Settings.Handlers {
    class GammaPartHandler : DocumentHandler {
        public GammaPartHandler(IRepository<GammaRecord> repository) {
            Filters.Add(new ActivatingFilter<GammaPart>("gamma"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
