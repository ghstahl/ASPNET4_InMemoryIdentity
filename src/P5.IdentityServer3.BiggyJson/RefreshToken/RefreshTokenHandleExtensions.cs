using System;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Stores;


namespace P5.IdentityServer3.BiggyJson
{
    static class RefreshTokenHandleExtensions
    {
        public static Guid CreateGuid(this RefreshTokenHandle handle, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, handle.Key);
        }
        public static RefreshToken ToToken(this RefreshTokenHandle handle, IClientStore store)
        {
            var token = new RefreshToken()
            {
                AccessToken = handle.AccessToken == null ? null : handle.AccessToken.ToToken(store),
                CreationTime = handle.CreationTime,
                LifeTime = handle.LifeTime,
                Version = handle.Version

            };
            return token;

        }
    }
}