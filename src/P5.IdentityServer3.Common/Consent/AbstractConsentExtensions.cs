using System;

namespace P5.IdentityServer3.Common
{
    public static class AbstractConsentExtensions
    {
        public static Guid CreateGuid<TScopes>(this AbstractConsentHandle<TScopes> consent, Guid @namespace) where TScopes : class
        {
            return GuidGenerator.CreateGuid(@namespace, consent.ClientId, consent.Subject);
        }
    }
}