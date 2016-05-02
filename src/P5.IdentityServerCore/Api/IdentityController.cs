using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace P5.IdentityServerCore.Api
{
    [Authorize]
    [Route("identity")]
    public class IdentityController : ApiController
    {
        public dynamic Get()
        {
            var principal = User as ClaimsPrincipal;

            return from c in principal.Identities.First().Claims
                select new
                {
                    c.Type,
                    c.Value
                };
        }
    }
}