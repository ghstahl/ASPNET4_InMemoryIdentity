using System;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class ProviderLoginHandle
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
 
    }
}
