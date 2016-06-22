using System;
using P5.Store.Core;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class CassandraUserHandle
    {
        public Guid GenerateIdFromUserData()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                throw new NullReferenceException("UserName cannot be null or empty.  Cannot generate userId.");
            }
            return GuidGenerator.CreateGuid(CassandraUser.NamespaceGuid, UserName);
        }

        public int AccessFailedCount { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool Enabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset LockoutEndDate { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string SecurityStamp { get; set; }
        public string Source { get; set; }
        public string SourceId { get; set; }
        public Guid TenantId { get; set; }
        public bool TwoFactorEnabled { get; set; }
        private Guid _userId;

        public Guid UserId
        {
            get
            {
                if (_userId == Guid.Empty)
                {
                    _userId = GenerateIdFromUserData();
                }
                return _userId;
            }
            set { _userId = value; }
        }

        public string UserName { get; set; }
    }
}