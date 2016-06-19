using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CustomClientCredentialHost.Models;
using Microsoft.AspNet.Identity.Owin;
using P5.AspNet.Identity.Cassandra;

namespace CustomClientCredentialHost.Areas.IdentityAdmin.Controllers
{
    public class HomeController : AsyncController
    {
        private ApplicationUserManager _userManager;
        private const string MasterBlasterEmail = "20cb8386-de23-4333-aa74-07e3a64ec064@masterblaster.com";
 

        private async Task<CassandraUser> FetchMasterUser()
        {
            var user = await UserManager.FindByEmailAsync(MasterBlasterEmail);
            if (user == null)
            {
                user = new ApplicationUser()
                {
                    UserName = MasterBlasterEmail,
                    Email = MasterBlasterEmail,
                    IsEmailConfirmed = true
                };
                await UserManager.CreateAsync(user);
            }
            return user;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public HomeController( )
        {
            
        }
        public HomeController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }
        // GET: IdentityAdmin/Home
        public async Task<ActionResult> Index(string remaining)
        {
            var user = await FetchMasterUser();
            return await Task.FromResult(View(Request));
        }
    }
}