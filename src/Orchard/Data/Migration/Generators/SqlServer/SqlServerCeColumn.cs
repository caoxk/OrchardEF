using FluentMigrator.Model;

namespace Orchard.Data.Migration.Generators.SqlServer
{
    internal class SqlServerCeColumn : SqlServerColumn
    {
        public SqlServerCeColumn(ITypeMap typeMap) : base(typeMap)
        {
        }

        protected override string FormatNullable(ColumnDefinition column)
        {
            if (column.IsNullable.GetValueOrDefault())
                return column.ModificationType == ColumnModificationType.Alter ? "NULL" : string.Empty;

            return "NOT NULL";
        }
    }
}