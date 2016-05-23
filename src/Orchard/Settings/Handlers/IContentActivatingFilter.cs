namespace Orchard.Settings.Handlers {
    public interface IContentActivatingFilter : IContentFilter {
        void Activating(ActivatingContentContext context);
    }
}
