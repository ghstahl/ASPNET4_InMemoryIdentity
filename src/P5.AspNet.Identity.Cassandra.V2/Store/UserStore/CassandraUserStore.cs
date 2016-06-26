using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using P5.AspNet.Identity.Cassandra.DAO;
using P5.CassandraStore.DAO;
using P5.CassandraStore.Settings;

namespace P5.AspNet.Identity.Cassandra
{
    public partial class CassandraUserStore : IFullUserStore

    {
        public CassandraUserStore()
        {

        }


        private Guid TenantId { get;  set; }
        private CassandraConfig CassandraConfig { get; set; }
        public CassandraUserStore(CassandraConfig config,Guid tenantId)
        {
            TenantId = tenantId;
            CassandraConfig = config;
        }
        private readonly bool _disposeOfSession;
        private ResilientSessionContainer _resilientSessionContainer;

        ResilientSessionContainer ResilientSessionContainer
        {
            get { return _resilientSessionContainer ?? (_resilientSessionContainer = new ResilientSessionContainer(CassandraConfig,TenantId)); }
        }

        protected void Dispose(bool disposing)
        {
            if (_disposeOfSession && _resilientSessionContainer != null)
                _resilientSessionContainer.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task CreateAsync(CassandraUser user)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
              async () =>
              {
                  await ResilientSessionContainer.EstablishSessionAsync();
                  await ResilientSessionContainer.ResilientSession.UpsertUserAsync(user);
              },
              async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task UpdateAsync(CassandraUser user)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
               async () =>
               {
                   await ResilientSessionContainer.EstablishSessionAsync();
                   await ResilientSessionContainer.ResilientSession.UpsertUserAsync(user);
               },
               async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task DeleteAsync(CassandraUser user)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
               async () =>
               {
                   await ResilientSessionContainer.EstablishSessionAsync();
                   await ResilientSessionContainer.ResilientSession.DeleteUserAsync(user);
               },
               async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<CassandraUser> FindByIdAsync(Guid userId)
        {
            var resultList = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                  async () =>
                  {
                      await ResilientSessionContainer.EstablishSessionAsync();
                      return await ResilientSessionContainer.ResilientSession.FindUserByIdAsync(userId);
                  },
                  async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<CassandraUser>>(ex));
            return resultList.FirstOrDefault();
        }

        public async Task<CassandraUser> FindByNameAsync(string userName)
        {
            var resultList = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                   async () =>
                   {
                       await ResilientSessionContainer.EstablishSessionAsync();
                       return await ResilientSessionContainer.ResilientSession.FindUserByUserNameAsync(userName);
                   },
                   async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<CassandraUser>>(ex));
            return resultList.FirstOrDefault();
        }
    }
}