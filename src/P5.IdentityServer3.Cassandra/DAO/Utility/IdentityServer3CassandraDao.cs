using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cassandra.Mapping;
using P5.CassandraStore.DAO;

namespace P5.IdentityServer3.Cassandra.DAO
{
	public partial class IdentityServer3CassandraDao
	{
		//-----------------------------------------------
		// PREPARED STATEMENTS for Utility
		//-----------------------------------------------

		#region PREPARED STATEMENTS for Token

		private  List<string> IndexStatements;
		private  List<string> CreateTableStatemens;


		private static List<string> _tables = new List<string>()
		{
			"scopeclaims_by_name_and_scopeid",
			"scopeclaims_by_name_and_scopename",
			"scopes_by_id",
			"scopes_by_name",
			"AuthorizationCodeHandle_By_Key",
			"AuthorizationCodeHandle_By_ClientId",
			"RefreshTokenHandle_By_Key",
			"RefreshTokenHandle_By_ClientId",
			"TokenHandle_By_Key",
			"TokenHandle_By_ClientId",
			"clients_by_id",
			"consent_by_id",
			"consent_by_clientid",
            "user_profile_by_id",
            "user_clientid",
            "user_scopename"

		};



		#endregion

		public  void PrepareUtilityStatements()
		{
			#region PREPARED STATEMENTS for Utility

			/*
						 ************************************************

CREATE TABLE IF NOT EXISTS clients_by_id (
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
	PRIMARY KEY (id)
);
						 ************************************************
						 */
		    CreateTableStatemens = new List<string>()
		    {
		        //////////////////////////////////////////////////////////////////////
		        //clients_by_id//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS clients_by_id (" +
		        @"Id uuid," +
		        @"AbsoluteRefreshTokenLifetime int," +
		        @"AccessTokenLifetime int," +
		        @"AccessTokenType int," +
		        @"AllowAccessToAllCustomGrantTypes boolean," +
		        @"AllowAccessToAllScopes boolean," +
		        @"AllowAccessTokensViaBrowser boolean," +
		        @"AllowClientCredentialsOnly boolean," +
		        @"AllowedCorsOrigins text," +
		        @"AllowedCustomGrantTypes text," +
		        @"AllowedScopes text," +
		        @"AllowRememberConsent boolean," +
		        @"AlwaysSendClientClaims boolean," +
		        @"AuthorizationCodeLifetime int," +
		        @"Claims text," +
		        @"ClientId text," +
		        @"ClientName text," +
		        @"ClientSecrets text," +
		        @"ClientUri text," +
		        @"Enabled boolean," +
		        @"EnableLocalLogin boolean," +
		        @"Flow int," +
		        @"IdentityProviderRestrictions text," +
		        @"IdentityTokenLifetime int," +
		        @"IncludeJwtId boolean," +
		        @"LogoUri text," +
		        @"LogoutSessionRequired boolean," +
		        @"LogoutUri text," +
		        @"PostLogoutRedirectUris text," +
		        @"PrefixClientClaims boolean," +
		        @"RedirectUris text," +
		        @"RefreshTokenExpiration int," +
		        @"RefreshTokenUsage int," +
		        @"RequireConsent boolean," +
		        @"RequireSignOutPrompt boolean," +
		        @"SlidingRefreshTokenLifetime int," +
		        @"UpdateAccessTokenClaimsOnRefresh boolean," +
		        @"PRIMARY KEY (Id))",

		        //////////////////////////////////////////////////////////////////////
		        //consent_by_id//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS consent_by_id (" +
		        @"id uuid," +
		        @"ClientId text," +
		        @"Scopes text," +
		        @"Subject text," +
		        @"PRIMARY KEY (id,Subject))",

		        //////////////////////////////////////////////////////////////////////
		        //consent_by_clientid//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS consent_by_clientid (" +
		        @"id uuid," +
		        @"ClientId text," +
		        @"Scopes text," +
		        @"Subject text," +
		        @"PRIMARY KEY (ClientId,Subject))",

		        //////////////////////////////////////////////////////////////////////
		        //TokenHandle_By_Key//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS TokenHandle_By_Key (" +
		        @"Audience text," +
		        @"Claims text," +
		        @"ClientId text," +
		        @"CreationTime timestamp," +
		        @"Expires timestamp," +
		        @"Issuer text," +
		        @"Key text," +
		        @"Lifetime int," +
		        @"SubjectId text," +
		        @"Type text," +
		        @"Version int," +
		        @"PRIMARY KEY (Key))",

		        //////////////////////////////////////////////////////////////////////
		        //TokenHandle_By_ClientId//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS TokenHandle_By_ClientId (" +
		        @"Audience text," +
		        @"Claims text," +
		        @"ClientId text," +
		        @"CreationTime timestamp," +
		        @"Expires timestamp," +
		        @"Issuer text," +
		        @"Key text," +
		        @"Lifetime int," +
		        @"SubjectId text," +
		        @"Type text," +
		        @"Version int," +
		        @"PRIMARY KEY (ClientId,Key))",

		        //////////////////////////////////////////////////////////////////////
		        //RefreshTokenHandle_By_Key//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS RefreshTokenHandle_By_Key (" +
		        @"AccessToken text," +
		        @"ClientId text," +
		        @"CreationTime timestamp," +
		        @"Expires timestamp," +
		        @"Key text," +
		        @"Lifetime int," +
		        @"SubjectId text," +
		        @"Version int," +
		        @"PRIMARY KEY (Key))",

		        //////////////////////////////////////////////////////////////////////
		        //RefreshTokenHandle_By_ClientId//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS RefreshTokenHandle_By_ClientId (" +
		        @"AccessToken text," +
		        @"ClientId text," +
		        @"CreationTime timestamp," +
		        @"Expires timestamp," +
		        @"Key text," +
		        @"Lifetime int," +
		        @"SubjectId text," +
		        @"Version int," +
		        @"PRIMARY KEY (ClientId,Key))",

		        //////////////////////////////////////////////////////////////////////
		        //AuthorizationCodeHandle_By_Key//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS AuthorizationCodeHandle_By_Key (" +
		        @"ClaimIdentityRecords text," +
		        @"ClientId text," +
		        @"CreationTime timestamp," +
		        @"Expires timestamp," +
		        @"IsOpenId boolean," +
		        @"Key text," +
		        @"Nonce text," +
		        @"RedirectUri text," +
		        @"RequestedScopes text," +
		        @"SubjectId text," +
		        @"WasConsentShown boolean," +
		        @"PRIMARY KEY (Key))",

		        //////////////////////////////////////////////////////////////////////
		        //AuthorizationCodeHandle_By_ClientId//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS AuthorizationCodeHandle_By_ClientId (" +
		        @"ClaimIdentityRecords text," +
		        @"ClientId text," +
		        @"CreationTime timestamp," +
		        @"Expires timestamp," +
		        @"IsOpenId boolean," +
		        @"Key text," +
		        @"Nonce text," +
		        @"RedirectUri text," +
		        @"RequestedScopes text," +
		        @"SubjectId text," +
		        @"WasConsentShown boolean," +
		        @"PRIMARY KEY (ClientId,Key))",

		        //////////////////////////////////////////////////////////////////////
		        //scopeclaims_by_name_and_scopeid//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS scopeclaims_by_name_and_scopeid (" +
		        @"Name text," +
		        @"ScopeId uuid," +
		        @"ScopeName text," +
		        @"PRIMARY KEY (Name,ScopeId))",

		        //////////////////////////////////////////////////////////////////////
		        //scopeclaims_by_name_and_scopename//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS scopeclaims_by_name_and_scopename (" +
		        @"Name text," +
		        @"ScopeId uuid," +
		        @"ScopeName text," +
		        @"PRIMARY KEY (Name,ScopeName))",

		        //////////////////////////////////////////////////////////////////////
		        //scopes_by_id//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS scopes_by_id (" +
		        @"id uuid," +
		        @"AllowUnrestrictedIntrospection boolean," +
		        @"ClaimsDocument text," +
		        @"ClaimsRule text," +
		        @"Description text," +
		        @"DisplayName text," +
		        @"Emphasize boolean," +
		        @"Enabled boolean," +
		        @"IncludeAllClaimsForUser boolean," +
		        @"Name text," +
		        @"Required boolean," +
		        @"ScopeSecretsDocument text," +
		        @"ShowInDiscoveryDocument boolean," +
		        @"ScopeType int," +
		        @"PRIMARY KEY (id))",

		        //////////////////////////////////////////////////////////////////////
		        //scopes_by_name//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS scopes_by_name (" +
		        @"id uuid," +
		        @"AllowUnrestrictedIntrospection boolean," +
		        @"ClaimsDocument text," +
		        @"ClaimsRule text," +
		        @"Description text," +
		        @"DisplayName text," +
		        @"Emphasize boolean," +
		        @"Enabled boolean," +
		        @"IncludeAllClaimsForUser boolean," +
		        @"Name text," +
		        @"Required boolean," +
		        @"ScopeSecretsDocument text," +
		        @"ShowInDiscoveryDocument boolean," +
		        @"ScopeType int," +
		        @"PRIMARY KEY (Name))",
		        //////////////////////////////////////////////////////////////////////
		        //user_profile_by_id//////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS user_profile_by_id (" +
		        @"id uuid," +
		        @"Enabled boolean," +
		        @"UserId text," +
		        @"UserName text," +
		        @"PRIMARY KEY (id))",
		        //////////////////////////////////////////////////////////////////////
		        //user_clientid///////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS user_clientid (" +
		        @"ClientId text," +
		        @"UserId text," +
		        @"PRIMARY KEY (UserId,ClientId))",
		        //////////////////////////////////////////////////////////////////////
		        //user_scopename//////////////////////////////////////////////////////
		        //////////////////////////////////////////////////////////////////////
		        @"CREATE TABLE IF NOT EXISTS user_scopename (" +
		        @"ScopeName text," +
		        @"UserId text," +
		        @"PRIMARY KEY (UserId,ScopeName))"
		    };






			IndexStatements = new List<string>()
			{
				@"CREATE INDEX IF NOT EXISTS ON clients_by_id (ClientId)",

				@"CREATE INDEX IF NOT EXISTS ON AuthorizationCodeHandle_By_ClientId (SubjectId)",
				@"CREATE INDEX IF NOT EXISTS ON AuthorizationCodeHandle_By_ClientId (Key)",

				@"CREATE INDEX IF NOT EXISTS ON RefreshTokenHandle_By_ClientId (SubjectId)",
				@"CREATE INDEX IF NOT EXISTS ON RefreshTokenHandle_By_ClientId (Key)",

				@"CREATE INDEX IF NOT EXISTS ON consent_by_clientid (Subject)",

				@"CREATE INDEX IF NOT EXISTS ON consent_by_id (ClientId)",
				@"CREATE INDEX IF NOT EXISTS ON consent_by_id (Subject)",

				@"CREATE INDEX IF NOT EXISTS ON TokenHandle_By_ClientId (SubjectId)",
				@"CREATE INDEX IF NOT EXISTS ON TokenHandle_By_ClientId (Key)",

				@"CREATE INDEX IF NOT EXISTS ON scopes_by_id (ShowInDiscoveryDocument)",
				@"CREATE INDEX IF NOT EXISTS ON scopes_by_id (Name);"

			};


			#endregion

		}




        public async Task CreateTablesAsync(
			CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var session = CassandraSession;
				IMapper mapper = new Mapper(session);
				cancellationToken.ThrowIfCancellationRequested();


				foreach (var statement in CreateTableStatemens)
				{
					await mapper.ExecuteAsync(statement);
				}

				foreach (var statement in IndexStatements)
				{
					try
					{
						await mapper.ExecuteAsync(statement);
					}
					catch (Exception e)
					{
						if (e.Message.Contains("duplicate of existing index"))
						{
						}
						else
						{
							throw;
						}
					}
				}
			}
			catch (Exception e)
			{
				// only here to catch during a debug unit test.
				throw;
			}
		}

        public async Task<bool> TruncateTablesAsync(
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var session = CassandraSession;
			return await CassandraDao.TruncateTablesAsync(session, _tables, cancellationToken);
		}
	}
}
