namespace Orchard.DocumentManagement.Handlers {
    public class DocumentHandlerBase : IDocumentHandler {
        public virtual void Activating(ActivatingDocumentContext context) {}
        public virtual void Activated(ActivatedDocumentContext context) {}
        public virtual void Initializing(InitializingDocumentContext context) { }
        public virtual void Initialized(InitializingDocumentContext context) { }
        public virtual void Creating(CreateDocumentContext context) { }
        public virtual void Created(CreateDocumentContext context) {}
        public virtual void Loading(LoadDocumentContext context) {}
        public virtual void Loaded(LoadDocumentContext context) {}
        public virtual void Updating(UpdateContentContext context) { }
        public virtual void Updated(UpdateContentContext context) { }

        public virtual void GetContentItemMetadata(GetDocumentItemMetadataContext context) {}
        public virtual void BuildDisplay(BuildDisplayContext context) {}
        public virtual void BuildEditor(BuildEditorContext context) {}
        public virtual void UpdateEditor(UpdateEditorContext context) {}
    }
}