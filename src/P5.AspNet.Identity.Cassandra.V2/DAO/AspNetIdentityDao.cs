using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using log4net;
using P5.CassandraStore.DAO;
using P5.CassandraStore.Settings;

namespace P5.AspNet.Identity.Cassandra.DAO
{


    public partial class AspNetIdentityDao
    {
        private static Guid _namespace = Guid.Empty;

        static Guid @Namespace
        {
            get
            {
                if (_namespace == Guid.Empty)
                {
                    _namespace = Guid.Parse("2b1cb768-6ff0-4be2-8635-f1f1a9a0610a");
                }
                return _namespace;

            }
        }

        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CassandraConfig _CassandraConfig;

        public CassandraConfig CassandraConfig
        {
            get
            {
                MyMappings.Init();
                return _CassandraConfig ?? (_CassandraConfig = new CassandraConfig
                {
                    ContactPoints = new List<string> {"cassandra"},
                    Credentials = new CassandraCredentials() {Password = "", UserName = ""},
                    KeySpace = "aspnetidentity"
                });
            }
            set
            {
                _CassandraConfig = value;
            }
        }
        public Guid TenantId { get; set; }
        public static Guid GobalTenantId { get { return Guid.Empty; } }
        private ISession _cassandraSession = null;
        public AspNetIdentityDao(CassandraConfig config, Guid tenantId)
        {
            TenantId = tenantId;
            CassandraConfig = config;
        }

        public async Task EstablishConnectionAsync()
        {
            try
            {
                if (CassandraSession == null)
                {
                    var dao = new CassandraDao(CassandraConfig);
                    CassandraSession = await dao.GetSessionAsync();

                    //-----------------------------------------------
                    // PREPARED STATEMENTS for Claims
                    //-----------------------------------------------
                    PrepareClaimsStatements();

                    //-----------------------------------------------
                    // PREPARED STATEMENTS for Roles
                    //-----------------------------------------------
                    PrepareRolesStatements();
                    
                    //-----------------------------------------------
                    // PREPARED STATEMENTS for UserRoles
                    //-----------------------------------------------
                    PrepareUserRolesStatements();
                    
                    //-----------------------------------------------
                    // PREPARED STATEMENTS for ProviderLogins
                    //-----------------------------------------------
                    PrepareProviderLoginsStatements();

                    //-----------------------------------------------
                    // PREPARED STATEMENTS for Users
                    //-----------------------------------------------
                    PrepareUserStatements();

                    MyMappings.Init();
                }
            }
            catch (Exception e)
            {
                CassandraSession = null;
            }

        }

        public ISession CassandraSession
        {
            get { return _cassandraSession; }
            private set { _cassandraSession = value; }
        }

        public void Dispose()
        {
            if(_cassandraSession != null)
                _cassandraSession.Dispose();
        }
    }
}
