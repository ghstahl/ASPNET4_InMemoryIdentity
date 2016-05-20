#if _USE_CASTLE
namespace FlattenedDocument.CassandraStore.DAO
{

    public class CassandraPageProxy<T>
    {
        public Cassandra.Mapping.IPage<T> Page { get; set; }

        public CassandraPageProxy(Cassandra.Mapping.IPage<T> page)
        {
            Page = page;
        }

    }
}
#endif
