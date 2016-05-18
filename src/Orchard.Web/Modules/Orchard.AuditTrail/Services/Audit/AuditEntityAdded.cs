// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Data.Entity.Core.Objects;
using Z.EntityFramework.Plus;

namespace Orchard.AuditTrail.Services.Audit
{
    public partial class Audit
    {
        /// <summary>Audit entity added.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>
        public static void AuditEntityAdded(Audit audit, ObjectStateEntry objectStateEntry)
        {
            var entry = new AuditEntry(audit, objectStateEntry)
            {
                State = AuditEntryState.EntityAdded
            };


            // CHECK if the key should be resolved in POST Action
            if (objectStateEntry.EntityKey.IsTemporary)
            {
                entry.DelayedKey = objectStateEntry;
            }
            AuditEntityAdded(entry, objectStateEntry.CurrentValues);

            audit.Entries.Add(entry);
        }

        /// <summary>Audit entity added.</summary>
        /// <param name="auditEntry">The audit entry.</param>
        /// <param name="record">The record.</param>
        /// <param name="prefix">The prefix.</param>
        public static void AuditEntityAdded(AuditEntry auditEntry, DbUpdatableDataRecord record, string prefix = "")
        {
            for (var i = 0; i < record.FieldCount; i++)
            {
                var name = record.GetName(i);
                var value = record.GetValue(i);

                var valueRecord = value as DbUpdatableDataRecord;
                if (valueRecord != null)
                {
                    // Complex Type
                    AuditEntityAdded(auditEntry, valueRecord, string.Concat(prefix, name, "."));
                }
                else if (auditEntry.Parent.CurrentOrDefaultConfiguration.IsAuditedProperty(auditEntry.Entry, name))
                {
                    auditEntry.Properties.Add(new AuditEntryProperty(auditEntry, string.Concat(prefix, name), null, value));
                }
            }
        }

    }
}