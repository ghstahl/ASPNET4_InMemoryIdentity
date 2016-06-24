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

        public async Task<IdentityServerStoreAppliedInfo> CreateIdentityServerUserAsync(IdentityServerUser user)
        {
            return await IdentityServer3UserStore.CreateIdentityServerUserAsync(user);
        }

        public async Task<bool> FindDoesUserExistByUserIdAsync(string userId)
        {
            return await IdentityServer3UserStore.FindDoesUserExistByUserIdAsync(userId);
        }

        public async Task<IdentityServerUser> FindIdentityServerUserByUserIdAsync(string userId)
        {
           return await IdentityServer3UserStore.FindIdentityServerUserByUserIdAsync(userId);
        }

        public async Task<IdentityServerStoreAppliedInfo> DeleteIdentityServerUserAsync(string userId)
        {
            return await IdentityServer3UserStore.DeleteIdentityServerUserAsync(userId);
        }

        public async Task<IEnumerable<string>> FindScopesByUserAsync(string userId)
        {
            return await IdentityServer3UserStore.FindScopesByUserAsync(userId);
        }

        public async Task<IEnumerable<string>> FindClientIdsByUserAsync(string userId)
        {
            return await IdentityServer3UserStore.FindClientIdsByUserAsync(userId);
        }

        public async Task<IdentityServerStoreAppliedInfo> AddScopesToIdentityServerUserAsync(string userId, IEnumerable<string> scopes)
        {
            return await IdentityServer3UserStore.AddScopesToIdentityServerUserAsync(userId, scopes);
        }

        public async Task<IdentityServerStoreAppliedInfo> AddClientIdToIdentityServerUserAsync(string userId, IEnumerable<string> clientIds)
        {
            return await IdentityServer3UserStore.AddClientIdToIdentityServerUserAsync(userId, clientIds);
        }

        public async Task<IdentityServerStoreAppliedInfo> DeleteScopesByUserIdAsync(string userId, IEnumerable<string> scopes)
        {
            return await IdentityServer3UserStore.DeleteScopesByUserIdAsync(userId, scopes);
        }

        public async Task<IdentityServerStoreAppliedInfo> DeleteClientIdsByUserIdAsync(string userId, IEnumerable<string> clientIds)
        {
            return await IdentityServer3UserStore.DeleteClientIdsByUserIdAsync(userId, clientIds);
        }
    }
}