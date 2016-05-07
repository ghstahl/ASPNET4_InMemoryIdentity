using System;
using System.Collections.Generic;
using System.Security.Claims;
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

        public override List<Claim> DeserializeClaims(List<ClaimTypeRecord> obj)
        {
            return obj == null ? null : obj.ToClaims();
        }
    }
}