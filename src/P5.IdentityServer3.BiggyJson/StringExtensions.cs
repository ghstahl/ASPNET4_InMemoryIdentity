using System;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.BiggyJson
{
    static class StringExtensions
    {
        public static Guid CreateGuid(this string key, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, key);
        }
    }
}