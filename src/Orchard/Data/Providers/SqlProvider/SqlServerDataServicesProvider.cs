using System;
using System.Data.Entity;

namespace Orchard.Data.Providers.SqlProvider {
    public class SqlServerDataServicesProvider : IDataServicesProvider {
        public static string ProviderName {
            get { return "SqlServer"; }
        }

        public DbConfiguration BuildConfiguration()
        {
            return new SqlServerConfiguration();
        }

        public DbContextOptions GetContextOptions(SessionFactoryParameters parameters) {
            return new DbContextOptions {ConnectionString = parameters.ConnectionString};
        }
    }
}
