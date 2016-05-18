﻿// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using Z.EntityFramework.Plus;

namespace Orchard.AuditTrail.Services.Audit
{
    public partial class Audit
    {
        /// <summary>Audit relationship added.</summary>
        /// <param name="audit">The audit to use to add changes made to the context.</param>
        /// <param name="objectStateEntry">The object state entry.</param>

        public static void AuditRelationAdded(Orchard.AuditTrail.Services.Audit.Audit audit, ObjectStateEntry objectStateEntry)
        {
            var entry = new AuditEntry(audit, objectStateEntry)
            {
                State = AuditEntryState.RelationshipAdded
            };

            var values = objectStateEntry.CurrentValues;


            var leftKeys = (EntityKey) values.GetValue(0);
            var rightKeys = (EntityKey) values.GetValue(1);

            if (leftKeys.IsTemporary || rightKeys.IsTemporary)
            {
                entry.DelayedKey = objectStateEntry;
            }
            else
            {
                var leftRelationName = values.GetName(0);
                var rightRelationName = values.GetName(1);

                foreach (var keyValue in leftKeys.EntityKeyValues)
                {
                    entry.Properties.Add(new AuditEntryProperty(entry, leftRelationName, keyValue.Key, null, keyValue.Value));
                }

                foreach (var keyValue in rightKeys.EntityKeyValues)
                {
                    entry.Properties.Add(new AuditEntryProperty(entry, rightRelationName, keyValue.Key, null, keyValue.Value));
                }
            }

            audit.Entries.Add(entry);
        }
    }
}