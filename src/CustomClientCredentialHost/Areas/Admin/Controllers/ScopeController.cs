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
        [HttpPost]
        public async Task<ActionResult> New(ScopeNewModel model)
        {
            try
            {
                Scope scope = new Scope { Name = model.Name,Type = model.SelectedScopeType,Enabled = true};

                var adminStore = new IdentityServer3AdminStore();
                var scopeCheck = await adminStore.FindScopeByNameAsync(scope.Name);
                if (scopeCheck == null)
                {
                    await adminStore.CreateScopeAsync(scope);
                    // Good doesn't exist
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, string.Format("The scope, {0}, already exists.",scope.Name));
                return View(model);
            }
            catch
            {
                return View();
            }
        }
        public async Task<ActionResult> Edit(string name)
        {
            var adminStore = new IdentityServer3AdminStore();
            var scope = await adminStore.FindScopeByNameAsync(name);
            if (scope == null)
            {
                return RedirectToAction("Index");

            }
            ScopeNewModel snm = scope.ToScopeNewModel();
            return View(snm);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(ScopeNewModel model)
        {
            try
            {
                Scope scope = new Scope { Name = model.Name, Type = model.SelectedScopeType, Enabled = true };

                var adminStore = new IdentityServer3AdminStore();
                await adminStore.CreateScopeAsync(scope);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
            ScopeViewModel model = new ScopeViewModel
            {
                Name = scope.Name,
                Description = scope.Description,
                ScopeType = scope.Type
            };
            return View(model);
        }

        // POST: Admin/Home/Create
        [HttpPost]
        public async Task<ActionResult> Delete(ScopeViewModel model)
        {
            try
            {
                Scope scope = new Scope {Name = model.Name};

                var adminStore = new IdentityServer3AdminStore();
                await adminStore.DeleteScopeAsync(scope);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}