using Orchard.Data;
using Orchard.DocumentManagement.Handlers;
using Orchard.Tests.Settings.Models;
using Orchard.Tests.Settings.Records;

namespace Orchard.Tests.Settings.Handlers {
    public class DeltaPartHandler : DocumentHandler {
        public DeltaPartHandler(IRepository<DeltaRecord> repository) {
            Filters.Add(new ActivatingFilter<DeltaPart>("delta"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
