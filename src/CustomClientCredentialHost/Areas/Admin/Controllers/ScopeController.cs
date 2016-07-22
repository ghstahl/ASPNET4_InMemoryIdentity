using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CustomClientCredentialHost.Areas.Admin.api;
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
    }
}