using Orchard.Data.Migration;
using Orchard.Roles.Services;

namespace Orchard.Roles {
    public class RolesDataMigration : DataMigrationImpl {
        private readonly IRoleService _roleService;

        public RolesDataMigration(IRoleService roleService) {
            _roleService = roleService;
        }

        public int Create() {
            SchemaBuilder.Create
                .Table("PermissionRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString()
                .WithColumn("FeatureName").AsString()
                .WithColumn("Description").AsString();

            SchemaBuilder.Create
                .Table("RoleRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString();

            SchemaBuilder.Create
                .Table("RolesPermissionsRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Role_id").AsInt32()
                .WithColumn("Permission_id").AsInt32()
                .WithColumn("RoleRecord_Id").AsInt32();

            SchemaBuilder.Create
                .Table("UserRolesPartRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32()
                .WithColumn("Role_id").AsInt32();

            return 2;
        }

        public int UpdateFrom1() {

            // creates default permissions for Orchard v1.4 instances and earlier
            _roleService.CreatePermissionForRole("Anonymous", CommonPermissions.ViewContent.Name);
            _roleService.CreatePermissionForRole("Authenticated", CommonPermissions.ViewContent.Name);

            return 2;
        }
    }
}