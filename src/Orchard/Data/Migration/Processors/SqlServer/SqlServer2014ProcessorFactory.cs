using FluentMigrator;
using Orchard.Data.Migration.Generators.SqlServer;

namespace Orchard.Data.Migration.Processors.SqlServer
{
    public class SqlServer2014ProcessorFactory : MigrationProcessorFactory
    {
        private readonly ITransactionManager _transactionManager;
        public SqlServer2014ProcessorFactory(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }
        public override IMigrationProcessor Create(IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var factory = new SqlServerDbFactory();
            //var connection = factory.CreateConnection(connectionString);
            return new SqlServerProcessor(_transactionManager, new SqlServer2014Generator(), announcer, options, factory);
        }
    }
}