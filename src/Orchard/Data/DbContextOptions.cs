using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data
{
    public class DbContextOptions {
        public string ConnectionString { get; set; }
        public DbCompiledModel Model {get;set;}
    }
}
