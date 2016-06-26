using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using P5.AspNet.Identity.Cassandra.DAO;
using P5.Store.Core.Models;

namespace P5.AspNet.Identity.Cassandra
{
    /// <summary>
    /// Interface that exposes basic user management apis
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IUserAdminStore<TUser, in TKey> : IDisposable where TUser : class, Microsoft.AspNet.Identity.IUser<TKey>
    {
        // Summary:
        //     page the user database
        //
        // Parameters:
        //   user:
        Task<Store.Core.Models.IPage<TUser>> PageUsersAsync(int pageSize, byte[] pagingState);
        Task<IList<Claim>> GetClaimsAsync(Guid id);
        Task<IPage<ClaimHandle>> PageClaimsAsync(TKey userId, int pageSize, byte[] pagingState);
    }
}
