using System.Collections.Generic;
using System.Linq;

namespace Orchard.ContentManagement.FieldStorage {
    public class FieldStorageProviderSelector : IFieldStorageProviderSelector {
        public const string Storage = "Storage";
        public const string DefaultProviderName = "Infoset";

        private readonly IEnumerable<IFieldStorageProvider> _storageProviders;

        public FieldStorageProviderSelector(IEnumerable<IFieldStorageProvider> storageProviders) {
            _storageProviders = storageProviders;
        }

        public IFieldStorageProvider GetProvider() {

            IFieldStorageProvider provider = null;

            return provider ?? Locate(DefaultProviderName);
        }

        private IFieldStorageProvider Locate(string providerName) {
            return _storageProviders.FirstOrDefault(provider => provider.ProviderName == providerName);
        }
    }
}