using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.Entity.SqlServerCompact;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.Providers.SqlCeProvider
{
    public class SqlCeConfiguration : DbConfiguration
    {
        public SqlCeConfiguration() {
            SetProviderServices(SqlCeProviderServices.ProviderInvariantName, SqlCeProviderServices.Instance);

            SetDefaultConnectionFactory(
                new SqlCeConnectionFactory(SqlCeProviderServices.ProviderInvariantName));
        }
    }
}
