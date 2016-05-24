namespace Orchard.DocumentManagement.Handlers {
    public interface IDocumentActivatingFilter : IDocumentFilter {
        void Activating(ActivatingDocumentContext context);
    }
}
