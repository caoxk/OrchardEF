using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace Orchard.Data.Migration
{
    public class MigrationContext : IMigrationContext
    {
        public virtual IMigrationConventions Conventions { get; set; }
        public virtual ICollection<IMigrationExpression> Expressions { get; set; }
        public virtual IQuerySchema QuerySchema { get; set; }
        public virtual IAssemblyCollection MigrationAssemblies { get; set; }

        /// <summary>The arbitrary application context passed to the task runner.</summary>
        public virtual object ApplicationContext { get; set; }

        /// <summary>
        /// Connection String from the runner.
        /// </summary>
        public string Connection { get; set; }

        public MigrationContext()
        {
            Expressions = new List<IMigrationExpression>();
        }
    }
}
