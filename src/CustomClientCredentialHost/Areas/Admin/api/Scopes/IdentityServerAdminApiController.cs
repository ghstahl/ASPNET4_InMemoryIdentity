using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using CustomClientCredentialHost.Areas.Admin.Models;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Cassandra;

namespace CustomClientCredentialHost.Areas.Admin.api
{
    public partial class IdentityServerAdminApiController : ApiController
    {
        // GET api/<controller>
        [Route("scopes/page/{pageSize:int}")]
        [HttpGet]
        public async Task<IDSScopePageRecord> PageScopsesAsync(int pageSize, string pagingState)
        {
            var adminStore = new IdentityServer3AdminStore();


            var page = await adminStore.PageScopesAsync(pageSize, null);
            var state = HttpUtility.UrlEncode(page.PagingState);
            var record = new IDSScopePageRecord()
            {
                CurrentPagingState = HttpUtility.UrlEncode(page.CurrentPagingState),
                PageSize = pageSize,
                PagingState = HttpUtility.UrlEncode(page.PagingState),
                Scopes = page
            };
            return record;
        }

        [Route("scopes")]
        [HttpGet]
        public async Task<Scope> FindScopeAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            var adminStore = new IdentityServer3AdminStore();
            var scope = await adminStore.FindScopeByNameAsync(name);
            return scope;
        }

        [Route("scopes")]
        [HttpDelete]
        public async Task DeleteScopeAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            var adminStore = new IdentityServer3AdminStore();
            await adminStore.DeleteScopeAsync(new Scope(){Name = name});
        }

        [Route("scopes")]
        [HttpPost]
        public async Task UpsertScopeAsync(Scope scope)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }
            var adminStore = new IdentityServer3AdminStore();
            await adminStore.CreateScopeAsync(scope);
        }
    }
}
