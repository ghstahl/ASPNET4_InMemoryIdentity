using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using P5.CassandraStore.DAO;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra
{
    public class IdentityServer3UserStore : IIdentityServer3UserStore
    {
        private ResilientSessionContainer _resilientSessionContainer;

        ResilientSessionContainer ResilientSessionContainer
        {
            get { return _resilientSessionContainer ?? (_resilientSessionContainer = new ResilientSessionContainer()); }
        }

        public async Task<IdentityServerStoreAppliedInfo> CreateIdentityServerUserAsync(IdentityServerUser user)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return await ResilientSessionContainer.ResilientSession.UpsertIdentityServerUserAsync(user);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IdentityServerStoreAppliedInfo>(ex));
            return result;
        }

        public async Task<IdentityServerUser> FindIdentityServerUserByUserIdAsync(string userId)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return
                        await
                            ResilientSessionContainer.ResilientSession.FindIdentityServerUserByUserIdAsync(userId);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IdentityServerUser>(ex));
            return result;
        }

        public async Task<IdentityServerStoreAppliedInfo> DeleteIdentityServerUserAsync(string userId)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return await ResilientSessionContainer.ResilientSession.DeleteUserByUserIdAsync(userId);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IdentityServerStoreAppliedInfo>(ex));
            return result;
        }

        public async Task<IEnumerable<string>> FindIdentityServerUsersAllowedScopesByUserIdAsync(string userId)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return
                        await
                            ResilientSessionContainer.ResilientSession.FindAllowedScopesByUserIdAsync(userId);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<string>>(ex));
            return result;
        }

        public async Task<IEnumerable<string>> FindIdentityServerUsersClientIdsByUserIdAsync(string userId)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return
                        await
                            ResilientSessionContainer.ResilientSession.FindClientIdsByUserIdAsync(userId);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<string>>(ex));
            return result;
        }

        public async Task<IdentityServerStoreAppliedInfo> AddIdentityServerUsersAllowedScopesByUserIdAsync(
            string userId, IEnumerable<string> scopes)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    var query = from item in scopes
                        let c = new IdentityServerUserAllowedScope {ScopeName = item, UserId = userId}
                        select c;
                    IdentityServerStoreAppliedInfo appliedInfo = null;
                    foreach (var insertRecord in query)
                    {
                        appliedInfo =
                            await
                                ResilientSessionContainer.ResilientSession.UpsertAllowedScopeIntoUsersAsync(insertRecord);
                        if (appliedInfo.Applied == false || appliedInfo.Exception != null)
                            break;
                    }
                    return appliedInfo;

                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IdentityServerStoreAppliedInfo>(ex));
            return result;
        }

        public async Task<IdentityServerStoreAppliedInfo> AddIdentityServerUsersClientIdsByUserIdAsync(string userId,
            IEnumerable<string> clientIds)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    var query = from item in clientIds
                        let c = new IdentityServerUserClientId() {ClientId = item, UserId = userId}
                        select c;
                    IdentityServerStoreAppliedInfo appliedInfo = null;
                    foreach (var insertRecord in query)
                    {
                        appliedInfo =
                            await ResilientSessionContainer.ResilientSession.UpsertClientIdIntoUsersAsync(insertRecord);
                        if (appliedInfo.Applied == false || appliedInfo.Exception != null)
                            break;
                    }
                    return appliedInfo;

                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IdentityServerStoreAppliedInfo>(ex));
            return result;
        }

        public async Task<IdentityServerStoreAppliedInfo> DeleteIdentityServerUsersAllowedScopesByUserIdAsync(
            string userId, IEnumerable<string> scopes)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    var query = from item in scopes
                        let c = new IdentityServerUserAllowedScope() {ScopeName = item, UserId = userId}
                        select c;
                    IdentityServerStoreAppliedInfo appliedInfo = null;
                    foreach (var insertRecord in query)
                    {
                        appliedInfo =
                            await
                                ResilientSessionContainer.ResilientSession.DeleteAllowedScopeFromUserAsync(insertRecord);
                        if (appliedInfo.Applied == false || appliedInfo.Exception != null)
                            break;
                    }
                    return appliedInfo;

                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IdentityServerStoreAppliedInfo>(ex));
            return result;
        }

        public async Task<IdentityServerStoreAppliedInfo> DeleteIdentityServerUsersClientIdsByUserIdAsync(string userId,
            IEnumerable<string> clientIds)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    var query = from item in clientIds
                        let c = new IdentityServerUserClientId() {ClientId = item, UserId = userId}
                        select c;
                    IdentityServerStoreAppliedInfo appliedInfo = null;
                    foreach (var insertRecord in query)
                    {
                        appliedInfo =
                            await ResilientSessionContainer.ResilientSession.DeleteClientIdFromUserAsync(insertRecord);
                        if (appliedInfo.Applied == false || appliedInfo.Exception != null)
                            break;
                    }
                    return appliedInfo;
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IdentityServerStoreAppliedInfo>(ex));
            return result;
        }
    }
}