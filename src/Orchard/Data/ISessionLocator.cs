using System;

namespace Orchard.Data {
    public interface ISessionLocator : IDependency {

        [Obsolete("Use ITransactionManager.GetSession() instead.")]
        DataContext For(Type entityType);
    }
}