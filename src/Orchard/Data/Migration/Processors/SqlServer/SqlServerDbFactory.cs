using System.Data.Common;
using System.Data.SqlClient;

namespace Orchard.Data.Migration.Processors.SqlServer
{
    public class SqlServerDbFactory : DbFactoryBase
    {
        public SqlServerDbFactory()
            : base(SqlClientFactory.Instance)
        {
        }

        protected override DbProviderFactory CreateFactory()
        {
            return SqlClientFactory.Instance;
        }
    }
}