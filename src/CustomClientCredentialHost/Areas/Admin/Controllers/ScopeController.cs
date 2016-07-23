using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CustomClientCredentialHost.Areas.Admin.api;
using CustomClientCredentialHost.Areas.Admin.Models;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Cassandra;

namespace CustomClientCredentialHost.Areas.Admin.Controllers
{
    public class ScopeController : Controller
    {
        // GET: Admin/Scope
        public async Task<ActionResult> Index()
        {
            var adminStore = new IdentityServer3AdminStore();

            int pageSize = 100;
            var page = await adminStore.PageScopesAsync(100, null);
            var state = HttpUtility.UrlEncode(page.PagingState);
            var record = new IDSScopePageRecord()
            {
                CurrentPagingState = HttpUtility.UrlEncode(page.CurrentPagingState),
                PageSize = pageSize,
                PagingState = HttpUtility.UrlEncode(page.PagingState),
                Scopes = page
            };
        

            return View(record);
        }

        public async Task<ActionResult> New()
        {
            var scope = new ScopeNewModel();
            
            return View(scope);
        }
        // GET: Admin/Home/Delete
        public async Task<ActionResult> Delete(string name)
        {
            var adminStore = new IdentityServer3AdminStore();
            var scope = await adminStore.FindScopeByNameAsync(name);
            if (scope == null)
            {
                return RedirectToAction("Index");

            }
            return View(scope);
        }

        // POST: Admin/Home/Create
        [HttpPost]
        public async Task<ActionResult> Delete(Scope scope)
        {
            try
            {
                var adminStore = new IdentityServer3AdminStore();
              //  await adminStore.DeleteScopesFromClientAsync()
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}