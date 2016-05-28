using System;
using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Common
{
    public class SecretComparer : IEqualityComparer<Secret>
    {
        private StringComparison StringComparisonType { get; set; }
        public static SecretComparer OrdinalIgnoreCase
        {
            get
            {
                return new SecretComparer(StringComparison.OrdinalIgnoreCase);
            }
        }

        public SecretComparer(StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            StringComparisonType = stringComparison;
        }
        public bool Equals(Secret x, Secret y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return string.Compare(x.Type, y.Type, StringComparisonType) == 0 &&
                   string.Compare(x.Value, y.Value, StringComparisonType) == 0;
        }

        public int GetHashCode(Secret obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;
            int hashType = obj.Type == null ? 0 : obj.Type.GetHashCode();
            int hashValue = obj.Value == null ? 0 : obj.Value.GetHashCode();
            return hashType ^ hashValue;
        }
    }
}