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
                .Table("Orchard_Roles_PermissionRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString()
                .WithColumn("FeatureName").AsString()
                .WithColumn("Description").AsString().Nullable();

            SchemaBuilder.Create
                .Table("Orchard_Roles_RoleRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString();

            SchemaBuilder.Create
                .Table("Orchard_Roles_RolesPermissionsRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Role_id").AsInt32()
                .WithColumn("Permission_Id").AsInt32().Nullable()
                .WithColumn("RoleRecord_Id").AsInt32().Nullable();

            SchemaBuilder.Create
                .Table("Orchard_Roles_UserRolesPartRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32().Nullable()
                .WithColumn("Role_Id").AsInt32().Nullable();

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