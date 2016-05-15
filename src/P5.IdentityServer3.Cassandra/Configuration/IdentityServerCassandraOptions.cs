using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P5.CassandraStore.Settings;

namespace P5.IdentityServer3.Cassandra.Configuration
{
    public class IdentityServerCassandraOptions
    {
        private readonly CassandraConfig _cassandraConfig = new CassandraConfig();

        public CassandraConfig CassandraConfig
        {
            get { return _cassandraConfig; }
        }

        public string KeySpace
        {
            get { return _cassandraConfig.KeySpace; }
            set { _cassandraConfig.KeySpace = value; }
        }

        public List<string> ContactPoints
        {
            get { return _cassandraConfig.ContactPoints; }
            set { _cassandraConfig.ContactPoints = value; }
        }

        public CassandraCredentials Credentials
        {
            get { return _cassandraConfig.Credentials; }
            set { _cassandraConfig.Credentials = value; }
        }

    }
}
