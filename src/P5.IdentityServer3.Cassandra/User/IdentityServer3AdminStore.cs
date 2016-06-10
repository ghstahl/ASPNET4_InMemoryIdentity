using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
using P5.Store.Core.Models;

namespace P5.IdentityServer3.Cassandra
{

    public partial class IdentityServer3AdminStore : IIdentityServer3AdminStore
    {
        private IdentityServer3UserStore _identityServer3UserStore;

        private IdentityServer3UserStore IdentityServer3UserStore
        {
            get { return _identityServer3UserStore ?? (_identityServer3UserStore = new IdentityServer3UserStore()); }
        }

        public async Task CreateIdentityServerUserAsync(IdentityServerUser user)
        {
            await IdentityServer3UserStore.CreateIdentityServerUserAsync(user);
        }

        public async Task<IdentityServerUser> FindIdentityServerUserByUserIdAsync(string userId)
        {
           return await IdentityServer3UserStore.FindIdentityServerUserByUserIdAsync(userId);
        }
    }
}