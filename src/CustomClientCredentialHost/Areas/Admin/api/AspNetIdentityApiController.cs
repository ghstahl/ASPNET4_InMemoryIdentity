using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using P5.AspNet.Identity.Cassandra;

namespace CustomClientCredentialHost.Areas.Admin.api
{
    [RoutePrefix("api/v1/IdentityAdmin")]
    public class AspNetIdentityApiController : ApiController
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        // GET api/<controller>
        [Route("users/page/{pageSize:int}")]
        [HttpGet]
        public async Task<PageRecord> PageUsersAsync(int pageSize,string pagingState)
        {
            var fullUserStore = UserManager.FullUserStore;
            var page = await fullUserStore.PageUsersAsync(pageSize, null);
            var state = HttpUtility.UrlEncode(page.PagingState);
            var record = new PageRecord()
            {
                CurrentPagingState = HttpUtility.UrlEncode(page.CurrentPagingState),
                PageSize = pageSize,
                PagingState = HttpUtility.UrlEncode(page.PagingState),
                Users = page
            };
            return record;
        }
        // GET api/<controller>
        [Route("user/email")]
        [HttpGet]
        public async Task<CassandraUser> GetUserByEmailAsync(string email)
        {
            var fullUserStore = UserManager.FullUserStore;
            var user = await fullUserStore.FindByEmailAsync(email);
            return user;
        }

        // GET api/<controller>
        [Route("user/id")]
        [HttpGet]
        public async Task<CassandraUser> GetUserByIdAsync(string id)
        {
            var fullUserStore = UserManager.FullUserStore;
            var user = await fullUserStore.FindByIdAsync(Guid.Parse(id));
            return user;
        }

        // GET api/<controller>
        [Route("user/Update")]
        [HttpPost]
        public async Task UpdateUserAsync(CassandraUser user)
        {
            var fullUserStore = UserManager.FullUserStore;
            await fullUserStore.UpdateAsync(user);
        }


    }
}