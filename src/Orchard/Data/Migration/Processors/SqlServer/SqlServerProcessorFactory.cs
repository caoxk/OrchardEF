using FluentMigrator;
using Orchard.Data.Migration.Generators.SqlServer;

namespace Orchard.Data.Migration.Processors.SqlServer
{
    public class SqlServerProcessorFactory : MigrationProcessorFactory
    {
        private readonly ITransactionManager _transactionManager;
        public SqlServerProcessorFactory(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        public override IMigrationProcessor Create(IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var factory = new SqlServerDbFactory();
            //var connection = factory.CreateConnection(connectionString);
            return new SqlServerProcessor(_transactionManager, new SqlServer2008Generator(), announcer, options, factory);
        }

        public override bool IsForProvider(string provider)
        {
            return provider.ToLower().Contains("sqlclient");
        }
    }
}