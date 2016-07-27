using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CustomClientCredentialHost.Areas.Admin.Models;
using IdentityServer3.Core.Models;
using Microsoft.AspNet.Identity;
using P5.IdentityServer3.Cassandra;
using P5.IdentityServer3.Common;

namespace CustomClientCredentialHost.Areas.NortonDeveloper.Controllers
{
    public class ClientCredentialsController : Controller
    {
        // GET: NortonDeveloper/ClientCredentials
        public async Task<ActionResult> Index()
        {
            var adminStore = new IdentityServer3AdminStore();
            var userId = User.Identity.GetUserId();
            var clients = await adminStore.FindClientIdsByUserAsync(userId);

            return View(clients);
        }
        public async Task<ActionResult> Manage(string id)
        {
            var adminStore = new IdentityServer3AdminStore();
            var userId = User.Identity.GetUserId();
            var client = await adminStore.FindClientByIdAsync(id);

            return View(client);
        }
        /*
         * new Client
                {
                    ClientName = "Silicon-only Client",
                    ClientId = "silicon",
                    Enabled = true,
                    Flow = Flows.ClientCredentials,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes = new List<string>
                    {
                        "api1"
                    },
                    AccessTokenType = AccessTokenType.Reference
                }
         * */
        public async Task<ActionResult> New()
        {
            var client = new Client {ClientId = Guid.NewGuid().ToString()};
            return View(client);
        }

        [HttpPost]
        public async Task<ActionResult> New(Client model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var adminStore = new IdentityServer3AdminStore();
            IdentityServerUser idsUser = new IdentityServerUser() { Enabled = true, UserId = model.UserId };
            await adminStore.CreateIdentityServerUserAsync(idsUser);

            return RedirectToAction("Index");
        }
    }
}