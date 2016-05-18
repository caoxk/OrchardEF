using System;
using System.Data.Entity;
using Orchard.Data;

namespace Orchard.Tests.Data {
    public class StubLocator : ISessionLocator {
        private readonly DataContext _session;

        public StubLocator(DataContext session) {
            _session = session;
        }

        #region ISessionLocator Members

        public DataContext For(Type entityType) {
            return _session;
        }

        #endregion
    }
}