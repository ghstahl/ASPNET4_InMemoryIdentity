using System;

namespace P5.AspNet.Identity.Cassandra
{
    public class ClaimHandle
    {

        public Guid UserId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}