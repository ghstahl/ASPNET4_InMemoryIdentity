using System.Security.Claims;

namespace P5.IdentityServer3.Common
{
    public class ClaimTypeRecord
    {
        public ClaimTypeRecord()
        {

        }
        public ClaimTypeRecord(Claim claim)
        {
            Type = claim.Type;
            Value = claim.Value;
            ValueType = claim.ValueType;
        }
        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
    }
}
