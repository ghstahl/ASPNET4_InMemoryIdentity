using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;


namespace P5.IdentityServer3.Cassandra
{
    public class ClientStore : IIdentityServer3AdminClientStore
    {
        public async Task<global::IdentityServer3.Core.Models.Client> FindClientByIdAsync(string clientId)
        {
            if (clientId == null)
            {
                throw new ArgumentNullException("clientId","Validation failed.");
            }
            var result =  await IdentityServer3CassandraDao.FindClientByClientIdAsync(clientId);
            return result;
        }

        public async Task CreateClientAsync(Client client)
        {
            await IdentityServer3CassandraDao.UpsertClientAsync(
               new FlattenedClientRecord(new FlattenedClientHandle(client)));
        }

        public async Task CleanupClientByIdAsync(string clientId)
        {
            await IdentityServer3CassandraDao.CleanupClientByIdAsync(clientId);
        }

        public async Task UpdateClientAsync(string clientId, IEnumerable<PropertyValue> properties)
        {
            await IdentityServer3CassandraDao.UpdateClientByIdAsync(clientId, properties);
        }

        public async Task AddScopesToClientAsync(string clientId, IEnumerable<string> scopes)
        {
            await IdentityServer3CassandraDao.AddScopesToClientByIdAsync(clientId, scopes);
        }

        public async Task DeleteScopesFromClientAsync(string clientId, IEnumerable<string> scopes)
        {
            await IdentityServer3CassandraDao.DeleteScopesFromClientByIdAsync(clientId, scopes);
        }

        public async Task AddAllowedCorsOriginsToClientAsync(string clientId, IEnumerable<string> allowedCorsOrigins)
        {
            await IdentityServer3CassandraDao.AddAllowedCorsOriginsToClientByClientIdAsync(clientId, allowedCorsOrigins);
        }

        public async Task DeleteAllowedCorsOriginsFromClientAsync(string clientId, IEnumerable<string> allowedCorsOrigins)
        {
            await IdentityServer3CassandraDao.DeleteAllowedCorsOriginsFromClientByClientIdAsync(clientId, allowedCorsOrigins);
        }

        public async Task AddAllowedCustomGrantTypesToClientAsync(string clientId, IEnumerable<string> allowedCustomGrantTypes)
        {
            await IdentityServer3CassandraDao.AddAllowedCustomGrantTypesByClientIdAsync(clientId, allowedCustomGrantTypes);
        }

        public async Task DeleteAllowedCustomGrantTypesFromClientAsync(string clientId, IEnumerable<string> allowedCustomGrantTypes)
        {
            await IdentityServer3CassandraDao.DeleteAllowedCustomGrantTypesFromClientByClientIdAsync(clientId, allowedCustomGrantTypes);
        }

        public async Task AddAllowedScopesToClientAsync(string clientId, IEnumerable<string> allowedScopes)
        {
            await IdentityServer3CassandraDao.AddScopesToClientByIdAsync(clientId, allowedScopes);
        }

        public async Task DeleteAllowedScopesFromClientAsync(string clientId, IEnumerable<string> allowedScopes)
        {
            await IdentityServer3CassandraDao.DeleteScopesFromClientByIdAsync(clientId, allowedScopes);
        }

        public async Task DeleteClientAsync(string clientId)
        {
            await IdentityServer3CassandraDao.DeleteClientByClientIdAsync(clientId);
        }

        public async Task DeleteClientAsync(Guid id)
        {
            await IdentityServer3CassandraDao.DeleteClientByIdAsync(id);
        }
    }
}
