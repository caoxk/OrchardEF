using Microsoft.Extensions.DependencyInjection;

namespace Orchard.Data.Providers.InMemoryProvider {
    public class InMemoryModule : IDataServicesModule {
        public static string ProviderName
        {
            get { return "InMemory"; }
        }
        public void Configure(IServiceCollection serviceCollection) {
            serviceCollection
                .AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<DataContext>();

            serviceCollection.AddScoped<IDataServicesProvider, InMemoryDataServicesProvider>();
        }
    }
}