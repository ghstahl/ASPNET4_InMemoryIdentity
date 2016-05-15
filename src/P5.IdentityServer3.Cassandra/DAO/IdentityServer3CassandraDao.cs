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
                        var dao = new CassandraDao(CassandraConfig);
                        _cassandraSession = dao.GetSession();


                        //-----------------------------------------------
                        // PREPARED STATEMENTS for Utility
                        //-----------------------------------------------
                        PrepareUtilityStatements();

                        //-----------------------------------------------
                        // PREPARED STATEMENTS for Scope
                        //-----------------------------------------------
                        PrepareScopeStatements();

                        //-----------------------------------------------
                        // PREPARED STATEMENTS for Client
                        //-----------------------------------------------
                        PrepareClientStatements();

                        //-----------------------------------------------
                        // PREPARED STATEMENTS for Token
                        //-----------------------------------------------
                        PrepareTokenHandleStatements();

                        //-----------------------------------------------
                        // PREPARED STATEMENTS for RefreshToken
                        //-----------------------------------------------
                        PrepareRefreshTokenStatements();

                        //-----------------------------------------------
                        // PREPARED STATEMENTS for Consent
                        //-----------------------------------------------
                        PrepareConsentStatements();

                        //-----------------------------------------------
                        // PREPARED STATEMENTS for AuthorizationCode
                        //-----------------------------------------------
                        PrepareAuthorizationCodeStatements();


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
