using System.Collections.Generic;
using FluentMigrator.Expressions;

namespace Orchard.Data.Migration.Generators
{
    public interface IDescriptionGenerator
    {
        IEnumerable<string> GenerateDescriptionStatements(CreateTableExpression expression);
        string GenerateDescriptionStatement(AlterTableExpression expression);
        string GenerateDescriptionStatement(CreateColumnExpression expression);
        string GenerateDescriptionStatement(AlterColumnExpression expression);
    }
}
