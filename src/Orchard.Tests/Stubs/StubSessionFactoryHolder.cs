using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Data;

namespace Orchard.Tests.Stubs {
    public class StubSessionFactoryHolder : ISessionFactoryHolder {
        private readonly Func<DataContext> _action;
        public StubSessionFactoryHolder(Func<DataContext> action) {
            _action = action;
        }

        public DataContext Create() {
            return _action();
        }
    }
}
