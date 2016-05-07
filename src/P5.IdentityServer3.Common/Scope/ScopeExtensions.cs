using System;
using System.Collections.Generic;
using System.Linq;

namespace P5.IdentityServer3.Common
{
    public static class ScopeExtensions
    {
        public static Guid CreateGuid(this global::IdentityServer3.Core.Models.Scope scope, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, scope.Name);
        }
        public static string ToName(this global::IdentityServer3.Core.Models.Scope scope)
        {
            return scope.Name;
        }
        public static List<string> ToNames(this List<global::IdentityServer3.Core.Models.Scope> scopes)
        {
            List<string> scopeNames = scopes.Select(scope => scope.Name).ToList();
            return scopeNames;
        }
        public static List<string> ToNames(this IEnumerable<global::IdentityServer3.Core.Models.Scope> scopes)
        {
            List<string> scopeNames = scopes.Select(scope => scope.Name).ToList();
            return scopeNames;
        }
    }
}