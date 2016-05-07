using System;

namespace P5.IdentityServer3.Common
{
    public static class ConsentExtensions
    {
        public static Guid CreateGuid(ConsentHandle consent, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, consent.ClientId, consent.Subject);
        }
    }
}