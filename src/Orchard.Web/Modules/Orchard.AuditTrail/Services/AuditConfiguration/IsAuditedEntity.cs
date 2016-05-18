﻿// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.


using System.Data.Entity.Core.Objects;

namespace Z.EntityFramework.Plus
{
    public partial class AuditConfiguration
    {
        /// <summary>Checks if the entity is audited.</summary>
        /// <param name="entry">The entry.</param>
        /// <returns>true if the entity is audited, false if not.</returns>
        public bool IsAuditedEntity(ObjectStateEntry entry)
        {
            if (ExcludeIncludeEntityPredicates.Count == 0)
            {
                return true;
            }

            var type = entry.Entity.GetType();
            var key = type.FullName;
            bool value;

            if (!IsAuditedDictionary.TryGetValue(key, out value))
            {
                value = true;

                foreach (var excludeIncludeEntityFunc in ExcludeIncludeEntityPredicates)
                {
                    var maybeIncluded = excludeIncludeEntityFunc(entry.Entity);
                    if (maybeIncluded.HasValue)
                    {
                        value = maybeIncluded.Value;
                    }
                }

                IsAuditedDictionary.TryAdd(key, value);
            }

            return value;
        }
    }
}