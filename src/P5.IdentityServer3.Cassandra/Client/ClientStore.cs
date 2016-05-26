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

        public async Task CreateClientByIdAsync(Client client)
        {
            await IdentityServer3CassandraDao.UpsertClientAsync(
               new FlattenedClientRecord(new FlattenedClientHandle(client)));
        }

        public async Task UpdateClientByIdAsync(string clientId, IEnumerable<PropertyValue> properties)
        {
            await IdentityServer3CassandraDao.UpdateClientByIdAsync(clientId, properties);
        }

        public async Task AddScopesToClientByIdAsync(string clientId, IEnumerable<string> scopes)
        {
            await IdentityServer3CassandraDao.AddScopesToClientByIdAsync(clientId, scopes);
        }

        public async Task DeleteScopesFromClientByIdAsync(string clientId, IEnumerable<string> scopes)
        {
            await IdentityServer3CassandraDao.DeleteScopesFromClientByIdAsync(clientId, scopes);
        }

        public async Task DeleteClientByClientIdAsync(string clientId)
        {
            await IdentityServer3CassandraDao.DeleteClientByClientIdAsync(clientId);
        }

        public async Task DeleteClientByIdAsync(Guid id)
        {
            await IdentityServer3CassandraDao.DeleteClientByIdAsync(id);
        }
    }
}
