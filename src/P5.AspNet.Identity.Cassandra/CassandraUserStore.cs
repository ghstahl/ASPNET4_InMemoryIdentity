using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cassandra;
using Microsoft.AspNet.Identity;

namespace P5.AspNet.Identity.Cassandra
{
    public class CassandraUserStore : IUserStore<CassandraUser, Guid>, IUserLoginStore<CassandraUser, Guid>, IUserClaimStore<CassandraUser, Guid>,
        IUserPasswordStore<CassandraUser, Guid>, IUserSecurityStampStore<CassandraUser, Guid>,
        IUserTwoFactorStore<CassandraUser, Guid>, IUserLockoutStore<CassandraUser, Guid>, 
        IUserPhoneNumberStore<CassandraUser, Guid>, IUserEmailStore<CassandraUser, Guid>, IUserRoleStore<CassandraUser, Guid>
    {

        private readonly Guid _tenantId;

        // A cached copy of some completed tasks
        private static readonly Task<bool> TrueTask = Task.FromResult(true);
        private static readonly Task<bool> FalseTask = Task.FromResult(false);
        private static readonly Task CompletedTask = TrueTask;

        private readonly ISession _session;
        private readonly bool _disposeOfSession;

        // Reusable prepared statements, lazy evaluated
        private readonly AsyncLazy<PreparedStatement> _createUserByUserName;
        private readonly AsyncLazy<PreparedStatement> _createUserByEmail;
        private readonly AsyncLazy<PreparedStatement> _deleteUserByUserName;
        private readonly AsyncLazy<PreparedStatement> _deleteUserByEmail; 

        private readonly AsyncLazy<PreparedStatement[]> _createUser;
        private readonly AsyncLazy<PreparedStatement[]> _updateUser;
        private readonly AsyncLazy<PreparedStatement[]> _deleteUser;

        private readonly AsyncLazy<PreparedStatement> _findById;
        private readonly AsyncLazy<PreparedStatement> _findByName;
        private readonly AsyncLazy<PreparedStatement> _findByEmail; 

        private readonly AsyncLazy<PreparedStatement[]> _addLogin;
        private readonly AsyncLazy<PreparedStatement[]> _removeLogin;
        private readonly AsyncLazy<PreparedStatement> _getLogins;
        private readonly AsyncLazy<PreparedStatement> _getLoginsByProvider;

        private readonly AsyncLazy<PreparedStatement> _getClaims;
        private readonly AsyncLazy<PreparedStatement> _addClaim;
        private readonly AsyncLazy<PreparedStatement> _removeClaim;

        private readonly AsyncLazy<PreparedStatement[]> _addToRoleAsync;
        private readonly AsyncLazy<PreparedStatement[]> _removeFromRoleAsync;
        private readonly AsyncLazy<PreparedStatement> _getRolesAsync;
        private readonly AsyncLazy<PreparedStatement> _isInRoleAsync;

        /// <summary>
        /// Creates a new instance of CassandraUserStore that will use the provided ISession instance to talk to Cassandra.  Optionally,
        /// specify whether the ISession instance should be Disposed when this class is Disposed.
        /// </summary>
        /// <param name="session">The session for talking to the Cassandra keyspace.</param>
        /// <param name="disposeOfSession">Whether to dispose of the session instance when this object is disposed.</param>
        /// <param name="createSchema">Whether to create the schema tables if they don't exist.</param>
        public CassandraUserStore(ISession session, Guid tenantId, bool disposeOfSession = false, bool createSchema = true)
        {
            _session = session;
            _disposeOfSession = disposeOfSession;
            _tenantId = tenantId;

            // Create some reusable prepared statements so we pay the cost of preparing once, then bind multiple times
            _createUserByUserName = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(
                "INSERT INTO users_by_username (tenantid, username, userid, password_hash, security_stamp, two_factor_enabled, access_failed_count, " +
                "lockout_enabled, lockout_end_date, phone_number, phone_number_confirmed, email, email_confirmed, created, modified, enabled, source, source_id) " +
                string.Format("VALUES ({0}, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",_tenantId)));
            _createUserByEmail = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(
                "INSERT INTO users_by_email (tenantid, email, userid, username, password_hash, security_stamp, two_factor_enabled, access_failed_count, " +
                "lockout_enabled, lockout_end_date, phone_number, phone_number_confirmed, email_confirmed, created, modified, enabled, source, source_id) " +
                string.Format("VALUES ({0}, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",_tenantId)));

