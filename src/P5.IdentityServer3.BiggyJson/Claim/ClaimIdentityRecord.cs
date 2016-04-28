using System.Collections.Generic;

namespace P5.IdentityServer3.BiggyJson
{
    public class ClaimIdentityRecord
    {
        public string AuthenticationType { get; set; }
        public List<ClaimTypeRecord> ClaimTypeRecords { get; set; }
    }
}