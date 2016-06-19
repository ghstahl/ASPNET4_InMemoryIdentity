using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using P5.AspNet.Identity.Cassandra;
using P5.CassandraStore;

namespace CustomClientCredentialHost.Areas.IdentityAdmin.Controllers
{

    public class TableRecord
    {
        public string name { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public int followers { get; set; }
        public bool worksWithReactabular { get; set; }
        public int id { get; set; }
    }


    [RoutePrefix("api/identityadmin")]
    public class IdentityAdminApiController : ApiController
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? new System.Web.HttpContextWrapper(System.Web.HttpContext.Current).GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public IdentityAdminApiController()
        {
           
        }
        
        // GET: api/IdentityAdminApi
        [HttpGet]
        [Route("PageUsers")]
        public async Task<PageProxyHandle<CassandraUser>> PageUsers(int count, string pageState)
        {
            var ps = string.IsNullOrEmpty(pageState) ? null : Convert.FromBase64String(pageState);
            var adminStore = UserManager.AdminStore;
            var result = await adminStore.PageUsersAsync(count, ps);
            var finalResult = result.ToPageProxyHandle();
            return finalResult;
        }

        // GET: api/IdentityAdminApi
        [HttpGet]
        [Route("TableData")]
        public async Task<IEnumerable<TableRecord>> GetTableData()
        {
            var adminStore = UserManager.AdminStore;
            var result = await adminStore.PageUsersAsync(2, null);
            var finalResult = result.ToPageProxyHandle();
            var data = new List<TableRecord>
            {
                new TableRecord()
                {
                    name = "React.js",
                    type = "library",
                    description = "Awesome library for handling view.",
                    followers = 23252,
                    worksWithReactabular = true,
                    id = 123
                },
                new TableRecord()
                {
                    name = "Angular.js",
                    type = "framework",
                    description = "Swiss-knife of frameworks. Kitchen sink not included.",
                    followers = 35159,
                    worksWithReactabular = false,
                    id = 456
                },
                new TableRecord()
                {
                    name = "Aurelia",
                    type = "framework",
                    description = "Framework for the next generation.",
                    followers = 229,
                    worksWithReactabular = false,
                    id = 789
                },
            };

            return data;
        }

        // GET: api/IdentityAdminApi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/IdentityAdminApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/IdentityAdminApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/IdentityAdminApi/5
        public void Delete(int id)
        {
        }
    }
}
