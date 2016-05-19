// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.


using System.Data.Entity.Core.Objects;

namespace Orchard.AuditTrail.Services
{
    public partial class AuditConfiguration
    {
        /// <summary>Gets the state of the entity modified (EntityModified, EntitySoftAdded, EntitySoftDeleted).</summary>
        /// <param name="entry">The entry.</param>
        /// <returns>The state of the entity modified (EntityModified, EntitySoftAdded, EntitySoftDeleted).</returns>
        public AuditEntryState GetEntityModifiedState(ObjectStateEntry entry)
        {
            // CHECK for soft added
            foreach (var softAddedFuncs in SoftAddedPredicates)
            {
                if (softAddedFuncs(entry.Entity))
                {
                    return AuditEntryState.EntitySoftAdded;
                }
            }

            // CHECK for soft deleted
            foreach (var softDeleteFunc in SoftDeletedPredicates)
            {
                if (softDeleteFunc(entry.Entity))
                {
                    return AuditEntryState.EntitySoftDeleted;
                }
            }

            return AuditEntryState.EntityModified;
        }
    }
}