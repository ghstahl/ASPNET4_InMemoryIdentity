using System.Threading.Tasks;
using System.Web.Mvc;

namespace P5.IdentityServer3.Admin.Areas.IdentityServer3Admin.Controllers
{
    public class HomeController : AsyncController
    {
        public async Task<ActionResult> Index(string remaining)
        {
            return await Task.FromResult(View(Request));
        }
    }
}
