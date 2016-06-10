CREATE KEYSPACE IF NOT EXISTS identityserver3
WITH replication = {'class':'SimpleStrategy', 'replication_factor' : 1};

use identityserver3;

CREATE TABLE IF NOT EXISTS user_profile_by_id (
    id uuid,
    Enabled boolean,
    UserId text,
    UserName text,
    PRIMARY KEY (id)
);


CREATE TABLE IF NOT EXISTS user_clientid (
    ClientId text,
   	UserId text,
    PRIMARY KEY (UserId,ClientId)
);
CREATE TABLE IF NOT EXISTS user_scopename (
    ScopeName text,
    UserId text,
    PRIMARY KEY (UserId,ScopeName)
);


CREATE TABLE IF NOT EXISTS scopeclaims_by_name_and_scopeid (
	Name text,
 	ScopeId uuid,
 	ScopeName text,
    PRIMARY KEY (Name,ScopeId)
);

CREATE TABLE IF NOT EXISTS scopeclaims_by_name_and_scopename (
	Name text,
 	ScopeId uuid,
 	ScopeName text,
    PRIMARY KEY (Name,ScopeName)
);

CREATE TABLE IF NOT EXISTS scopes_by_id (
    id uuid,
    AllowUnrestrictedIntrospection boolean,
    ClaimsDocument text,
    ClaimsRule text,
    Description text,
    DisplayName text,
    Emphasize boolean,
    Enabled boolean,
    IncludeAllClaimsForUser boolean,
    Name text,
    Required boolean,
    ScopeSecretsDocument text,
    ShowInDiscoveryDocument boolean,
    ScopeType int,
    PRIMARY KEY (id)
);


CREATE TABLE IF NOT EXISTS scopes_by_name (
    id uuid,
    AllowUnrestrictedIntrospection boolean,
    ClaimsDocument text,
    ClaimsRule text,
    Description text,
    DisplayName text,
    Emphasize boolean,
    Enabled boolean,
    IncludeAllClaimsForUser boolean,
    Name text,
    Required boolean,
  	ScopeSecretsDocument text,
    ShowInDiscoveryDocument boolean,
    ScopeType int,
    PRIMARY KEY (Name)
);

CREATE TABLE IF NOT EXISTS AuthorizationCodeHandle_By_Key (
    ClaimIdentityRecords text,
    ClientId text,
    CreationTime timestamp,
    Expires timestamp,
    IsOpenId boolean,
    Key text,
    Nonce text,
    RedirectUri text,
    RequestedScopes text,
    SubjectId text,
    WasConsentShown boolean,
    PRIMARY KEY (Key)
);

CREATE TABLE IF NOT EXISTS AuthorizationCodeHandle_By_ClientId (
    ClaimIdentityRecords text,
    ClientId text,
    CreationTime timestamp,
    Expires timestamp,
    IsOpenId boolean,
    Key text,
    Nonce text,
    RedirectUri text,
    RequestedScopes text,
    SubjectId text,
    WasConsentShown boolean,
    PRIMARY KEY (ClientId,Key)
);

CREATE TABLE IF NOT EXISTS RefreshTokenHandle_By_Key (
    AccessToken text,
    ClientId text,
    CreationTime timestamp,
    Expires timestamp,
    Key text,
    Lifetime int,
    SubjectId text,
    Version int,
    PRIMARY KEY (Key)
);
CREATE TABLE IF NOT EXISTS RefreshTokenHandle_By_ClientId (
    AccessToken text,
    ClientId text,
    CreationTime timestamp,
    Expires timestamp,
    Key text,
    Lifetime int,
    SubjectId text,
    Version int,
    PRIMARY KEY (ClientId,Key)
);


CREATE TABLE IF NOT EXISTS TokenHandle_By_Key (
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
    PRIMARY KEY (Key)
);
CREATE TABLE IF NOT EXISTS TokenHandle_By_ClientId (
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
    PRIMARY KEY (ClientId,Key)
);

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

CREATE TABLE IF NOT EXISTS consent_by_id (
    id uuid,
    ClientId text,
    Scopes text,
    Subject text,
    PRIMARY KEY (id,Subject)
);

CREATE TABLE IF NOT EXISTS consent_by_clientid (
    id uuid,
    ClientId text,
    Scopes text,
    Subject text,
    PRIMARY KEY (ClientId,Subject)
);

CREATE INDEX IF NOT EXISTS
ON AuthorizationCodeHandle_By_ClientId (SubjectId);

CREATE INDEX IF NOT EXISTS
ON AuthorizationCodeHandle_By_ClientId (Key);

CREATE INDEX IF NOT EXISTS
ON RefreshTokenHandle_By_ClientId (SubjectId);

CREATE INDEX IF NOT EXISTS
ON RefreshTokenHandle_By_ClientId (Key);

CREATE INDEX IF NOT EXISTS
ON consent_by_clientid (Subject);

CREATE INDEX IF NOT EXISTS
ON consent_by_id (ClientId);

CREATE INDEX IF NOT EXISTS
ON consent_by_id (Subject);

CREATE INDEX IF NOT EXISTS
ON TokenHandle_By_ClientId (SubjectId);

CREATE INDEX IF NOT EXISTS
ON TokenHandle_By_ClientId (Key);

CREATE INDEX IF NOT EXISTS
ON scopes_by_id (ShowInDiscoveryDocument);

CREATE INDEX IF NOT EXISTS
ON user_profile_by_id (UserId);


