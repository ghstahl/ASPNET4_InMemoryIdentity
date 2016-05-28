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
        public static ScopeClaimComparer DeepScopeClaimComparer
        {
            get { return new ScopeClaimComparer(true); }
        }

        public static ScopeClaimComparer MinimalScopeClaimComparer
        {
            get { return new ScopeClaimComparer(); }
        }

        private bool _deep;

        public ScopeClaimComparer(bool deep = false)
        {
            _deep = deep;
        }
        public bool Equals(ScopeClaim x, ScopeClaim y)
        {
            if (_deep)
            {
                return DeepEquals(x,y);
            }
            else
            {
                return MinimalEquals(x, y);
            }
        }
        public int GetHashCode(ScopeClaim obj)
        {
            if (_deep)
            {
                return GetDeepHashCode(obj);
            }
            else
            {
                return GetMinimalHashCode(obj);
            }
        }
        public bool MinimalEquals(ScopeClaim x, ScopeClaim y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return x.Name == y.Name;
        }

        public bool DeepEquals(ScopeClaim x, ScopeClaim y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return x.AlwaysIncludeInIdToken == y.AlwaysIncludeInIdToken &&
                   x.Description == y.Description &&
                   x.Name == y.Name;
        }
        public int GetDeepHashCode(ScopeClaim obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            int hashName = obj.Name == null ? 0 : obj.Name.GetHashCode();
            int hashDescription = obj.Description == null ? 0 : obj.Description.GetHashCode();
            int hashAlwaysIncludeInIdToken = obj.AlwaysIncludeInIdToken.GetHashCode();

            return hashName ^ hashDescription ^ hashAlwaysIncludeInIdToken;
        }

        public int GetMinimalHashCode(ScopeClaim obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            int hashName = obj.Name == null? 0 : obj.Name.GetHashCode();

            return hashName ;
        }
    }
}
