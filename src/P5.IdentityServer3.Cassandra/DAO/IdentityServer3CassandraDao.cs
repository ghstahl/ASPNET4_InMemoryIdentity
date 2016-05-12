using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using log4net;
using Newtonsoft.Json;
using P5.CassandraStore.DAO;
using P5.CassandraStore.Extensions;
using P5.CassandraStore.Settings;
using P5.IdentityServer3.Common.Models;


namespace P5.IdentityServer3.Cassandra.DAO
{
    public partial class IdentityServer3CassandraDao
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

                        #region PREPARED STATEMENTS for Scope

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

                        #endregion

                        //-----------------------------------------------
                        // PREPARED STATEMENTS for Client
                        //-----------------------------------------------

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

                        #endregion

                        //-----------------------------------------------
                        // PREPARED STATEMENTS for Token
                        //-----------------------------------------------
                        #region PREPARED STATEMENTS for Token
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
                        #endregion

                        //-----------------------------------------------
                        // PREPARED STATEMENTS for RefreshToken
                        //-----------------------------------------------
                        #region PREPARED STATEMENTS for RefreshToken
                        /*
                         ************************************************
                            AccessToken text,
                            ClientId text,
                            CreationTime timestamp,
                            Expires timestamp,
                            Key text,
                            Lifetime int,
                            SubjectId text,
                            Version int,
                         ************************************************
                         */
                        _CreateRefreshTokenByClientId =
                            new AsyncLazy<PreparedStatement>(
                                () =>
                                {
                                    var result = _cassandraSession.PrepareAsync(
                                        @"INSERT INTO " +
                                        @"RefreshTokenHandle_By_ClientId(AccessToken, ClientId,CreationTime,Expires,Key,Lifetime,SubjectId,Version) " +
                                        @"VALUES(?,?,?,?,?,?,?,?)");
                                    return result;
                                });
                        _CreateRefreshTokenByKey =
                            new AsyncLazy<PreparedStatement>(
                                () =>
                                {
                                    var result = _cassandraSession.PrepareAsync(
                                        @"INSERT INTO " +
                                        @"RefreshTokenHandle_By_ClientId(AccessToken, ClientId,CreationTime,Expires,Key,Lifetime,SubjectId,Version) " +
                                        @"VALUES(?,?,?,?,?,?,?,?)");
                                    return result;
                                });

                        _DeleteRefreshTokenByClientIdAndKey =
                            new AsyncLazy<PreparedStatement>(
                                () =>
                                {
                                    var result = _cassandraSession.PrepareAsync(
                                        @"Delete FROM Refreshtokenhandle_by_clientid " +
                                        @"WHERE clientid = ? " +
                                        @"AND key = ?");
                                    return result;
                                });
                        _DeleteRefreshTokenByKey =
                            new AsyncLazy<PreparedStatement>(
                                () =>
                                {
                                    var result = _cassandraSession.PrepareAsync(
                                        @"Delete FROM Refreshtokenhandle_by_key " +
                                        @"WHERE key = ?");
                                    return result;
                                });
                        #endregion

                        //-----------------------------------------------
                        // PREPARED STATEMENTS for Consent
                        //-----------------------------------------------
                        #region PREPARED STATEMENTS for Consent
                        /*
                         ************************************************
                            id uuid,
                            ClientId text,
                            Scopes text,
                            Subject text,
                         ************************************************
                         */
                        _CreateConsentById =
                            new AsyncLazy<PreparedStatement>(
                                () =>
                                {
                                    var result = _cassandraSession.PrepareAsync(
                                        @"INSERT INTO " +
                                        @"consent_by_id(id,ClientId,Scopes,Subject) " +
                                        @"VALUES(?,?,?,?)");
                                    return result;
                                });
                        _CreateConsentByClientId =
                            new AsyncLazy<PreparedStatement>(
                                () =>
                                {
                                    var result = _cassandraSession.PrepareAsync(
                                        @"INSERT INTO " +
                                        @"consent_by_clientid(id,ClientId,Scopes,Subject) " +
                                        @"VALUES(?,?,?,?)");
                                    return result;
                                });

                        #endregion
                    }
                }
                catch (Exception e)
                {
                    _cassandraSession = null;
                }
                return _cassandraSession;
            }
        }




    }
}
