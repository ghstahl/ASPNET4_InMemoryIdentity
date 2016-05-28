using System;
using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Common
{
    public class ScopeComparer : IEqualityComparer<Scope>
    {
        private StringComparison StringComparisonType { get; set; }
        public static ScopeComparer OrdinalIgnoreCase
        {
            get
            {
                return new ScopeComparer(StringComparison.OrdinalIgnoreCase);
            }
        }
        public ScopeComparer(StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            StringComparisonType = stringComparison;
        }
        public bool Equals(Scope x, Scope y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return string.Compare(x.Name, y.Name, StringComparisonType) == 0;
        }

        public int GetHashCode(Scope obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            int hashName = obj.Name == null ? 0 : obj.Name.GetHashCode();

            return hashName ;
        }
    }
}