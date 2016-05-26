using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer3.Core.Models;
using P5.CassandraStore;
using P5.CassandraStore.Extensions;

using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.Extensions;

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
        private static AsyncLazy<PreparedStatement> _DeleteClientById { get; set; }


        #endregion

        public static void PrepareClientStatements()
        {
            #region PREPARED STATEMENTS for Client

            /*
                         ************************************************
                            id uuid,
                            AbsoluteRefreshTokenLifetime int,
                            AccessTokenLifetime int,
                            AccessTokenType int,
                            AllowAccessToAllCustomGrantTypes boolean,
                            AllowAccessToAllScopes boolean,
                            AllowAccessTokensViaBrowser boolean,
                            AllowClientCredentialsOnly boolean,
                            AllowedCorsOrigins text,
                            AllowedCustomGrantTypes text,
                            AllowedScopes text,
                            AllowRememberConsent boolean,
                            AlwaysSendClientClaims boolean,
                            AuthorizationCodeLifetime int,
 	                        Claims text,
                            ClientId text,
                            ClientName text,
                            ClientSecrets text,
                            ClientUri text,
                            Enabled boolean,
                            EnableLocalLogin boolean,
                            Flow int,
 	                        IdentityProviderRestrictions text,
                            IdentityTokenLifetime int,
                            IncludeJwtId boolean,
                            LogoUri text,
                            LogoutSessionRequired boolean,
                            LogoutUri text,
                            PostLogoutRedirectUris text,
                            PrefixClientClaims boolean,
 	                        RedirectUris text,
                            RefreshTokenExpiration int,
                            RefreshTokenUsage int,
                            RequireConsent boolean,
                            RequireSignOutPrompt boolean,
                            SlidingRefreshTokenLifetime int,
                            UpdateAccessTokenClaimsOnRefresh boolean,
                         ************************************************
                         */
            _FindClientById =
                new AsyncLazy<PreparedStatement>(
                    () => _cassandraSession.PrepareAsync("SELECT * " +
                                                         "FROM clients_by_id " +
                                                         "WHERE id = ?"));
            _CreateClientById =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"INSERT INTO " +
                            @"clients_by_id (id,AbsoluteRefreshTokenLifetime,AccessTokenLifetime,AccessTokenType,AllowAccessToAllCustomGrantTypes,AllowAccessToAllScopes ,AllowAccessTokensViaBrowser ,AllowClientCredentialsOnly ,AllowedCorsOrigins ,AllowedCustomGrantTypes ,AllowedScopes ,AllowRememberConsent ,AlwaysSendClientClaims ,AuthorizationCodeLifetime ,Claims ,ClientId ,ClientName ,ClientSecrets ,ClientUri ,Enabled ,EnableLocalLogin ,Flow ,IdentityProviderRestrictions ,IdentityTokenLifetime ,IncludeJwtId ,LogoUri ,LogoutSessionRequired ,LogoutUri ,PostLogoutRedirectUris ,PrefixClientClaims ,RedirectUris ,RefreshTokenExpiration ,RefreshTokenUsage ,RequireConsent ,RequireSignOutPrompt,SlidingRefreshTokenLifetime,UpdateAccessTokenClaimsOnRefresh) " +
                            @"VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)");
                        return result;
                    });
            _DeleteClientById =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"Delete FROM clients_by_id " +
                            @"WHERE id = ?");
                        return result;
                    });


            #endregion
        }

        public static async Task<bool> DeleteClientByIdAsync(Guid id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentNullException("id");
                var session = CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();

                PreparedStatement prepared = await _DeleteClientById;
                BoundStatement bound = prepared.Bind(id);

                await session.ExecuteAsync(bound).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static async Task<bool> DeleteClientByClientIdAsync(string clientId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (clientId == null)
                    throw new ArgumentNullException("clientId");
                var record =
                    new FlattenedClientRecord(new FlattenedClientHandle(new global::IdentityServer3.Core.Models.Client()
                    {
                        ClientId = clientId
                    }));
                return await DeleteClientByIdAsync(record.Id);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task<bool> UpsertManyClientAsync(IList<Client> clients,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (clients == null)
                    throw new ArgumentNullException("clients");
                 if (clients.Count == 0)
                    throw new ArgumentException("clients is empty");
                 var query = from item in clients
                             let c = new FlattenedClientRecord(new FlattenedClientHandle(item))
                             select c;

                return await UpsertManyClientAsync(query.ToList(), cancellationToken);
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static async Task<bool> UpsertManyClientAsync(IList<FlattenedClientRecord> clients,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (clients == null)
                    throw new ArgumentNullException("clients");
                if (clients.Count == 0)
                    throw new ArgumentException("clients is empty");

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

        public static async Task<bool> UpsertClientAsync(FlattenedClientRecord client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (client == null)
                    throw new ArgumentNullException("client");
                var myList = new List<FlattenedClientRecord> {client};
                return await UpsertManyClientAsync(myList, cancellationToken);
            }
            catch (Exception e)
            {
                return false;
            }

        }
        public static async Task<bool> UpsertClientAsync(Client client,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (client == null)
                    throw new ArgumentNullException("client");
                return await UpsertClientAsync(new FlattenedClientRecord(new FlattenedClientHandle(client)));
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static async Task<global::IdentityServer3.Core.Models.Client> FindClientByClientIdAsync(string clientId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Guid id = clientId.ClientIdToGuid();
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await mapper.SingleAsync<FlattenedClientHandle>("SELECT * FROM clients_by_id WHERE id = ?", id);
                IClientHandle ch = record;
                var result = await ch.MakeClientAsyc();
                return result;
            }
            catch (Exception e)
            {
                return null;
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
                var result = await ch.MakeClientAsyc();
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
                    (int) client.AccessTokenType,
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
                    (int) client.Flow,
                    client.IdentityProviderRestrictions,
                    client.IdentityTokenLifetime,
                    client.IncludeJwtId,
                    client.LogoUri,
                    client.LogoutSessionRequired,
                    client.LogoutUri,
                    client.PostLogoutRedirectUris,
                    client.PrefixClientClaims,
                    client.RedirectUris,
                    (int) client.RefreshTokenExpiration,
                    (int) client.RefreshTokenUsage,
                    client.RequireConsent,
                    client.RequireSignOutPrompt,
                    client.SlidingRefreshTokenLifetime,
                    client.UpdateAccessTokenClaimsOnRefresh
                    );
                result.Add(bound);
            }
            return result;
        }

        public static async Task UpdateClientByIdAsync(string clientId, IEnumerable<PropertyValue> properties,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var value in properties)
            {
                stored.SetPropertyValue(value.Name, value.Value);
            }
            await UpsertClientAsync(new FlattenedClientRecord(new FlattenedClientHandle(stored)), cancellationToken);

        }

        public static async Task AddScopesToClientByIdAsync(string clientId, IEnumerable<string> scopes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            List<string> ulist = stored.AllowedScopes.Union(scopes).ToList();
            stored.AllowedScopes = ulist;
            await UpsertClientAsync(stored, cancellationToken);
        }

        public static async Task DeleteScopesFromClientByIdAsync(string clientId, IEnumerable<string> scopes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var query = from item in stored.AllowedScopes
                where !scopes.Contains(item)
                select item;
            stored.AllowedScopes = query.ToList();
            await UpsertClientAsync(stored, cancellationToken);

        }
    }
}
