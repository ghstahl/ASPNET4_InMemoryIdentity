using System;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Stores;


namespace P5.IdentityServer3.BiggyJson
{
    static class TokenHandleExtensions
    {
        public static Guid CreateGuid(this TokenHandle tokenHandle, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, tokenHandle.Key);
        }

        public static Token ToToken(this TokenHandle tokenHandle, IClientStore store)
        {
            var token = new Token(tokenHandle.Type)
            {
                Audience = tokenHandle.Audience,
                Claims = tokenHandle.Claims.ToClaims(),
                Client = store.FindClientByIdAsync(tokenHandle.ClientId).Result,
                CreationTime = tokenHandle.CreationTime,
                Issuer = tokenHandle.Issuer,
                Lifetime = tokenHandle.Lifetime,
                Type = tokenHandle.Type,
                Version = tokenHandle.Version
               
            };
            return token;

        }
        public static TokenHandle ToTokenHandle(this Token token)
        {
            var tokenHandle = new TokenHandle("",token);
            return tokenHandle;
        }
    }
}