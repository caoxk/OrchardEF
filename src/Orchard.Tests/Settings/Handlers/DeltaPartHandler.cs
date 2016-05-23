using Orchard.Data;
using Orchard.Settings.Handlers;
using Orchard.Tests.Settings.Models;
using Orchard.Tests.Settings.Records;

namespace Orchard.Tests.Settings.Handlers {
    public class DeltaPartHandler : ContentHandler {
        public DeltaPartHandler(IRepository<DeltaRecord> repository) {
            Filters.Add(new ActivatingFilter<DeltaPart>("delta"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
