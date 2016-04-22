﻿#region License

// Copyright (c) 2007-2009, Sean Chambers <schambers80@gmail.com>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orchard.Data.Migration.Extensions;

namespace Orchard.Data.Migration.Processors
{
    public interface IMigrationProcessorFactoryProvider: IDependency {
        IMigrationProcessorFactory GetFactory(string name);
    }

    public class MigrationProcessorFactoryProvider : IMigrationProcessorFactoryProvider {
        private readonly IEnumerable<IMigrationProcessorFactory> _migrationProcessorFactories; 

        public MigrationProcessorFactoryProvider(IEnumerable<IMigrationProcessorFactory> migrationProcessorFactories) {
            _migrationProcessorFactories = migrationProcessorFactories;
        }

        public virtual IMigrationProcessorFactory GetFactory(string name)
        {
            return _migrationProcessorFactories
                .Where(pair => pair.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .Select(pair => pair)
                .FirstOrDefault();
        }
    }
}