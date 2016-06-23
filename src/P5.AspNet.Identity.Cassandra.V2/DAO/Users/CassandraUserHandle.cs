using System;
using System.CodeDom;
using P5.Store.Core;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class CassandraUserHandle
    {

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
                    throw new Exception("_userid cannot be Guid.Empty");
                }
                return _userId;
            }
            set { _userId = value; }
        }

        public string UserName { get; set; }
    }
}