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
                }
            };
        }
    }
}