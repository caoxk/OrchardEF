namespace Orchard.DocumentManagement.Handlers {
    public interface IDocumentStorageFilter : IDocumentFilter {
        void Activated(ActivatedDocumentContext context);
        void Initializing(InitializingDocumentContext context);
        void Initialized(InitializingDocumentContext context);
        void Creating(CreateDocumentContext context);
        void Created(CreateDocumentContext context);
        void Loading(LoadDocumentContext context);
        void Loaded(LoadDocumentContext context);
        void Updating(UpdateContentContext context);
        void Updated(UpdateContentContext context);
        void Destroying(DestroyDocumentContext context);
        void Destroyed(DestroyDocumentContext context);
    }
}
