using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
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
        [Route("users/email")]
        [HttpGet]
        public async Task<CassandraUser> GetUserByEmailAsync(string email)
        {
            var fullUserStore = UserManager.FullUserStore;
            var user = await fullUserStore.FindByEmailAsync(email);
            return user;
        }

        // GET api/<controller>
        [Route("users/id")]
        [HttpGet]
        public async Task<CassandraUser> GetUserByIdAsync(string id)
        {
            var fullUserStore = UserManager.FullUserStore;
            var user = await fullUserStore.FindByIdAsync(Guid.Parse(id));
            return user;
        }
        // GET api/<controller>
        [Route("users/roles")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetRolesByUserIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var fullUserStore = UserManager.FullUserStore;
            var claims = await fullUserStore.GetClaimsAsync(Guid.Parse(id));

            var queryRoles = from item in claims
                             where item.Type == ClaimTypes.Role
                             select item.Value;
            return queryRoles;
        }
        // GET api/<controller>
        [Route("users/roles/exists")]
        [HttpGet]
        public async Task<bool> ExistsRolesByUserIdAsync(string id, string role)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException("role");
            }
            var fullUserStore = UserManager.FullUserStore;
            var claims = await fullUserStore.GetClaimsAsync(Guid.Parse(id));

            var queryRoles = from item in claims
                             where item.Type == ClaimTypes.Role && string.Compare(item.Value,role,StringComparison.OrdinalIgnoreCase) == 0
                             select item;
            return queryRoles.Any();
        }

        // GET api/<controller>
        [Route("users/roles")]
        [HttpPost]
        public async Task AddRoleToUserAsync(string id, string role)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException("role");
            }

            var fullUserStore = UserManager.FullUserStore;
            var claims = await fullUserStore.GetClaimsAsync(Guid.Parse(id));

            var queryRoles = (from item in claims
                where (item.Type == ClaimTypes.Role && string.Compare(item.Value,role,StringComparison.OrdinalIgnoreCase) == 0)
                select item).ToList();

            if (!queryRoles.Any())
            {

                var user = await fullUserStore.FindByIdAsync(Guid.Parse(id));
                var claim = new Claim(ClaimTypes.Role, role);
                await fullUserStore.AddClaimAsync(user, claim);
            }
        }


        // GET api/<controller>
        [Route("users/roles")]
        [HttpDelete]
        public async Task DeleteRoleFromUserAsync(string id, string role)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException("role");
            }

            var fullUserStore = UserManager.FullUserStore;
            var claims = await fullUserStore.GetClaimsAsync(Guid.Parse(id));

            var queryRoles = (from item in claims
                              where (item.Type == ClaimTypes.Role && string.Compare(item.Value, role, StringComparison.OrdinalIgnoreCase) == 0)
                              select item).ToList();

            if (queryRoles.Any())
            {

                var user = await fullUserStore.FindByIdAsync(Guid.Parse(id));
                var claim = new Claim(ClaimTypes.Role, role);
                await fullUserStore.RemoveClaimAsync(user, claim);
            }
        }
        // GET api/<controller>
        [Route("users/Update")]
        [HttpPost]
        public async Task UpdateUserAsync(CassandraUser user)
        {
            var fullUserStore = UserManager.FullUserStore;
            await fullUserStore.UpdateAsync(user);
        }


    }
}