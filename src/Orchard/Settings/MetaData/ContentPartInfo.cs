using System;

namespace Orchard.Settings.MetaData {
    public class ContentPartInfo {
        public string PartName { get; set; }
        public Func<ContentPart> Factory { get; set; }
    }
}
