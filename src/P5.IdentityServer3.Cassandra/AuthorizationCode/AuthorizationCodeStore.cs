using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.CassandraStore.DAO;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra
{
    public class AuthorizationCodeStore : IAuthorizationCodeStore
    {
        private ResilientSessionContainer _resilientSessionContainer;

        ResilientSessionContainer ResilientSessionContainer
        {
            get { return _resilientSessionContainer ?? (_resilientSessionContainer = new ResilientSessionContainer()); }
        }

        public async Task StoreAsync(string key, global::IdentityServer3.Core.Models.AuthorizationCode value)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.CreateAuthorizationCodeHandleAsync(key, value);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<AuthorizationCode> GetAsync(string key)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    var clientStore = new ClientStore();
                    var scopeStore = new ScopeStore();
                    return
                        await
                            ResilientSessionContainer.ResilientSession.FindAuthorizationCodeByKey(key, clientStore,
                                scopeStore);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<AuthorizationCode>(ex));
            return result;
        }

        public async Task RemoveAsync(string key)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.DeleteAuthorizationCodeByKey(key);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    var clientStore = new ClientStore();
                    var scopeStore = new ScopeStore();
                    return
                        await
                            ResilientSessionContainer.ResilientSession.FindAuthorizationCodeMetadataBySubject(subject,
                                clientStore, scopeStore);
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
                        ResilientSessionContainer.ResilientSession.DeleteAuthorizationCodesByClientIdAndSubjectId(
                            client, subject);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }
    }
}
