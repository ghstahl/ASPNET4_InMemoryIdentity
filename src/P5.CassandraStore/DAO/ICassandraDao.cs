using System.Threading.Tasks;
using Cassandra;

namespace P5.CassandraStore.DAO
{
    public interface ICassandraDAO
    {
        Task<ISession> GetSessionAsync();
    }
}
