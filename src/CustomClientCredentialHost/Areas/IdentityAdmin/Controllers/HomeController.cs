using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CustomClientCredentialHost.Areas.IdentityAdmin.Controllers
{
    public class HomeController : AsyncController
    {
        // GET: IdentityAdmin/Home
        public async Task<ActionResult> Index(string remaining)
        {
            return await Task.FromResult(View(Request));
        }
    }
}