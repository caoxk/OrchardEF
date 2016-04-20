namespace Orchard.Users.Models {
    public class RegistrationSettingsPart {
        public bool UsersCanRegister {
            get;set;
        }

        public bool UsersMustValidateEmail {
            get; set;
        }

        public string ValidateEmailRegisteredWebsite {
            get; set;
        }
        
        public string ValidateEmailContactEMail {
            get; set;
        }

        public bool UsersAreModerated {
            get; set;
        }

        public bool NotifyModeration {
            get; set;
        }

        public string NotificationsRecipients {
            get; set;
        }

        public bool EnableLostPassword {
            get; set;
        }

    }
}