            _deleteUserByUserName = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(string.Format("DELETE FROM users_by_username WHERE tenantid = {0} AND username = ?",_tenantId)));
            _deleteUserByEmail = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(string.Format("DELETE FROM users_by_email WHERE tenantid = {0} AND email = ?",_tenantId)));
            
            // All the statements needed by the CreateAsync method
            _createUser = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new []
            {
                _session.PrepareAsync("INSERT INTO users (userid, tenantid, username, password_hash, security_stamp, two_factor_enabled, access_failed_count, " +
                                      "lockout_enabled, lockout_end_date, phone_number, phone_number_confirmed, email, email_confirmed, created, modified, enabled, source, source_id) " +
                                      string.Format("VALUES (?, {0}, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",_tenantId)),
                _createUserByUserName.Value,
                _createUserByEmail.Value
            }));

            // All the statements needed by the DeleteAsync method
            _deleteUser = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
            {
                _session.PrepareAsync(string.Format("DELETE FROM users WHERE userid = ?")),
                _deleteUserByUserName.Value,
                _deleteUserByEmail.Value
            }));

            // All the statements needed by the UpdateAsync method
            _updateUser = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new []
            {
                _session.PrepareAsync("UPDATE users SET username = ?, password_hash = ?, security_stamp = ?, two_factor_enabled = ?, access_failed_count = ?, " +
                                      "lockout_enabled = ?, lockout_end_date = ?, phone_number = ?, phone_number_confirmed = ?, email = ?, email_confirmed = ?, " +
                                      "modified = ? , enabled = ?, source = ?, source_id = ?" + 
                                      "WHERE userid = ?"),
                _session.PrepareAsync("UPDATE users_by_username SET password_hash = ?, security_stamp = ?, two_factor_enabled = ?, access_failed_count = ?, " +
                                      "lockout_enabled = ?, lockout_end_date = ?, phone_number = ?, phone_number_confirmed = ?, email = ?, email_confirmed = ?, " +
                                      "modified = ? , enabled = ?, source = ?, source_id = ?" + 
                                      string.Format("WHERE tenantid = {0} AND username = ?",_tenantId)),
                _deleteUserByUserName.Value,
                _createUserByUserName.Value,
                _session.PrepareAsync("UPDATE users_by_email SET username = ?, password_hash = ?, security_stamp = ?, two_factor_enabled = ?, access_failed_count = ?, " +
                                      "lockout_enabled = ?, lockout_end_date = ?, phone_number = ?, phone_number_confirmed = ?, email_confirmed = ?, " +
                                      "modified = ? , enabled = ?, source = ?, source_id = ?" + 
                                      string.Format("WHERE tenantid = {0} AND email = ?",_tenantId)),
                _deleteUserByEmail.Value,
                _createUserByEmail.Value
            }));
            
            _findById = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(string.Format("SELECT * FROM users WHERE userid = ?")));
            _findByName = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(string.Format("SELECT * FROM users_by_username WHERE tenantid = {0} AND username = ?",_tenantId)));
            _findByEmail = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(string.Format("SELECT * FROM users_by_email WHERE tenantid = {0} AND email = ?",_tenantId)));
            
            _addLogin = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new []
            {
                _session.PrepareAsync(string.Format("INSERT INTO logins (userid, login_provider, provider_key, tenantid) VALUES (?, ?, ?, {0})",_tenantId)),
                _session.PrepareAsync(string.Format("INSERT INTO logins_by_provider (login_provider, provider_key, tenantid, userid) VALUES (?, ?, {0}, ?)",_tenantId))
            }));
            _removeLogin = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new []
            {
                _session.PrepareAsync(string.Format("DELETE FROM logins WHERE userId = ? and login_provider = ? and provider_key = ?")),
                _session.PrepareAsync(string.Format("DELETE FROM logins_by_provider WHERE login_provider = ? AND provider_key = ? AND tenantid = {0}",_tenantId))
            }));
            _getLogins = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(string.Format("SELECT * FROM logins WHERE userId = ?")));
            _getLoginsByProvider = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(
                string.Format("SELECT * FROM logins_by_provider WHERE login_provider = ? AND provider_key = ? AND tenantid = {0}",_tenantId)));

            _getClaims = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(string.Format("SELECT * FROM claims WHERE userId = ?")));
            _addClaim = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(
                string.Format("INSERT INTO claims (userid, type, value) VALUES (?, ?, ?)")));
            _removeClaim = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(
                string.Format("DELETE FROM claims WHERE userId = ? AND type = ? AND value = ?")));

            _addToRoleAsync = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
            {
                _session.PrepareAsync(string.Format("INSERT INTO user_roles (userid, rolename, assigned) VALUES (?, ?, ?)")),
                _session.PrepareAsync(string.Format("INSERT INTO user_roles_by_role (rolename, userid, assigned) VALUES (?, ?, ?)"))
            }));

            _removeFromRoleAsync = new AsyncLazy<PreparedStatement[]>(() => Task.WhenAll(new[]
            {
                _session.PrepareAsync(string.Format("DELETE FROM user_roles WHERE userId = ? and rolename = ?")),
                _session.PrepareAsync(string.Format("DELETE FROM user_roles_by_role WHERE rolename = ? AND userId = ?"))
            }));

            _getRolesAsync = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(
                "SELECT rolename FROM user_roles WHERE userid = ?"));
            _isInRoleAsync = new AsyncLazy<PreparedStatement>(() => _session.PrepareAsync(
                "SELECT rolename FROM user_roles WHERE userId = ? and rolename = ?"));

            // Create the schema if necessary
            if (createSchema)
                SchemaCreationHelper.CreateSchemaIfNotExists(session);
        }

        /// <summary>
        /// The tenant id that the user store has been initialised with.
        /// </summary>
        public Guid TenantId { get { return _tenantId; } }

        /// <summary>
        /// Insert a new user.
        /// </summary>
        public async Task CreateAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            // TODO:  Support uniqueness for usernames/emails at the C* level using LWT?

            var id = user.GenerateIdFromUserData();
            var createdDate = DateTimeOffset.UtcNow;
            user.Created = createdDate;

            PreparedStatement[] prepared = await _createUser;
            var batch = new BatchStatement();

            // INSERT INTO users ...
            batch.Add(prepared[0].Bind(id, user.UserName, user.PasswordHash, user.SecurityStamp, user.IsTwoFactorEnabled, user.AccessFailedCount,
                user.IsLockoutEnabled, user.LockoutEndDate, user.PhoneNumber, user.IsPhoneNumberConfirmed, user.Email,
                user.IsEmailConfirmed, createdDate, null, user.Enabled, user.Source, user.SourceId));

            // Only insert into username and email tables if those have a value
            if (string.IsNullOrEmpty(user.UserName) == false)
            {
                // INSERT INTO users_by_username ...
                batch.Add(prepared[1].Bind(user.UserName, id, user.PasswordHash, user.SecurityStamp, user.IsTwoFactorEnabled, user.AccessFailedCount,
                    user.IsLockoutEnabled, user.LockoutEndDate, user.PhoneNumber, user.IsPhoneNumberConfirmed, user.Email,
                    user.IsEmailConfirmed, createdDate, null, user.Enabled, user.Source, user.SourceId));
            }

            if (string.IsNullOrEmpty(user.Email) == false)
            {
                // INSERT INTO users_by_email ...
                batch.Add(prepared[2].Bind(user.Email, id, user.UserName, user.PasswordHash, user.SecurityStamp, user.IsTwoFactorEnabled,
                    user.AccessFailedCount, user.IsLockoutEnabled, user.LockoutEndDate, user.PhoneNumber,
                    user.IsPhoneNumberConfirmed, user.IsEmailConfirmed, createdDate, null, user.Enabled, user.Source, user.SourceId));
            }

            await _session.ExecuteAsync(batch).ConfigureAwait(false);
        }

        /// <summary>
        /// Update a user.
        /// </summary>
        public async Task UpdateAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            var id = user.GenerateIdFromUserData();

            var modifiedDate = DateTimeOffset.UtcNow;
            user.Modified = modifiedDate;

            PreparedStatement[] prepared = await _updateUser;
            var batch = new BatchStatement();

            // UPDATE users ...
            batch.Add(prepared[0].Bind(user.UserName, user.PasswordHash, user.SecurityStamp, user.IsTwoFactorEnabled, user.AccessFailedCount,
                user.IsLockoutEnabled, user.LockoutEndDate, user.PhoneNumber, user.IsPhoneNumberConfirmed, user.Email,
                user.IsEmailConfirmed, modifiedDate, user.Enabled, user.Source, user.SourceId, id));

            // See if the username changed so we can decide whether we need a different users_by_username record
            string oldUserName;
            if (user.HasUserNameChanged(out oldUserName) == false && string.IsNullOrEmpty(user.UserName) == false)
            {
                // UPDATE users_by_username ... (since username hasn't changed)
                batch.Add(prepared[1].Bind(user.PasswordHash, user.SecurityStamp, user.IsTwoFactorEnabled, user.AccessFailedCount,
                    user.IsLockoutEnabled, user.LockoutEndDate, user.PhoneNumber, user.IsPhoneNumberConfirmed, user.Email,
                    user.IsEmailConfirmed, modifiedDate, user.Enabled, user.Source, user.SourceId, user.UserName));
            }
            else
            {
                // DELETE FROM users_by_username ... (delete old record since username changed)
                if (string.IsNullOrEmpty(oldUserName) == false)
                {
                    batch.Add(prepared[2].Bind(oldUserName));
                }

                // INSERT INTO users_by_username ... (insert new record since username changed)
                if (string.IsNullOrEmpty(user.UserName) == false)
                {
                    batch.Add(prepared[3].Bind(user.UserName, id, user.PasswordHash, user.SecurityStamp, user.IsTwoFactorEnabled,
                        user.AccessFailedCount, user.IsLockoutEnabled, user.LockoutEndDate, user.PhoneNumber,
                        user.IsPhoneNumberConfirmed, user.Email, user.IsEmailConfirmed, user.Created, modifiedDate, user.Enabled, user.Source, user.SourceId));
                }
            }

            // See if the email changed so we can decide if we need a different users_by_email record
            string oldEmail;
            if (user.HasEmailChanged(out oldEmail) == false && string.IsNullOrEmpty(user.Email) == false)
            {
                // UPDATE users_by_email ... (since email hasn't changed)
                batch.Add(prepared[4].Bind(user.UserName, user.PasswordHash, user.SecurityStamp, user.IsTwoFactorEnabled, user.AccessFailedCount,
                    user.IsLockoutEnabled, user.LockoutEndDate, user.PhoneNumber, user.IsPhoneNumberConfirmed,
                    user.IsEmailConfirmed, modifiedDate, user.Enabled, user.Source, user.SourceId, user.Email));
            }
            else
            {
                // DELETE FROM users_by_email ... (delete old record since email changed)
                if (string.IsNullOrEmpty(oldEmail) == false)
                {
                    batch.Add(prepared[5].Bind(oldEmail));
                }

                // INSERT INTO users_by_email ... (insert new record since email changed)
                if (string.IsNullOrEmpty(user.Email) == false)
                {
                    batch.Add(prepared[6].Bind(user.Email, id, user.UserName, user.PasswordHash, user.SecurityStamp, user.IsTwoFactorEnabled,
                        user.AccessFailedCount, user.IsLockoutEnabled, user.LockoutEndDate, user.PhoneNumber,
                        user.IsPhoneNumberConfirmed, user.IsEmailConfirmed, user.Created, modifiedDate, user.Enabled, user.Source, user.SourceId));
                }
            }

            await _session.ExecuteAsync(batch).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a user.
        /// </summary>
        public async Task DeleteAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            var id = user.GenerateIdFromUserData();

            PreparedStatement[] prepared = await _deleteUser;
            var batch = new BatchStatement();

            // DELETE FROM users ...
            batch.Add(prepared[0].Bind(id));

            // Make sure the username didn't change before deleting from users_by_username (not sure this is possible, but protect ourselves anyway)
            string userName;
            if (user.HasUserNameChanged(out userName) == false)
                userName = user.UserName;

            // DELETE FROM users_by_username ...
            if (string.IsNullOrEmpty(userName) == false)
                batch.Add(prepared[1].Bind(userName));

            // Make sure email didn't change before deleting from users_by_email (also not sure this is possible)
            string email;
            if (user.HasEmailChanged(out email) == false)
                email = user.Email;

            // DELETE FROM users_by_email ...
            if (string.IsNullOrEmpty(email) == false)
                batch.Add(prepared[2].Bind(email));

            await _session.ExecuteAsync(batch).ConfigureAwait(false);
        }

        /// <summary>
        /// Finds a user by userId.
        /// </summary>
        public async Task<CassandraUser> FindByIdAsync(Guid userId)
        {
            PreparedStatement prepared = await _findById;
            BoundStatement bound = prepared.Bind(userId);

            RowSet rows = await _session.ExecuteAsync(bound).ConfigureAwait(false);
            return CassandraUser.FromRow(rows.SingleOrDefault());
        }

        /// <summary>
        /// Find a user by name (assumes usernames are unique).
        /// </summary>
        public async Task<CassandraUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("userName cannot be null or empty", "userName");

            PreparedStatement prepared = await _findByName;
            BoundStatement bound = prepared.Bind(userName);

            RowSet rows = await _session.ExecuteAsync(bound).ConfigureAwait(false);
            return CassandraUser.FromRow(rows.SingleOrDefault());
        }

        /// <summary>
        /// Adds a user login with the specified provider and key
        /// </summary>
        public async Task AddLoginAsync(CassandraUser user, UserLoginInfo login)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (login == null) throw new ArgumentNullException("login");
            var id = user.GenerateIdFromUserData();
            PreparedStatement[] prepared = await _addLogin;
            var batch = new BatchStatement();

            // INSERT INTO logins ...
            batch.Add(prepared[0].Bind(id, login.LoginProvider, login.ProviderKey));

            // INSERT INTO logins_by_provider ...
            batch.Add(prepared[1].Bind(login.LoginProvider, login.ProviderKey, id));

            await _session.ExecuteAsync(batch).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the user login with the specified combination if it exists
        /// </summary>
        public async Task RemoveLoginAsync(CassandraUser user, UserLoginInfo login)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (login == null) throw new ArgumentNullException("login");
            var id = user.GenerateIdFromUserData();

            PreparedStatement[] prepared = await _removeLogin;
            var batch = new BatchStatement();

            // DELETE FROM logins ...
            batch.Add(prepared[0].Bind(id, login.LoginProvider, login.ProviderKey));

            // DELETE FROM logins_by_provider ...
            batch.Add(prepared[1].Bind(login.LoginProvider, login.ProviderKey));

            await _session.ExecuteAsync(batch).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the linked accounts for this user
        /// </summary>
        public async Task<IList<UserLoginInfo>> GetLoginsAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            var id = user.GenerateIdFromUserData();
            PreparedStatement prepared = await _getLogins;
            BoundStatement bound = prepared.Bind(id);

            RowSet rows = await _session.ExecuteAsync(bound).ConfigureAwait(false);
            return rows.Select(row => new UserLoginInfo(row.GetValue<string>("login_provider"), row.GetValue<string>("provider_key"))).ToList();
        }

        /// <summary>
        /// Returns the user associated with this login
        /// </summary>
        public async Task<CassandraUser> FindAsync(UserLoginInfo login)
        {
            if (login == null) throw new ArgumentNullException("login");

            PreparedStatement prepared = await _getLoginsByProvider;
            BoundStatement bound = prepared.Bind(login.LoginProvider, login.ProviderKey);

            RowSet loginRows = await _session.ExecuteAsync(bound).ConfigureAwait(false);
            Row loginResult = loginRows.FirstOrDefault();
            if (loginResult == null)
                return null;

            prepared = await _findById;
            bound = prepared.Bind(loginResult.GetValue<Guid>("userid"));

            RowSet rows = await _session.ExecuteAsync(bound).ConfigureAwait(false);
            return CassandraUser.FromRow(rows.SingleOrDefault());
        }

        /// <summary>
        /// Returns the claims for the user with the issuer set
        /// </summary>
        public async Task<IList<Claim>> GetClaimsAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            var id = user.GenerateIdFromUserData();
            PreparedStatement prepared = await _getClaims;
            BoundStatement bound = prepared.Bind(id);

            RowSet rows = await _session.ExecuteAsync(bound).ConfigureAwait(false);
            return rows.Select(row => new Claim(row.GetValue<string>("type"), row.GetValue<string>("value"))).ToList();
        }

        /// <summary>
        /// Add a new user claim
        /// </summary>
        public async Task AddClaimAsync(CassandraUser user, Claim claim)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (claim == null) throw new ArgumentNullException("claim");
            var id = user.GenerateIdFromUserData();
            PreparedStatement prepared = await _addClaim;
            BoundStatement bound = prepared.Bind(id, claim.Type, claim.Value);
            await _session.ExecuteAsync(bound).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove a user claim
        /// </summary>
        public async Task RemoveClaimAsync(CassandraUser user, Claim claim)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (claim == null) throw new ArgumentNullException("claim");
            var id = user.GenerateIdFromUserData();

            PreparedStatement prepared = await _removeClaim;
            BoundStatement bound = prepared.Bind(id, claim.Type, claim.Value);

            await _session.ExecuteAsync(bound).ConfigureAwait(false);
        }

        /// <summary>
        /// Set the user password hash
        /// </summary>
        public Task SetPasswordHashAsync(CassandraUser user, string passwordHash)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Password hash can be null when removing a password from a user
            user.PasswordHash = passwordHash;
            return CompletedTask;
        }

        /// <summary>
        /// Get the user password hash
        /// </summary>
        public Task<string> GetPasswordHashAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        /// Returns true if a user has a password set
        /// </summary>
        public Task<bool> HasPasswordAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return string.IsNullOrEmpty(user.PasswordHash) ? FalseTask : TrueTask;
        }

        /// <summary>
        /// Set the security stamp for the user
        /// </summary>
        public Task SetSecurityStampAsync(CassandraUser user, string stamp)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (stamp == null) throw new ArgumentNullException("stamp");

            user.SecurityStamp = stamp;
            return CompletedTask;
        }

        /// <summary>
        /// Get the user security stamp
        /// </summary>
        public Task<string> GetSecurityStampAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        /// Sets whether two factor authentication is enabled for the user
        /// </summary>
        public Task SetTwoFactorEnabledAsync(CassandraUser user, bool enabled)
        {
            if (user == null) throw new ArgumentNullException("user");

            user.IsTwoFactorEnabled = enabled;
            return CompletedTask;
        }

        /// <summary>
        /// Returns whether two factor authentication is enabled for the user
        /// </summary>
        public Task<bool> GetTwoFactorEnabledAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.IsTwoFactorEnabled);
        }

        /// <summary>
        /// Returns the DateTimeOffset that represents the end of a user's lockout, any time in the past should be considered
        /// not locked out.
        /// </summary>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.LockoutEndDate);
        }

        /// <summary>
        /// Locks a user out until the specified end date (set to a past date, to unlock a user)
        /// </summary>
        public Task SetLockoutEndDateAsync(CassandraUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null) throw new ArgumentNullException("user");

            user.LockoutEndDate = lockoutEnd;
            return CompletedTask;
        }

        /// <summary>
        /// Used to record when an attempt to access the user has failed
        /// </summary>
        public Task<int> IncrementAccessFailedCountAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            // NOTE:  Since we aren't using C* counters and an increment operation, the value for the counter we loaded could be stale when we
            // increment this way and so the count could be incorrect (i.e. this increment in not atomic)
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Used to reset the access failed count, typically after the account is successfully accessed
        /// </summary>
        public Task ResetAccessFailedCountAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Same note as above in Increment applies here
            user.AccessFailedCount = 0;
            return CompletedTask;
        }

        /// <summary>
        /// Returns the current number of failed access attempts.  This number usually will be reset whenever the password is
        /// verified or the account is locked out.
        /// </summary>
        public Task<int> GetAccessFailedCountAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Returns whether the user can be locked out.
        /// </summary>
        public Task<bool> GetLockoutEnabledAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            return Task.FromResult(user.IsLockoutEnabled);
        }

        /// <summary>
        /// Sets whether the user can be locked out.
        /// </summary>
        public Task SetLockoutEnabledAsync(CassandraUser user, bool enabled)
        {
            if (user == null) throw new ArgumentNullException("user");

            user.IsLockoutEnabled = enabled;
            return CompletedTask;
        }

        /// <summary>
        /// Returns the user associated with this email
        /// </summary>
        public async Task<CassandraUser> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email cannot be null or empty", "email");

            PreparedStatement prepared = await _findByEmail;
            BoundStatement bound = prepared.Bind(email);

            RowSet rows = await _session.ExecuteAsync(bound).ConfigureAwait(false);
            return CassandraUser.FromRow(rows.SingleOrDefault());
        }

        /// <summary>
        /// Set the user email
        /// </summary>
        public Task SetEmailAsync(CassandraUser user, string email)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (email == null) throw new ArgumentNullException("email");

            user.Email = email;
            return CompletedTask;
        }

        /// <summary>
        /// Get the user email
        /// </summary>
        public Task<string> GetEmailAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Returns true if the user email is confirmed
        /// </summary>
        public Task<bool> GetEmailConfirmedAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.IsEmailConfirmed);
        }

        /// <summary>
        /// Sets whether the user email is confirmed
        /// </summary>
        public Task SetEmailConfirmedAsync(CassandraUser user, bool confirmed)
        {
            if (user == null) throw new ArgumentNullException("user");

            user.IsEmailConfirmed = confirmed;
            return CompletedTask;
        }

        /// <summary>
        /// Set the user's phone number
        /// </summary>
        public Task SetPhoneNumberAsync(CassandraUser user, string phoneNumber)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (phoneNumber == null) throw new ArgumentNullException("phoneNumber");

            user.PhoneNumber = phoneNumber;
            return CompletedTask;
        }

        /// <summary>
        /// Get the user phone number
        /// </summary>
        public Task<string> GetPhoneNumberAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// Returns true if the user phone number is confirmed
        /// </summary>
        public Task<bool> GetPhoneNumberConfirmedAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.IsPhoneNumberConfirmed);
        }

        /// <summary>
        /// Sets whether the user phone number is confirmed
        /// </summary>
        public Task SetPhoneNumberConfirmedAsync(CassandraUser user, bool confirmed)
        {
            if (user == null) throw new ArgumentNullException("user");

            user.IsPhoneNumberConfirmed = confirmed;
            return CompletedTask;
        }

        protected void Dispose(bool disposing)
        {
            if (_disposeOfSession)
                _session.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task AddToRoleAsync(CassandraUser user, string roleName)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (roleName == null) throw new ArgumentNullException("roleName");
            var id = user.GenerateIdFromUserData();
            var createdDate = DateTimeOffset.UtcNow;
            user.Created = createdDate;

            PreparedStatement[] prepared = await _addToRoleAsync;
            var batch = new BatchStatement();

            // INSERT INTO user_roles ...
            batch.Add(prepared[0].Bind(id, roleName, createdDate));


            // INSERT INTO user_roles_by_role ...
            batch.Add(prepared[1].Bind(roleName, id, createdDate));
       
            await _session.ExecuteAsync(batch).ConfigureAwait(false);
        }

        public async Task RemoveFromRoleAsync(CassandraUser user, string roleName)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (roleName == null) throw new ArgumentNullException("roleName");
            var id = user.GenerateIdFromUserData();
            PreparedStatement[] prepared = await _removeFromRoleAsync;
            var batch = new BatchStatement();

            // DELETE FROM user_roles ...
            batch.Add(prepared[0].Bind(id, roleName));

            // DELETE FROM user_roles_by_role ...
            batch.Add(prepared[1].Bind(roleName, id));

            await _session.ExecuteAsync(batch).ConfigureAwait(false);
        }

        public async Task<IList<string>> GetRolesAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            var id = user.GenerateIdFromUserData();
            // SELECT FROM user_roles ...
            PreparedStatement prepared = await _getRolesAsync;
            BoundStatement bound = prepared.Bind(id);
            RowSet rows = await _session.ExecuteAsync(bound).ConfigureAwait(false);
            return rows.Select(r => r.GetValue<string>("rolename")).ToList();
        }

        public async Task<bool> IsInRoleAsync(CassandraUser user, string roleName)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (roleName == null) throw new ArgumentNullException("roleName");
            var id = user.GenerateIdFromUserData();
            // SELECT FROM user_roles ...
            PreparedStatement prepared = await _isInRoleAsync;
            BoundStatement bound = prepared.Bind(id, roleName);
            RowSet rows = await _session.ExecuteAsync(bound).ConfigureAwait(false);
            return rows.Count() > 0;
        }
    }
}