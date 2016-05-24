using System;

namespace Orchard.DocumentManagement.MetaData {
    public class ContentPartInfo {
        public string PartName { get; set; }
        public Func<DocumentPart> Factory { get; set; }
    }
}
