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

using FluentMigrator;
using Orchard.Data.Migration.Generators.SqlServer;

namespace Orchard.Data.Migration.Processors.SqlServer
{
    public class SqlServerCeProcessorFactory : MigrationProcessorFactory
    {
        private readonly ITransactionManager _transactionManager;
        public SqlServerCeProcessorFactory(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }
        public override IMigrationProcessor Create(IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var factory = new SqlServerCeDbFactory();
            //var connection = factory.CreateConnection(connectionString);
            return new SqlServerCeProcessor(_transactionManager, new SqlServerCeGenerator(), announcer, options, factory);
        }
    }
}