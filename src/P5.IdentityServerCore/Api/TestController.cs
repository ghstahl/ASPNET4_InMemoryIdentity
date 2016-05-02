using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace P5.IdentityServerCore.Api
{
    [Route("test")]
    public class TestController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Json(new
            {
                message = "Hello from "+this.GetType().Name
            });
        }
    }
}
