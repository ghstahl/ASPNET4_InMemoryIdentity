using System;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common.RefreshToken
{
    public static class RefreshTokenHandleExtensions
    {
        public static Guid CreateGuid(this IRefreshTokenHandle tokenHandle, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, tokenHandle.Key);
        }
        public static RefreshTokenHandle ToRefeshTokenHandle(this global::IdentityServer3.Core.Models.RefreshToken token)
        {
            var handle = new RefreshTokenHandle("", token);
            return handle;
        }
        public static FlattenedRefreshTokenHandle ToFlattenedRefreshTokenHandle(this global::IdentityServer3.Core.Models.RefreshToken token)
        {
            var handle = new FlattenedRefreshTokenHandle("", token);
            return handle;
        }
        public static global::IdentityServer3.Core.Models.RefreshToken ToRefreshToken(this IRefreshTokenHandle handle, IClientStore store)
        {
            var token = new global::IdentityServer3.Core.Models.RefreshToken()
            {
                AccessToken = handle.MakeAccessToken(store),
                CreationTime = handle.CreationTime,
                LifeTime = handle.LifeTime,
                Version = handle.Version

            };
            return token;

        }
    }
}