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

        private AsyncLazy<PreparedStatement[]> _createRole;
        private AsyncLazy<PreparedStatement> _createRoleByName;
        private AsyncLazy<PreparedStatement[]> _updateRole;
        private AsyncLazy<PreparedStatement[]> _deleteRole;
        private AsyncLazy<PreparedStatement> _deleteRoleByName;
        private AsyncLazy<PreparedStatement> _findById;
        private AsyncLazy<PreparedStatement> _findByName;
        private AsyncLazy<PreparedStatement> _find;

        #endregion

        public void PrepareUserRolesStatements()
        {

            _createRole = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
                {
                    CassandraSession.PrepareAsync(
                        "INSERT INTO roles (roleid, name, displayname, is_systemrole, is_global, tenantid, created, modified) " +
                        "VALUES (?, ?, ?, ?, ?, ?, ?, ?)"),
                    _createRoleByName.Value
                }));

            _createRoleByName = new AsyncLazy<PreparedStatement>(() => CassandraSession.PrepareAsync(
                "INSERT INTO roles_by_name (roleid, name, displayname, is_systemrole, is_global, tenantid, created, modified) " +
                "VALUES (?, ?, ?, ?, ?, ?, ?, ?)"));
            // All the statements needed by the UpdateAsync method
            _updateRole = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
                {
                    CassandraSession.PrepareAsync(
                        "UPDATE roles SET name = ?, displayname = ?, is_systemrole = ?, modified = ? " +
                        "WHERE roleid = ?"),
                    CassandraSession.PrepareAsync("UPDATE roles_by_name SET displayname = ?, is_systemrole = ?, modified = ? " +
                                          "WHERE tenantid = ? AND name = ?"),
                    _deleteRoleByName.Value,
                    _createRoleByName.Value,
                }));

            _deleteRole = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
                {
                    CassandraSession.PrepareAsync(
                        "DELETE FROM roles WHERE roleid = ?"),
                    _deleteRoleByName.Value
                }));

            _deleteRoleByName = new AsyncLazy<PreparedStatement>(() => CassandraSession.PrepareAsync(
                "DELETE FROM roles_by_name WHERE name = ? AND tenantid = ?"));
        }
        public async Task CreateRoleAsync(CassandraRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
                throw new ArgumentNullException("role");

            var createdDate = DateTimeOffset.UtcNow;
            role.Created = createdDate;
            role.TenantId = role.IsGlobal ? Guid.Empty : TenantId;

            cancellationToken.ThrowIfCancellationRequested();

            PreparedStatement[] preparedStatements = await _createRole;

            var batch = new BatchStatement();

            foreach (var preparedStatement in preparedStatements)
            {
                batch.Add(preparedStatement.Bind(role.Id, role.Name, role.DisplayName, role.IsSystemRole, role.IsGlobal,
                    role.TenantId, createdDate, null));
            }

            await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
        }

        public async Task<IEnumerable<CassandraRole>> FindRolesByTenantIdAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var records =
                await
                    mapper.FetchAsync<CassandraRole>("SELECT * FROM roles_by_name WHERE tenantid = ?", TenantId);
            return records;
        }

        public async Task<IEnumerable<CassandraRole>> FindRoleByTenantIdAsync(Guid tenantId, string name,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var cqlQuery = string.Format("SELECT * FROM roles_by_name WHERE name = ? AND tenantid IN (?, {0})",
                Guid.Empty);
            var records = await mapper.FetchAsync<CassandraRole>(cqlQuery,name, tenantId);
            return records;
        }

        public async Task<IEnumerable<CassandraRole>> FindRoleByIdAsync(Guid roleId,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var records =
                await
                    mapper.FetchAsync<CassandraRole>("SELECT * FROM roles WHERE roleid = ?", roleId);
            return records;
        }

        public async Task DeleteAsync(CassandraRole role)
        {
            PreparedStatement[] prepared = await _deleteRole;
            var batch = new BatchStatement();

            // DELETE FROM roles ...
            batch.Add(prepared[0].Bind(role.Id));

            // DELETE FROM roles_by_rolename ...
            batch.Add(prepared[1].Bind(role.Name, role.TenantId));
            await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
        }

        public async Task DeleteRolesByTenantIdAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {

            var roles = await FindRolesByTenantIdAsync(cancellationToken);

            foreach (var role in roles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await DeleteAsync(role);
            }
        }








       
      
        public async Task<Store.Core.Models.IPage<UserRoleHandle>> PageRolesAsync(
            Guid userId, int pageSize,byte[] pagingState,
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
