using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using P5.AspNet.Identity.Cassandra.DAO;
using P5.CassandraStore.DAO;
using P5.Store.Core.Models;

namespace P5.AspNet.Identity.Cassandra
{
    public partial class CassandraUserStore :
        IUserAdminStore<CassandraUser, Guid>
    {

        public async Task<IPage<CassandraUser>> PageUsersAsync(int pageSize, byte[] pagingState)
        {
            var resultList = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                 async () =>
                 {
                     await ResilientSessionContainer.EstablishSessionAsync();
                     return await ResilientSessionContainer.ResilientSession.PageUsersAsync(pageSize, pagingState);
                 },
                 async (ex) => ResilientSessionContainer.HandleCassandraException<IPage<CassandraUser>>(ex));
            return resultList;
        }

        public async Task<IList<Claim>> GetClaimsAsync(Guid id)
        {
            var resultList = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
               async () =>
               {
                   await ResilientSessionContainer.EstablishSessionAsync();
                   return await ResilientSessionContainer.ResilientSession.FindClaimHandleByUserIdAsync(id);
               },
               async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<ClaimHandle>>(ex));
            var query = from item in resultList
                let c = new Claim(item.Type, item.Value)
                select c;
            return query.ToList();
        }

        public async Task<IPage<ClaimHandle>> PageClaimsAsync(Guid userId, int pageSize, byte[] pagingState)
        {
            var resultList = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return await ResilientSessionContainer.ResilientSession.PageClaimsAsync(userId,pageSize, pagingState);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IPage<ClaimHandle>>(ex));
            return resultList;
        }
    }
}