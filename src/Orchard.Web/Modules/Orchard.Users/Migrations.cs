using System;
using Orchard.Data.Migration;

namespace Orchard.Users {
    public class UsersDataMigration : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.Create
                .Table("Orchard_Users_UserPartRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("UserName").AsString().Nullable()
                .WithColumn("Email").AsString().Nullable()
                .WithColumn("NormalizedUserName").AsString().Nullable()
                .WithColumn("Password").AsString()
                .WithColumn("PasswordFormat").AsString()
                .WithColumn("HashAlgorithm").AsString()
                .WithColumn("PasswordSalt").AsString()
                .WithColumn("RegistrationStatus").AsString().WithDefaultValue("Approved")
                .WithColumn("EmailStatus").AsString().WithDefaultValue("Approved")
                .WithColumn("EmailChallengeToken").AsString().Nullable()
                .WithColumn("CreatedUtc").AsDateTime()
                .WithColumn("LastLoginUtc").AsDateTime().Nullable()
                .WithColumn("LastLogoutUtc").AsDateTime().Nullable();
            return 4;
        }

        public int UpdateFrom1() {

            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder.Alter
                .Table("Orchard_Users_UserPartRecord")
                .AddColumn("CreatedUtc").AsDateTime()
                .AddColumn("LastLoginUtc").AsDateTime().Nullable();

            return 3;
        }

        public int UpdateFrom3() {
            SchemaBuilder.Alter
                .Table("Orchard_Users_UserPartRecord")
                .AddColumn("LastLogoutUtc").AsDateTime().Nullable();

            return 4;
        }
    }
}