using System;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public class IdentityServerUserAllowedScope
    {
        public string UserId { get; set; }
        public string ScopeName { get; set; }
    }
}