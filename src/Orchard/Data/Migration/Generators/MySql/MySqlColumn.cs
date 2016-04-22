using System;
using FluentMigrator;
using FluentMigrator.Model;
using Orchard.Data.Migration.Generators.Base;

namespace Orchard.Data.Migration.Generators.MySql
{
    internal class MySqlColumn : ColumnBase
    {
        public MySqlColumn()
            : base(new MySqlTypeMap(), new MySqlQuoter())
        {
        }

        protected override string FormatIdentity(ColumnDefinition column)
        {
            return column.IsIdentity ? "AUTO_INCREMENT" : string.Empty;
        }

        protected override string FormatSystemMethods(SystemMethods systemMethod)
        {
            switch (systemMethod)
            {
                case SystemMethods.CurrentDateTime:
                    return "CURRENT_TIMESTAMP";
            }

            throw new NotImplementedException();
        }
    }
}
