using System;
using System.Security.Claims;
using Newtonsoft.Json;

namespace P5.AspNet.Identity.Biggy
{
    public class UserClaim
    {
        public UserClaim(Claim claim)
        {
            if (claim == null) throw new ArgumentNullException("claim");

            Type = claim.Type;
            Value = claim.Value;
        }

        [JsonConstructor]
        public UserClaim(string claimType, string claimValue)
        {
            if (claimType == null) throw new ArgumentNullException("claimType");
            if (claimValue == null) throw new ArgumentNullException("claimValue");

            Type = claimType;
            Value = claimValue;
        }

        public string Type { get; private set; }
        public string Value { get; private set; }
    }
}