using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using P5.IdentityServer3.Cassandra;

namespace CustomClientCredentialHost.Areas.Admin.api
{
    [RoutePrefix("api/v1/IDSAdmin")]
    public class IdentityServerAdminApiController : ApiController
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
        
    }
}