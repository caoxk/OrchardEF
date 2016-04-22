using System;
using System.Data;
using System.IO;
using FluentMigrator;
using FluentMigrator.Builders.Execute;

#region License
// 
// Copyright (c) 2007-2009, Sean Chambers <schambers80@gmail.com>
// Copyright (c) 2010, Nathan Brown
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

namespace Orchard.Data.Migration.Processors.SqlServer
{
    public sealed class SqlServer2000Processor : GenericProcessorBase
    {
        public SqlServer2000Processor(ITransactionManager transactionManager, IMigrationGenerator generator, IAnnouncer announcer, IMigrationProcessorOptions options, IDbFactory factory)
            : base(transactionManager, factory, generator, announcer, options)
        {
        }

        //public override bool SupportsTransactions
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        public override string DatabaseType
        {
            get { return "SqlServer2000"; }
        }

        public override bool SchemaExists(string schemaName)
        {
            return true;
        }

        public override bool TableExists(string schemaName, string tableName)
        {
            try
            {
                return Exists("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}'", FormatHelper.FormatSqlEscape(tableName));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        public override bool ColumnExists(string schemaName, string tableName, string columnName)
        {
            return Exists("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}' AND COLUMN_NAME = '{1}'", 
                FormatHelper.FormatSqlEscape(tableName),
                FormatHelper.FormatSqlEscape(columnName));
        }

        public override bool ConstraintExists(string schemaName, string tableName, string constraintName)
        {
            return Exists("SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_CATALOG = DB_NAME() AND TABLE_NAME = '{0}' AND CONSTRAINT_NAME = '{1}'",
                FormatHelper.FormatSqlEscape(tableName), FormatHelper.FormatSqlEscape(constraintName));
        }

        public override bool IndexExists(string schemaName, string tableName, string indexName)
        {
            return Exists("SELECT NULL FROM sysindexes WHERE name = '{0}'", FormatHelper.FormatSqlEscape(indexName));
        }

        public override bool SequenceExists(string schemaName, string sequenceName)
        {
            return false;
        }

        public override bool DefaultValueExists(string schemaName, string tableName, string columnName, object defaultValue)
        {
            return false;
        }

        public override DataSet ReadTableData(string schemaName, string tableName)
        {
            return Read("SELECT * FROM [{0}]", tableName);
        }

        public override DataSet Read(string template, params object[] args)
        {
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

        public override bool Exists(string template, params object[] args)
        {
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

        public override void Execute(string template, params object[] args)
        {
            Process(String.Format(template, args));
        }

        protected override void Process(string sql)
        {
            Announcer.Sql(sql);

            if (Options.PreviewOnly || string.IsNullOrEmpty(sql))
                return;

            if (sql.Contains("GO"))
            {
                ExecuteBatchNonQuery(sql);

            }
            else
            {
                ExecuteNonQuery(sql);
            }
        }

        private void ExecuteNonQuery(string sql)
        {
            var session = _transactionManager.GetSession();

            using (var command = session.Connection.CreateCommand())
            {
                try
                {
                    command.CommandTimeout = Options.Timeout;
                    command.CommandText = sql;
                    session.Transaction.Enlist(command);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    using (var message = new StringWriter())
                    {
                        message.WriteLine("An error occured executing the following sql:");
                        message.WriteLine(sql);
                        message.WriteLine("The error was {0}", ex.Message);

                        throw new Exception(message.ToString(), ex);
                    }
                }
            }
        }

        private void ExecuteBatchNonQuery(string sql)
        {
            sql += "\nGO";   // make sure last batch is executed.
            string sqlBatch = string.Empty;

            var session = _transactionManager.GetSession();

            using (var command = session.Connection.CreateCommand())
            {
                try
                {
                    foreach (string line in sql.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (line.ToUpperInvariant().Trim() == "GO")
                        {
                            if (!string.IsNullOrEmpty(sqlBatch))
                            {
                                command.CommandText = sqlBatch;
                                session.Transaction.Enlist(command);
                                command.ExecuteNonQuery();
                                sqlBatch = string.Empty;
                            }
                        }
                        else
                        {
                            sqlBatch += line + "\n";
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (var message = new StringWriter())
                    {
                        message.WriteLine("An error occured executing the following sql:");
                        message.WriteLine(sql);
                        message.WriteLine("The error was {0}", ex.Message);

                        throw new Exception(message.ToString(), ex);
                    }
                }
            }
        }
        
        public override void Process(PerformDBOperationExpression expression)
        {
            var session = _transactionManager.GetSession();

            if (expression.Operation != null)
                expression.Operation(session.Connection, null);
        }
    }
}
