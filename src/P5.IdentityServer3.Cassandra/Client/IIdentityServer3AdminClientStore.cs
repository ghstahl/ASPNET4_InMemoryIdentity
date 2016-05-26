using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Cassandra
{
    public interface IIdentityServer3AdminClientStore : IClientStore
    {
        /// <summary>
        /// Upsert a brand new client or overwrite a matching one.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        Task CreateClientByIdAsync(global::IdentityServer3.Core.Models.Client client);

        /// <summary>
        /// This is a generic way to granularly modify any item in a client object
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        Task UpdateClientByIdAsync(string clientId, IEnumerable<PropertyValue> properties);

        Task AddScopesToClientByIdAsync(string clientId, IEnumerable<string> scopes);

        Task DeleteScopesFromClientByIdAsync(string clientId, IEnumerable<string> scopes);

        Task DeleteClientByClientIdAsync(string clientId);

        Task DeleteClientByIdAsync(Guid id);
    }
}