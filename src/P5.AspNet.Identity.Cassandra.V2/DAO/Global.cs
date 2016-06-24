using System;

namespace P5.AspNet.Identity.Cassandra.DAO
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
                    new AspNetIdentityDao(null,TenantId);

            }
        }
        public static AspNetIdentityDao GlobalTenantDao
        {
            get
            {
                return
                    new AspNetIdentityDao(null,Guid.Empty);

            }
        }
    }
}