CREATE KEYSPACE IF NOT EXISTS notforproduction
WITH replication = {'class':'SimpleStrategy', 'replication_factor' : 1};

use notforproduction;

CREATE TABLE IF NOT EXISTS notforproduction_by_id (
	id uuid,
	documentjson text,
	documenttype text,
	documentversion text,
	PRIMARY KEY (id));

CREATE TABLE  IF NOT EXISTS notforproduction_by_type_and_version (
	documenttype text,
	documentversion text,
	documentjson text,
	id uuid,
	PRIMARY KEY (documenttype, documentversion)
);

SELECT * from notforproduction_by_id;