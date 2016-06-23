using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using P5.CassandraStore;
using P5.CassandraStore.Extensions;
using P5.Store.Core;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public static class UserDaoExtensions
    {

    }

    public partial class AspNetIdentityDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for Role
        //-----------------------------------------------

        #region PREPARED STATEMENTS for Role
        private AsyncLazy<PreparedStatement> _createUserByUserName;
        private AsyncLazy<PreparedStatement> _createUserByEmail;
        private AsyncLazy<PreparedStatement> _createUserById;
        private AsyncLazy<PreparedStatement> _deleteUserByUserName;
        private AsyncLazy<PreparedStatement> _deleteUserByEmail;
        private AsyncLazy<PreparedStatement> _deleteUserById;
        #endregion

        public void PrepareUserStatements()
        {
            string columns = "tenantid, " +
                             "username, " +
                             "userid, " +
                             "password_hash, " +
                             "security_stamp, " +
                             "two_factor_enabled, " +
                             "access_failed_count," +
                             "lockout_enabled, " +
                             "lockout_end_date, " +
                             "phone_number, " +
                             "phone_number_confirmed, " +
                             "email, " +
                             "email_confirmed, " +
                             "created, " +
                             "modified, " +
                             "enabled, " +
                             "source, " +
                             "source_id";
            // Create some reusable prepared statements so we pay the cost of preparing once, then bind multiple times
            _createUserByUserName = new AsyncLazy<PreparedStatement>(() => CassandraSession.PrepareAsync(
                "INSERT INTO users_by_username (" + columns + ") " +
                string.Format("VALUES ({0}, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", TenantId)));
            _createUserByEmail = new AsyncLazy<PreparedStatement>(() => CassandraSession.PrepareAsync(
                "INSERT INTO users_by_email (" + columns + ") " +
                string.Format("VALUES ({0}, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", TenantId)));
            _createUserById = new AsyncLazy<PreparedStatement>(() => CassandraSession.PrepareAsync(
                "INSERT INTO users (" + columns + ") " +
                string.Format("VALUES ({0}, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", TenantId)));
            _deleteUserByUserName =
                    new AsyncLazy<PreparedStatement>(
                        () =>
                            CassandraSession.PrepareAsync(
                                string.Format(
                                    "DELETE FROM users_by_username WHERE tenantid = {0} AND username = ?", TenantId)));
            _deleteUserByEmail =
                new AsyncLazy<PreparedStatement>(
                    () =>
                        CassandraSession.PrepareAsync(
                            string.Format("DELETE FROM users_by_email WHERE tenantid = {0} AND email = ?",
                                TenantId)));
            _deleteUserById=
                   new AsyncLazy<PreparedStatement>(
                       () =>
                           CassandraSession.PrepareAsync(
                               string.Format("DELETE FROM users WHERE userid = ?",
                                   TenantId)));

        }



        public async Task ChangeUserNameAsync(string oldUserName, string newUserName,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            if (oldUserName == null)
                throw new ArgumentNullException("oldUserName");
            if (newUserName == null)
                throw new ArgumentNullException("newUserName");

            var foundUserResult = await FindUserByUserNameAsync(oldUserName, cancellationToken);
            var foundUserList = foundUserResult.ToList();
            if (foundUserList.Any())
            {
                var user = foundUserList[0];
                var foundNewResult = await FindUserByUserNameAsync(newUserName, cancellationToken);
                if (foundNewResult.Any())
                {
                    throw new Exception(String.Format("User: {0} exists, so you can't rename to this.  Pick another name",newUserName));
                }
                var batch = new BatchStatement();

                PreparedStatement prepared = await _deleteUserByEmail;
                BoundStatement bound = prepared.Bind(oldUserName);
                batch.Add(bound);

                prepared = await _deleteUserByUserName;
                bound = prepared.Bind(oldUserName);
                batch.Add(bound);

                user.UserName = newUserName;
                user.Email = newUserName;
                prepared = await _createUserByUserName;
                bound = prepared.Bind(user.UserName, user.Id, user.PasswordHash, user.SecurityStamp,
                    user.TwoFactorEnabled, user.AccessFailedCount,
                    user.LockoutEnabled, user.LockoutEndDate, user.PhoneNumber, user.PhoneNumberConfirmed,
                    user.Email,
                    user.EmailConfirmed, user.Created, user.Modified, user.Enabled, user.Source, user.SourceId);
                batch.Add(bound);

                prepared = await _createUserByEmail;
                bound = prepared.Bind(user.UserName, user.Id, user.PasswordHash, user.SecurityStamp,
                    user.TwoFactorEnabled, user.AccessFailedCount,
                    user.LockoutEnabled, user.LockoutEndDate, user.PhoneNumber, user.PhoneNumberConfirmed,
                    user.Email,
                    user.EmailConfirmed, user.Created, user.Modified, user.Enabled, user.Source, user.SourceId);
                batch.Add(bound);

                prepared = await _createUserById;
                bound = prepared.Bind(user.UserName, user.Id, user.PasswordHash, user.SecurityStamp,
                   user.TwoFactorEnabled, user.AccessFailedCount,
                   user.LockoutEnabled, user.LockoutEndDate, user.PhoneNumber, user.PhoneNumberConfirmed,
                   user.Email,
                   user.EmailConfirmed, user.Created, user.Modified, user.Enabled, user.Source, user.SourceId);
                batch.Add(bound);
                cancellationToken.ThrowIfCancellationRequested();
                await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
            }
            else
            {
                throw new Exception(string.Format("user:{0} does not exist",oldUserName));
            }
        }
        public async Task UpsertUserAsync(CassandraUser user,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.UserName))
                throw new ArgumentNullException("user", "user.UserName cannot be null or empty");

            var now = DateTimeOffset.UtcNow;

            var foundUserResult = await FindUserByEmailAsync(user.Email, cancellationToken);
            var foundUserList = foundUserResult.ToList();
            if (foundUserList.Any())
            {
                // we have an update, and not a create.
                // we don't let you update the username/email.  That is done by ChangeUserNameAsync
                var foundUser = foundUserList[0];
                user.Email = foundUser.Email;
                user.UserName = foundUser.UserName;
            }
            else
            {
                // We have a brand new user,
                // we want to make the Id and user record immutable.
                // This allows us to change out the email address, which is the only thing we will allow for what looks like a username.
                user.Id = Guid.NewGuid();
                user.Created = now;
            }
           
            user.Modified = now;

            var batch = new BatchStatement();

            var prepared = await _createUserByUserName;
            var bound = prepared.Bind(user.UserName, user.Id, user.PasswordHash, user.SecurityStamp,
                user.TwoFactorEnabled, user.AccessFailedCount,
                user.LockoutEnabled, user.LockoutEndDate, user.PhoneNumber, user.PhoneNumberConfirmed,
                user.Email,
                user.EmailConfirmed, user.Created, user.Modified, user.Enabled, user.Source, user.SourceId);
            batch.Add(bound);

            prepared = await _createUserByEmail;
            bound = prepared.Bind(user.UserName, user.Id, user.PasswordHash, user.SecurityStamp,
                user.TwoFactorEnabled, user.AccessFailedCount,
                user.LockoutEnabled, user.LockoutEndDate, user.PhoneNumber, user.PhoneNumberConfirmed,
                user.Email,
                user.EmailConfirmed, user.Created, user.Modified, user.Enabled, user.Source, user.SourceId);
            batch.Add(bound);

            prepared = await _createUserById;
            bound = prepared.Bind(user.UserName, user.Id, user.PasswordHash, user.SecurityStamp,
               user.TwoFactorEnabled, user.AccessFailedCount,
               user.LockoutEnabled, user.LockoutEndDate, user.PhoneNumber, user.PhoneNumberConfirmed,
               user.Email,
               user.EmailConfirmed, user.Created, user.Modified, user.Enabled, user.Source, user.SourceId);
            batch.Add(bound);


            cancellationToken.ThrowIfCancellationRequested();

            await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
        }

        public async Task DeleteUserAsync(CassandraUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.UserName))
                throw new ArgumentNullException("user", "user.UserName cannot be null or empty");

            var foundUserResult = await FindUserByUserNameAsync(user.UserName, cancellationToken);
            var foundUserlist = foundUserResult.ToList();
            if (!foundUserlist.Any())
                return;
            user = foundUserlist[0];
            

            var batch = new BatchStatement();

            PreparedStatement prepared = await _deleteUserById;
            BoundStatement bound = prepared.Bind(user.Id);
            batch.Add(bound);

            prepared = await _deleteUserByEmail;
            bound = prepared.Bind(user.Email);
            batch.Add(bound);

            prepared = await _deleteUserByUserName;
            bound = prepared.Bind(user.UserName);
            batch.Add(bound);


            await RemoveLoginsFromUserAsync(user.Id, cancellationToken);
            await DeleteUserFromRolesAsync(user.Id, cancellationToken);
            await DeleteClaimHandleByUserIdAsync(user.Id, cancellationToken);
            await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
        }

        public async Task<IEnumerable<CassandraUser>> FindUserByIdAsync(Guid userId,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("userId", "userId cannot be Guid.Empty");

            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var cqlQuery = string.Format("SELECT * FROM users WHERE userid = ?",
                Guid.Empty);
            var records = await mapper.FetchAsync<CassandraUser>(cqlQuery, userId);
            return records;
        }

        public async Task<IEnumerable<CassandraUser>> FindUserByEmailAsync(string email,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("email", "email cannot be null or empty");


            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var records =
                await
                    mapper.FetchAsync<CassandraUser>("SELECT * FROM users_by_email WHERE tenantid = ? AND email = ?", TenantId, email);
            return records;
        }
        public async Task<IEnumerable<CassandraUser>> FindUserByUserNameAsync(string username,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username", "username cannot be null or empty");

            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var records =
                await
                    mapper.FetchAsync<CassandraUser>("SELECT * FROM users_by_username WHERE tenantid = ? AND username = ?", TenantId, username);
            return records;
        }

        public async Task<Store.Core.Models.IPage<CassandraUser>> PageUsersAsync(int pageSize, byte[] pagingState,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await PageUsersByTenantIdAsync(TenantId, pageSize, pagingState, cancellationToken);
        }

        public async Task<Store.Core.Models.IPage<CassandraUser>> PageUsersByTenantIdAsync(Guid tenantId, int pageSize, byte[] pagingState,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            IPage<CassandraUser> page;
            string cqlQuery = string.Format("SELECT * FROM users_by_username WHERE tenantid ={0}", tenantId);
            if (pagingState == null)
            {
                page = await mapper.FetchPageAsync<CassandraUser>(
                    Cql.New(cqlQuery).WithOptions(opt =>
                        opt.SetPageSize(pageSize)));
            }
            else
            {
                page = await mapper.FetchPageAsync<CassandraUser>(
                    Cql.New(cqlQuery).WithOptions(opt =>
                        opt.SetPageSize(pageSize).SetPagingState(pagingState)));
            }

            // var result = CreatePageProxy(page);
            var result = new PageProxy<CassandraUser>(page);

            return result;
        }
        public async Task<Store.Core.Models.IPage<CassandraUser>> PageUsersOfAllTenantsAsync(int pageSize, byte[] pagingState,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            IPage<CassandraUser> page;
            string cqlQuery = string.Format("SELECT * FROM users");
            if (pagingState == null)
            {
                page = await mapper.FetchPageAsync<CassandraUser>(
                    Cql.New(cqlQuery).WithOptions(opt =>
                        opt.SetPageSize(pageSize)));
            }
            else
            {
                page = await mapper.FetchPageAsync<CassandraUser>(
                    Cql.New(cqlQuery).WithOptions(opt =>
                        opt.SetPageSize(pageSize).SetPagingState(pagingState)));
            }

            // var result = CreatePageProxy(page);
            var result = new PageProxy<CassandraUser>(page);

            return result;
        }
    }
}
