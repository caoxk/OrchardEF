namespace Orchard.DocumentManagement {
    public interface IDocument {
        DocumentItem ContentItem { get; }

        /// <summary>
        /// The ContentItem's identifier.
        /// </summary>
        int Id { get; }
    }
}
