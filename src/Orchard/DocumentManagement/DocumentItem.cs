using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Orchard.DocumentManagement.Records;

namespace Orchard.DocumentManagement {
    public class DocumentItem : DynamicObject, IDocument {
        public DocumentItem() {
            _parts = new List<DocumentPart>();
        }

        private readonly IList<DocumentPart> _parts;
        public string ContentType { get; set; }
        DocumentItem IDocument.ContentItem { get { return this; } }

        public dynamic Content { get { return (dynamic)this; } }

        public int Id { get { return Record == null ? 0 : Record.Id; } }

        public DocumentItemRecord Record { get; set; }

        public IEnumerable<DocumentPart> Parts { get { return _parts; } }

        public IDocumentManager ContentManager { get; set; }

        public bool Has(Type partType) {
            return partType == typeof(DocumentItem) || _parts.Any(partType.IsInstanceOfType);
        }

        public IDocument Get(Type partType) {
            if (partType == typeof(DocumentItem))
                return this;
            return _parts.FirstOrDefault(partType.IsInstanceOfType);
        }

        public void Weld(DocumentPart part) {
            part.ContentItem = this;
            _parts.Add(part);
        }
    }
}
