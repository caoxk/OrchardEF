namespace Orchard.Data.Migration.Processors.Oracle
{
    public class OracleManagedDbFactory : ReflectionBasedDbFactory
    {
        public OracleManagedDbFactory()
            : base("Oracle.ManagedDataAccess", "Oracle.ManagedDataAccess.Client.OracleClientFactory")
        {
        }
    }
}