namespace Orchard.DocumentManagement.Handlers {
    public abstract class TemplateFilterBase<TPart> : IDocumentTemplateFilter where TPart : class, IDocument {
        protected virtual void GetContentItemMetadata(GetDocumentItemMetadataContext context, TPart instance) { }
        protected virtual void BuildDisplayShape(BuildDisplayContext context, TPart instance) { }
        protected virtual void BuildEditorShape(BuildEditorContext context, TPart instance) { }
        protected virtual void UpdateEditorShape(UpdateEditorContext context, TPart instance) { }

        void IDocumentTemplateFilter.GetContentItemMetadata(GetDocumentItemMetadataContext context) {
            if (context.ContentItem.Is<TPart>())
                GetContentItemMetadata(context, context.ContentItem.As<TPart>());
        }

        void IDocumentTemplateFilter.BuildDisplayShape(BuildDisplayContext context) {
            if (context.ContentItem != null && context.ContentItem.Is<TPart>())
                BuildDisplayShape(context, context.ContentItem.As<TPart>());
        }

        void IDocumentTemplateFilter.BuildEditorShape(BuildEditorContext context) {
            if (context.ContentItem.Is<TPart>())
                BuildEditorShape(context, context.ContentItem.As<TPart>());
        }

        void IDocumentTemplateFilter.UpdateEditorShape(UpdateEditorContext context) {
            if (context.ContentItem.Is<TPart>())
                UpdateEditorShape(context, context.ContentItem.As<TPart>());
        }
    }
}
