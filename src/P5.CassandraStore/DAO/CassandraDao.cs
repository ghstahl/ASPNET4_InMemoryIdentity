using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using P5.CassandraStore.Settings;

namespace P5.CassandraStore.DAO
{
   public class CassandraDao : ICassandraDAO
    {
        private  Cluster _cluster;

       private Cluster Cluster
       {
           get { return _cluster ?? (_cluster = Connect()); }
       }

        private ISession _session;

        private ISession Session
        {
            get
            {
                if (_session == null)
                {
                    try
                    {
                        _session = Cluster.Connect(CassandraConfig.KeySpace);
                    }
                    catch (Exception e)
                    {
                        _session = null;
                        throw;
                    }
                }
                return _session;

            }
        }

        private CassandraConfig CassandraConfig { get; set; }

        public CassandraDao(CassandraConfig cassandraConfig )
        {
            CassandraConfig = cassandraConfig;
        }

        private Cluster Connect()
        {
            try
            {
                QueryOptions queryOptions = new QueryOptions()
                    .SetConsistencyLevel(ConsistencyLevel.One);
                var builder = Cassandra.Cluster.Builder();
                builder.AddContactPoints(CassandraConfig.ContactPoints);

                if (!string.IsNullOrEmpty(CassandraConfig.Credentials.UserName) &&
                    !string.IsNullOrEmpty(CassandraConfig.Credentials.Password))
                {
                    builder.WithCredentials(
                        CassandraConfig.Credentials.UserName,
                        CassandraConfig.Credentials.Password);
                }
                builder.WithQueryOptions(queryOptions);
                Cluster cluster = builder.Build();

                return cluster;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public ISession GetSession()
        {
            return Session;
        }

        public static async Task<bool> TruncateTablesAsync(ISession session,IEnumerable<string> tables,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();

                string template = "truncate {0}";
                foreach (string cql in tables.Select(table => string.Format(template, table)))
                {
                    await mapper.ExecuteAsync(cql);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}