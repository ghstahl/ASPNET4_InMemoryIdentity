using System.Collections.Generic;
using System.Security.Claims;

namespace P5.IdentityServer3.Common
{
    public class AuthorizationCodeHandle : AbstractAuthorizationCodeHandle<List<ClaimIdentityRecord>,List<string>>
    {
        protected override List<string> SerializeRequestScopes(List<string> scopeNames)
        {
            return scopeNames;
        }

        protected override List<ClaimIdentityRecord> SerializeClaimsIdentityRecords(List<ClaimIdentityRecord> toClaimIdentityRecords)
        {
            return toClaimIdentityRecords;
        }

        protected override ClaimsPrincipal DeserializeSubject(List<ClaimIdentityRecord> claimIdentityRecords)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentities(claimIdentityRecords.ToClaimsIdentitys());
            return claimsPrincipal;
        }

        protected override IEnumerable<string> DeserializeScopes(List<string> requestedScopes)
        {
            return requestedScopes;
        }
    }
}