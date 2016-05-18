using System;
using System.Data.Entity;

namespace Orchard.Data {
    public interface ISessionLocator : IDependency {

        [Obsolete("Use ITransactionManager.GetSession() instead.")]
        DataContext For(Type entityType);
    }
}