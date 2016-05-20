using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using Cassandra;
using log4net;
using P5.CassandraStore.DAO;
using P5.CassandraStore.Settings;
using ProductStore.Cassandra.DAO;

namespace FlattenedDocument.CassandraStore.DAO
{
    /*
     *********************************************************
     *********************************************************
         CREATE TABLE IF NOT EXISTS MyFlattenedDocuments_by_id (
	        Id uuid,
  	        DocumentType text,
  	        DocumentVersion text,
	        Document text,
           PRIMARY KEY (Id)
        );

        CREATE TABLE IF NOT EXISTS MyFlattenedDocuments_by_type (
	        Id uuid,
  	        DocumentType text,
  	        DocumentVersion text,
	        Document text,
           PRIMARY KEY (DocumentType,DocumentVersion)
        );
     *********************************************************
     *********************************************************
     *
     * https://en.wikipedia.org/wiki/Cardinality_(SQL_statements)
     * NOTE: I choose to NOT do indexes on DocumentType and DocumentVersion on the
     *       *_by_id tabel due to the High-cardinality potential of the DocumentType.
     *       I expect Low-cardinality for the DocumentVersion.
     *
     *      Because of this, I have 2 tables to make the queries on the *_by_type table more efficient.
     *********************************************************
     */

    public partial class FlattenedDocumentCassandraDao
    {
        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static CassandraStoreOptions _CassandraStoreOptions;

        private static bool _flattenedMappingInit = false;

        public static CassandraStoreOptions CassandraStoreOptions
        {
            get
            {

                var options = _CassandraStoreOptions ?? (_CassandraStoreOptions = new CassandraStoreOptions
                {
                    CassandraConfig = new CassandraConfig
                    {
                        ContactPoints = new List<string> {"cassandra"},
                        Credentials = new CassandraCredentials() {Password = "", UserName = ""},
                        KeySpace = "notforproduction"
                    },
                    TableName = "NotForProduction"
                });

                if (_flattenedMappingInit == false)
                {
                    MyMappings.Init(options.TableName);
                    _flattenedMappingInit = true;
                }
                return options;
            }
            set { _CassandraStoreOptions = value; }
        }

        private static ISession _cassandraSession = null;

        internal static string TableByIdName(string seedTableName)
        {
            return seedTableName + "_by_id";
        }

        internal static string TableByTypeAndVersionName(string seedTableName)
        {
            return seedTableName + "_by_type_and_version";
        }

        public static ISession CassandraSession
        {
            get
            {
                try
                {
                    if (_cassandraSession == null)
                    {
                        var dao = new CassandraDao(CassandraStoreOptions.CassandraConfig);
                        _cassandraSession = dao.GetSession();

                        //-----------------------------------------------
                        // PREPARED STATEMENTS for FlattenedDocument
                        //-----------------------------------------------
                        PrepareFlattenedDocumentStatements(CassandraStoreOptions.TableName);
                        PrepareFlattenedDocumentUtilityStatements(CassandraStoreOptions.TableName);

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
