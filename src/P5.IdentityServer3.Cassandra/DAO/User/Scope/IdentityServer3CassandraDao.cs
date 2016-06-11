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

        public async Task<IdentityServerStoreAppliedInfo> DeleteAllowedScopeFromUserAsync(IdentityServerUserAllowedScope userAllowedScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var userExists = await FindDoesUserExistByUserIdAsync(userAllowedScope.UserId, cancellationToken);
            if (!userExists)
            {
                // not allowed
                return new IdentityServerStoreAppliedInfo
                {
                    Applied = false,
                    Exception = new UserDoesNotExitException()
                };
            }
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var record = await
                mapper.DeleteIfAsync<IdentityServerUserAllowedScope>(
                    "WHERE scopename = ? AND userid = ?", userAllowedScope.ScopeName, userAllowedScope.UserId);
            return new IdentityServerStoreAppliedInfo(record.Applied);
        }

        public async Task<IdentityServerStoreAppliedInfo> UpsertAllowedScopeIntoUsersAsync(IdentityServerUserAllowedScope userAllowedScope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var userExists = await FindDoesUserExistByUserIdAsync(userAllowedScope.UserId, cancellationToken);
            if (!userExists)
            {
                // not allowed
                return new IdentityServerStoreAppliedInfo
                {
                    Applied = false,
                    Exception = new UserDoesNotExitException()
                };
            }
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var record = await
                mapper.InsertIfNotExistsAsync(userAllowedScope);
            return new IdentityServerStoreAppliedInfo(record.Applied);
        }
    }
}
