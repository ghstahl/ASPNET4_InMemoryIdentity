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
    public static class RoleDaoExtensions
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
        private AsyncLazy<PreparedStatement> _getRolesAsync;
        private AsyncLazy<PreparedStatement> _isInRoleAsync;
        #endregion

        public void PrepareRolesStatements()
        {
            _addToRoleAsync = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
                {
                    CassandraSession.PrepareAsync(
                        string.Format("INSERT INTO user_roles (userid, rolename, assigned) VALUES (?, ?, ?)")),
                    CassandraSession.PrepareAsync(
                        string.Format("INSERT INTO user_roles_by_role (rolename, userid, assigned) VALUES (?, ?, ?)"))
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
                    CassandraSession.PrepareAsync(string.Format("DELETE FROM user_roles WHERE userId = ?"  )),
                    CassandraSession.PrepareAsync(
                        string.Format("DELETE FROM user_roles_by_role WHERE userId = ? AND rolename = ?"))
                }));
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

            PreparedStatement[] prepared = await _addToRoleAsync;
            var batch = new BatchStatement();

            // INSERT INTO user_roles ...
            batch.Add(prepared[0].Bind(userId, roleName, createdDate));


            // INSERT INTO user_roles_by_role ...
            batch.Add(prepared[1].Bind(roleName, userId, createdDate));

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

        public async Task RemoveFromRoleAsync(Guid userId, string roleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("userId");
            if (roleName == null)
                throw new ArgumentNullException("roleName");

            cancellationToken.ThrowIfCancellationRequested();
            PreparedStatement[] prepared = await _removeFromRoleAsync;
            var batch = new BatchStatement();

            // DELETE FROM user_roles ...
            batch.Add(prepared[0].Bind(userId, roleName));

            // DELETE FROM user_roles_by_role ...
            batch.Add(prepared[1].Bind(userId, roleName));

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
                    mapper.FetchAsync<string>("SELECT rolename FROM user_roles WHERE userId = ? and rolename = ?", userId,rolename);
             return records;
        }
        public async Task<bool> IsUserInRoleAsync(Guid userId, string rolename,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await FindRoleAsync(userId, rolename, cancellationToken);
            return result.Any();
        }
        public async Task<Store.Core.Models.IPage<RoleHandle>> PageRolesAsync(
            Guid userId, int pageSize,byte[] pagingState,CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            IPage<RoleHandle> page;
            string cqlQuery = string.Format("Where userid={0}", userId.ToString());
            if (pagingState == null)
            {
                page = await mapper.FetchPageAsync<RoleHandle>(
                    Cql.New(cqlQuery).WithOptions(opt =>
                        opt.SetPageSize(pageSize)));
            }
            else
            {
                page = await mapper.FetchPageAsync<RoleHandle>(
                    Cql.New(cqlQuery).WithOptions(opt =>
                        opt.SetPageSize(pageSize).SetPagingState(pagingState)));
            }

            // var result = CreatePageProxy(page);
            var result = new PageProxy<RoleHandle>(page);

            return result;

        }

    }
}
