using System;
using Cassandra;
using Microsoft.AspNet.Identity;

namespace P5.AspNet.Identity.Cassandra
{
    public class CassandraRole : IRole<Guid>
    {
        private readonly string _originalName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CassandraRole"/> class.
        /// </summary>
        public CassandraRole()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CassandraRole"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CassandraRole(string name)
        {
            // Track the original name from when the object is created so we can tell if it changes
            _originalName = name;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CassandraRole"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        public CassandraRole(string name, Guid id)
            : this(name)
        {
            Id = id;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }


        /// <summary>
        /// Gets or sets the display name of the role
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }


        /// <summary>
        /// Gets or sets IsSystemRole
        /// </summary>
        /// <value>
        /// Value determining if the role should be treated as a system role
        /// </value>
        public bool IsSystemRole { get; set; }

        private bool _isGlobal;

        public bool IsGlobal
        {
            get
            {
                return _isGlobal;
            }
            set
            {
                _isGlobal = value;

                if (value)
                {
                    TenantId = new Guid(); //Set to empty guid if global
                }
            }
        }

        private Guid _tenantId;

        public Guid TenantId
        {
            get
            {
                return _tenantId;
            }
            set
            {
                _tenantId = value;
                _isGlobal = value == new Guid();
            }
        }

        /// <summary>
        /// The date and time the user was created.
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// The date and time the user was last modified.
        /// </summary>
        public DateTimeOffset? Modified { get; set; }

        /// <summary>
        /// Indicates whether the name of the role has changed from the original name used when the CassandraRole was
        /// created/loaded from C*.  Returns the original name in an out parameter if true.
        /// </summary>
        internal bool HasNameChanged(out string originalName)
        {
            originalName = _originalName;
            return Name != _originalName;
        }

        /// <summary>
        /// Creates a CassandraUser from a Row.  If the Row is null, returns null.
        /// </summary>
        internal static CassandraRole FromRow(Row row)
        {
            if (row == null) return null;

            var role = new CassandraRole(row.GetValue<string>("name"), row.GetValue<Guid>("roleid"))
            {
                DisplayName = row.GetValue<string>("displayname"),
                IsSystemRole = row.GetValue<bool>("is_systemrole"),
                IsGlobal = row.GetValue<bool>("is_global"),
                TenantId = row.GetValue<Guid>("tenantid"),
                Created = row.GetValue<DateTimeOffset>("created"),
                Modified = row.IsNull("modified") ? new DateTimeOffset?() : row.GetValue<DateTimeOffset>("modified"),
            };

            return role;
        }
    }
}