namespace Orchard.DocumentManagement.Handlers {
    interface IDocumentTemplateFilter : IDocumentFilter {
        void GetContentItemMetadata(GetDocumentItemMetadataContext context);
        void BuildDisplayShape(BuildDisplayContext context);
        void BuildEditorShape(BuildEditorContext context);
        void UpdateEditorShape(UpdateEditorContext context);
    }
}
