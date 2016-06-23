using System;
using System.Threading.Tasks;
using Cassandra;
using P5.AspNet.Identity.Cassandra.DAO;
using P5.CassandraStore.DAO;

namespace P5.AspNet.Identity.Cassandra
{
    public class ResilientSessionContainer
    {
        private static Guid _tenantId = Guid.Parse("43dc5fa3-e7b2-4ee1-bb30-943636367967");

        static Guid TenantId
        {
            get { return _tenantId; }
        }
        public void Dispose()
        {
            if (ResilientSession != null)
            {
                ResilientSession.Dispose();
            }
        }
        public static AspNetIdentityDao TenantDao
        {
            get
            {
                return
                    new AspNetIdentityDao(TenantId);

            }
        }
        public AspNetIdentityDao ResilientSession { get; private set; }

        public TryWithAwaitInCatchExcpetionHandleResult<T> HandleCassandraException<T>(Exception ex)
        {
            var nhae = ex as NoHostAvailableException;
            if (nhae != null)
            {
                ResilientSession = null;
            }
            return new TryWithAwaitInCatchExcpetionHandleResult<T>
            {
                RethrowException = true
            };
        }
        public async Task EstablishSessionAsync()
        {
            if (ResilientSession == null)
            {
                var rs = TenantDao;
                await rs.EstablishConnectionAsync();
                ResilientSession = rs;
            }
        }
    }
}