using System.ComponentModel.DataAnnotations;
using Orchard.Users.Models;

namespace Orchard.Users.ViewModels {
    public class UserEditViewModel  {
        [Required]
        public string UserName {
            get { return User.UserName; }
            set { User.UserName = value; }
        }

        [Required]
        public string Email {
            get { return User.Email; }
            set { User.Email = value; }
        }

        public UserPartRecord User { get; set; }
    }
}