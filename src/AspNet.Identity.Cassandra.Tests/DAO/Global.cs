using System;
using System.Collections.Generic;
using P5.AspNet.Identity.Cassandra;
using P5.AspNet.Identity.Cassandra.DAO;
using P5.CassandraStore.Settings;

namespace AspNet.Identity.Cassandra.Tests
{
    public class Global
    {
        private static Guid _tenantId = Guid.Parse("43dc5fa3-e7b2-4ee1-bb30-943636367967");

        static Guid TenantId
        {
            get { return _tenantId; }
        }

        private static CassandraConfig _cassandraConfig;
        public static CassandraConfig CassandraConfig
        {
            get
            {
                return _cassandraConfig ?? (_cassandraConfig = new CassandraConfig
                {
                    ContactPoints = new List<string> { "cassandra" },
                    Credentials = new CassandraCredentials() { Password = "", UserName = "" },
                    KeySpace = "aspnetidentity"
                });
            }
        }

        public static CassandraRoleStore GlobalCassandraRoleStore
        {
            get { return new CassandraRoleStore(CassandraConfig, Guid.Empty); }
        }
        public static CassandraRoleStore TanantCassandraRoleStore
        {
            get { return new CassandraRoleStore(CassandraConfig,TenantId); }
        }
        public static CassandraUserStore GlobalCassandraUserStore
        {
            get { return new CassandraUserStore(CassandraConfig, Guid.Empty); }
        }
        public static CassandraUserStore TanantCassandraUserStore
        {
            get { return new CassandraUserStore(CassandraConfig,TenantId); }
        }


        public static AspNetIdentityDao TenantDao
        {
            get
            {
                return
                    new AspNetIdentityDao(CassandraConfig,TenantId);

            }
        }
        public static AspNetIdentityDao GlobalTenantDao
        {
            get
            {
                return
                    new AspNetIdentityDao(CassandraConfig,Guid.Empty);

            }
        }
    }
}