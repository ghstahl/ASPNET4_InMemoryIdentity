using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Common
{
    public class ScopeHandle : AbstractScopeHandle<List<ScopeClaim>, List<Secret>>
    {
        public ScopeHandle(global::IdentityServer3.Core.Models.Scope scope = null) : base(scope)
        {
        }

        public override async Task<List<ScopeClaim>> DeserializeClaimsAsync(List<ScopeClaim> claims)
        {
            return await Task.FromResult(claims);
        }

        public override async Task<List<Secret>> DeserializeSecretsAsync(List<Secret> scopeSecrets)
        {
            return await Task.FromResult(scopeSecrets);
        }

        public override List<Secret> Serialize(List<Secret> scopeSecrets)
        {
            return scopeSecrets;
        }

        public override List<ScopeClaim> Serialize(List<ScopeClaim> claims)
        {
            return claims;
        }

    }
}