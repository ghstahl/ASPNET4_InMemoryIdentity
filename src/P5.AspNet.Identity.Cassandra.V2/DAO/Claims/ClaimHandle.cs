using System;
using System.Security.Claims;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class ClaimHandle
    {
        public Guid UserId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}