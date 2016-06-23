// Users, keyed by user id
CREATE KEYSPACE IF NOT EXISTS aspnetidentity
WITH replication = {'class':'SimpleStrategy', 'replication_factor' : 1};
use aspnetidentity;

CREATE TABLE IF NOT EXISTS users ( 
	access_failed_count int,
	created timestamp,
	email text,
	email_confirmed boolean,
	enabled boolean,
	lockout_enabled boolean,
	lockout_end_date timestamp,
	modified timestamp,
	password_hash text,
	phone_number text,
	phone_number_confirmed boolean,
	security_stamp text,
	source text,
	source_id text,
	tenantid uuid,
	two_factor_enabled boolean,
	userid uuid,
	username text,
	
	PRIMARY KEY (userid)
);

// Users, keyed by username
CREATE TABLE IF NOT EXISTS users_by_username (
	access_failed_count int,
	created timestamp,
	email text,
	email_confirmed boolean,
	enabled boolean,
	lockout_enabled boolean,
	lockout_end_date timestamp,
	modified timestamp,
	password_hash text,
	phone_number text,
	phone_number_confirmed boolean,
	security_stamp text,
	source text,
	source_id text,
	tenantid uuid,
	two_factor_enabled boolean,
	userid uuid,
	username text,
	
    PRIMARY KEY (tenantid, username)
);

// Users, keyed by email address
CREATE TABLE IF NOT EXISTS users_by_email (
    access_failed_count int,
	created timestamp,
	email text,
	email_confirmed boolean,
	enabled boolean,
	lockout_enabled boolean,
	lockout_end_date timestamp,
	modified timestamp,
	password_hash text,
	phone_number text,
	phone_number_confirmed boolean,
	security_stamp text,
	source text,
	source_id text,
	tenantid uuid,
	two_factor_enabled boolean,
	userid uuid,
	username text,
	
    PRIMARY KEY ((tenantid, email))
);

// Logins, keyed by user id
CREATE TABLE IF NOT EXISTS logins (
    tenantid uuid,
	userid uuid,
	login_provider text,
    provider_key text,
    PRIMARY KEY(userid, login_provider, provider_key)
);

// Logins, keyed by provider and provider key
CREATE TABLE IF NOT EXISTS logins_by_provider (
    tenantid uuid,
	userid uuid,
	login_provider text,
    provider_key text,
    PRIMARY KEY ((login_provider, provider_key), tenantid)
);

// Claims, keyed by user id
CREATE TABLE IF NOT EXISTS claims (
    userid uuid,
    type text,
    value text,
    PRIMARY KEY (userid, type, value)
);

CREATE TABLE IF NOT EXISTS roles (
    roleid uuid,
    name text,
	displayname text,
	is_systemrole boolean,
	is_global boolean,
    tenantid uuid,
	created timestamp,
	modified timestamp,
    PRIMARY KEY (roleid)
);

CREATE TABLE IF NOT EXISTS roles_by_name (
    roleid uuid,
    name text,
	displayname text,
	is_systemrole boolean,
	is_global boolean,
    tenantid uuid,
	created timestamp,
	modified timestamp,
    PRIMARY KEY (tenantid, name)
);

CREATE TABLE IF NOT EXISTS user_roles (
    userid uuid,
    rolename text,
	assigned timestamp,
    PRIMARY KEY (userid, rolename)
);

CREATE TABLE IF NOT EXISTS user_roles_by_role (
    rolename text,
    userid uuid,
	assigned timestamp,
    PRIMARY KEY (rolename, userid)
);