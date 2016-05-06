using System;

namespace P5.IdentityServer3.Stores
{
    public static class StringExtensions
    {
        public static Guid CreateGuid(this string key, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, key);
        }
    }
}