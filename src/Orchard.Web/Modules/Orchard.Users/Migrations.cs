using System;
using Orchard.Data.Migration;

namespace Orchard.Users {
    public class UsersDataMigration : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.Create
                .Table("UserPartRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("UserName").AsString()
                .WithColumn("Email").AsString()
                .WithColumn("NormalizedUserName").AsString()
                .WithColumn("Password").AsString()
                .WithColumn("PasswordFormat").AsString()
                .WithColumn("HashAlgorithm").AsString()
                .WithColumn("PasswordSalt").AsString()
                .WithColumn("RegistrationStatus").AsString().WithDefaultValue("Approved")
                .WithColumn("EmailStatus").AsString().WithDefaultValue("Approved")
                .WithColumn("EmailChallengeToken").AsString()
                .WithColumn("CreatedUtc").AsDateTime()
                .WithColumn("LastLoginUtc").AsDateTime()
                .WithColumn("LastLogoutUtc").AsDateTime();
            return 4;
        }

        public int UpdateFrom1() {

            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder.Alter
                .Table("UserPartRecord")
                .AddColumn("CreatedUtc").AsDateTime()
                .AddColumn("LastLoginUtc").AsDateTime();

            return 3;
        }

        public int UpdateFrom3() {
            SchemaBuilder.Alter
                .Table("UserPartRecord")
                .AddColumn("LastLogoutUtc").AsDateTime();

            return 4;
        }
    }
}