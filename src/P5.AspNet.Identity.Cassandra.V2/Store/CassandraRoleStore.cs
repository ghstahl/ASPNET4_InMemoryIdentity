using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using P5.AspNet.Identity.Cassandra.DAO;
using P5.CassandraStore.DAO;

namespace P5.AspNet.Identity.Cassandra
{
    public class CassandraRoleStore : IQueryableRoleStore<CassandraRole, Guid>
    {
        private readonly bool _disposeOfSession;
        private ResilientSessionContainer _resilientSessionContainer;

        ResilientSessionContainer ResilientSessionContainer
        {
            get { return _resilientSessionContainer ?? (_resilientSessionContainer = new ResilientSessionContainer()); }
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

        public async Task CreateAsync(CassandraRole role)
        {
            
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.CreateRoleAsync(role);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task UpdateAsync(CassandraRole role)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
               async () =>
               {
                   await ResilientSessionContainer.EstablishSessionAsync();
                   await ResilientSessionContainer.ResilientSession.UpdateRoleAsync(role);
               },
               async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task DeleteAsync(CassandraRole role)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
               async () =>
               {
                   await ResilientSessionContainer.EstablishSessionAsync();
                   await ResilientSessionContainer.ResilientSession.DeleteRoleAsync(role);
               },
               async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<CassandraRole> FindByIdAsync(Guid roleId)
        {
            var resultList = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                  async () =>
                  {
                      await ResilientSessionContainer.EstablishSessionAsync();
                      return await ResilientSessionContainer.ResilientSession.FindRoleByIdAsync(roleId);
                  },
                  async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<CassandraRole>>(ex));
            return resultList.FirstOrDefault();
        }

        public async Task<CassandraRole> FindByNameAsync(string roleName)
        {
            var resultList = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                 async () =>
                 {
                     await ResilientSessionContainer.EstablishSessionAsync();
                     return await ResilientSessionContainer.ResilientSession.FindRoleByNameAsync(roleName);
                 },
                 async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<CassandraRole>>(ex));
            return resultList.FirstOrDefault();
        }

        public IQueryable<CassandraRole> Roles
        {
            get { return GetAllRoles().Result; }
        }
        public async Task<IQueryable<CassandraRole>> GetAllRoles()
        {
            Exception ex = null;
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync<IEnumerable<CassandraRole>>(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return await ResilientSessionContainer.ResilientSession.FindRolesByTenantIdAsync();
                   
                },
                async (e) =>
                {
                    ex = e;
                    return new TryWithAwaitInCatchExcpetionHandleResult<IEnumerable<CassandraRole>>
                    {
                        RethrowException = true,
                        DefaultResult = null
                    };
                }
                );
            return result.AsQueryable();
        }
    }
}
