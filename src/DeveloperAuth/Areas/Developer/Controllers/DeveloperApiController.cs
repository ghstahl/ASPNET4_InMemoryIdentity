using System.Threading.Tasks;
using System.Web.Http;
using P5.IdentityServer3.Admin.Areas.IdentityServer3Admin.Controllers;

namespace DeveloperAuth.Areas.Developer.Controllers
{
    [System.Web.Mvc.RoutePrefix("v1/Developer")]
    public class DeveloperApiController : ApiController
    {

        public DeveloperApiController()
        {

        }


        // GET /CountryEmbargoInformation
        [System.Web.Mvc.Route("Test")]
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