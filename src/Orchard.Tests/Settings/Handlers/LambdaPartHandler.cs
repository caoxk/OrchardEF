using Orchard.Data;
using Orchard.Settings.Handlers;
using Orchard.Tests.Settings.Models;
using Orchard.Tests.Settings.Records;

namespace Orchard.Tests.Settings.Handlers {
    public class LambdaPartHandler : ContentHandler {
        public LambdaPartHandler(IRepository<LambdaRecord> repository) {
            Filters.Add(new ActivatingFilter<LambdaPart>("lambda"));
            Filters.Add(StorageFilter.For(repository));

        }
    }
}
