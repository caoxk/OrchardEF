using FluentMigrator;
using Orchard.Data.Migration.Generators.Oracle;

namespace Orchard.Data.Migration.Processors.Oracle
{
    public class OracleProcessorFactory : MigrationProcessorFactory
    {
        private readonly ITransactionManager _transactionManager;
        public OracleProcessorFactory(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }
        public override IMigrationProcessor Create(IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var factory = new OracleDbFactory();
            //var connection = factory.CreateConnection(connectionString);
            return new OracleProcessor(_transactionManager, new OracleGenerator(Quoted(options.ProviderSwitches)), announcer, options, factory);
        }

        private bool Quoted(string options)
        {
            return !string.IsNullOrEmpty(options) && 
                options.ToUpper().Contains("QUOTEDIDENTIFIERS=TRUE");
        }
    }
}