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

        public void PrepareClientIdByUserStatements()
        {}

        public async Task<IEnumerable<string>> FindClientIdsByUserIdAsync(string userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var record = await mapper.FetchAsync<string>("Select clientid FROM user_clientid WHERE userid = ?", userId);
            return record;
        }

        public async Task<IdentityServerStoreAppliedInfo> DeleteClientIdFromUserAsync(IdentityServerUserClientId userClientId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = await FindIdentityServerUserByUserIdAsync(userClientId.UserId, cancellationToken);
            if (user == null)
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
                mapper.DeleteIfAsync<IdentityServerUserClientId>(
                    "WHERE clientid = ? AND userid = ?", userClientId.ClientId, userClientId.UserId);
            return new IdentityServerStoreAppliedInfo(record.Applied);
        }

        public async Task<IdentityServerStoreAppliedInfo> UpsertClientIdIntoUsersAsync(IdentityServerUserClientId userClientId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = await FindIdentityServerUserByUserIdAsync(userClientId.UserId, cancellationToken);
            if (user == null)
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
                mapper.InsertIfNotExistsAsync(user);
            return new IdentityServerStoreAppliedInfo(record.Applied); 
        }
    }
}
