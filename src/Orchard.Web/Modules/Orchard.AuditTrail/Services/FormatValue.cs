﻿// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace Orchard.AuditTrail.Services
{
    public partial class AuditConfiguration
    {
        /// <summary>Format a value for the specified entry and property name.</summary>
        /// <param name="entry">The entry.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The formatted value.</returns>
        public string FormatValue(ObjectStateEntry entry, string propertyName, object currentValue)
        {
            if (EntityValueFormatters.Count > 0)
            {
                if (entry.State != EntityState.Detached && entry.Entity != null)
                {
                    var type = entry.Entity.GetType();
                    var key = string.Concat(type.FullName, ";", propertyName);
                    Func<object, object> formatter;

                    if (!ValueFormatterDictionary.TryGetValue(key, out formatter))
                    {
                        if (EntityValueFormatters.Count > 0)
                        {
                            foreach (var formatProperty in EntityValueFormatters)
                            {
                                formatter = formatProperty(entry.Entity, propertyName);

                                if (formatter != null)
                                {
                                    break;
                                }
                            }
                        }

                        ValueFormatterDictionary.TryAdd(key, formatter);
                    }

                    if (formatter != null)
                    {
                        currentValue = formatter(currentValue);
                    }
                }
            }

            return currentValue != null ? currentValue.ToString() : null;
        }
    }
}