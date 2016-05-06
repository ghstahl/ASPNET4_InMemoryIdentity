using System;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Stores;


namespace P5.IdentityServer3.BiggyJson
{
    static class ConsentExtensions
    {
        public static Guid CreateGuid(this Consent consent, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, consent.ClientId, consent.Subject);
        }
    }
}