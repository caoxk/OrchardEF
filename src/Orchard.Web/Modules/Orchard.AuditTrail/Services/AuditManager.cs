﻿// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

namespace Z.EntityFramework.Plus
{
    /// <summary>Manager for audits.</summary>
    public static class AuditManager
    {
        /// <summary>Static constructor.</summary>
        static AuditManager()
        {
            DefaultConfiguration = new Orchard.AuditTrail.Services.AuditConfiguration.AuditConfiguration();
        }

        /// <summary>Gets or sets the default audit configuration.</summary>
        /// <value>The default audit configuration.</value>
        public static Orchard.AuditTrail.Services.AuditConfiguration.AuditConfiguration DefaultConfiguration { get; set; }
    }
}