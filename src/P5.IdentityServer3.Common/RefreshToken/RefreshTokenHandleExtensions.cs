using System;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common.RefreshToken
{
    public static class RefreshTokenHandleExtensions
    {
        public static Guid CreateGuid(this IRefreshTokenHandle tokenHandle)
        {
            return GuidGenerator.CreateGuid(RefreshTokenConstants.Namespace, tokenHandle.Key);
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

    }
}