using System;
using Microsoft.AspNet.Identity;
using ProductStore.Core;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class CassandraRole : IRole<Guid>
    {
        private readonly string _originalName;

        public CassandraRole()
        {
            TenantId = Guid.Empty;
            Id = Guid.Empty;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CassandraRole"/> class.
        /// </summary>
        public CassandraRole(Guid tenantId):this()
        {
            TenantId = tenantId;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CassandraRole"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CassandraRole(Guid tenantId,string name):this(tenantId)
        {
            // Track the original name from when the object is created so we can tell if it changes
            _originalName = name;
            Name = name;
        }
 

        private Guid _id;
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id {
            get
            {
                if (Guid.Empty == _id)
                {
                    _id = GuidGenerator.CreateGuid(TenantId, Name);
                }
                return _id;

            } set { _id = value; } }

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
                    TenantId = Guid.Empty; //Set to empty guid if global
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
                _isGlobal = value == Guid.Empty;
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

       
    }
}