using System;
using FluentMigrator.Builders.Alter;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Execute;
using FluentMigrator.Builders.IfDatabase;
using FluentMigrator.Builders.Insert;
using FluentMigrator.Builders.Rename;
using FluentMigrator.Builders.Schema;
using FluentMigrator.Builders.Update;
using FluentMigrator.Infrastructure;
using Orchard.Data.Migration.Interpreters;
using Orchard.Localization;
using Orchard.Exceptions;

namespace Orchard.Data.Migration.Schema {
    //public class SchemaBuilder {
    //    private readonly IDataMigrationInterpreter _interpreter;
    //    private readonly string _featurePrefix;
    //    private readonly Func<string, string> _formatPrefix;

    //    public Localizer T { get; set; }

    //    public SchemaBuilder(IDataMigrationInterpreter interpreter, string featurePrefix = null, Func<string, string> formatPrefix = null) {
    //        _interpreter = interpreter;
    //        _featurePrefix = featurePrefix ?? String.Empty;
    //        _formatPrefix = formatPrefix ?? (s => s ?? String.Empty);
    //        T = NullLocalizer.Instance;
    //    }

    //    public IDataMigrationInterpreter Interpreter {
    //        get { return _interpreter; }
    //    }
      
    //    public string FeaturePrefix {
    //        get { return _featurePrefix; }
    //    }
      
    //    public Func<string, string> FormatPrefix {
    //        get { return _formatPrefix; }
    //    }

    //    /// <summary>
    //    /// Translate Table name into database table name - including prefixes.
    //    /// </summary>
    //    public virtual string TableDbName(string srcTable) {
    //        return _interpreter.PrefixTableName(String.Concat(FormatPrefix(FeaturePrefix), srcTable));
    //    }

    //    /// <summary>
    //    /// Removes the data table prefix from the specified table name.
    //    /// </summary>
    //    public virtual string RemoveDataTablePrefix(string prefixedTableName) {
    //        return _interpreter.RemovePrefixFromTableName(prefixedTableName);
    //    }

    //    public SchemaBuilder CreateTable(string name, Action<CreateTableCommand> table) {
    //        var createTable = new CreateTableCommand(String.Concat(_formatPrefix(_featurePrefix), name));
    //        table(createTable);
    //        Run(createTable);
    //        return this;
    //    }

    //    public SchemaBuilder AlterTable(string name, Action<AlterTableCommand> table) {
    //        var alterTable = new AlterTableCommand(String.Concat(_formatPrefix(_featurePrefix), name));
    //        table(alterTable);
    //        Run(alterTable);
    //        return this;
    //    }

    //    public SchemaBuilder DropTable(string name) {
    //        var deleteTable = new DropTableCommand(String.Concat(_formatPrefix(_featurePrefix), name));
    //        Run(deleteTable);
    //        return this;
    //    }

    //    public SchemaBuilder ExecuteSql(string sql, Action<SqlStatementCommand> statement = null) {
    //        try {
    //            var sqlStatmentCommand = new SqlStatementCommand(sql);
    //            if (statement != null) {
    //                statement(sqlStatmentCommand);
    //            }
    //            Run(sqlStatmentCommand);
    //            return this;
    //        } catch (Exception ex) {
    //            if (ex.IsFatal()) {  
    //                throw;
    //            } 
    //            throw new OrchardException(T("An unexpected error occured while executing the SQL statement: {0}", sql), ex); // Add the sql to the nested exception information
    //        }
    //    }

    //    private void Run(ISchemaBuilderCommand command) {
    //        _interpreter.Visit(command);
    //    }

    //    public SchemaBuilder CreateForeignKey(string name, string srcTable, string[] srcColumns, string destTable, string[] destColumns) {
    //        var command = new CreateForeignKeyCommand(name, String.Concat(_formatPrefix(_featurePrefix), srcTable), srcColumns, String.Concat(_formatPrefix(_featurePrefix), destTable), destColumns);
    //        Run(command);
    //        return this;
    //    }

    //    public SchemaBuilder CreateForeignKey(string name, string srcModule, string srcTable, string[] srcColumns, string destTable, string[] destColumns) {
    //        var command = new CreateForeignKeyCommand(name, String.Concat(_formatPrefix(srcModule), srcTable), srcColumns, String.Concat(_formatPrefix(_featurePrefix), destTable), destColumns);
    //        Run(command);
    //        return this;
    //    }

    //    public SchemaBuilder CreateForeignKey(string name, string srcTable, string[] srcColumns, string destModule, string destTable, string[] destColumns) {
    //        var command = new CreateForeignKeyCommand(name, String.Concat(_formatPrefix(_featurePrefix), srcTable), srcColumns, String.Concat(_formatPrefix(destModule), destTable), destColumns);
    //        Run(command);
    //        return this;
    //    }

    //    public SchemaBuilder CreateForeignKey(string name, string srcModule, string srcTable, string[] srcColumns, string destModule, string destTable, string[] destColumns) {
    //        var command = new CreateForeignKeyCommand(name, String.Concat(_formatPrefix(srcModule), srcTable), srcColumns, String.Concat(_formatPrefix(destModule), destTable), destColumns);
    //        Run(command);
    //        return this;
    //    }

    //    public SchemaBuilder DropForeignKey(string srcTable, string name) {
    //        var command = new DropForeignKeyCommand(String.Concat(_formatPrefix(_featurePrefix), srcTable), name);
    //        Run(command);
    //        return this;
    //    }

    //    public SchemaBuilder DropForeignKey(string srcModule, string srcTable, string name) {
    //        var command = new DropForeignKeyCommand(String.Concat(_formatPrefix(srcModule), srcTable), name);
    //        Run(command);
    //        return this;
    //    }

    //}
    public class SchemaBuilder
    {
        internal IMigrationContext _context;

        public IAlterExpressionRoot Alter
        {
            get { return new AlterExpressionRoot(_context); }
        }

        public ICreateExpressionRoot Create
        {
            get { return new CreateExpressionRoot(_context); }
        }

        public IRenameExpressionRoot Rename
        {
            get { return new RenameExpressionRoot(_context); }
        }

        public IInsertExpressionRoot Insert
        {
            get { return new InsertExpressionRoot(_context); }
        }

        public ISchemaExpressionRoot Schema
        {
            get { return new SchemaExpressionRoot(_context); }
        }

        public IIfDatabaseExpressionRoot IfDatabase(params string[] databaseType)
        {
            return new IfDatabaseExpressionRoot(_context, databaseType);
        }
        public IDeleteExpressionRoot Delete
        {
            get { return new DeleteExpressionRoot(_context); }
        }

        public IExecuteExpressionRoot Execute
        {
            get { return new ExecuteExpressionRoot(_context); }
        }

        public IUpdateExpressionRoot Update
        {
            get { return new UpdateExpressionRoot(_context); }
        }
    }
}
