using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Cassandra;
using Microsoft.AspNet.Identity;
using P5.Store.Core;

namespace P5.AspNet.Identity.Cassandra
{
    /*
     access_failed_count int,
	created timestamp,
	email text,
	email_confirmed boolean,
	enabled boolean,
	lockout_enabled boolean,
	lockout_end_date timestamp,
	modified timestamp,
	password_hash text,
	phone_number text,
	phone_number_confirmed boolean,
	security_stamp text,
	source text,
	source_id text,
	tenantid uuid,
	two_factor_enabled boolean,
	userid uuid,
	username text,
     */

    /// <summary>
    /// Represents a user.
    /// </summary>
    public class CassandraUser : IUser<Guid>
    {
        public const string IdGuidS = "45d29c4d-9b8b-47ff-baa6-c8ae7ef657d3";
        private static Guid _namespaceGuidGuid;

        public static Guid NamespaceGuid
        {
            get
            {
                if (_namespaceGuidGuid == Guid.Empty)
                {
                    _namespaceGuidGuid = Guid.Parse(IdGuidS);
                }
                return _namespaceGuidGuid;
            }
        }

        private readonly string _originalUserName;
        private readonly string _originalEmail;

        /// <summary>
        /// The unique Id of the user.
        /// </summary>
        private Guid _id;

        public Guid Id
        {
            get
            {
                if (_id == Guid.Empty)
                {
                    throw new Exception("_id cannot be Guid.Empty");
                }
                return _id;
            }
            set { _id = value; }
        }

        public Guid GenerateIdFromUserData()
        {
            return GuidGenerator.CreateGuid(NamespaceGuid, UserName);
        }

        /// <summary>
        /// The tenant the user belongs to
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// The user's username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The password hash for the user.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// The security stamp for the user.
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// Whether or not two factor authentication is enabled for the user.
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// The number of times the user has tried and failed to login.
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// Whether or not lockout is enabled for the user.
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// When the user's lockout period will end.
        /// </summary>
        public DateTimeOffset LockoutEndDate { get; set; }

        /// <summary>
        /// The user's phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Whether the user's phone number has been confirmed.
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// The user's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Whether the user's email address has been confirmed.
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// The date and time the user was created.
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// The date and time the user was last modified.
        /// </summary>
        public DateTimeOffset? Modified { get; set; }

        /// <summary>
        /// Whether the user is enabled or not
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Initial source from which the user was created.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Initial account id of source from which the user was created.
        /// </summary>
        public string SourceId { get; set; }

        public CassandraUser()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Creates a new CassandraUser with the Id specified.
        /// </summary>
        public CassandraUser(Guid userId, Guid tenantId)
            : this(userId, tenantId, null, null)
        {
        }

        public CassandraUser(Guid userId, Guid tenantId, string userName, string email)
            : this()
        {
            Id = userId;
            TenantId = tenantId;
            UserName = userName;
            Email = email;

            // Track the original username and email from when the object is created so we can tell if it changes
            _originalUserName = userName;
            _originalEmail = email;
        }

        /// <summary>
        /// Indicates whether the username for the user has changed from the original username used when the CassandraUser was
        /// created/loaded from C*.  Returns the original username in an out parameter if true.
        /// </summary>
        internal bool HasUserNameChanged(out string originalUserName)
        {
            originalUserName = _originalUserName;
            return UserName != _originalUserName;
        }

        /// <summary>
        /// Indicates whether the email address for the user has changed from the original email used when the CassandraUser was
        /// created/loaded from C*.  Returns the original email in an out parameter if true.
        /// </summary>
        internal bool HasEmailChanged(out string originalEmail)
        {
            originalEmail = _originalEmail;
            return Email != _originalEmail;
        }

      
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<CassandraUser, Guid> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}