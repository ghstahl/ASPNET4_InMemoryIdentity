using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.CassandraStore.DAO;
using P5.IdentityServer3.Common;
using P5.Store.Core.Models;


namespace P5.IdentityServer3.Cassandra
{
    public class ClientStore : IIdentityServer3AdminClientStore
    {
        private ResilientSessionContainer _resilientSessionContainer;

        ResilientSessionContainer ResilientSessionContainer
        {
            get { return _resilientSessionContainer ?? (_resilientSessionContainer = new ResilientSessionContainer()); }
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return await ResilientSessionContainer.ResilientSession.FindClientByClientIdAsync(clientId);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Client>(ex));
            return result;
        }

        public async Task CreateClientAsync(Client client)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.UpsertClientAsync(
                        new FlattenedClientRecord(new FlattenedClientHandle(client)));
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task DeleteClientAsync(string clientId)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.DeleteClientByClientIdAsync(clientId);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task DeleteClientAsync(Guid id)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.DeleteClientByIdAsync(id);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task CleanupClientByIdAsync(string clientId)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.CleanupClientByIdAsync(clientId);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task UpdateClientAsync(string clientId, IEnumerable<PropertyValue> properties)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.UpdateClientByIdAsync(clientId, properties);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task AddScopesToClientAsync(string clientId, IEnumerable<string> scopes)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.AddScopesToClientByIdAsync(clientId, scopes);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task DeleteScopesFromClientAsync(string clientId, IEnumerable<string> scopes)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.DeleteScopesFromClientByIdAsync(clientId, scopes);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task AddAllowedCorsOriginsToClientAsync(string clientId, IEnumerable<string> allowedCorsOrigins)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.AddAllowedCorsOriginsToClientByClientIdAsync(
                            clientId, allowedCorsOrigins);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task DeleteAllowedCorsOriginsFromClientAsync(string clientId,
            IEnumerable<string> allowedCorsOrigins)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.DeleteAllowedCorsOriginsFromClientByClientIdAsync(
                            clientId,
                            allowedCorsOrigins);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task AddAllowedCustomGrantTypesToClientAsync(string clientId,
            IEnumerable<string> allowedCustomGrantTypes)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.AddAllowedCustomGrantTypesByClientIdAsync(clientId,
                            allowedCustomGrantTypes);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task DeleteAllowedCustomGrantTypesFromClientAsync(string clientId,
            IEnumerable<string> allowedCustomGrantTypes)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession
                            .DeleteAllowedCustomGrantTypesFromClientByClientIdAsync(clientId,
                                allowedCustomGrantTypes);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));


        }

        public async Task AddIdentityProviderRestrictionsToClientAsync(string clientId,
            IEnumerable<string> identityProviderRestrictions)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.AddIdentityProviderRestrictionsToClientByIdAsync(
                            clientId,
                            identityProviderRestrictions);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task DeleteIdentityProviderRestrictionsFromClientAsync(string clientId,
            IEnumerable<string> identityProviderRestrictions)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.DeleteIdentityProviderRestrictionsFromClientByIdAsync
                            (clientId,
                                identityProviderRestrictions);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task AddPostLogoutRedirectUrisToClientAsync(string clientId,
            IEnumerable<string> postLogoutRedirectUris)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.AddPostLogoutRedirectUrisToClientByIdAsync(clientId,
                            postLogoutRedirectUris);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task DeletePostLogoutRedirectUrisFromClientAsync(string clientId,
            IEnumerable<string> postLogoutRedirectUris)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.DeletePostLogoutRedirectUrisFromClientByIdAsync(
                            clientId,
                            postLogoutRedirectUris);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task AddRedirectUrisToClientAsync(string clientId, IEnumerable<string> redirectUris)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.AddRedirectUrisToClientByIdAsync(clientId,
                            redirectUris);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task DeleteRedirectUrisFromClientAsync(string clientId, IEnumerable<string> redirectUris)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.DeleteRedirectUrisFromClientByIdAsync(clientId,
                            redirectUris);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task AddClientSecretsToClientAsync(string clientId, IEnumerable<Secret> clientSecrets)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.AddClientSecretsToClientByIdAsync(clientId,
                            clientSecrets);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task DeleteClientSecretsFromClientAsync(string clientId, IEnumerable<Secret> clientSecrets)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.DeleteClientSecretsFromClientByIdAsync(clientId,
                            clientSecrets);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task AddClaimsToClientAsync(string clientId, IEnumerable<Claim> claims)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.AddClaimsToClientByIdAsync(clientId, claims);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task DeleteClaimsFromClientAsync(string clientId, IEnumerable<Claim> claims)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.DeleteClaimsFromClientByIdAsync(clientId, claims);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task UpdateClaimsInClientAsync(string clientId, IEnumerable<Claim> claims)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.UpdateClaimsInClientByIdAsync(clientId, claims);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<IPage<FlattenedClientHandle>> PageClientsAsync(int pageSize, byte[] pagingState)
        {
            IPage<FlattenedClientHandle> result =
                await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync<IPage<FlattenedClientHandle>>(
                    async () =>
                    {
                        await ResilientSessionContainer.EstablishSessionAsync();
                        return await ResilientSessionContainer.ResilientSession.PageClientsAsync(pageSize, pagingState);
                    },
                    async (ex) => ResilientSessionContainer.HandleCassandraException<IPage<FlattenedClientHandle>>(ex));
            return result;
        }
    }
}
