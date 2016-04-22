using FluentMigrator;
using Orchard.Data.Migration.Generators.Oracle;

namespace Orchard.Data.Migration.Processors.Oracle
{
    public class OracleManagedProcessorFactory : MigrationProcessorFactory
    {
        private readonly ITransactionManager _transactionManager;
        public OracleManagedProcessorFactory(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }
        public override IMigrationProcessor Create(IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var factory = new OracleManagedDbFactory();
            //var connection = factory.CreateConnection(connectionString);
            return new OracleProcessor(_transactionManager, new OracleGenerator(this.Quoted(options.ProviderSwitches)), announcer, options, factory);
        }

        private bool Quoted(string options)
        {
            return !string.IsNullOrEmpty(options) && 
                options.ToUpper().Contains("QUOTEDIDENTIFIERS=TRUE");
        }
    }
}