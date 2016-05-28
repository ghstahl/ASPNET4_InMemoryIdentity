using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra
{

    public partial class IdentityServer3AdminStore : IIdentityServer3AdminStore
    {
        private ClientStore _ClientStore;

        private ClientStore ClientStore
        {
            get { return _ClientStore ?? (_ClientStore = new ClientStore()); }
        }

        public async Task CreateClientAsync(global::IdentityServer3.Core.Models.Client client)
        {
            await ClientStore.CreateClientAsync(client);
        }

        public async Task DeleteClientAsync(string clientId)
        {
            await ClientStore.DeleteClientAsync(clientId);
        }

        public async Task DeleteClientAsync(Guid id)
        {
            await ClientStore.DeleteClientAsync(id);
        }

        public async Task CleanupClientByIdAsync(string clientId)
        {
            await ClientStore.CleanupClientByIdAsync(clientId);
        }

        public async Task<global::IdentityServer3.Core.Models.Client> FindClientByIdAsync(string clientId)
        {
            return await ClientStore.FindClientByIdAsync(clientId);
        }

        public async Task UpdateClientAsync(string clientId, IEnumerable<PropertyValue> properties)
        {
            await ClientStore.UpdateClientAsync(clientId, properties);
        }

        public async Task AddScopesToClientAsync(string clientId, IEnumerable<string> scopes)
        {
            await ClientStore.AddScopesToClientAsync(clientId, scopes);
        }

        public async Task DeleteScopesFromClientAsync(string clientId, IEnumerable<string> scopes)
        {
            await ClientStore.DeleteScopesFromClientAsync(clientId, scopes);
        }

        public async Task AddAllowedCorsOriginsToClientAsync(string clientId, IEnumerable<string> allowedCorsOrigins)
        {
            await ClientStore.AddAllowedCorsOriginsToClientAsync(clientId, allowedCorsOrigins);
        }

        public async Task DeleteAllowedCorsOriginsFromClientAsync(string clientId,
            IEnumerable<string> allowedCorsOrigins)
        {
            await ClientStore.DeleteAllowedCorsOriginsFromClientAsync(clientId, allowedCorsOrigins);
        }

        public async Task AddAllowedCustomGrantTypesToClientAsync(string clientId,
            IEnumerable<string> allowedCustomGrantTypes)
        {
            await ClientStore.AddAllowedCustomGrantTypesToClientAsync(clientId, allowedCustomGrantTypes);
        }

        public async Task DeleteAllowedCustomGrantTypesFromClientAsync(string clientId,
            IEnumerable<string> allowedCustomGrantTypes)
        {
            await ClientStore.DeleteAllowedCustomGrantTypesFromClientAsync(clientId, allowedCustomGrantTypes);
        }

        public async Task AddAllowedScopesToClientAsync(string clientId, IEnumerable<string> allowedScopes)
        {
            await ClientStore.AddAllowedScopesToClientAsync(clientId, allowedScopes);
        }

        public async Task DeleteAllowedScopesFromClientAsync(string clientId, IEnumerable<string> allowedScopes)
        {
            await ClientStore.DeleteAllowedScopesFromClientAsync(clientId, allowedScopes);
        }


    }
}