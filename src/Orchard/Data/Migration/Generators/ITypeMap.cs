using System.Data;

namespace Orchard.Data.Migration.Generators
{
    public interface ITypeMap
    {
        string GetTypeMap(DbType type, int size, int precision);
    }
}
