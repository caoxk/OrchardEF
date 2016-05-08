using System;
using System.Data.Entity;
using Orchard.Data;

namespace Orchard.Tests.Data {
    public class StubLocator : ISessionLocator {
        private readonly DbContext _session;

        public StubLocator(DbContext session) {
            _session = session;
        }

        #region ISessionLocator Members

        public DbContext For(Type entityType) {
            return _session;
        }

        #endregion
    }
}