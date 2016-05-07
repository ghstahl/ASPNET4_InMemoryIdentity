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
using IdentityServer3.Core.Services;
using log4net;
using Newtonsoft.Json;
using P5.CassandraStore.DAO;
using P5.CassandraStore.Extensions;
using P5.CassandraStore.Settings;
using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.Models;


namespace P5.IdentityServer3.Cassandra.DAO
{
    public class MyMappings : Mappings
    {
        private static bool _init { get; set; }

        public static void Init()
        {
            if (!_init)
            {
                MappingConfiguration.Global.Define<MyMappings>();
                _init = true;
            }

        }
        public MyMappings()
        {
            // Define mappings in the constructor of your class
            // that inherits from Mappings
            For<FlattenedTokenHandle>()
                .TableName("tokenhandle_by_key");
        }
    }

    public class IdentityServer3CassandraDao
    {
        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static CassandraConfig _CassandraConfig;

        public static CassandraConfig CassandraConfig
        {
            get
            {
                MyMappings.Init();
                return _CassandraConfig ?? (_CassandraConfig = new CassandraConfig
                {
                    ContactPoints = new List<string> {"cassandra"},
                    Credentials = new CassandraCredentials() {Password = "", UserName = ""},
                    KeySpace = "identityserver3"
                });
            }
            set { _CassandraConfig = value; }
        }

        private static ISession _cassandraSession = null;
        //-----------------------------------------------
        // PREPARED STATEMENTS for Scope
        //-----------------------------------------------
        private static AsyncLazy<PreparedStatement> _CreateScopeById { get; set; }
        private static AsyncLazy<PreparedStatement> _CreateScopeByName { get; set; }
        private static AsyncLazy<PreparedStatement[]> _CreateScope { get; set; }
        private static AsyncLazy<PreparedStatement> _CreateScopeClaimByNameAndScopeId { get; set; }
        private static AsyncLazy<PreparedStatement> _CreateScopeClaimByNameAndScopeName { get; set; }

        private static AsyncLazy<PreparedStatement[]> _CreateScopeClaim { get; set; }

        private static AsyncLazy<PreparedStatement> _FindScopeById { get; set; }
        private static AsyncLazy<PreparedStatement> _FindScopeByNamee { get; set; }

        private static AsyncLazy<PreparedStatement> _CreateClientById { get; set; }
        private static AsyncLazy<PreparedStatement> _FindClientById { get; set; }

        private static AsyncLazy<PreparedStatement> _CreateTokenByClientId { get; set; }
        private static AsyncLazy<PreparedStatement> _CreateTokenByKey { get; set; }

        private static AsyncLazy<PreparedStatement> _DeleteTokenByClientIdAndKey { get; set; }
        private static AsyncLazy<PreparedStatement> _DeleteTokenByKey { get; set; }



