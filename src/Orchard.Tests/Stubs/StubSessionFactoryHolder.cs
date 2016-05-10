using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Data;

namespace Orchard.Tests.Stubs {
    public class StubSessionFactoryHolder : ISessionFactoryHolder {
        private readonly Func<DbContext> _action;
        public StubSessionFactoryHolder(Func<DbContext> action) {
            _action = action;
        }

        public DbContext Create() {
            return _action();
        }
    }
}
