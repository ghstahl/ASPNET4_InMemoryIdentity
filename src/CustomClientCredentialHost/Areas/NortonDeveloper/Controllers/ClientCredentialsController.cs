using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CustomClientCredentialHost.Areas.Admin.Models;
using CustomClientCredentialHost.Areas.NortonDeveloper.Models;
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
            ClientViewModel cvModel = new ClientViewModel()
            {
                AccessTokenType = client.AccessTokenType,
                AllowedScopes = client.AllowedScopes,
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                ClientSecrets = client.ClientSecrets,
                Enabled = client.Enabled,
                Flow = client.Flow
            };
            return View(cvModel);
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
            var client = new ClientViewModel { ClientId = Guid.NewGuid().ToString(), Flow = Flows.ClientCredentials,AccessTokenType = AccessTokenType.Jwt};
            return View(client);
        }

        [HttpPost]
        public async Task<ActionResult> New(ClientViewModel model)
        {
            if (!ModelState.IsValid)
            {/*
                ClientViewModel cvModel = new ClientViewModel()
                {
                    AccessTokenType = model.AccessTokenType,
                    AllowedScopes = model.AllowedScopes,
                    ClientId = model.ClientId,
                    ClientName = model.ClientName,
                    ClientSecrets = model.ClientSecrets,
                    Enabled = model.Enabled,
                    Flow = model.Flow
                };
                */
                return View(model);
            }

            Client client = new Client()
            {
                AccessTokenType = model.AccessTokenType,
                Enabled = model.Enabled,
                AllowedScopes = model.AllowedScopes,
                ClientId = model.ClientId,
                ClientName = model.ClientName,
                ClientSecrets = model.ClientSecrets,
                Flow = model.Flow
            };
            var adminStore = new IdentityServer3AdminStore();
            await adminStore.CreateClientAsync(client);
            var clients = new List<string> {client.ClientId};
            await adminStore.AddClientIdToIdentityServerUserAsync(User.Identity.GetUserId(), clients);

            return RedirectToAction("Index");
        }
    }
}