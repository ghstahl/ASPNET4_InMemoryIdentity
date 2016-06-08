using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using Microsoft.AspNet.Identity;
using P5.CassandraStore.DAO;

namespace P5.AspNet.Identity.Cassandra
{
    public class CassandraRoleStore : IQueryableRoleStore<CassandraRole, Guid>
    {
        private ICassandraDAO _dao;
        private readonly bool _disposeOfSession;
        private bool _createSchema = false;
        private ResilientSession _resilientSession;
        private readonly Guid _tenantId;

        class ResilientSession
        {
            private ISession _session;
            private ICassandraDAO _dao;
            private readonly Guid _tenantId;
            private readonly bool _disposeOfSession;
            private readonly bool _createSchema;

            private  AsyncLazy<PreparedStatement[]> _createRole;
            private  AsyncLazy<PreparedStatement> _createRoleByName;
            private  AsyncLazy<PreparedStatement[]> _updateRole;
            private  AsyncLazy<PreparedStatement[]> _deleteRole;
            private  AsyncLazy<PreparedStatement> _deleteRoleByName;
            private  AsyncLazy<PreparedStatement> _findById;
            private  AsyncLazy<PreparedStatement> _findByName;
            private  AsyncLazy<PreparedStatement> _find;

            public ResilientSession(ICassandraDAO dao, Guid tenantId, bool disposeOfSession = false , bool createSchema = true)
            {
                _dao = dao;
                _tenantId = tenantId;
                _disposeOfSession = disposeOfSession;
                _createSchema = createSchema;
            }
            public void Dispose()
            {
                if (_session != null)
                {
                    _session.Dispose();
                }
            }
            public async Task EstablishConnectionAsync()
            {
                _createRole = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
                {
                    _session.PrepareAsync(
                        "INSERT INTO roles (roleid, name, displayname, is_systemrole, is_global, tenantid, created, modified) " +
                        "VALUES (?, ?, ?, ?, ?, ?, ?, ?)"),
                    _createRoleByName.Value
                }));

