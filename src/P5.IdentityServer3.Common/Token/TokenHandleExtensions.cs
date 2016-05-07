using System;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common
{
    public static class TokenHandleExtensions
    {
        public static Guid CreateGuid(this ITokenHandle tokenHandle, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, tokenHandle.Key);
        }
        public static TokenHandle ToTokenHandle(this Token token)
        {
            var tokenHandle = new TokenHandle("", token);
            return tokenHandle;
        }
        public static FlattenedTokenHandle ToFlattenedTokenHandle(this Token token)
        {
            var tokenHandle = new FlattenedTokenHandle("", token);
            return tokenHandle;
        }

    }
}