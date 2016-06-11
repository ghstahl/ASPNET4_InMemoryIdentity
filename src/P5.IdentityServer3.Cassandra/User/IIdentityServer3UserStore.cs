using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra
{
    public interface IIdentityServer3UserStore
    {
        /// <summary>
        /// Upsert a brand new IdentityServer3 User or overwrite a matching one.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IdentityServerStoreAppliedInfo> CreateIdentityServerUserAsync(IdentityServerUser user);


        Task<IdentityServerUser> FindIdentityServerUserByUserIdAsync(string userId);
        Task<IdentityServerStoreAppliedInfo> DeleteIdentityServerUserAsync(string userId);

        Task<IEnumerable<string>> FindIdentityServerUsersAllowedScopesByUserIdAsync(string userId);
        Task<IEnumerable<string>> FindIdentityServerUsersClientIdsByUserIdAsync(string userId);

        Task<IdentityServerStoreAppliedInfo> AddIdentityServerUsersAllowedScopesByUserIdAsync(string userId, IEnumerable<string> scopes);
        Task<IdentityServerStoreAppliedInfo> AddIdentityServerUsersClientIdsByUserIdAsync(string userId, IEnumerable<string> clientIds);

        Task<IdentityServerStoreAppliedInfo> DeleteIdentityServerUsersAllowedScopesByUserIdAsync(string userId, IEnumerable<string> scopes);
        Task<IdentityServerStoreAppliedInfo> DeleteIdentityServerUsersClientIdsByUserIdAsync(string userId, IEnumerable<string> clientIds);

    }

}
