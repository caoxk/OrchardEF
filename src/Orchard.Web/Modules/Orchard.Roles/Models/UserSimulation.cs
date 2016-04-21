using System.Collections.Generic;
using Orchard.Security;

namespace Orchard.Roles.Models {
    public static class UserSimulation {
        public static IUser Create(string role) {
            var user = new SimulatedUser {
                Roles = new[] {role}
            };
            return user;
        }

        class SimulatedUser : IUser, IUserRoles
        {
            public int Id { get { return 0; } }
            public string UserName { get { return null; } }
            public string Email { get { return null; } }
            public IList<string> Roles { get; set; }
        }
    }
}
