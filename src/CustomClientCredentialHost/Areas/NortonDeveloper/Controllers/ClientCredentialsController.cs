using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CustomClientCredentialHost.Areas.Admin.Controllers;
using CustomClientCredentialHost.Areas.Admin.Models;
using CustomClientCredentialHost.Areas.NortonDeveloper.Models;
using IdentityServer3.Core.Models;
using Microsoft.AspNet.Identity;
using P5.IdentityServer3.Cassandra;
using P5.IdentityServer3.Cassandra.Crypto;
using P5.IdentityServer3.Common;

namespace CustomClientCredentialHost.Areas.NortonDeveloper.Controllers
{
    public class ScopeEnabledRecord
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }
    public class ClientScopeModel
    {
        public string ClientId { get; set; }

        public List<ScopeEnabledRecord> UserScopes { get; set; }
        public List<ScopeEnabledRecord> AllowedScopes { get; set; }
    }
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

        [HttpPost]
        public async Task<ActionResult> Manage(ClientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var adminStore = new IdentityServer3AdminStore();
            var client = await adminStore.FindClientByIdAsync(model.ClientId);
            client.ClientName = model.ClientName;
            client.Enabled = model.Enabled;

            await adminStore.CreateClientAsync(client);
            var clients = new List<string> { client.ClientId };
            await adminStore.AddClientIdToIdentityServerUserAsync(User.Identity.GetUserId(), clients);

            return RedirectToAction("Index");
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
        public async Task<ActionResult> Delete(string clientId)
        {
            var adminStore = new IdentityServer3AdminStore();
            var client = await adminStore.FindClientByIdAsync(clientId);

            var clientVM = new ClientViewModel
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName
            };
            return View(clientVM);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(ClientViewModel model)
        {
            var adminStore = new IdentityServer3AdminStore();
            await
                adminStore.DeleteClientIdsByUserIdAsync(User.Identity.GetUserId(), new List<string>() {model.ClientId});
            await adminStore.DeleteClientAsync(model.ClientId);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> New()
        {
            var client = new ClientViewModel
            {
                ClientId = Guid.NewGuid().ToString(),
                Enabled = true,
                Flow = Flows.ClientCredentials,
                AccessTokenType = AccessTokenType.Jwt
            };
            return View(client);
        }
        [HttpPost]
        public async Task<ActionResult> New(ClientViewModel model)
        {
            if (!ModelState.IsValid)
            {
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
        public async Task<ActionResult> ShowSecret(string clientId,string hash)
        {
            var model = new ShowSecretViewModel()
            {
                ClientId = clientId,
                Hash = hash,
                OpenSecret = "hidden for now"
            };
            return View(model);
        }
        public async Task<ActionResult> ShowOpenSecret(string clientId, string hash, string passCode, string openSecret)
        {
            var model = new ShowSecretViewModel()
            {
                ClientId = clientId,
                Hash = hash,
                OpenSecret = openSecret,
                PassCode = passCode
            };
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> ShowSecret(ShowSecretViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var adminStore = new IdentityServer3AdminStore();
            ProtectedSecretQueryValues queryValues = new ProtectedSecretQueryValues()
            {
                ClientId = model.ClientId,
                Value = model.Hash
            };
            var record = await adminStore.FindSecretProtectedValue(queryValues);
            var myCrypto = new TripleDesEncryption(model.PassCode);
            model.OpenSecret = myCrypto.Decrypt(record.ProtectedValue);
            return RedirectToAction("ShowOpenSecret",
                new { clientId = model.ClientId,hash = model.Hash, openSecret = model.OpenSecret, passCode = model.PassCode });

        }

        public async Task<ActionResult> ManageScopes(string clientId)
        {
            var adminStore = new IdentityServer3AdminStore();
            var userScopes = await adminStore.FindScopesByUserAsync(User.Identity.GetUserId());
            var queryUserScopes = from item in userScopes
                let c = new ScopeEnabledRecord()
                {
                    Enabled = false,
                    Name = item
                }
                select c;

            var currentClient = await adminStore.FindClientByIdAsync(clientId);

            var queryAllowedScopes = from item in currentClient.AllowedScopes
                                  let c = new ScopeEnabledRecord()
                                  {
                                      Enabled = true,
                                      Name = item
                                  }
                                  select c;

            var model = new ClientScopeModel()
            {
                ClientId = clientId,
                UserScopes = queryUserScopes.ToList(),
                AllowedScopes = queryAllowedScopes.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ManageScopes(ClientScopeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var adminStore = new IdentityServer3AdminStore();
            if (model.AllowedScopes != null)
            {
                // remove the ones that need to be removed
                var queryToBeDeleted = (from item in model.AllowedScopes
                                        where item.Enabled == false
                                        select item.Name).ToList();
                if (queryToBeDeleted.Any())
                {
                    await adminStore.DeleteScopesFromClientAsync(model.ClientId, queryToBeDeleted);
                }
            }
            if (model.UserScopes != null)
            {
                var queryToBeAdded = (from item in model.UserScopes
                                      where item.Enabled
                                      select item.Name).ToList();
                if (queryToBeAdded.Any())
                {
                    await adminStore.AddScopesToClientAsync(model.ClientId, queryToBeAdded);
                }
            }


            return RedirectToAction("ManageScopes", new { clientId = model.ClientId});
        }

        public async Task<ActionResult> Secret(string clientId)
        {
            var model = new SecretViewModel()
            {
                ClientId = clientId,
                OpenClientSecret = Guid.NewGuid().ToString()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Secret(SecretViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var adminStore = new IdentityServer3AdminStore();
            var myCrypto = new TripleDesEncryption(model.PassCode);
            var protectedClientSecret = myCrypto.Encrypt(model.OpenClientSecret);
            var hashedClientSecret = model.OpenClientSecret.Sha256();
            var secret = new Secret(hashedClientSecret);
            var secrets = new List<Secret> {secret};
            ProtectedSecretHandle protectedSecretHandle = new ProtectedSecretHandle()
            {
                ClientId = model.ClientId,
                Value = hashedClientSecret,
                ProtectedValue = protectedClientSecret
            };
            await adminStore.AddSecretProtectedValue(protectedSecretHandle);
            await adminStore.AddClientSecretsToClientAsync(model.ClientId, secrets);
            return RedirectToAction("Index");
        }
    }


}