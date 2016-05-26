using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Common
{
    public class SecretComparer : IEqualityComparer<Secret>
    {

        public bool Equals(Secret x, Secret y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return x.Type == y.Type  &&
                   x.Value == y.Value;
        }

        public int GetHashCode(Secret obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;
            int hashType = obj.Type == null ? 0 : obj.Type.GetHashCode();

            int hashAlwaysIncludeInIdToken = obj.Value == null ? 0 : obj.Value.GetHashCode();
            return hashType ^ hashAlwaysIncludeInIdToken;
        }
    }
}