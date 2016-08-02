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
        IUserClaimStore<CassandraUser, Guid>  
    {
        public async Task<IList<Claim>> GetClaimsAsync(CassandraUser user)
        {
            var resultList = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                  async () =>
                  {
                      await ResilientSessionContainer.EstablishSessionAsync();
                      return await ResilientSessionContainer.ResilientSession.FindClaimHandleByUserIdAsync(user.Id);
                  },
                  async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<ClaimHandle>>(ex));
            var query = from item in resultList
                        let c = new Claim(item.Type,item.Value)
                        select c;

            return query.ToList();
        }

        public async Task RemoveClaimAsync(CassandraUser user, Claim claim)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
               async () =>
               {
                   await ResilientSessionContainer.EstablishSessionAsync();
                   await ResilientSessionContainer.ResilientSession.DeleteClaimHandleByUserIdTypeAndValueAsync(user.Id, claim.Type,claim.Value);
               },
               async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task AddClaimAsync(CassandraUser user, Claim claim)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
              async () =>
              {
                  await ResilientSessionContainer.EstablishSessionAsync();
                  var claimHandle = new ClaimHandle()
                  {
                      Type = claim.Type,
                      Value = claim.Value,
                      UserId = user.Id
                  };
                  await ResilientSessionContainer.ResilientSession.CreateClaimAsync(claimHandle);
              },
              async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
           
        }

    }
}