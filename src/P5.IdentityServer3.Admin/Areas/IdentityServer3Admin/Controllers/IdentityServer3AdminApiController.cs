using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace P5.IdentityServer3.Admin.Areas.IdentityServer3Admin.Controllers
{
    [RoutePrefix("v1/IdentityServer3Admin")]
    public class IdentityServer3AdminApiController : ApiController
    {

        public IdentityServer3AdminApiController()
        {
           
        }


        // GET /CountryEmbargoInformation
        [Route("Test")]
        public async Task<IHttpActionResult> GetTest()
        {
            var result = new CountryInformationResponse
            {
                CountryCode = "USA",
                isEmbargoed = true
            };
            return Ok(result);
        }
    }
}