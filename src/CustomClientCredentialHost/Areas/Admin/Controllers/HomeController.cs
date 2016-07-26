using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CustomClientCredentialHost.Areas.Admin.Models;
using Microsoft.AspNet.Identity.Owin;
using P5.IdentityServer3.Cassandra;
using P5.IdentityServer3.Common;
using IdentityServerUserModel = CustomClientCredentialHost.Areas.Admin.Models.IdentityServerUserModel;

namespace CustomClientCredentialHost.Areas.Admin.Controllers
{
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



        // GET: Admin/Home/Manage/5
        public async Task<ActionResult> Manage(string id, string email)
        {
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
                AllowedScopes = new List<string>()
            };

            return View(isum);
        }
        [HttpPost]
        public async Task<ActionResult> Manage(IdentityServerUserModel model)
        {
            if (model.Exists == false)
            {
                ModelState.AddModelError(string.Empty,  "Please check the Exists.");
                return View(model);
            }
            var adminStore = new IdentityServer3AdminStore();
            IdentityServerUser user = new IdentityServerUser() {Enabled = true, UserId = model.UserId};
            await adminStore.CreateIdentityServerUserAsync(user);


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
