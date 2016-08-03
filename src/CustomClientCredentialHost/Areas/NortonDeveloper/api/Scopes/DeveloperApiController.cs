using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityServer3.Core.Models;
using Microsoft.AspNet.Identity;
using P5.IdentityServer3.Cassandra;

namespace CustomClientCredentialHost.Areas.NortonDeveloper.api
{

    public partial class DeveloperApiController : ApiController
    {
        [Route("scopes")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetUserScopesAsync( )
        {
            var adminStore = new IdentityServer3AdminStore();
            var userId = User.Identity.GetUserId();
            var scopes = await adminStore.FindScopesByUserAsync(userId);
            return scopes;
        }
        [Route("scopes")]
        [HttpGet]
        public async Task<Scope> GetScopeAsync(string scope)
        {
            var adminStore = new IdentityServer3AdminStore();
            var userId = User.Identity.GetUserId();
            var res = await adminStore.FindScopeByNameAsync(scope);
            return res;
        }
    }
}
