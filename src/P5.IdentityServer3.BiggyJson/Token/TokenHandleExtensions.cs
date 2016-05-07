using System;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Common;



namespace P5.IdentityServer3.BiggyJson
{
    public static class TokenHandleExtensions
    {
        public static Token ToToken3(this TokenHandle tokenHandle, IClientStore store)
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


    }
}