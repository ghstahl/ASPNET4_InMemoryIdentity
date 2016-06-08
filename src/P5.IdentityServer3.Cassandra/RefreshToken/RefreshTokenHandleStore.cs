using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.CassandraStore.DAO;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.RefreshToken;

namespace P5.IdentityServer3.Cassandra
{
    public class RefreshTokenHandleStore : IRefreshTokenStore
    {
        private ResilientSessionContainer _resilientSessionContainer;

        ResilientSessionContainer ResilientSessionContainer
        {
            get { return _resilientSessionContainer ?? (_resilientSessionContainer = new ResilientSessionContainer()); }
        }

        public RefreshTokenHandleStore()
        {
        }

        public async Task StoreAsync(string key, RefreshToken value)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    var flat = new FlattenedRefreshTokenHandle(key, value);
                    await ResilientSessionContainer.ResilientSession.CreateRefreshTokenHandleAsync(flat);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<RefreshToken> GetAsync(string key)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    IClientStore cs = new ClientStore();
                    return await ResilientSessionContainer.ResilientSession.FindRefreshTokenByKey(key, cs);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<RefreshToken>(ex));
            return result;
        }

        public async Task RemoveAsync(string key)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.DeleteRefreshTokenByKey(key);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    IClientStore cs = new ClientStore();
                    return
                        await ResilientSessionContainer.ResilientSession.FindRefreshTokenMetadataBySubject(subject, cs);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<ITokenMetadata>>(ex));
            return result;
        }

        public async Task RevokeAsync(string subject, string client)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.DeleteRefreshTokensByClientIdAndSubjectId(client,
                            subject);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }
    }
}