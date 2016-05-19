// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

namespace Orchard.AuditTrail.Services
{
    public partial class Audit
    {
        /// <summary>Audit relationship deleted.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>

        public static void AuditRelationDeleted(Audit audit, ObjectStateEntry objectStateEntry)
        {
            var entry = new AuditEntry(audit, objectStateEntry)
            {
                State = AuditEntryState.RelationshipDeleted
            };

            var values = objectStateEntry.OriginalValues;
            for (var i = 0; i < values.FieldCount; i++)
            {
                var relationName = values.GetName(i);
                var value = (EntityKey) values.GetValue(i);
                foreach (var keyValue in value.EntityKeyValues)
                {
                    entry.Properties.Add(new AuditEntryProperty(entry, relationName, keyValue.Key, keyValue.Value, null));
                }
            }

            audit.Entries.Add(entry);
        }
    }
}