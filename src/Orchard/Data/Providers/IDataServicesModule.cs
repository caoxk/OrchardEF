using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Orchard.Data.Providers {
    public interface IDataServicesModule {
        void Configure(IServiceCollection serviceCollection);
    }
}
