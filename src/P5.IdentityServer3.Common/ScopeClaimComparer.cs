using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Common
{
    public class ScopeClaimComparer : IEqualityComparer<ScopeClaim>
    {
        public bool Equals(ScopeClaim x, ScopeClaim y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) ||ReferenceEquals(y, null))
                return false;
            return x.Name == y.Name;
        }

        public int GetHashCode(ScopeClaim obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            int hashName = obj.Name == null? 0 : obj.Name.GetHashCode();

            return hashName ;
        }
    }
}
