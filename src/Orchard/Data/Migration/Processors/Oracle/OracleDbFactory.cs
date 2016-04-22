namespace Orchard.Data.Migration.Processors.Oracle
{
    public class OracleDbFactory : ReflectionBasedDbFactory
    {
        public OracleDbFactory()
            : base("Oracle.DataAccess", "Oracle.DataAccess.Client.OracleClientFactory")
        {
        }
    }
}