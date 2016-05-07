using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;

namespace P5.CassandraStore.Extensions
{
    public static class CassandraExtensions
    {
        public static void AddRange(this BatchStatement batchStatement, IEnumerable<BoundStatement> statements)
        {
            foreach (var statement in statements)
            {
                batchStatement.Add(statement);
            }
        }
    }
}
