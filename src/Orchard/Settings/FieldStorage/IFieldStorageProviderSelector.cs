
namespace Orchard.ContentManagement.FieldStorage {
    public interface IFieldStorageProviderSelector : IDependency {
        IFieldStorageProvider GetProvider();
    }
}