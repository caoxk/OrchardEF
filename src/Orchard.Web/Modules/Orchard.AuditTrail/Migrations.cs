using System;
using Orchard.Data.Migration;

namespace Orchard.AuditTrail {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            

            return 2;
        }

        public int UpdateFrom1() {
            return 2;
        }
    }
}