using System;
using Cassandra.Mapping;

namespace P5.IdentityServer3.Cassandra
{
    public class IdentityServerStoreAppliedInfo
    {
        public IdentityServerStoreAppliedInfo()
        {
            Applied = true;
        }
        public IdentityServerStoreAppliedInfo(bool applied, Exception ex = null)
        {
            Applied = applied;
            Exception = ex;
        }
        public bool Applied { get; set; }
        public Exception Exception { get; set; }

    }
}