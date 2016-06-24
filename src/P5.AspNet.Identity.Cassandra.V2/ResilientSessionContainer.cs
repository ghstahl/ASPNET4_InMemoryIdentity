using System;
using System.Threading.Tasks;
using Cassandra;
using P5.AspNet.Identity.Cassandra.DAO;
using P5.CassandraStore.DAO;
using P5.CassandraStore.Settings;

namespace P5.AspNet.Identity.Cassandra
{
    public class ResilientSessionContainer
    {
        private CassandraConfig CassandraConfig { get; set; }
        public ResilientSessionContainer(CassandraConfig config, Guid tentantId)
        {
            TenantId = tentantId;
            CassandraConfig = config;
        }
        private Guid TenantId { get; set; }
       
        public void Dispose()
        {
            if (ResilientSession != null)
            {
                ResilientSession.Dispose();
            }
        }
        public AspNetIdentityDao TenantDao
        {
            get
            {
                return
                    new AspNetIdentityDao(CassandraConfig, TenantId);

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