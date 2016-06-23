using System;
using P5.CassandraStore.Settings;

namespace P5.AspNet.Identity.Cassandra
{
    public class CassandraAspNetIdentityOptions : IDisposable
    {
        public static CassandraConfig CassandraConfig { get; set; }
        public void Dispose()
        {

        }
    }
}