using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common
{
    public class TokenHandle : AbstractTokenHandle<List<ClaimTypeRecord>>
    {
        public TokenHandle(string key, Token token)
            : base(key, token)
        {
        }

        public TokenHandle()
            : base()
        {
        }

        public override List<ClaimTypeRecord> Serialize(List<Claim> claims)
        {
            return claims == null ? null : claims.ToClaimTypeRecords();
        }

        public override async Task<List<Claim>> DeserializeClaimsAsync(List<ClaimTypeRecord> obj)
        {
            return await Task.FromResult(obj == null ? null : obj.ToClaims());
        }
    }
}