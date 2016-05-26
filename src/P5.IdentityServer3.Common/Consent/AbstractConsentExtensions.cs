using System;

namespace P5.IdentityServer3.Common
{
    public static class AbstractConsentExtensions
    {
        public static Guid CreateGuid<TScopes>(this AbstractConsentHandle<TScopes> consent) where TScopes : class
        {
            return GuidGenerator.CreateGuid(ConsentConstants.Namespace, consent.ClientId, consent.Subject);
        }
    }
}