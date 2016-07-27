using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CustomClientCredentialHost.Areas.Admin.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using P5.IdentityServer3.Cassandra;
using P5.IdentityServer3.Common;
using IdentityServerUserModel = CustomClientCredentialHost.Areas.Admin.Models.IdentityServerUserModel;

namespace CustomClientCredentialHost.Areas.Admin.Controllers
{
    public class UserScopeRecord
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }
    public class UserScopeModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<UserScopeRecord> UserScopeRecords { get; set; }
        public List<UserScopeRecord> AllowedScopes { get; set; }
    }
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        // GET: Admin/Home
        public async Task<ActionResult> Index()
        {
            var fullUserStore = UserManager.FullUserStore;

            int pageSize = 100;
            var page = await fullUserStore.PageUsersAsync(pageSize, null);
            var enumerable = page.AsEnumerable();

            var record = new AspNetIdentityPageRecord()
            {
                CurrentPagingState = HttpUtility.UrlEncode(page.CurrentPagingState),
                PageSize = pageSize,
                PagingState = HttpUtility.UrlEncode(page.PagingState),
                Users = page
            };
            return View(record);
        }

        [HttpPost]
        public async Task<ActionResult> Scopes(UserScopeModel model)
        {
            var fullUserStore = UserManager.FullUserStore;
            var adminStore = new IdentityServer3AdminStore();
            if (model.UserScopeRecords != null)
            {
                // remove the ones that need to be removed
                var queryToBeDeleted = (from item in model.UserScopeRecords
                    where item.Enabled == false
                    select item.Name).ToList();
                await adminStore.DeleteScopesByUserIdAsync(model.UserId, queryToBeDeleted);
            }

            var queryToBeAdded = (from item in model.AllowedScopes
                where item.Enabled
                select item.Name).ToList();
            // add the ones that need to be added.
            if (queryToBeAdded.Any())
            {
                await adminStore.AddScopesToIdentityServerUserAsync(model.UserId, queryToBeAdded);
            }
            
            return RedirectToAction("Index");
        }

        // GET: Admin/Home/Manage/5
        public async Task<ActionResult> Scopes(string id, string email)
        {
            // note this is POC.  We need a dynamic ajax page that does paging
            var usm = new UserScopeModel
            {
                AllowedScopes = new List<UserScopeRecord>(),
                UserScopeRecords = new List<UserScopeRecord>(),
                Email = email,
                UserId = id
            };

            var fullUserStore = UserManager.FullUserStore;
            var adminStore = new IdentityServer3AdminStore();
            var userScopes = await adminStore.FindScopesByUserAsync(id);
            foreach (var scope in userScopes)
            {
                usm.UserScopeRecords.Add(new UserScopeRecord()
                {
                    Enabled = true,Name = scope

                });
            }
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
            foreach (var scope in page)
            {
                usm.AllowedScopes.Add(new UserScopeRecord()
                {
                    Enabled = false,
                    Name = scope.Name

                });
            }
            return View(usm);
        }

        // GET: Admin/Home/Manage/5
        public async Task<ActionResult> Manage(string id, string email)
        {
            var fullUserStore = UserManager.FullUserStore;
            var claims = await fullUserStore.GetClaimsAsync(Guid.Parse(id));
            var query = from item in claims
                where item.Type == "http://schemas.identityserver.org/ws/2008/06/identity/claims/accesstype" && item.Value == "Developer"
                select item;

            var adminStore = new IdentityServer3AdminStore();
            ViewBag.Email = email;
            var exists = await adminStore.FindDoesUserExistByUserIdAsync(id);
            if (exists)
            {
                var scopes = await adminStore.FindScopesByUserAsync(id);
            }
            IdentityServerUserModel isum = new IdentityServerUserModel()
            {
                UserId = id,
                Exists = exists,
                AllowedScopes = new List<string>(),
                Developer = query.Any()
            };

            return View(isum);
        }
        [HttpPost]
        public async Task<ActionResult> Manage(IdentityServerUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var fullUserStore = UserManager.FullUserStore;
            var user = await fullUserStore.FindByIdAsync(Guid.Parse(model.UserId));
            var claim = new Claim("http://schemas.identityserver.org/ws/2008/06/identity/claims/accesstype", "Developer");
            await fullUserStore.AddClaimAsync(user, claim);

            var adminStore = new IdentityServer3AdminStore();
            IdentityServerUser idsUser = new IdentityServerUser() {Enabled = true, UserId = model.UserId};
            await adminStore.CreateIdentityServerUserAsync(idsUser);

            return RedirectToAction("Index");
        }
        // GET: Admin/Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Home/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Home/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Home/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Home/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
