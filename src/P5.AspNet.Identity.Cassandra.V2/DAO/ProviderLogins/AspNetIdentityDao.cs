using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using P5.CassandraStore;
using P5.CassandraStore.Extensions;

namespace P5.AspNet.Identity.Cassandra.DAO
{

    /*
     * // Logins, keyed by user id
CREATE TABLE IF NOT EXISTS logins (
    tenantid uuid,
	userid uuid,
	login_provider text,
    provider_key text,
    PRIMARY KEY(userid, login_provider, provider_key)
);

// Logins, keyed by provider and provider key
CREATE TABLE IF NOT EXISTS logins_by_provider (
    tenantid uuid,
	userid uuid,
	login_provider text,
    provider_key text,
    PRIMARY KEY ((login_provider, provider_key), tenantid)
);
*/

    public partial class AspNetIdentityDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for ProviderLogins
        //-----------------------------------------------

        #region PREPARED STATEMENTS for ProviderLogins

        private AsyncLazy<PreparedStatement> _createLoginByUserId;
        private AsyncLazy<PreparedStatement> _createLoginByLoginProvider;

        private AsyncLazy<PreparedStatement> _deleteLoginByUserId;
        private AsyncLazy<PreparedStatement> _deleteLoginByLoginProvider;

        #endregion

        public void PrepareProviderLoginsStatements()
        {
            string columns =
                "tenantid, " +
                "userid, " +
                "login_provider, " +
                "provider_key";

            // Create some reusable prepared statements so we pay the cost of preparing once, then bind multiple times
            _createLoginByUserId = new AsyncLazy<PreparedStatement>(() => CassandraSession.PrepareAsync(
                "INSERT INTO logins (" + columns + ") " +
                string.Format("VALUES ({0}, ?, ?, ?)", TenantId)));
            _createLoginByLoginProvider = new AsyncLazy<PreparedStatement>(() => CassandraSession.PrepareAsync(
                "INSERT INTO logins_by_provider (" + columns + ") " +
                string.Format("VALUES ({0}, ?, ?, ?)", TenantId)));

            _deleteLoginByUserId =
                new AsyncLazy<PreparedStatement>(
                    () =>
                        CassandraSession.PrepareAsync(
                            "DELETE FROM logins WHERE userId = ? and login_provider = ? and provider_key = ?"));
            _deleteLoginByLoginProvider =
                new AsyncLazy<PreparedStatement>(
                    () =>
                        CassandraSession.PrepareAsync(
                            string.Format(
                                "DELETE FROM logins_by_provider WHERE login_provider = ? AND provider_key = ? AND tenantid = {0}",
                                TenantId)));
        }

        public async Task UpsertLoginsAsync(ProviderLoginHandle login,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (login == null)
                throw new ArgumentNullException("login");

            cancellationToken.ThrowIfCancellationRequested();

            var batch = new BatchStatement();

            var prepared = await _createLoginByUserId;
            var bound = prepared.Bind(login.UserId, login.LoginProvider, login.ProviderKey);
            batch.Add(bound);
            cancellationToken.ThrowIfCancellationRequested();

            prepared = await _createLoginByLoginProvider;
            bound = prepared.Bind(login.UserId, login.LoginProvider, login.ProviderKey);
            batch.Add(bound);
            cancellationToken.ThrowIfCancellationRequested();

            await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
        }

        public async Task DeleteLoginsAsync(ProviderLoginHandle login,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (login == null)
                throw new ArgumentNullException("login");

            cancellationToken.ThrowIfCancellationRequested();

            var batch = new BatchStatement();

            PreparedStatement prepared = await _deleteLoginByUserId;
            BoundStatement bound = prepared.Bind(login.UserId, login.LoginProvider, login.ProviderKey);
            batch.Add(bound);

            prepared = await _deleteLoginByLoginProvider;
            bound = prepared.Bind(login.LoginProvider, login.ProviderKey);
            batch.Add(bound);

            await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ProviderLoginHandle>> FindLoginsByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("userId", "userId cannot be Guid.Empty");

            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var cqlQuery = string.Format("SELECT * FROM logins WHERE userid = ?");

            var records = await mapper.FetchAsync<ProviderLoginHandle>(cqlQuery, userId);
            return records;
        }
         public async Task<IEnumerable<ProviderLoginHandle>> FindLoginByProviderAsync(string loginProvider,string providerKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(loginProvider))
                throw new ArgumentNullException("loginProvider", "loginProvider cannot be null or empty");
             if (string.IsNullOrEmpty(providerKey))
                 throw new ArgumentNullException("providerKey", "providerKey cannot be null or empty");

            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var cqlQuery = string.Format("SELECT * FROM logins_by_provider WHERE login_provider = ? AND provider_key = ? AND tenantid = {0}",TenantId);

            var records = await mapper.FetchAsync<ProviderLoginHandle>(cqlQuery, loginProvider,providerKey);
            return records;
        }

        public async Task RemoveLoginsFromUserAsync(Guid userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await FindLoginsByUserIdAsync(userId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var item in result)
            {
                await DeleteLoginsAsync(item, cancellationToken);
            }
        }

    }
}
