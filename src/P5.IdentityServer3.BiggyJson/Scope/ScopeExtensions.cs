using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Resources;

namespace P5.IdentityServer3.BiggyJson
{
    static class ScopeExtensions
    {
        public static Guid CreateGuid(this Scope scope, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, scope.Name);
        }
        public static string ToName(this Scope scope)
        {
            return scope.Name;
        }
        public static List<string> ToNames(this List<Scope> scopes)
        {
            List<string> scopeNames = scopes.Select(scope => scope.Name).ToList();
            return scopeNames;
        }
        public static List<string> ToNames(this IEnumerable<Scope> scopes)
        {
            List<string> scopeNames = scopes.Select(scope => scope.Name).ToList();
            return scopeNames;
        }
    }
}