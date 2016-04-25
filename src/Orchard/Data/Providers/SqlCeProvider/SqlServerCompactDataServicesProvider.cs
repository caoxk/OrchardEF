using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Orchard.Data.Providers.SqlCeProvider {
    public class SqlServerCompactDataServicesProvider : AbstractDataServicesProvider {
        public static string ProviderName
        {
            get { return "SqlServerCe"; }
        }

        public override void ConfigureContextOptions(DbContextOptionsBuilder optionsBuilders, string connectionString) {
            optionsBuilders.UseSqlCe(@"Data Source=C:\data\Blogging.sdf");
        }
    }
}
