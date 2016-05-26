using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Common
{
    public class ScopeComparer : IEqualityComparer<Scope>
    {
        public bool Equals(Scope x, Scope y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return x.Name == y.Name;
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