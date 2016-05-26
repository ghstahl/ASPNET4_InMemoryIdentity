using System;

namespace P5.IdentityServer3.Common
{
    public static class AbstractAuthorizationCodeHandleExtensions
    {
        public static Guid CreateGuid<TClaimIdentityRecords, TRequestedScopes>(this AbstractAuthorizationCodeHandle<TClaimIdentityRecords, TRequestedScopes> codeHandle)
            where TClaimIdentityRecords : class
            where TRequestedScopes : class
        {
            return GuidGenerator.CreateGuid(AuthorizationCodeConstants.Namespace, codeHandle.Key);
        }
    }
}