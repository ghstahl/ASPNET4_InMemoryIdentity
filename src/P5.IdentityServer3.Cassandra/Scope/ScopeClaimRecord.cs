using System;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Cassandra
{
    public class ScopeClaimRecord : ScopeClaim
    {
        public ScopeClaimRecord(Guid scopeId, string scopeName, ScopeClaim scopeClaim)
        {
            ScopeId = scopeId;
            ScopeName = scopeName;
            AlwaysIncludeInIdToken = scopeClaim.AlwaysIncludeInIdToken;
            Description = scopeClaim.Description;
            Name = scopeClaim.Name;
        }

        public Guid ScopeId { get; set; }
        public string ScopeName { get; set; }
        public bool AlwaysIncludeInIdToken { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}