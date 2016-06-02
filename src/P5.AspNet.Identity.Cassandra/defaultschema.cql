CREATE TABLE IF NOT EXISTS users (
    userid uuid,
	tenantid uuid,
    username text,
    password_hash text,
    security_stamp text,
    two_factor_enabled boolean,
    access_failed_count int,
    lockout_enabled boolean,
    lockout_end_date timestamp,
    phone_number text,
    phone_number_confirmed boolean,
    email text,
    email_confirmed boolean,
	created timestamp,
	modified timestamp,
	enabled boolean,
	source text,
	source_id text,
    PRIMARY KEY (userid)
);

// Users, keyed by username
CREATE TABLE IF NOT EXISTS users_by_username (
	tenantid uuid,
    username text,
    userid uuid,
    password_hash text,
    security_stamp text,
    two_factor_enabled boolean,
    access_failed_count int,
    lockout_enabled boolean,
    lockout_end_date timestamp,
    phone_number text,
    phone_number_confirmed boolean,
    email text,
    email_confirmed boolean,
	created timestamp,
	modified timestamp,
	enabled boolean,
	source text,
	source_id text,
    PRIMARY KEY (tenantid, username)
);

// Users, keyed by email address
CREATE TABLE IF NOT EXISTS users_by_email (
    email text,
    userid uuid,
	tenantid uuid,
    username text,
    password_hash text,
    security_stamp text,
    two_factor_enabled boolean,
    access_failed_count int,
    lockout_enabled boolean,
    lockout_end_date timestamp,
    phone_number text,
    phone_number_confirmed boolean,
    email_confirmed boolean,
	created timestamp,
	modified timestamp,
	enabled boolean,
	source text,
	source_id text,
    PRIMARY KEY ((tenantid, email))
);

// Logins, keyed by user id
CREATE TABLE IF NOT EXISTS logins (
    userid uuid,
    login_provider text,
    provider_key text,
	tenantid uuid,
    PRIMARY KEY(userid, login_provider, provider_key)
);

// Logins, keyed by provider and provider key
CREATE TABLE IF NOT EXISTS logins_by_provider (
    login_provider text,
    provider_key text,
	tenantid uuid,
    userid uuid,
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