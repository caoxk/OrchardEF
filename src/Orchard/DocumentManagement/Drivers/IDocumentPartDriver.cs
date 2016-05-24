using System.Collections.Generic;
using Orchard.DocumentManagement.Handlers;
using Orchard.DocumentManagement.MetaData;

namespace Orchard.DocumentManagement.Drivers {
    public interface IDocumentPartDriver : IDependency {
        DriverResult BuildDisplay(BuildDisplayContext context);
        DriverResult BuildEditor(BuildEditorContext context);
        DriverResult UpdateEditor(UpdateEditorContext context);
        IEnumerable<ContentPartInfo> GetPartInfo();
        void GetContentItemMetadata(GetDocumentItemMetadataContext context);
    }
}