        public static ISession CassandraSession
        {
            get
            {
                try
                {
                    if (_cassandraSession == null)
                    {
                        var cc = new CassandraConfig
                        {
                            ContactPoints = new List<string> {"cassandra"},
                            Credentials = new CassandraCredentials() {Password = "", UserName = ""},
                            KeySpace = "identityserver3"
                        };
                        var dao = new CassandraDao(cc);
                        _cassandraSession = dao.GetSession();

                        //-----------------------------------------------
                        // PREPARED STATEMENTS for Scope
                        //-----------------------------------------------
                        _CreateScopeClaimByNameAndScopeId =
                            new AsyncLazy<PreparedStatement>(
                                () =>
                                {
                                    var result = _cassandraSession.PrepareAsync(
                                        @"INSERT INTO " +
                                        @"scopeclaims_by_name_and_scopeid(Name,ScopeId,ScopeName) " +
                                        @"VALUES(?,?,?)");
                                    return result;
                                });
                        _CreateScopeClaimByNameAndScopeName =
                            new AsyncLazy<PreparedStatement>(
                                () =>
                                {
                                    var result = _cassandraSession.PrepareAsync(
                                        @"INSERT INTO " +
                                        @"scopeclaims_by_name_and_scopename(Name,ScopeId,ScopeName) " +
                                        @"VALUES(?,?,?)");
                                    return result;
                                });
                        _CreateScopeClaim = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
                        {
                            _CreateScopeClaimByNameAndScopeId.Value,
                            _CreateScopeClaimByNameAndScopeName.Value,
                        }));

                        /*
                        INSERT
                        INTO scopes (Id,AllowUnrestrictedIntrospection,ClaimsRule,Description,DisplayName,
                         *          Emphasize,Enabled,IncludeAllClaimsForUser,name,Required,ScopeSecrets,ShowInDiscoveryDocument,ScopeType)
                        VALUES (1f65aebc-bf07-4afc-aa05-e9a1ed48e0b0,true,'1 ClaimsRule','1 Description','1 DisplayName',true,true,true,'1 name',true,[ 'rivendell', 'rohan' ],true,1 );
                        */
                        _CreateScopeById =
                            new AsyncLazy<PreparedStatement>(
                                () =>
                                {
                                    var result = _cassandraSession.PrepareAsync(
                                        @"INSERT INTO " +
                                        @"scopes_by_id (Id,AllowUnrestrictedIntrospection,ClaimsDocument,ClaimsRule,Description,DisplayName,Emphasize,Enabled,IncludeAllClaimsForUser,name,Required,ScopeSecretsDocument,ShowInDiscoveryDocument,ScopeType) " +
                                        @"VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)");
                                    return result;
                                });

                        _CreateScopeByName =
                            new AsyncLazy<PreparedStatement>(
                                () =>
                                {
                                    var result = _cassandraSession.PrepareAsync(
                                        @"INSERT INTO " +
                                        @"scopes_by_name (Id,AllowUnrestrictedIntrospection,ClaimsDocument,ClaimsRule,Description,DisplayName,Emphasize,Enabled,IncludeAllClaimsForUser,name,Required,ScopeSecretsDocument,ShowInDiscoveryDocument,ScopeType) " +
                                        @"VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)");
                                    return result;
                                });
                        // All the statements needed by the CreateAsync method
                        _CreateScope = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
                        {
                            _CreateScopeById.Value,
                            _CreateScopeByName.Value,
                        }));

                        _FindScopeById =
                            new AsyncLazy<PreparedStatement>(
                                () => _cassandraSession.PrepareAsync("SELECT * " +
                                                                     "FROM scopes_by_id " +
                                                                     "WHERE id = ?"));

                        _FindScopeByNamee =
                            new AsyncLazy<PreparedStatement>(
                                () => _cassandraSession.PrepareAsync("SELECT * " +
                                                                     "FROM scopes_by_name " +
                                                                     "WHERE name = ?"));

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


                        /*
                         ************************************************
                            Audience text,
                            Claims text,
                            ClientId text,
                            CreationTime timestamp,
                            Expires timestamp,
                            Issuer text,
                            Key text,
                            Lifetime int,
                            SubjectId text,
                            Type text,
                            Version int,
                         ************************************************
                         */
                        _CreateTokenByClientId =
                          new AsyncLazy<PreparedStatement>(
                              () =>
                              {
                                  var result = _cassandraSession.PrepareAsync(
                                      @"INSERT INTO " +
                                      @"TokenHandle_By_ClientId(Audience,Claims,ClientId,CreationTime,Expires,Issuer,Key,Lifetime,SubjectId,Type,Version) " +
                                      @"VALUES(?,?,?,?,?,?,?,?,?,?,?)");
                                  return result;
                              });
                        _CreateTokenByKey =
                          new AsyncLazy<PreparedStatement>(
                              () =>
                              {
                                  var result = _cassandraSession.PrepareAsync(
                                      @"INSERT INTO " +
                                      @"TokenHandle_By_Key(Audience,Claims,ClientId,CreationTime,Expires,Issuer,Key,Lifetime,SubjectId,Type,Version) " +
                                      @"VALUES(?,?,?,?,?,?,?,?,?,?,?)");
                                  return result;
                              });

