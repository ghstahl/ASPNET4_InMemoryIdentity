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
CREATE INDEX IF NOT EXISTS
ON scopes_by_id (ShowInDiscoveryDocument);

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


