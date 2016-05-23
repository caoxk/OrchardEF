using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Orchard.Settings.Records;

namespace Orchard.Settings {
    public class ContentItem : DynamicObject, IContent {
        public ContentItem() {
            _parts = new List<ContentPart>();
        }

        private readonly IList<ContentPart> _parts;
        public string ContentType { get; set; }
        ContentItem IContent.ContentItem { get { return this; } }

        public dynamic Content { get { return (dynamic)this; } }

        public int Id { get { return Record == null ? 0 : Record.Id; } }

        public ContentItemRecord Record { get; set; }

        public IEnumerable<ContentPart> Parts { get { return _parts; } }

        public IContentManager ContentManager { get; set; }

        public bool Has(Type partType) {
            return partType == typeof(ContentItem) || _parts.Any(partType.IsInstanceOfType);
        }

        public IContent Get(Type partType) {
            if (partType == typeof(ContentItem))
                return this;
            return _parts.FirstOrDefault(partType.IsInstanceOfType);
        }

        public void Weld(ContentPart part) {
            part.ContentItem = this;
            _parts.Add(part);
        }
    }
}
