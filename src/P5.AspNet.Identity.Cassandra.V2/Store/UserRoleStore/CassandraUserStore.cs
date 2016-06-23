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
        IUserRoleStore<CassandraUser, Guid>
    {
        public async Task AddToRoleAsync(CassandraUser user, string roleName)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.AddToRoleAsync(user.Id, roleName);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task RemoveFromRoleAsync(CassandraUser user, string roleName)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.RemoveFromRoleAsync(user.Id, roleName);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<IList<string>> GetRolesAsync(CassandraUser user)
        {
            var resultList = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                   async () =>
                   {
                       await ResilientSessionContainer.EstablishSessionAsync();
                       return await ResilientSessionContainer.ResilientSession.FindRoleNamesByUserIdAsync(user.Id);
                   },
                   async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<string>>(ex));
            return resultList.ToList();
        }

        public async Task<bool> IsInRoleAsync(CassandraUser user, string roleName)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                   async () =>
                   {
                       await ResilientSessionContainer.EstablishSessionAsync();
                       return await ResilientSessionContainer.ResilientSession.IsUserInRoleAsync(user.Id, roleName);
                   },
                   async (ex) => ResilientSessionContainer.HandleCassandraException<bool>(ex));
            return result;
        }

    }
}