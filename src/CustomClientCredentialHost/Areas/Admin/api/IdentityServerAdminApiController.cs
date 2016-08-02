using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using CustomClientCredentialHost.Areas.Admin.Models;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Cassandra;

namespace CustomClientCredentialHost.Areas.Admin.api
{
    [RoutePrefix("api/v1/IDSAdmin")]
    public partial class IdentityServerAdminApiController : ApiController
    {
       
        [Authorize]
        [Route("who")]
        [HttpGet]
        public async Task<object> WhoMe()
        {
            var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var principal = User as ClaimsPrincipal;
            if (claimsIdentity != null)
            {
                 return from c in claimsIdentity.Claims
                   select new
                   {
                       c.Type,
                       c.Value
                   };
            }
            return new{};
        }

    }
}