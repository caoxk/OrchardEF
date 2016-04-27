using System;
using Microsoft.Data.Entity;

namespace Orchard.Data {
    public interface ISessionLocator : IDependency {

        [Obsolete("Use ITransactionManager.GetSession() instead.")]
        DbContext For(Type entityType);
    }
}