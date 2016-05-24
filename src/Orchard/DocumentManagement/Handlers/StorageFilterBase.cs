namespace Orchard.DocumentManagement.Handlers {
    public abstract class StorageFilterBase<TPart> : IDocumentStorageFilter where TPart : class, IDocument {

        protected virtual void Activated(ActivatedDocumentContext context, TPart instance) { }
        protected virtual void Activating(ActivatingDocumentContext context, TPart instance) { }
        protected virtual void Initializing(InitializingDocumentContext context, TPart instance) { }
        protected virtual void Initialized(InitializingDocumentContext context, TPart instance) { }
        protected virtual void Creating(CreateDocumentContext context, TPart instance) { }
        protected virtual void Created(CreateDocumentContext context, TPart instance) { }
        protected virtual void Loading(LoadDocumentContext context, TPart instance) { }
        protected virtual void Loaded(LoadDocumentContext context, TPart instance) { }
        protected virtual void Updating(UpdateContentContext context, TPart instance) { }
        protected virtual void Updated(UpdateContentContext context, TPart instance) { }
        protected virtual void Destroying(DestroyDocumentContext context, TPart instance) { }
        protected virtual void Destroyed(DestroyDocumentContext context, TPart instance) { }


        void IDocumentStorageFilter.Activated(ActivatedDocumentContext context) {
            if (context.ContentItem.Is<TPart>())
                Activated(context, context.ContentItem.As<TPart>());
        }

        void IDocumentStorageFilter.Initializing(InitializingDocumentContext context) {
            if (context.ContentItem.Is<TPart>())
                Initializing(context, context.ContentItem.As<TPart>());
        }

        void IDocumentStorageFilter.Initialized(InitializingDocumentContext context) {
            if (context.ContentItem.Is<TPart>())
                Initialized(context, context.ContentItem.As<TPart>());
        }

        void IDocumentStorageFilter.Creating(CreateDocumentContext context) {
            if (context.ContentItem.Is<TPart>())
                Creating(context, context.ContentItem.As<TPart>());
        }

        void IDocumentStorageFilter.Created(CreateDocumentContext context) {
            if (context.ContentItem.Is<TPart>())
                Created(context, context.ContentItem.As<TPart>());
        }

        void IDocumentStorageFilter.Loading(LoadDocumentContext context) {
            if (context.ContentItem.Is<TPart>())
                Loading(context, context.ContentItem.As<TPart>());
        }

        void IDocumentStorageFilter.Loaded(LoadDocumentContext context) {
            if (context.ContentItem.Is<TPart>())
                Loaded(context, context.ContentItem.As<TPart>());
        }

        void IDocumentStorageFilter.Updating(UpdateContentContext context) {
            if (context.ContentItem.Is<TPart>())
                Updating(context, context.ContentItem.As<TPart>());
        }

        void IDocumentStorageFilter.Updated(UpdateContentContext context) {
            if (context.ContentItem.Is<TPart>())
                Updated(context, context.ContentItem.As<TPart>());
        }

        void IDocumentStorageFilter.Destroying(DestroyDocumentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Destroying(context, context.ContentItem.As<TPart>());
        }

        void IDocumentStorageFilter.Destroyed(DestroyDocumentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Destroyed(context, context.ContentItem.As<TPart>());
        }
    }
}
