using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer3.Core.Models;
using P5.CassandraStore;
using P5.CassandraStore.Extensions;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public partial class IdentityServer3CassandraDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for User
        //-----------------------------------------------

        public void PrepareAllowedScopesByUserStatements()
        {
           
        }

        public async Task<IEnumerable<string>> FindAllowedScopesByUserIdAsync(string userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var record = await mapper.FetchAsync<string>("Select scopename FROM user_scopename WHERE userid = ?", userId);
            return record;
        }

        public async Task DeleteAllowedScopeFromUserAsync(IdentityServerUserAllowedScope userScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var record = await
                mapper.DeleteIfAsync<IdentityServerUserAllowedScope>(
                    "WHERE scopename = ? AND userid = ?", userScope.ScopeName, userScope.UserId);
        }

        public async Task<AppliedInfo<IdentityServerUserAllowedScope>>  UpsertAllowedScopeIntoUsersAsync(IdentityServerUserAllowedScope user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var record = await
                mapper.InsertIfNotExistsAsync(user);
            return record;
        }
    }
}
