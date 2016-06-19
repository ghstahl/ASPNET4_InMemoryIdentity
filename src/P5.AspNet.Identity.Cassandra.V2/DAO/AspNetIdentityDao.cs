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
                    KeySpace = "aspnetidentity"
                });
            }
            set { _CassandraConfig = value; }
        }

        private ISession _cassandraSession = null;
        public AspNetIdentityDao() { }
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
    }
}
