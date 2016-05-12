using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using P5.CassandraStore.Extensions;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public partial class IdentityServer3CassandraDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for Client
        //-----------------------------------------------

        #region PREPARED STATEMENTS for Client

        private static AsyncLazy<PreparedStatement> _CreateClientById { get; set; }
        private static AsyncLazy<PreparedStatement> _FindClientById { get; set; }

        #endregion
        public static async Task<bool> CreateManyClientAsync(IList<FlattenedClientRecord> clients,
        CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (clients == null)
                    throw new ArgumentNullException("clientRecords");
                if (clients.Count == 0)
                    throw new ArgumentException("clientRecords is empty");

                var session = CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();

                var batch = new BatchStatement();
                var boundStatements = await BuildBoundStatements_ForCreate(clients);
                batch.AddRange(boundStatements);
                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static async Task<bool> CreateClientAsync(FlattenedClientRecord client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (client == null)
                    throw new ArgumentNullException("client");
                var myList = new List<FlattenedClientRecord> { client };
                return await CreateManyClientAsync(myList, cancellationToken);
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static async Task<global::IdentityServer3.Core.Models.Client> FindClientIdAsync(Guid id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await mapper.SingleAsync<FlattenedClientHandle>("SELECT * FROM clients_by_id WHERE id = ?", id);
                IClientHandle ch = record;
                var result = ch.MakeClient();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
    IEnumerable<FlattenedClientRecord> flattenedClientRecords)
        {
            var result = new List<BoundStatement>();
            foreach (var record in flattenedClientRecords)
            {
                var client = record.Record;
                PreparedStatement prepared = await _CreateClientById;
                BoundStatement bound = prepared.Bind(
                    record.Id,
                    client.AbsoluteRefreshTokenLifetime,
                    client.AccessTokenLifetime,
                    (int)client.AccessTokenType,
                    client.AllowAccessToAllCustomGrantTypes,
                    client.AllowAccessToAllScopes,
                    client.AllowAccessTokensViaBrowser,
                    client.AllowClientCredentialsOnly,
                    client.AllowedCorsOrigins,
                    client.AllowedCustomGrantTypes,
                    client.AllowedScopes,
                    client.AllowRememberConsent,
                    client.AlwaysSendClientClaims,
                    client.AuthorizationCodeLifetime,
                    client.Claims,
                    client.ClientId,
                    client.ClientName,
                    client.ClientSecrets,
                    client.ClientUri,
                    client.Enabled,
                    client.EnableLocalLogin,
                    (int)client.Flow,
                    client.IdentityProviderRestrictions,
                    client.IdentityTokenLifetime,
                    client.IncludeJwtId,
                    client.LogoUri,
                    client.LogoutSessionRequired,
                    client.LogoutUri,
                    client.PostLogoutRedirectUris,
                    client.PrefixClientClaims,
                    client.RedirectUris,
                    (int)client.RefreshTokenExpiration,
                    (int)client.RefreshTokenUsage,
                    client.RequireConsent,
                    client.RequireSignOutPrompt,
                    client.SlidingRefreshTokenLifetime,
                    client.UpdateAccessTokenClaimsOnRefresh
                    );
                result.Add(bound);
            }
            return result;
        }

    }
}
