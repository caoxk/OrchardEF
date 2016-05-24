namespace Orchard.DocumentManagement.Handlers {
    public interface IDocumentHandler : IDependency {
        void Activating(ActivatingDocumentContext context);
        void Activated(ActivatedDocumentContext context);
        void Initializing(InitializingDocumentContext context);
        void Initialized(InitializingDocumentContext context);
        void Creating(CreateDocumentContext context);
        void Created(CreateDocumentContext context);
        void Loading(LoadDocumentContext context);
        void Loaded(LoadDocumentContext context);
        void Updating(UpdateContentContext context);
        void Updated(UpdateContentContext context);
        //void Publishing(PublishContentContext context);
        //void Published(PublishContentContext context);
        //void Unpublishing(PublishContentContext context);
        //void Unpublished(PublishContentContext context);
        //void Removing(RemoveContentContext context);
        //void Removed(RemoveContentContext context);

        void GetContentItemMetadata(GetDocumentItemMetadataContext context);
        void BuildDisplay(BuildDisplayContext context);
        void BuildEditor(BuildEditorContext context);
        void UpdateEditor(UpdateEditorContext context);
    }
}
