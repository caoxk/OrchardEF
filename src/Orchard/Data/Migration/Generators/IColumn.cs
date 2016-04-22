using System.Collections.Generic;
using FluentMigrator.Model;

namespace Orchard.Data.Migration.Generators
{
    public interface IColumn
    {
        string Generate(ColumnDefinition column);
        string Generate(IEnumerable<ColumnDefinition> columns, string tableName);
    }
}
