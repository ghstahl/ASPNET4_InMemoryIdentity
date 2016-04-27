using System;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.BiggyJson
{
    static class ScopeExtensions
    {
        public static Guid CreateGuid(this Scope scope, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, scope.Name);
        }
    }
}