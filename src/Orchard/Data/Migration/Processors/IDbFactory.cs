using System.Data;

namespace Orchard.Data.Migration.Processors
{
    public interface IDbFactory
    {
        IDbConnection CreateConnection(string connectionString);
        IDbCommand CreateCommand(string commandText, IDbConnection connection, IDbTransaction transaction);
        IDbDataAdapter CreateDataAdapter(IDbCommand command);
        IDbCommand CreateCommand(string commandText, IDbConnection connection);
    }
}