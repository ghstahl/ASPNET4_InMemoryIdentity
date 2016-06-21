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
    public static class UserRoleDaoExtensions
    {

    }

    public partial class AspNetIdentityDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for Role
        //-----------------------------------------------

        #region PREPARED STATEMENTS for Role

        private AsyncLazy<PreparedStatement[]> _addToRoleAsync;
        private AsyncLazy<PreparedStatement[]> _removeFromRoleAsync;
        private AsyncLazy<PreparedStatement[]> _deleteRolesFromUserIdAsync;


        #endregion

        public void PrepareRolesStatements()
        {
            _addToRoleAsync = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
            {
                CassandraSession.PrepareAsync(
                    string.Format("INSERT INTO user_roles (userid, rolename, assigned) VALUES (?, ?, ?)")),
                CassandraSession.PrepareAsync(
                    string.Format("INSERT INTO user_roles_by_role (userid, rolename, assigned) VALUES (?, ?, ?)"))
            }));

            _removeFromRoleAsync = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
            {
                CassandraSession.PrepareAsync(string.Format("DELETE FROM user_roles " +
                                                            "WHERE userId = ? and rolename = ?")),
                CassandraSession.PrepareAsync(string.Format("DELETE FROM user_roles_by_role " +
                                                            "WHERE userId = ? and rolename = ?"))
            }));

            _deleteRolesFromUserIdAsync = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
            {
                CassandraSession.PrepareAsync(string.Format("DELETE FROM user_roles WHERE userId = ?")),
                CassandraSession.PrepareAsync(
                    string.Format("DELETE FROM user_roles_by_role WHERE userId = ? AND rolename = ?"))
            }));
        }


        public async Task<IEnumerable<UserRoleHandle>> FindUserRolesByNameAsync(string roleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var records =
                await
                    mapper.FetchAsync<UserRoleHandle>("SELECT * FROM user_roles_by_role WHERE rolename = ?", roleName);
            return records;
        }

        public async Task RenameRoleNameInUsersAsync(string oldRoleName, string newRoleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            var createdDate = DateTimeOffset.UtcNow;
            cancellationToken.ThrowIfCancellationRequested();

            var records = (await FindUserRolesByNameAsync(oldRoleName, cancellationToken)).ToList();
            cancellationToken.ThrowIfCancellationRequested();
            
            foreach (var record in records)
            {
                var batch = new BatchStatement();
                var statements = await BuildAddToRole(record.UserId, newRoleName, createdDate);
                batch.AddRange(statements);
                statements = await BuildRemoveFromRole(record.UserId, oldRoleName);
                batch.AddRange(statements);
                await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false); 
               
            }
        }

        public async Task<IEnumerable<BoundStatement>> BuildAddToRole(Guid userId, string roleName,
            DateTimeOffset createdDate)
        {

            List<BoundStatement> result = new List<BoundStatement>();
            PreparedStatement[] preparedStatements = await _addToRoleAsync;

            foreach (var preparedStatement in preparedStatements)
            {
                result.Add(preparedStatement.Bind(userId, roleName, createdDate));
            }
            return result;
        }

        public async Task AddToRoleAsync(Guid userId, string roleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("userId");
            if (roleName == null)
                throw new ArgumentNullException("roleName");
            var createdDate = DateTimeOffset.UtcNow;

            cancellationToken.ThrowIfCancellationRequested();

            var statements = await BuildAddToRole(userId, roleName, createdDate);

            var batch = new BatchStatement();
            batch.AddRange(statements);

            await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
        }

        public async Task DeleteUserFromRolesAsync(Guid userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            //TODO: User the pager here to loop through in a more orderly fashion.
            // you can't send too many batch statements
            // Currently this method is crazy inneficient.
            if (userId == Guid.Empty)
                throw new ArgumentNullException("userId");

            cancellationToken.ThrowIfCancellationRequested();

            var roleNames = await FindRoleNamesByUserIdAsync(userId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            PreparedStatement[] prepared = await _deleteRolesFromUserIdAsync;
            var batch = new BatchStatement();
            foreach (var roleName in roleNames)
            {
                // TODO: Don't do this, especially for large data sets.
                await RemoveFromRoleAsync(userId, roleName);
                // DELETE FROM user_roles_by_role ...
                //batch.Add(prepared[1].Bind(userId,roleName));
            }
            // DELETE FROM user_roles ...
            batch.Add(prepared[0].Bind(userId));


            await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
        }
        public async Task<IEnumerable<BoundStatement>> BuildRemoveFromRole(Guid userId, string roleName)
        {

            List<BoundStatement> result = new List<BoundStatement>();
            PreparedStatement[] preparedStatements = await _removeFromRoleAsync;

            foreach (var preparedStatement in preparedStatements)
            {
                result.Add(preparedStatement.Bind(userId, roleName));
            }
            return result;
        }

        public async Task RemoveFromRoleAsync(Guid userId, string roleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("userId");
            if (roleName == null)
                throw new ArgumentNullException("roleName");

            cancellationToken.ThrowIfCancellationRequested();
            PreparedStatement[] preparedStatements = await _removeFromRoleAsync;
            var batch = new BatchStatement();
            var statements = await BuildRemoveFromRole(userId, roleName);
       
            batch.AddRange(statements);
            await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> FindRoleNamesByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var records =
                await
                    mapper.FetchAsync<string>("SELECT rolename FROM user_roles WHERE userid = ?", userId);
            return records;
        }

        public async Task<IEnumerable<string>> FindRoleAsync(Guid userId, string rolename,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var records =
                await
                    mapper.FetchAsync<string>("SELECT rolename FROM user_roles WHERE userId = ? and rolename = ?",
                        userId, rolename);
            return records;
        }

        public async Task<bool> IsUserInRoleAsync(Guid userId, string rolename,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await FindRoleAsync(userId, rolename, cancellationToken);
            return result.Any();
        }

        public async Task<Store.Core.Models.IPage<UserRoleHandle>> PageUserRolesAsync(
            Guid userId, int pageSize, byte[] pagingState,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            IPage<UserRoleHandle> page;
            string cqlQuery = string.Format("Where userid={0}", userId.ToString());
            if (pagingState == null)
            {
                page = await mapper.FetchPageAsync<UserRoleHandle>(
                    Cql.New(cqlQuery).WithOptions(opt =>
                        opt.SetPageSize(pageSize)));
            }
            else
            {
                page = await mapper.FetchPageAsync<UserRoleHandle>(
                    Cql.New(cqlQuery).WithOptions(opt =>
                        opt.SetPageSize(pageSize).SetPagingState(pagingState)));
            }

            // var result = CreatePageProxy(page);
            var result = new PageProxy<UserRoleHandle>(page);

            return result;

        }

    }
}
