using Cassandra;

namespace P5.CassandraStore.DAO
{
    public interface ICassandraDAO
    {
        ISession GetSession();
    }
}
