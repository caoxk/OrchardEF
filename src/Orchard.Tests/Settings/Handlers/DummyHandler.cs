using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Settings.Handlers;

namespace Orchard.Tests.ContentManagement.Handlers {
    public class DummyHandler : ContentHandler {
        public DummyHandler() {
            Filters.Add(new ActivatingFilter<IdentityPart>("Dummy"));
            Filters.Add(new ActivatingFilter<TitlePart>("Dummy"));
        }
    }
}
