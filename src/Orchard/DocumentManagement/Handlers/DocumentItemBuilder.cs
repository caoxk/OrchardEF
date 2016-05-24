using System.Linq;

namespace Orchard.DocumentManagement.Handlers {
    /// <summary>
    /// Builds a contentitem based on its the type definition (<seealso cref="ContentTypeDefinition"/>).
    /// </summary>
    public class DocumentItemBuilder {
        private readonly DocumentItem _item;

        /// <summary>
        /// Constructs a new Content Item Builder instance.
        /// </summary>
        public DocumentItemBuilder(string contentType) {

            // TODO: could / should be done on the build method ?
            _item = new DocumentItem {
                ContentType = contentType
            };
        }

        public DocumentItem Build() {
            return _item;
        }

        /// <summary>
        /// Welds a new part to the content item. If a part of the same type is already welded nothing is done.
        /// </summary>
        /// <typeparam name="TPart">The type of the part to be welded.</typeparam>
        /// <returns>A new Content Item Builder with the item having the new part welded.</returns>
        public DocumentItemBuilder Weld<TPart>() where TPart : DocumentPart, new() {

            // if the part hasn't be weld yet
            if (_item.Parts.FirstOrDefault(part => part.GetType().Equals(typeof(TPart))) == null) {
                var partName = typeof(TPart).Name;

                // build and weld the part
                var part = new TPart { };
                _item.Weld(part);
            }

            return this;
        }

        /// <summary>
        /// Welds a part to the content item.
        /// </summary>
        public DocumentItemBuilder Weld(DocumentPart contentPart) {
            _item.Weld(contentPart);
            return this;
        }
    }
}
