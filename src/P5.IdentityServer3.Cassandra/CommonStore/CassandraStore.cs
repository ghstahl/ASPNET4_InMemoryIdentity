using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra
{
    public abstract class CassandraStore<TWrappedRecord, T>
        where TWrappedRecord : WrappedRecord<T>, new()
        where T : class
    {

    }
}
