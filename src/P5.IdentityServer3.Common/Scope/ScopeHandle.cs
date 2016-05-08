using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Common
{
    public class ScopeHandle : AbstractScopeHandle<List<ScopeClaim>, List<Secret>>
    {
        public ScopeHandle(global::IdentityServer3.Core.Models.Scope scope = null) : base(scope) { }
        protected override List<Secret> DeserializeSecretes(List<Secret> scopeSecrets)
        {
            return scopeSecrets;
        }
        protected override List<Secret> Serialize(List<Secret> scopeSecrets)
        {
            return scopeSecrets;
        }

        protected override List<ScopeClaim> DeserializeClaims(List<ScopeClaim> claims)
        {
            return claims;
        }

        protected override List<ScopeClaim> Serialize(List<ScopeClaim> claims)
        {
            return claims;
        }

    }
}