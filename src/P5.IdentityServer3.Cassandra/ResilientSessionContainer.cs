using System;
using System.Threading.Tasks;
using Cassandra;
using P5.CassandraStore.DAO;
using P5.IdentityServer3.Cassandra.DAO;

namespace P5.IdentityServer3.Cassandra
{
    public class ResilientSessionContainer
    {
        public IdentityServer3CassandraDao ResilientSession { get; private set; }

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
                var rs = new IdentityServer3CassandraDao();
                await rs.EstablishConnectionAsync();
                ResilientSession = rs;
            }
        }
    }
}