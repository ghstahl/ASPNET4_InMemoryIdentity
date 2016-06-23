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
        IUserLoginStore<CassandraUser, Guid>
    {
        

        public async Task AddLoginAsync(CassandraUser user, UserLoginInfo login)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
               async () =>
               {
                   ProviderLoginHandle providerLogin = new ProviderLoginHandle()
                   {
                       LoginProvider = login.LoginProvider,
                       ProviderKey = login.ProviderKey,
                       TenantId = ResilientSessionContainer.ResilientSession.TenantId,
                       UserId = user.Id
                   };
                   await ResilientSessionContainer.EstablishSessionAsync();
                   await ResilientSessionContainer.ResilientSession.UpsertLoginsAsync(providerLogin);
               },
               async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task RemoveLoginAsync(CassandraUser user, UserLoginInfo login)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
              async () =>
              {
                  ProviderLoginHandle providerLogin = new ProviderLoginHandle()
                  {
                      LoginProvider = login.LoginProvider,
                      ProviderKey = login.ProviderKey,
                      TenantId = ResilientSessionContainer.ResilientSession.TenantId,
                      UserId = user.Id
                  };
                  await ResilientSessionContainer.EstablishSessionAsync();
                  await ResilientSessionContainer.ResilientSession.DeleteLoginsAsync(providerLogin);
              },
              async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(CassandraUser user)
        {
            var resultList = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                   async () =>
                   {
                       await ResilientSessionContainer.EstablishSessionAsync();
                       return await ResilientSessionContainer.ResilientSession.FindLoginsByUserIdAsync(user.Id);
                   },
                   async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<ProviderLoginHandle>>(ex));
            var query = from item in resultList
                let c = new UserLoginInfo(item.LoginProvider, item.ProviderKey)
                select c;
            return query.ToList();
        }

        public async Task<CassandraUser> FindAsync(UserLoginInfo login)
        {
            var resultList = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                 async () =>
                 {
                     await ResilientSessionContainer.EstablishSessionAsync();
                     return await ResilientSessionContainer.ResilientSession.FindLoginByProviderAsync(login.LoginProvider,login.ProviderKey);
                 },
                 async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<ProviderLoginHandle>>(ex));
            var plh = resultList.ToList()[0];

            var result = await FindByIdAsync(plh.UserId);
            return result;
        }

      
    }
}