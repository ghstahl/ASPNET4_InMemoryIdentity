using System.Collections.Generic;
using System.Security.Claims;

namespace P5.IdentityServer3.Common
{
    public class AuthorizationCodeHandle : AbstractAuthorizationCodeHandle<List<ClaimIdentityRecord>, List<string>>
    {
        public AuthorizationCodeHandle() : base()
        {
        }

        public AuthorizationCodeHandle(string key, global::IdentityServer3.Core.Models.AuthorizationCode code)
            : base(key, code)
        {

        }

        public override List<string> SerializeRequestScopes(List<string> scopeNames)
        {
            return scopeNames;
        }

        public override List<ClaimIdentityRecord> SerializeClaimsIdentityRecords(
            List<ClaimIdentityRecord> toClaimIdentityRecords)
        {
            return toClaimIdentityRecords;
        }

        public override ClaimsPrincipal DeserializeSubject(List<ClaimIdentityRecord> claimIdentityRecords)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentities(claimIdentityRecords.ToClaimsIdentitys());
            return claimsPrincipal;
        }

        public override IEnumerable<string> DeserializeScopes(List<string> requestedScopes)
        {
            return requestedScopes;
        }
    }
}