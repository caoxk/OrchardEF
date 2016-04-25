using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Orchard.Data.Providers {
    public abstract class AbstractDataServicesProvider : IDataServicesProvider {
        public abstract void ConfigureContextOptions(DbContextOptionsBuilder optionsBuilders, string connectionString);
    }
}
