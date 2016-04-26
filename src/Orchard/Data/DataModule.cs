using System.Reflection;
using Autofac;
using Orchard.Data.Providers;
using Module = Autofac.Module;

namespace Orchard.Data {
    public class DataModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerDependency();
        }
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration) {
            registration.Activated += Registration_Activated;
            //if (typeof(IDataServicesProvider).IsAssignableFrom(registration.Activator.LimitType)) {
            //    var propertyInfo = registration.Activator.LimitType.GetProperty("ProviderName", BindingFlags.Static | BindingFlags.Public);
            //    if (propertyInfo != null) {
            //        registration.Metadata["ProviderName"] = propertyInfo.GetValue(null, null);
            //    }
            //}
            //registration.Activating += (s, e) => {
            //    e.
            //};
            //if (typeof(System.IServiceProvider).IsAssignableFrom(registration.Activator.LimitType)) {
            //    Microsoft.Data.Entity.Internal.DbContextActivator.ServiceProvider =

            ////
            //}
        }

        private void Registration_Activated(object sender, Autofac.Core.ActivatedEventArgs<object> e) {
            var c = e.Context;
        }
    }
}