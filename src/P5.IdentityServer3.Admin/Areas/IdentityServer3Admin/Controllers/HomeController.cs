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

        public void ScopesAsync(string id, string email)
        {
            throw new System.NotImplementedException();
        }

        public ActionResult ScopesCompleted()
        {
            throw new System.NotImplementedException();
        }
    }
}
