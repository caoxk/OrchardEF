using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using FluentMigrator;
using FluentMigrator.Builders.Execute;
using Orchard.Data.Migration.Generators;
using Orchard.Data.Migration.Generators.Oracle;

#region License
// 
// Copyright (c) 2007-2009, Sean Chambers <schambers80@gmail.com>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

namespace Orchard.Data.Migration.Processors.Oracle
{
    public class OracleProcessor : GenericProcessorBase
    {
        public override string DatabaseType
        {
            get { return "Oracle"; }
        }

        public OracleProcessor(ITransactionManager transactionManager, IMigrationGenerator generator, IAnnouncer announcer, IMigrationProcessorOptions options, IDbFactory factory)
            : base(transactionManager, factory, generator, announcer, options)
        {
        }

        public IQuoter Quoter
        {
            get { return ((OracleGenerator)this.Generator).Quoter; }
        }

        public override bool SchemaExists(string schemaName)
        {
            if (schemaName == null)
                throw new ArgumentNullException("schemaName");

            if (schemaName.Length == 0)
                return false;

            return Exists("SELECT 1 FROM ALL_USERS WHERE USERNAME = '{0}'", schemaName.ToUpper());
        }

        public override bool TableExists(string schemaName, string tableName)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            if (tableName.Length == 0)
                return false;

            if (string.IsNullOrEmpty(schemaName))
                return Exists("SELECT 1 FROM USER_TABLES WHERE upper(TABLE_NAME) = '{0}'", FormatHelper.FormatSqlEscape(tableName.ToUpper()));

            return Exists("SELECT 1 FROM ALL_TABLES WHERE upper(OWNER) = '{0}' AND upper(TABLE_NAME) = '{1}'", schemaName.ToUpper(), FormatHelper.FormatSqlEscape(tableName.ToUpper()));
        }

        public override bool ColumnExists(string schemaName, string tableName, string columnName)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");
            if (columnName == null)
                throw new ArgumentNullException("columnName");

            if (columnName.Length == 0 || tableName.Length == 0)
                return false;

            if (string.IsNullOrEmpty(schemaName))
                return Exists("SELECT 1 FROM USER_TAB_COLUMNS WHERE upper(TABLE_NAME) = '{0}' AND upper(COLUMN_NAME) = '{1}'",
                    FormatHelper.FormatSqlEscape(tableName.ToUpper()),
                    FormatHelper.FormatSqlEscape(columnName.ToUpper()));

            return Exists("SELECT 1 FROM ALL_TAB_COLUMNS WHERE upper(OWNER) = '{0}' AND upper(TABLE_NAME) = '{1}' AND upper(COLUMN_NAME) = '{2}'",
                schemaName.ToUpper(), FormatHelper.FormatSqlEscape(tableName.ToUpper()), FormatHelper.FormatSqlEscape(columnName.ToUpper()));
        }

        public override bool ConstraintExists(string schemaName, string tableName, string constraintName)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");
            if (constraintName == null)
                throw new ArgumentNullException("constraintName");

            //In Oracle DB constraint name is unique within the schema, so the table name is not used in the query

            if (constraintName.Length == 0)
                return false;

            if (String.IsNullOrEmpty(schemaName))
                return Exists("SELECT 1 FROM USER_CONSTRAINTS WHERE upper(CONSTRAINT_NAME) = '{0}'", FormatHelper.FormatSqlEscape(constraintName.ToUpper()));

            return Exists("SELECT 1 FROM ALL_CONSTRAINTS WHERE upper(OWNER) = '{0}' AND upper(CONSTRAINT_NAME) = '{1}'", schemaName.ToUpper(), 
                FormatHelper.FormatSqlEscape(constraintName.ToUpper()));
        }

        public override bool IndexExists(string schemaName, string tableName, string indexName)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");
            if (indexName == null)
                throw new ArgumentNullException("indexName");

            //In Oracle DB index name is unique within the schema, so the table name is not used in the query

            if (indexName.Length == 0)
                return false;

            if (String.IsNullOrEmpty(schemaName))
                return Exists("SELECT 1 FROM USER_INDEXES WHERE upper(INDEX_NAME) = '{0}'", FormatHelper.FormatSqlEscape(indexName.ToUpper()));

            return Exists("SELECT 1 FROM ALL_INDEXES WHERE upper(OWNER) = '{0}' AND upper(INDEX_NAME) = '{1}'", schemaName.ToUpper(), FormatHelper.FormatSqlEscape(indexName.ToUpper()));
        }

        public override bool SequenceExists(string schemaName, string sequenceName)
        {
            return false;
        }

        public override bool DefaultValueExists(string schemaName, string tableName, string columnName, object defaultValue)
        {
            return false;
        }

        public override void Execute(string template, params object[] args)
        {
            Process(string.Format(template, args));
        }

        public override bool Exists(string template, params object[] args)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            Announcer.Sql(String.Format(template, args));

            var session = _transactionManager.GetSession();

            using (var command = session.Connection.CreateCommand())
            {
                command.CommandTimeout = Options.Timeout;
                command.CommandText = String.Format(template, args);
                session.Transaction.Enlist(command);
                using (var reader = command.ExecuteReader())
                {
                    return reader.Read();
                }
            }
        }

        public override DataSet ReadTableData(string schemaName, string tableName)
        {
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            if (String.IsNullOrEmpty(schemaName))
                return Read("SELECT * FROM {0}", Quoter.QuoteTableName(tableName));

            return Read("SELECT * FROM {0}.{1}", Quoter.QuoteSchemaName(schemaName), Quoter.QuoteTableName(tableName));
        }

        public override DataSet Read(string template, params object[] args)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            var session = _transactionManager.GetSession();

            var ds = new DataSet();
            using (var command = session.Connection.CreateCommand())
            {
                command.CommandTimeout = Options.Timeout;
                command.CommandText = String.Format(template, args);
                session.Transaction.Enlist(command);
                var adapter = Factory.CreateDataAdapter(command);
                adapter.Fill(ds);
                return ds;
            }
        }

        public override void Process(PerformDBOperationExpression expression)
        {
            var session = _transactionManager.GetSession();

            if (expression.Operation != null)
                expression.Operation(session.Connection, null);
        }

        protected override void Process(string sql)
        {
            Announcer.Sql(sql);

            if (Options.PreviewOnly || string.IsNullOrEmpty(sql))
                return;

            var batches = Regex.Split(sql, @"^\s*;\s*$", RegexOptions.Multiline)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x));

            var session = _transactionManager.GetSession();
            
            foreach (var batch in batches)
            {
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandTimeout = Options.Timeout;
                    command.CommandText = batch;
                    session.Transaction.Enlist(command);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
