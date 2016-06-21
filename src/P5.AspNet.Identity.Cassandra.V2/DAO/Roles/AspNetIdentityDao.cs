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
                        "UPDATE roles " +
                        "SET name = ?, displayname = ?, is_systemrole = ?, modified = ? " +
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

        private async Task<IEnumerable<BoundStatement>> BuildCreateStatements(CassandraRole role)
        {
            var createdDate = DateTimeOffset.UtcNow;
            role.Created = createdDate;
            role.TenantId = role.IsGlobal ? Guid.Empty : TenantId;

            PreparedStatement[] preparedStatements = await _createRole;
            List<BoundStatement> boundStatements = new List<BoundStatement>();
            var batch = new BatchStatement();

            foreach (var preparedStatement in preparedStatements)
            {
                boundStatements.Add(preparedStatement.Bind(role.Id, role.Name, role.DisplayName, role.IsSystemRole, role.IsGlobal,
                    role.TenantId, createdDate, null));
            }
            return boundStatements;
        }
        private async Task<IEnumerable<BoundStatement>> BuildDeleteStatements(CassandraRole role)
        {
            List<BoundStatement> boundStatements = new List<BoundStatement>();
            PreparedStatement[] prepared = await _deleteRole;

            // DELETE FROM roles ...
            boundStatements.Add(prepared[0].Bind(role.Id));

            // DELETE FROM roles_by_rolename ...
            boundStatements.Add(prepared[1].Bind(role.Name, role.TenantId));
            return boundStatements;
        }
        public async Task CreateRoleAsync(CassandraRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
                throw new ArgumentNullException("role");
            var boundStatements = await BuildCreateStatements(role);
            cancellationToken.ThrowIfCancellationRequested();
            
            var batch = new BatchStatement();
            batch.AddRange(boundStatements);

            await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
        }

        public async Task UpdateRoleAsync(CassandraRole role,CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
                throw new ArgumentNullException("role");
            if(string.IsNullOrEmpty(role.Name))
                throw new ArgumentNullException("role","role.Name cannot be null or empty");

            var modifiedDate = DateTimeOffset.UtcNow;
            role.Modified = modifiedDate;
            var batch = new BatchStatement();
            string oldName;
            var changed = role.HasNameChanged(out oldName);
            if (changed)
            {
                // This a Create/Delete move
                // can only change the name if that target name does not exist.
                var findResult = await FindRoleByNameAsync(role.Name);
                if (findResult.Any())
                {
                    // sorry, no go, this is record must either be deleted first or use another name.
                    throw new Exception(
                        string.Format(
                            "Cannot change role name:[{0}] to an existing role, pick another name, or delete this one first",
                            role.Name));
                }
                var boundStatements = await BuildCreateStatements(role);
                batch.AddRange(boundStatements);
                var oldRole = new CassandraRole(role.TenantId, oldName);
                boundStatements = await BuildDeleteStatements(oldRole);
                batch.AddRange(boundStatements);
                await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
                await RenameRoleNameInUsersAsync(oldName, role.Name, cancellationToken);

            }
            else
            {
                PreparedStatement[] prepared = await _updateRole;

                // UPDATE roles ...
                batch.Add(prepared[0].Bind(role.Name, role.DisplayName, role.IsSystemRole, modifiedDate, role.Id));
                // UPDATE roles_by_name ... (since username hasn't changed)
                batch.Add(prepared[1].Bind(role.DisplayName, role.IsSystemRole, modifiedDate, role.TenantId, role.Name));

                await CassandraSession.ExecuteAsync(batch).ConfigureAwait(false);
            }
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

        public async Task<IEnumerable<CassandraRole>> FindRoleByNameAsync( string name,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var cqlQuery = string.Format("SELECT * FROM roles_by_name WHERE name = ? AND tenantid IN (?, {0})",
                Guid.Empty);
            var records = await mapper.FetchAsync<CassandraRole>(cqlQuery,name, TenantId);
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
