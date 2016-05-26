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

        public async Task CreateClientByIdAsync(global::IdentityServer3.Core.Models.Client client)
        {
            await ClientStore.CreateClientByIdAsync(client);
        }

        public async Task UpdateClientByIdAsync(string clientId, IEnumerable<PropertyValue> properties)
        {
            await ClientStore.UpdateClientByIdAsync(clientId, properties);
        }

        public async Task AddScopesToClientByIdAsync(string clientId, IEnumerable<string> scopes)
        {
            await ClientStore.AddScopesToClientByIdAsync(clientId, scopes);
        }

        public async Task DeleteScopesFromClientByIdAsync(string clientId, IEnumerable<string> scopes)
        {
            await ClientStore.DeleteScopesFromClientByIdAsync(clientId, scopes);
        }

        public async Task DeleteClientByClientIdAsync(string clientId)
        {
            await ClientStore.DeleteClientByClientIdAsync(clientId);
        }

        public async Task DeleteClientByIdAsync(Guid id)
        {
            await ClientStore.DeleteClientByIdAsync(id);
        }

        public async Task<global::IdentityServer3.Core.Models.Client> FindClientByIdAsync(string clientId)
        {
            return await ClientStore.FindClientByIdAsync(clientId);
        }
       
    }
}