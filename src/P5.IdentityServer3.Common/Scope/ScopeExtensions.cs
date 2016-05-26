using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Common
{
    public static class ScopeExtensions
    {
        public static Guid ScopeNameToGuid(this string name)
        {
            return GuidGenerator.CreateGuid(ScopeConstants.Namespace, name);
        }

        public static Guid CreateGuid<TScopeClaims, TScecrets>(this AbstractScopeHandle<TScopeClaims, TScecrets> scope
            )
            where TScopeClaims : class
            where TScecrets : class
        {
            return GuidGenerator.CreateGuid(ScopeConstants.Namespace, scope.Name);
        }

        public static string ToName<TScopeClaims, TScecrets>(this AbstractScopeHandle<TScopeClaims, TScecrets> scope)
            where TScopeClaims : class
            where TScecrets : class
        {
            return scope.Name;
        }
        public static List<string> ToNames<TScopeClaims, TScecrets>(this List<AbstractScopeHandle<TScopeClaims, TScecrets>> scopes)
            where TScopeClaims : class
            where TScecrets : class
        {
            List<string> scopeNames = scopes.Select(scope => scope.Name).ToList();
            return scopeNames;
        }
        public static List<string> ToNames<TScopeClaims, TScecrets>(this IEnumerable<AbstractScopeHandle<TScopeClaims, TScecrets>> scopes)
            where TScopeClaims : class
            where TScecrets : class
        {
            List<string> scopeNames = scopes.Select(scope => scope.Name).ToList();
            return scopeNames;
        }
    }
}