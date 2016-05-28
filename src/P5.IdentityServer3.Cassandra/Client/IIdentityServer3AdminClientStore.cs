using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
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
        Task CreateClientAsync(global::IdentityServer3.Core.Models.Client client);

        /// <summary>
        /// Delete a client record.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task DeleteClientAsync(string clientId);

        /// <summary>
        /// Delete a client record.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteClientAsync(Guid id);
        /// <summary>
        /// Makes sure that data is trued up by pruning scopes that no longer exit
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task CleanupClientByIdAsync(string clientId);
        /// <summary>
        /// This is a generic way to granularly modify any item in a client object
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        Task UpdateClientAsync(string clientId, IEnumerable<PropertyValue> properties);

        /// <summary>
        /// Adds new scopes to a client record
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="scopes"></param>
        /// <returns></returns>
        Task AddScopesToClientAsync(string clientId, IEnumerable<string> scopes);

        /// <summary>
        /// Delete a scope from a client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="scopes"></param>
        /// <returns></returns>
        Task DeleteScopesFromClientAsync(string clientId, IEnumerable<string> scopes);

        /// <summary>
        /// Adds new CorsOrigins to a client record
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="allowedCorsOrigins"></param>
        /// <returns></returns>
        Task AddAllowedCorsOriginsToClientAsync(string clientId, IEnumerable<string> allowedCorsOrigins);

        /// <summary>
        /// Deletes CorsOrigins from a client record
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="allowedCorsOrigins"></param>
        /// <returns></returns>
        Task DeleteAllowedCorsOriginsFromClientAsync(string clientId, IEnumerable<string> allowedCorsOrigins);

        Task AddAllowedCustomGrantTypesToClientAsync(string clientId, IEnumerable<string> allowedCustomGrantTypes);
        Task DeleteAllowedCustomGrantTypesFromClientAsync(string clientId, IEnumerable<string> allowedCustomGrantTypes);

        Task AddAllowedScopesToClientAsync(string clientId, IEnumerable<string> allowedScopes);
        Task DeleteAllowedScopesFromClientAsync(string clientId, IEnumerable<string> allowedScopes);
        
        
    }


}