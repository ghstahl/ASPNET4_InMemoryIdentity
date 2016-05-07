using System;

namespace P5.IdentityServer3.Common
{
    public static class StringExtensions
    {
        public static Guid CreateGuid(this string key, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, key);
        }
    }
}