using System.Collections.Generic;

namespace Orchard.Roles.Models {
    public class RoleRecord {
        public RoleRecord() {
            RolesPermissions = new List<RolesPermissionsRecord>();
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public virtual IList<RolesPermissionsRecord> RolesPermissions { get; set; }
    }
}