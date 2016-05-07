CREATE KEYSPACE IF NOT EXISTS identityserver3
WITH replication = {'class':'SimpleStrategy', 'replication_factor' : 1};

use identityserver3;


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

CREATE TABLE IF NOT EXISTS tokenhandle_by_key (
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



CREATE INDEX IF NOT EXISTS
ON scopes_by_id (ShowInDiscoveryDocument);
