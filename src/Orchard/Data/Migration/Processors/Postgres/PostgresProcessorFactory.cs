using FluentMigrator;
using Orchard.Data.Migration.Generators.Postgres;

namespace Orchard.Data.Migration.Processors.Postgres
{
    public class PostgresProcessorFactory : MigrationProcessorFactory
    {
        private readonly ITransactionManager _transactionManager;
        public PostgresProcessorFactory(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }
        public override IMigrationProcessor Create(IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var factory = new PostgresDbFactory();
            //var connection = factory.CreateConnection(connectionString);
            return new PostgresProcessor(_transactionManager, new PostgresGenerator(), announcer, options, factory);
        }
    }
}