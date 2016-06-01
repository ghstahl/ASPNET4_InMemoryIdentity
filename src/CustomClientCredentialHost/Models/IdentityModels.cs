using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using P5.AspNet.Identity.Cassandra;

namespace CustomClientCredentialHost.Models
{
    public class ApplicationUser : CassandraUser
    {
        public ApplicationUser() : base()
        {
           
        }

        /// <summary>
        /// Creates a new CassandraUser with the Id specified.
        /// </summary>
        public ApplicationUser(Guid userId, Guid tenantId)
            : base(userId, tenantId)
        {
        }

        private ApplicationUser(Guid userId, Guid tenantId, string userName, string email)
            : base(userId, tenantId)
        {
            UserName = userName;
            Email = email;
        }
    }
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
#if ENTITY_ASPNET_IDENTITY
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
      public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
#endif
    
    

  
}