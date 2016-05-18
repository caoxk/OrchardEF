using System.Data.Entity;
using Autofac;
using Autofac.Core;

namespace Orchard.Data.Providers {
    public interface IDataServicesProvider : ITransientDependency {
        void BuildConfiguration();
        DbContextOptions GetContextOptions(SessionFactoryParameters parameters);
    }
}
