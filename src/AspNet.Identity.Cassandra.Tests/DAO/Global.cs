using System;
using P5.AspNet.Identity.Cassandra.DAO;

namespace AspNet.Identity.Cassandra.Tests
{
    public class Global
    {
        private static Guid _tenantId = Guid.Parse("43dc5fa3-e7b2-4ee1-bb30-943636367967");

        static Guid TenantId
        {
            get { return _tenantId; }
        }

        public static AspNetIdentityDao TenantDao
        {
            get
            {
                return
                    new AspNetIdentityDao(TenantId);

            }
        }
        public static AspNetIdentityDao GlobalTenantDao
        {
            get
            {
                return
                    new AspNetIdentityDao(Guid.Empty);

            }
        }
    }
}