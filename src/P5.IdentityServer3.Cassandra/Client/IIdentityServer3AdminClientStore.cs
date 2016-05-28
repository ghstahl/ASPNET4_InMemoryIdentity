using System;
using System.Collections.Generic;
using System.Security.Claims;
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

        /// <summary>
        /// Adds new grant types to the list of existing ones
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="allowedCustomGrantTypes"></param>
        /// <returns></returns>
        Task AddAllowedCustomGrantTypesToClientAsync(string clientId, IEnumerable<string> allowedCustomGrantTypes);

        /// <summary>
        /// Deletes grants types for the list of existing ones
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="allowedCustomGrantTypes"></param>
        /// <returns></returns>
        Task DeleteAllowedCustomGrantTypesFromClientAsync(string clientId, IEnumerable<string> allowedCustomGrantTypes);

        /// <summary>
        /// Adds new scopes to the list of existing IdentityProviderRestrictions
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="identityProviderRestrictions"></param>
        /// <returns></returns>
        Task AddIdentityProviderRestrictionsToClientAsync(string clientId, IEnumerable<string> identityProviderRestrictions);

        /// <summary>
        /// Deletes scopes from the list of existing IdentityProviderRestrictions
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="identityProviderRestrictions"></param>
        /// <returns></returns>
        Task DeleteIdentityProviderRestrictionsFromClientAsync(string clientId, IEnumerable<string> identityProviderRestrictions);

        /// <summary>
        /// Adds new scopes to the list of existing PostLogoutRedirectUris
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="postLogoutRedirectUris"></param>
        /// <returns></returns>
        Task AddPostLogoutRedirectUrisToClientAsync(string clientId, IEnumerable<string> postLogoutRedirectUris);

        /// <summary>
        /// Deletes scopes from the list of existing PostLogoutRedirectUris
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="postLogoutRedirectUris"></param>
        /// <returns></returns>
        Task DeletePostLogoutRedirectUrisFromClientAsync(string clientId, IEnumerable<string> postLogoutRedirectUris);

        /// <summary>
        /// Adds new scopes to the list of existing RedirectUris
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="redirectUris"></param>
        /// <returns></returns>
        Task AddRedirectUrisToClientAsync(string clientId, IEnumerable<string> redirectUris);

        /// <summary>
        /// Deletes scopes from the list of existing RedirectUris
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="redirectUris"></param>
        /// <returns></returns>
        Task DeleteRedirectUrisFromClientAsync(string clientId, IEnumerable<string> redirectUris);

        /// <summary>
        /// Adds new scopes to the list of existing ClientSecrets
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecrets"></param>
        /// <returns></returns>
        Task AddClientSecretsToClientAsync(string clientId, IEnumerable<Secret> clientSecrets);

        /// <summary>
        /// Deletes scopes from the list of existing ClientSecrets
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecrets"></param>
        /// <returns></returns>
        Task DeleteClientSecretsFromClientAsync(string clientId, IEnumerable<Secret> clientSecrets);

        /// <summary>
        /// Adds new scopes to the list of existing Claims
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        Task AddClaimsToClientAsync(string clientId, IEnumerable<Claim> claims);

        /// <summary>
        /// Deletes scopes from the list of existing Claims
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        Task DeleteClaimsFromClientAsync(string clientId, IEnumerable<Claim> claims);

        /// <summary>
        /// Deletes scopes from the list of existing Claims
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        Task UpdateClaimsInClientAsync(string clientId, IEnumerable<Claim> claims);


    }


}