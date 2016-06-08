using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityModel;
using IdentityServer3.Core.Models;
using P5.CassandraStore;
using P5.CassandraStore.Extensions;

using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.Extensions;
using ClaimComparer = P5.IdentityServer3.Common.ClaimComparer;
using StringComparer = System.StringComparer;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public partial class IdentityServer3CassandraDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for Client
        //-----------------------------------------------

        #region PREPARED STATEMENTS for Client

        private AsyncLazy<PreparedStatement> _CreateClientById { get; set; }
        private AsyncLazy<PreparedStatement> _FindClientById { get; set; }
        private AsyncLazy<PreparedStatement> _DeleteClientById { get; set; }

        private const string SelectClientQuery = @"SELECT * FROM clients_by_id";

        #endregion

        public  void PrepareClientStatements()
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

        public async Task<bool> DeleteClientByIdAsync(Guid id,
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

        public async Task<bool> DeleteClientByClientIdAsync(string clientId,
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


        public async Task<bool> UpsertClientAsync(FlattenedClientRecord client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (client == null)
                    throw new ArgumentNullException("clients");
                var session = CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();

                var batch = new BatchStatement();
                var boundStatements = await BuildBoundStatements_ForCreate(client);
                batch.AddRange(boundStatements);
                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                throw;
                return false;
            }

        }

        public async Task<bool> UpsertClientAsync(Client client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (client == null)
                    throw new ArgumentNullException("client");
                return
                    await
                        UpsertClientAsync(new FlattenedClientRecord(new FlattenedClientHandle(client)),
                            cancellationToken);
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public async Task<global::IdentityServer3.Core.Models.Client> FindClientByClientIdAsync(string clientId,
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

        public async Task<global::IdentityServer3.Core.Models.Client> FindClientIdAsync(Guid id,
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

        public async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
            FlattenedClientRecord record)
        {
            var result = new List<BoundStatement>();

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

            return result;
        }

        public async Task UpdateClientByIdAsync(string clientId, IEnumerable<PropertyValue> properties,
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

        public async Task CleanupClientByIdAsync(string clientId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var scopeRecords = await FindScopesByNamesAsync(stored.AllowedScopes, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var scopeRecordsList = scopeRecords.ToList();
            if (stored.AllowedScopes.Count != scopeRecordsList.Count)
            {
                var query = from item in scopeRecordsList
                    let c = item.Name
                    select c;

                await UpdateClientByIdAsync(clientId, new List<PropertyValue>()
                {
                    new PropertyValue()
                    {
                        Name = "AllowedScopes",
                        Value = query.ToList()
                    }
                });
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public async Task AddScopesToClientByIdAsync(string clientId, IEnumerable<string> scopes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var scopeList = scopes.ToList();
            var scopeRecords = await FindScopesByNamesAsync(scopeList, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var scopeRecordsList = scopeRecords.ToList();
            if (scopeList.Count != scopeRecordsList.Count)
            {
                throw new ArgumentException("one or more scopes requested do not exist, so they cannot be added",
                    "scopes");
            }

            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            if (stored == null)
            {
                throw new Exception(string.Format("CASSANDRA Exception: Cannot find record for ClientId:[{0}]", clientId));
            }
            cancellationToken.ThrowIfCancellationRequested();

            List<string> ulist = stored.AllowedScopes.Union(scopeList).ToList();
            stored.AllowedScopes = ulist;
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task DeleteScopesFromClientByIdAsync(string clientId, IEnumerable<string> scopes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var query = from item in stored.AllowedScopes
                where !scopes.Contains(item, StringComparer.OrdinalIgnoreCase)
                select item;
            stored.AllowedScopes = query.ToList();
            await UpsertClientAsync(stored, cancellationToken);

        }

        public async Task AddAllowedCorsOriginsToClientByClientIdAsync(string clientId,
            IEnumerable<string> allowedCorsOrigins,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            List<string> ulist =
                stored.AllowedCorsOrigins.Union(allowedCorsOrigins, StringComparer.OrdinalIgnoreCase).ToList();
            stored.AllowedCorsOrigins = ulist;
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task DeleteAllowedCorsOriginsFromClientByClientIdAsync(string clientId,
            IEnumerable<string> allowedCorsOrigins,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var query = from item in stored.AllowedCorsOrigins
                where !allowedCorsOrigins.Contains(item, StringComparer.OrdinalIgnoreCase)
                select item;
            stored.AllowedCorsOrigins = query.ToList();
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task AddAllowedCustomGrantTypesByClientIdAsync(string clientId,
            IEnumerable<string> allowedCustomGrantTypes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            List<string> ulist =
                stored.AllowedCustomGrantTypes.Union(allowedCustomGrantTypes, StringComparer.OrdinalIgnoreCase).ToList();
            stored.AllowedCustomGrantTypes = ulist;
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task DeleteAllowedCustomGrantTypesFromClientByClientIdAsync(string clientId,
            IEnumerable<string> allowedCustomGrantTypes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var query = from item in stored.AllowedCustomGrantTypes
                where !allowedCustomGrantTypes.Contains(item, StringComparer.OrdinalIgnoreCase)
                select item;
            stored.AllowedCustomGrantTypes = query.ToList();
            await UpsertClientAsync(stored, cancellationToken);
        }


        public async Task AddIdentityProviderRestrictionsToClientByIdAsync(string clientId,
            IEnumerable<string> identityProviderRestrictions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            List<string> ulist =
                stored.IdentityProviderRestrictions.Union(identityProviderRestrictions, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            stored.IdentityProviderRestrictions = ulist;
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task DeleteIdentityProviderRestrictionsFromClientByIdAsync(string clientId,
            IEnumerable<string> identityProviderRestrictions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var query = from item in stored.IdentityProviderRestrictions
                where !identityProviderRestrictions.Contains(item, StringComparer.OrdinalIgnoreCase)
                select item;
            stored.IdentityProviderRestrictions = query.ToList();
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task AddPostLogoutRedirectUrisToClientByIdAsync(string clientId,
            IEnumerable<string> postLogoutRedirectUris,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            List<string> ulist =
                stored.PostLogoutRedirectUris.Union(postLogoutRedirectUris, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            stored.PostLogoutRedirectUris = ulist;
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task DeletePostLogoutRedirectUrisFromClientByIdAsync(string clientId,
            IEnumerable<string> postLogoutRedirectUris,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var query = from item in stored.PostLogoutRedirectUris
                where !postLogoutRedirectUris.Contains(item, StringComparer.OrdinalIgnoreCase)
                select item;
            stored.PostLogoutRedirectUris = query.ToList();
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task AddRedirectUrisToClientByIdAsync(string clientId, IEnumerable<string> redirectUris,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            List<string> ulist =
                stored.RedirectUris.Union(redirectUris, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            stored.RedirectUris = ulist;
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task DeleteRedirectUrisFromClientByIdAsync(string clientId, IEnumerable<string> redirectUris,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var query = from item in stored.RedirectUris
                where !redirectUris.Contains(item, StringComparer.OrdinalIgnoreCase)
                select item;
            stored.RedirectUris = query.ToList();
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task AddClientSecretsToClientByIdAsync(string clientId,
            IEnumerable<Secret> clientSecrets,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            List<Secret> ulist =
                stored.ClientSecrets.Union(clientSecrets, SecretComparer.OrdinalIgnoreCase)
                    .ToList();
            stored.ClientSecrets = ulist;
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task DeleteClientSecretsFromClientByIdAsync(string clientId,
            IEnumerable<Secret> clientSecrets,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var query = from item in stored.ClientSecrets
                where !clientSecrets.Contains(item, SecretComparer.OrdinalIgnoreCase)
                select item;
            stored.ClientSecrets = query.ToList();
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task AddClaimsToClientByIdAsync(string clientId, IEnumerable<Claim> claims,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            List<Claim> ulist =
                stored.Claims.Union(claims, ClaimComparer.MinimalComparer)
                    .ToList();
            stored.Claims = ulist;
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task DeleteClaimsFromClientByIdAsync(string clientId, IEnumerable<Claim> claims,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var query = from item in stored.Claims
                where !claims.Contains(item, ClaimComparer.MinimalComparer)
                select item;
            stored.Claims = query.ToList();
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task UpdateClaimsInClientByIdAsync(string clientId, IEnumerable<Claim> claims,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindClientByClientIdAsync(clientId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var query = from item in stored.Claims
                where !claims.Contains(item, ClaimComparer.MinimalComparer)
                select item;
            var finalList = new List<Claim>();
            finalList.AddRange(query.ToList());
            finalList.AddRange(claims);
            stored.Claims = finalList;
            await UpsertClientAsync(stored, cancellationToken);
        }

        public async Task<Store.Core.Models.IPage<FlattenedClientHandle>> PageClientsAsync(int pageSize, byte[] pagingState,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                IPage<FlattenedClientHandle> page;
                if (pagingState == null)
                {
                    page = await mapper.FetchPageAsync<FlattenedClientHandle>(
                        Cql.New(SelectClientQuery).WithOptions(opt =>
                            opt.SetPageSize(pageSize)));
                }
                else
                {
                    page = await mapper.FetchPageAsync<FlattenedClientHandle>(
                        Cql.New(SelectClientQuery).WithOptions(opt =>
                            opt.SetPageSize(pageSize).SetPagingState(pagingState)));
                }

                // var result = CreatePageProxy(page);
                var result = new PageProxy<FlattenedClientHandle>(page);

                return result;
            }
            catch (Exception e)
            {
                // only here to catch during a debug unit test.
                throw;
            }
        }
    }


}

