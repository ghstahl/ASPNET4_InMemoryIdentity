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
        public static IdentityServerUserModel ToIdentityServerUserModel(this IdentityServerUser user)
        {
            var handle = new IdentityServerUserModel()
            {
                Enabled = user.Enabled,
                UserId = user.UserId,
                UserName = user.UserName
            };
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