                        _DeleteTokenByClientIdAndKey =
                          new AsyncLazy<PreparedStatement>(
                              () =>
                              {
                                  var result = _cassandraSession.PrepareAsync(
                                      @"Delete FROM tokenhandle_by_clientid " +
                                      @"WHERE clientid = ? " +
                                      @"AND key = ?");
                                  return result;
                              });
                        _DeleteTokenByKey =
                          new AsyncLazy<PreparedStatement>(
                              () =>
                              {
                                  var result = _cassandraSession.PrepareAsync(
                                      @"Delete FROM tokenhandle_by_key " +
                                      @"WHERE key = ?");
                                  return result;
                              });


                    }
                }
                catch (Exception e)
                {
                    _cassandraSession = null;
                }
                return _cassandraSession;
            }
        }

        public static async Task<List<BoundStatement>> BuildBoundStatements_ForTokenHandleDelete(string clientId,
            string key)
        {
            var result = new List<BoundStatement>();
            PreparedStatement prepared_DeleteTokenByClientIdAndKey = await _DeleteTokenByClientIdAndKey;
            PreparedStatement prepared_DeleteTokenByKey = await _DeleteTokenByKey;

            BoundStatement bound_DeleteTokenByClientIdAndKey = prepared_DeleteTokenByClientIdAndKey.Bind(
                clientId, key);

            BoundStatement bound_DeleteTokenByKey = prepared_DeleteTokenByKey.Bind(key);

            result.Add(bound_DeleteTokenByClientIdAndKey);
            result.Add(bound_DeleteTokenByKey);
            return result;
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

        public static async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
            IEnumerable<ScopeClaimRecord> scopeClaimRecords)
        {
            var result = new List<BoundStatement>();
            foreach (var scopeClaimRecord in scopeClaimRecords)
            {
                PreparedStatement[] prepared = await _CreateScopeClaim;
                PreparedStatement preparedByNameAndScopeId = prepared[0];
                PreparedStatement preparedByNameAndScopeName = prepared[1];

                /*
                 * @"scopeclaims_by_name_and_scopeid(Name,ScopeId,ScopeName,AlwaysIncludeInIdToken,Description) " +
                */
                BoundStatement statementByNameAndScopeId = preparedByNameAndScopeId.Bind(
                    scopeClaimRecord.Name,
                    scopeClaimRecord.ScopeId,
                    scopeClaimRecord.ScopeName
                    );
                BoundStatement statementByNameAndScopeName = preparedByNameAndScopeName.Bind(
                    scopeClaimRecord.Name,
                    scopeClaimRecord.ScopeId,
                    scopeClaimRecord.ScopeName
                    );

                result.Add(statementByNameAndScopeId);
                result.Add(statementByNameAndScopeName);
            }
            return result;
        }

        public static async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
        IEnumerable<FlattenedTokenHandle> flattenedTokenHandles)
        {

            var result = new List<BoundStatement>();
            foreach (var flattenedTokenHandle in flattenedTokenHandles)
            {
                PreparedStatement prepared_CreateTokenByClientId = await _CreateTokenByClientId;
                PreparedStatement prepared_CreateTokenByKey = await _CreateTokenByKey;
                BoundStatement bound_CreateTokenByClientId = prepared_CreateTokenByClientId.Bind(
                    flattenedTokenHandle.Audience,
                    flattenedTokenHandle.Claims,
                    flattenedTokenHandle.ClientId,
                    flattenedTokenHandle.CreationTime,
                    flattenedTokenHandle.Expires,
                    flattenedTokenHandle.Issuer,
                    flattenedTokenHandle.Key,
                    flattenedTokenHandle.Lifetime,
                    flattenedTokenHandle.SubjectId,
                    flattenedTokenHandle.Type,
                    flattenedTokenHandle.Version
                    );
                BoundStatement bound_CreateTokenByKey = prepared_CreateTokenByKey.Bind(
                     flattenedTokenHandle.Audience,
                     flattenedTokenHandle.Claims,
                     flattenedTokenHandle.ClientId,
                     flattenedTokenHandle.CreationTime,
                     flattenedTokenHandle.Expires,
                     flattenedTokenHandle.Issuer,
                     flattenedTokenHandle.Key,
                     flattenedTokenHandle.Lifetime,
                     flattenedTokenHandle.SubjectId,
                     flattenedTokenHandle.Type,
                     flattenedTokenHandle.Version
                     );
                result.Add(bound_CreateTokenByClientId);
                result.Add(bound_CreateTokenByKey);
            }
            return result;
        }

        public static async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
            IEnumerable<ScopeRecord> scopeRecords)
        {
            var result = new List<BoundStatement>();
            foreach (var scopeRecord in scopeRecords)
            {
                PreparedStatement[] prepared = await _CreateScope;
                var scope = scopeRecord.Record;
                var scopeSecretsDocument = new SimpleDocument<List<Secret>>(scope.ScopeSecrets);
                var claimsDocument = new SimpleDocument<List<ScopeClaim>>(scope.Claims);
                int scopeType = (int) scope.Type;
                var preparedById = prepared[0];
                var preparedByName = prepared[1];
                /*@"scopes_by_id (
                 * Id,
                 * AllowUnrestrictedIntrospection,
                 * ClaimsRule,
                 * Description,
                 * DisplayName,
                 * Emphasize,
                 * Enabled,
                 * IncludeAllClaimsForUser,
                 * Name,
                 * Required,
                 * ScopeSecrets,
                 * ShowInDiscoveryDocument,
                 * ScopeType) " +
                */
                BoundStatement boundById = preparedById.Bind(
                    scopeRecord.Id,
                    scope.AllowUnrestrictedIntrospection,
                    claimsDocument.DocumentJson,
                    scope.ClaimsRule,
                    scope.Description,
                    scope.DisplayName,
                    scope.Emphasize,
                    scope.Enabled,
                    scope.IncludeAllClaimsForUser,
                    scope.Name,
                    scope.Required,
                    scopeSecretsDocument.DocumentJson,
                    scope.ShowInDiscoveryDocument,
                    scopeType);

                //@"producttemplates_by_type(documenttype,documentversion,id,document) " +
                BoundStatement boundByName = preparedByName.Bind(
                    scopeRecord.Id,
                    scope.AllowUnrestrictedIntrospection,
                    claimsDocument.DocumentJson,
                    scope.ClaimsRule,
                    scope.Description,
                    scope.DisplayName,
                    scope.Emphasize,
                    scope.Enabled,
                    scope.IncludeAllClaimsForUser,
                    scope.Name,
                    scope.Required,
                    scopeSecretsDocument.DocumentJson,
                    scope.ShowInDiscoveryDocument,
                    scopeType);


                result.Add(boundById);
                result.Add(boundByName);


                var claimsQuery = from scopeClaim in scope.Claims
                    select new ScopeClaimRecord(scopeRecord.Id, scopeRecord.Record.Name, scopeClaim);

                var scopeClaimBoundStatements = await BuildBoundStatements_ForCreate(claimsQuery);
                result.AddRange(scopeClaimBoundStatements);
            }
            return result;
        }

        public static async Task<bool> CreateManyScopeClaimAsync(List<ScopeClaimRecord> scopeClaimsRecords,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            cancellationToken.ThrowIfCancellationRequested();

            if (scopeClaimsRecords == null)
                throw new ArgumentNullException("scopeClaims");
            if (scopeClaimsRecords.Count == 0)
                throw new ArgumentException("scopeClaims is empty");



            var batch = new BatchStatement();
            var boundStatements = await BuildBoundStatements_ForCreate(scopeClaimsRecords);
            batch.AddRange(boundStatements);

            await session.ExecuteAsync(batch).ConfigureAwait(false);
            return true;
        }

        public static async Task<bool> CreateScopeAsync(ScopeRecord scopeRecord,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IList<ScopeRecord> scopeRecords = new List<ScopeRecord>();
                scopeRecords.Add(scopeRecord);
                return await CreateManyScopeAsync(scopeRecords, cancellationToken);
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static async Task<bool> CreateManyScopeAsync(IList<ScopeRecord> scopeRecords,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {

                var session = CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();

                if (scopeRecords == null)
                    throw new ArgumentNullException("scopeRecord");
                if (scopeRecords.Count == 0)
                    throw new ArgumentException("scopeRecords is empty");

                var batch = new BatchStatement();
                var boundStatements = await BuildBoundStatements_ForCreate(scopeRecords);
                batch.AddRange(boundStatements);
                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static async Task<global::IdentityServer3.Core.Models.Scope> FindScopeByIdAsync(Guid id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record = await mapper.SingleAsync<ScopeMappedRecord>("SELECT * FROM scopes_by_id WHERE id = ?", id);
                record.Claims = JsonConvert.DeserializeObject<List<ScopeClaim>>(record.ClaimsDocument);
                return record;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<global::IdentityServer3.Core.Models.Scope> FindScopeByName(string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await mapper.SingleAsync<ScopeMappedRecord>("SELECT * FROM scopes_by_name WHERE name = ?", name);
                record.Claims = JsonConvert.DeserializeObject<List<ScopeClaim>>(record.ClaimsDocument);
                return record;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<IEnumerable<global::IdentityServer3.Core.Models.Scope>> FindScopesByNamesAsync(
            IEnumerable<string> scopeNames,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var queryInValues = from item in scopeNames
                    select string.Format("'{0}'", item);

                var inValues = string.Join(",", queryInValues);
                var query = string.Format("SELECT * FROM scopes_by_name WHERE name IN ({0})", inValues);


                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                var scopeMappedRecords = (await mapper.FetchAsync<ScopeMappedRecord>(query)).ToList();


                foreach (var scopeMappedRecord in scopeMappedRecords)
                {
                    scopeMappedRecord.Claims =
                        JsonConvert.DeserializeObject<List<ScopeClaim>>(scopeMappedRecord.ClaimsDocument);
                }
                var queryFinal = from item in scopeMappedRecords
                    select (Scope) item;
                return queryFinal;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<IEnumerable<global::IdentityServer3.Core.Models.Scope>> FindScopesAsync(
            bool publicOnly = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var query = string.Format("SELECT * FROM scopes_by_id WHERE ShowInDiscoveryDocument = {0}",
                    publicOnly ? "true" : "false");
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                var scopeMappedRecords = (await mapper.FetchAsync<ScopeMappedRecord>(query)).ToList();

                foreach (var scopeMappedRecord in scopeMappedRecords)
                {
                    scopeMappedRecord.Claims =
                        JsonConvert.DeserializeObject<List<ScopeClaim>>(scopeMappedRecord.ClaimsDocument);
                }
                var queryFinal = from item in scopeMappedRecords
                    select (Scope) item;
                return queryFinal;
            }
            catch (Exception e)
            {
                return null;
            }
        }

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
                var myList = new List<FlattenedClientRecord> {client};
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

         public static async Task<global::IdentityServer3.Core.Models.Token> FindTokenByKey(string key,IClientStore clientStore,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await mapper.SingleAsync<FlattenedTokenHandle>("SELECT * FROM tokenhandle_by_key WHERE key = ?", key);
                ITokenHandle ch = record;
                var result = ch.MakeIdentityServerToken(clientStore);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
         public static async Task<IEnumerable<ITokenMetadata>> FindTokenMetadataBySubject(string subject, IClientStore clientStore,
          CancellationToken cancellationToken = default(CancellationToken))
         {
             try
             {
                 MyMappings.Init();
                 var session = CassandraSession;
                 IMapper mapper = new Mapper(session);
                 cancellationToken.ThrowIfCancellationRequested();
                 var record =
                     await mapper.FetchAsync<FlattenedTokenHandle>("SELECT * FROM tokenhandle_by_clientid WHERE subjectid = ?", subject);
                 var query = from item in record
                     select item.MakeIdentityServerToken(clientStore);
                 return query;
             }
             catch (Exception e)
             {
                 return null;
             }
         }
 
        public static async Task<bool> DeleteTokensByClientId (string client,
           CancellationToken cancellationToken = default(CancellationToken))
         {
             try
             {
                 MyMappings.Init();
                 var session = CassandraSession;
                 IMapper mapper = new Mapper(session);
                 cancellationToken.ThrowIfCancellationRequested();

                 // first find all the keys that are associated with this client
                 var record_find =
                   await mapper.FetchAsync<FlattenedTokenHandle>("SELECT * FROM tokenhandle_by_clientid WHERE ClientId = ?",client);
                 cancellationToken.ThrowIfCancellationRequested();

                 // now that we gots ourselves the record, we have the primary key
                 // we can now build a big batch delete
                 var batch = new BatchStatement();
                 foreach (var rFind in record_find)
                 {

                     var boundStatements = await BuildBoundStatements_ForTokenHandleDelete(rFind.ClientId, rFind.Key);
                     batch.AddRange(boundStatements);
                 }
                 await session.ExecuteAsync(batch).ConfigureAwait(false);
                 return true;
             }
             catch (Exception e)
             {
                 return false;
             }
         }
         
        public static async Task<bool> DeleteTokensByClientIdAndSubjectId(string client, string subject,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                // first find all the keys that are associated with this client and subject
                var record_find =
                  await mapper.FetchAsync<FlattenedTokenHandle>("SELECT * FROM tokenhandle_by_clientid WHERE ClientId = ? AND subjectId = ?",
                  client,subject);
                cancellationToken.ThrowIfCancellationRequested();

                // now that we gots ourselves the record, we have the primary key
                // we can now build a big batch delete
                var batch = new BatchStatement();
                foreach (var rFind in record_find)
                {

                    var boundStatements = await BuildBoundStatements_ForTokenHandleDelete(rFind.ClientId, rFind.Key);
                    batch.AddRange(boundStatements);
                }
                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task<bool> DeleteTokenByKey(string key,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record_find =
                  await mapper.SingleAsync<FlattenedTokenHandle>("SELECT * FROM tokenhandle_by_key WHERE key = ?", key);

                // now that we gots ourselves the record, we have the primary key

                cancellationToken.ThrowIfCancellationRequested();

                var batch = new BatchStatement();
                var boundStatements = await BuildBoundStatements_ForTokenHandleDelete(record_find.ClientId,key);
                batch.AddRange(boundStatements);

                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static async Task<bool> CreateTokenHandleAsync(FlattenedTokenHandle tokenHandle,
             CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var list = new List<FlattenedTokenHandle> {tokenHandle};
                return await CreateManyTokenHandleAsync(list, cancellationToken);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task<bool> CreateManyTokenHandleAsync(IList<FlattenedTokenHandle> flattenedTokenHandles,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (flattenedTokenHandles == null)
                    throw new ArgumentNullException("flattenedTokenHandles");
                if (flattenedTokenHandles.Count == 0)
                    throw new ArgumentException("flattenedTokenHandles is empty");

                var session = CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();

                var batch = new BatchStatement();
                var boundStatements = await BuildBoundStatements_ForCreate(flattenedTokenHandles);
                batch.AddRange(boundStatements);

                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}
