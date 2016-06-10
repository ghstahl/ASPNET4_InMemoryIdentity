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

        private ISession _cassandraSession = null;
        public IdentityServer3CassandraDao() { }

        public async Task EstablishConnectionAsync()
        {
            try
            {
                if (CassandraSession == null)
                {
                    var dao = new CassandraDao(CassandraConfig);
                    CassandraSession = await dao.GetSessionAsync();

                    //-----------------------------------------------
                    // PREPARED STATEMENTS for User
                    //-----------------------------------------------
                    PrepareClientIdByUserStatements();

                    //-----------------------------------------------
                    // PREPARED STATEMENTS for User
                    //-----------------------------------------------
                    PrepareAllowedScopesByUserStatements();

                    //-----------------------------------------------
                    // PREPARED STATEMENTS for User
                    //-----------------------------------------------
                    PrepareUserStatements();
                    
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
                CassandraSession = null;
            }
           
        }
       
        public  ISession CassandraSession
        {
            get { return _cassandraSession; }
            private set { _cassandraSession = value; }
        }
    }
}