                _createRoleByName = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(
                    "INSERT INTO roles_by_name (name, tenantid, roleid, displayname, is_systemrole, is_global, created, modified) " +
                    "VALUES (?, ?, ?, ?, ?, ?, ?, ?)"));
                // All the statements needed by the UpdateAsync method
                _updateRole = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
                {
                    _session.PrepareAsync(
                        "UPDATE roles SET name = ?, displayname = ?, is_systemrole = ?, modified = ? " +
                        "WHERE roleid = ?"),
                    _session.PrepareAsync("UPDATE roles_by_name SET displayname = ?, is_systemrole = ?, modified = ? " +
                                          "WHERE tenantid = ? AND name = ?"),
                    _deleteRoleByName.Value,
                    _createRoleByName.Value,
                }));

                _deleteRole = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
                {
                    _session.PrepareAsync(
                        "DELETE FROM roles WHERE roleid = ?"),
                    _deleteRoleByName.Value
                }));

                _deleteRoleByName = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(
                    "DELETE FROM roles_by_name WHERE name = ? AND tenantid = ?"));

                _findById =
                    new AsyncLazy<PreparedStatement>(
                        () => _session.PrepareAsync(string.Format("SELECT * FROM roles WHERE roleid = ?")));
                _findByName =
                    new AsyncLazy<PreparedStatement>(
                        () =>
                            _session.PrepareAsync(
                                string.Format("SELECT * FROM roles_by_name WHERE name = ? AND tenantid IN (?, {0})",
                                    new Guid())));
                _find =
                    new AsyncLazy<PreparedStatement>(
                        () =>
                            _session.PrepareAsync(string.Format(
                                "SELECT * FROM roles_by_name WHERE tenantid IN (?, {0})",
                                new Guid())));

                // Create the schema if necessary
                if (_createSchema)
                    SchemaCreationHelper.CreateSchemaIfNotExists(_session);
            }
            public async Task<IQueryable<CassandraRole>> GetAllRoles()
            {
                PreparedStatement prepared = await _find;
                BoundStatement bound = prepared.Bind(_tenantId);
                RowSet rows = await _session.ExecuteAsync(bound).ConfigureAwait(false);
                return rows.Select(r => CassandraRole.FromRow(r)).ToArray().AsQueryable();
            }
            public async Task CreateAsync(CassandraRole role)
            {
                if (role == null)
                    throw new ArgumentNullException("role");

                var createdDate = DateTimeOffset.UtcNow;
                role.Created = createdDate;
                role.TenantId = role.IsGlobal ? new Guid() : _tenantId;

                PreparedStatement[] prepared = await _createRole;

                var batch = new BatchStatement();

                //INSERT INTO roles...
                batch.Add(prepared[0].Bind(role.Id, role.Name, role.DisplayName, role.IsSystemRole, role.IsGlobal,
                    role.TenantId, createdDate, null));

                //INSERT INTO roles_by_rolename... 
                batch.Add(prepared[1].Bind(role.Name, role.TenantId, role.Id, role.DisplayName, role.IsSystemRole,
                    role.IsGlobal, createdDate, null));
                await _session.ExecuteAsync(batch).ConfigureAwait(false);
            }
            public async Task<CassandraRole> FindByIdAsync(Guid roleId)
            {
                PreparedStatement prepared = await _findById;
                BoundStatement bound = prepared.Bind(roleId);

                RowSet rows = await _session.ExecuteAsync(bound).ConfigureAwait(false);

                var role = CassandraRole.FromRow(rows.SingleOrDefault());
                return role != null && (role.IsGlobal || role.TenantId == _tenantId) ? role : null;
            }
            public async Task<CassandraRole> FindByNameAsync(string roleName)
            {
                PreparedStatement prepared = await _findByName;
                BoundStatement bound = prepared.Bind(roleName, _tenantId);

                RowSet rows = await _session.ExecuteAsync(bound).ConfigureAwait(false);

                return CassandraRole.FromRow(rows.SingleOrDefault());
            }
            public async Task DeleteAsync(CassandraRole role)
            {
                PreparedStatement[] prepared = await _deleteRole;
                var batch = new BatchStatement();

                // DELETE FROM roles ...
                batch.Add(prepared[0].Bind(role.Id));

                // DELETE FROM roles_by_rolename ...
                batch.Add(prepared[1].Bind(role.Name, role.TenantId));
                await _session.ExecuteAsync(batch).ConfigureAwait(false);
            }
            public async Task UpdateAsync(CassandraRole role)
            {
                if (role == null)
                    throw new ArgumentNullException("role");

                var modifiedDate = DateTimeOffset.UtcNow;
                role.Modified = modifiedDate;

                PreparedStatement[] prepared = await _updateRole;
                var batch = new BatchStatement();

                // UPDATE roles ...
                batch.Add(prepared[0].Bind(role.Name, role.DisplayName, role.IsSystemRole, modifiedDate, role.Id));

                // See if the name changed we can decide whether we need a different roles_by_name record
                string oldName;
                if (role.HasNameChanged(out oldName) == false)
                {
                    // UPDATE roles_by_name ... (since username hasn't changed)
                    batch.Add(prepared[1].Bind(role.DisplayName, role.IsSystemRole, modifiedDate, role.TenantId, oldName));
                }
                else
                {
                    // DELETE FROM roles_by_name ... (delete old record since name changed)
                    if (string.IsNullOrEmpty(oldName) == false)
                    {
                        batch.Add(prepared[2].Bind(oldName, role.TenantId));
                    }

                    // INSERT INTO roles_by_name ... (insert new record since name changed)
                    if (string.IsNullOrEmpty(role.Name) == false)
                    {
                        batch.Add(prepared[3].Bind(role.Name, role.TenantId, role.Id, role.DisplayName, role.IsSystemRole,
                            role.IsGlobal, role.Created, modifiedDate));
                    }
                }

                await _session.ExecuteAsync(batch).ConfigureAwait(false);
            }
        }
        private async Task EstablishSession()
        {
            if (_resilientSession == null)
            {
                var rs = new CassandraRoleStore.ResilientSession(_dao, _tenantId, _disposeOfSession, _createSchema);
                await rs.EstablishConnectionAsync();
                _resilientSession = rs;
            }
        }
        /// <summary>
        /// Creates a new instance of CassandraRoleStore that will use the provided ISession instance to talk to Cassandra.  Optionally,
        /// specify whether the ISession instance should be Disposed when this class is Disposed.
        /// </summary>
        /// <param name="session">The session for talking to the Cassandra keyspace.</param>
        /// <param name="disposeOfSession">Whether to dispose of the session instance when this object is disposed.</param>
        /// <param name="createSchema">Whether to create the schema tables if they don't exist.</param>
        public CassandraRoleStore(ICassandraDAO dao, Guid tenantId, bool disposeOfSession = false,
            bool createSchema = true)
        {
            _dao = dao;
            _disposeOfSession = disposeOfSession;
            _tenantId = tenantId;
            _createSchema = createSchema;
        }

        /// <summary>
        /// The tenant id that the user store has been initialised with.
        /// </summary>
        public Guid TenantId { get { return _tenantId; } }
        public IQueryable<CassandraRole> Roles
        {
            get { return GetAllRoles().Result; }
        }
        public async Task<IQueryable<CassandraRole>> GetAllRoles()
        {
            Exception ex = null;
            IQueryable<CassandraRole> result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync<IQueryable<CassandraRole>>(
                async () =>
                {
                    await EstablishSession();
                    return await _resilientSession.GetAllRoles();
                },
                async (e) =>
                {
                    ex = e;
                    return new TryWithAwaitInCatchExcpetionHandleResult<IQueryable<CassandraRole>>
                    {
                        RethrowException = true,
                        DefaultResult = null
                    };
                }
                );
            return result;
        }
        public async Task CreateAsync(CassandraRole role)
        {
            Exception ex = null;
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await EstablishSession();
                    await _resilientSession.CreateAsync(role);
                },
                async (e) =>
                {
                    ex = e;
                    return new TryWithAwaitInCatchExcpetionHandleResult<Task>
                    {
                        RethrowException = true
                    };
                }
                );
        }
        public async Task<CassandraRole> FindByIdAsync(Guid roleId)
        {
            Exception ex = null;
            CassandraRole result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync<CassandraRole>(
                async () =>
                {
                    await EstablishSession();
                    return await _resilientSession.FindByIdAsync(roleId);
                },
                async (e) =>
                {
                    ex = e;
                    return new TryWithAwaitInCatchExcpetionHandleResult<CassandraRole>
                    {
                        RethrowException = true,
                        DefaultResult = null
                    };
                }
                );
            return result;
            
        }
        public async Task<CassandraRole> FindByNameAsync(string roleName)
        {
            Exception ex = null;
            CassandraRole result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync<CassandraRole>(
                async () =>
                {
                    await EstablishSession();
                    return await _resilientSession.FindByNameAsync(roleName);
                },
                async (e) =>
                {
                    ex = e;
                    return new TryWithAwaitInCatchExcpetionHandleResult<CassandraRole>
                    {
                        RethrowException = true,
                        DefaultResult = null
                    };
                }
                );
            return result;
        }
        public async Task UpdateAsync(CassandraRole role)
        {
            Exception ex = null;
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await EstablishSession();
                    await _resilientSession.UpdateAsync(role);
                },
                async (e) =>
                {
                    ex = e;
                    return new TryWithAwaitInCatchExcpetionHandleResult<Task>
                    {
                        RethrowException = true
                    };
                }
                );
        }
        public async Task DeleteAsync(CassandraRole role)
        {
            Exception ex = null;
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await EstablishSession();
                    await _resilientSession.DeleteAsync(role);
                },
                async (e) =>
                {
                    ex = e;
                    return new TryWithAwaitInCatchExcpetionHandleResult<Task>
                    {
                        RethrowException = true
                    };
                }
                );
        }
        protected void Dispose(bool disposing)
        {
            if (_disposeOfSession && _resilientSession != null)
                _resilientSession.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
