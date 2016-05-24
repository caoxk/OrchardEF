using Orchard.Data;
using Orchard.DocumentManagement.Handlers;
using Orchard.Tests.Settings.Models;
using Orchard.Tests.Settings.Records;

namespace Orchard.Tests.Settings.Handlers {
    public class LambdaPartHandler : DocumentHandler {
        public LambdaPartHandler(IRepository<LambdaRecord> repository) {
            Filters.Add(new ActivatingFilter<LambdaPart>("lambda"));
            Filters.Add(StorageFilter.For(repository));

        }
    }
}
