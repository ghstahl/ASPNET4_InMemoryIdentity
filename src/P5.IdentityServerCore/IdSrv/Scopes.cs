using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace P5.IdentityServerCore.IdSrv

{
    public static class Scopes
    {
        public static List<Scope> Get()
        {
            return new List<Scope>
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.Email,
                StandardScopes.Address,
                StandardScopes.OfflineAccess,
                StandardScopes.RolesAlwaysInclude,
                StandardScopes.AllClaims,
                new Scope
                {
                    Name = "api1"
                },
                new Scope
                {
                    Name = "read",
                    DisplayName = "Read data",
                    Type = ScopeType.Resource,
                    Emphasize = false
                },
                 new Scope
                {
                    Name = "WebApi1",
                    Type = ScopeType.Resource,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("name", true)
                    }
                },
                new Scope
                {
                    Name = "WebApi2",
                    Type = ScopeType.Resource,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("name", true)
                    }
                }
            };
        }
    }
}