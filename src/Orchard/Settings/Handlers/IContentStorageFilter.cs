namespace Orchard.Settings.Handlers {
    public interface IContentStorageFilter : IContentFilter {
        void Activated(ActivatedContentContext context);
        void Initializing(InitializingContentContext context);
        void Initialized(InitializingContentContext context);
        void Creating(CreateContentContext context);
        void Created(CreateContentContext context);
        void Loading(LoadContentContext context);
        void Loaded(LoadContentContext context);
        void Updating(UpdateContentContext context);
        void Updated(UpdateContentContext context);
        void Destroying(DestroyContentContext context);
        void Destroyed(DestroyContentContext context);
    }
}
