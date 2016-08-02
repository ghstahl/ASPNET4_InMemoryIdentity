using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using CustomClientCredentialHost.Areas.Admin.Models;
using IdentityServer3.Core.Models;
using Microsoft.AspNet.Identity;
using P5.IdentityServer3.Cassandra;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;

namespace CustomClientCredentialHost.Areas.Admin.api
{
    public partial class IdentityServerAdminApiController : ApiController
    {

        [Route("users/create")]
        [HttpPost]
        public async Task CreateIdentityServerUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            var fullUserStore = UserManager.FullUserStore;
            var user = await fullUserStore.FindByIdAsync(Guid.Parse(userId));
            if (user == null)
            {
                throw new UserDoesNotExitException();
            }
            var adminStore = new IdentityServer3AdminStore();
            var idsUserExists = await adminStore.FindDoesUserExistByUserIdAsync(userId);
            if (!idsUserExists)
            {
                var claim = new Claim(ClaimTypes.Role, "Developer");
                await fullUserStore.AddClaimAsync(user, claim);
                var idsUser = new IdentityServerUser() { Enabled = true, UserId = userId };
                await adminStore.CreateIdentityServerUserAsync(idsUser);
            }
        }
        [Route("users/update/enabled")]
        [HttpPost]
        public async Task UpdateIdentityServerUser(string userId, bool enabled)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }
            var adminStore = new IdentityServer3AdminStore();
            var user = await adminStore.FindIdentityServerUserByUserIdAsync(userId);
            if (user != null)
            {
                user.Enabled = enabled;
                await adminStore.CreateIdentityServerUserAsync(user);
            }
        }

        [Route("users")]
        [HttpGet]
        public async Task<IdentityServerUser> GetIdentityServerUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }
            var adminStore = new IdentityServer3AdminStore();
            var user = await adminStore.FindIdentityServerUserByUserIdAsync(userId);
            return user;
        }

        [Route("users")]
        [HttpDelete]
        public async Task DeleteIdentityServerUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }
            var adminStore = new IdentityServer3AdminStore();
            await adminStore.DeleteIdentityServerUserAsync(userId);
        }
    }
}
