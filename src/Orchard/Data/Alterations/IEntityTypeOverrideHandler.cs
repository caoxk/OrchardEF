using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace Orchard.Data.Alterations {
    public interface IEntityTypeOverrideHandler : IDependency {
        void Alter(ModelBuilder modelBuilder);
    }
}
