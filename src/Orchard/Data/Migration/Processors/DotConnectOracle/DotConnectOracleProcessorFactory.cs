using FluentMigrator;
using Orchard.Data.Migration.Generators.Oracle;

namespace Orchard.Data.Migration.Processors.DotConnectOracle
{
    public class DotConnectOracleProcessorFactory : MigrationProcessorFactory
    {
        private readonly ITransactionManager _transactionManager;
        public DotConnectOracleProcessorFactory(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }
        public override IMigrationProcessor Create(IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var factory = new DotConnectOracleDbFactory();
            //var connection = factory.CreateConnection(connectionString);
            return new DotConnectOracleProcessor(_transactionManager, new OracleGenerator(), announcer, options, factory);
        }
    }
}