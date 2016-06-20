using System;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class UserRoleHandle
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; }
        public DateTimeOffset Assigned { get; set; }
    }
}