using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace P5.AspNet.Identity.Biggy
{
    public class User : IUser<int>
    {
        public User()
        {
            this.Roles = new List<string>();
            this.Claims = new List<UserClaim>();
            this.Logins = new List<UserLoginInfo>();
        }

        public User(string userName)
            : this()
        {
            this.UserName = userName;
        }

        public User(int id, string userName)
            : this()
        {
            this.Id = Id;
            this.UserName = userName;
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public bool LockoutEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool TwoFactorEnabled { get; set; }

        public IList<string> Roles { get; private set; }
        public IList<UserClaim> Claims { get; private set; }
        public List<UserLoginInfo> Logins { get; private set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}