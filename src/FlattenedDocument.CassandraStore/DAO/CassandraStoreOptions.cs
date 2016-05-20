using P5.CassandraStore.Settings;

namespace FlattenedDocument.CassandraStore.DAO
{
    public class CassandraStoreOptions
    {
        public CassandraConfig CassandraConfig { get; set; }
        public string TableName { get; set; }
    }
}