using FluentMigrator;
using Orchard.Data.Migration.Generators.MySql;

namespace Orchard.Data.Migration.Processors.MySql
{
    public class MySqlProcessorFactory : MigrationProcessorFactory
    {
        private readonly ITransactionManager _transactionManager;
        public MySqlProcessorFactory(ITransactionManager transactionManager) {
            _transactionManager = transactionManager;
        }

        public override IMigrationProcessor Create(IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var factory = new MySqlDbFactory();
            //var connection = factory.CreateConnection(connectionString);
            return new MySqlProcessor(_transactionManager, new MySqlGenerator(), announcer, options, factory);
        }
    }
}