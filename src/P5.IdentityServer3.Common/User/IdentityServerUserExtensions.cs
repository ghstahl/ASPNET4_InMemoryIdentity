using System;

namespace P5.IdentityServer3.Common
{
    public static class IdentityServerUserExtensions
    {
        public static Guid CreateGuid(this IIdentityServerUserHandle userHandle, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, userHandle.UserId);
        }
        public static Guid CreateGuid(this IIdentityServerUserHandle userHandle)
        {
            return GuidGenerator.CreateGuid(IdentityServerUserConstants.Namespace, userHandle.UserId);
        }
        public static IdentityServerUserHandle ToIdentityServerUserHandle(this IdentityServerUser user)
        {
            var handle = new IdentityServerUserHandle(user);
            return handle;
        }

        public static Guid UserIdToGuid(this string userId, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, userId);
        }
        public static Guid UserIdToGuid(this string userId)
        {
            return userId.ClientIdToGuid(IdentityServerUserConstants.Namespace);
        }
    }
}