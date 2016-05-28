using System.Collections.Generic;
using System.Security.Claims;


namespace P5.IdentityServer3.Common
{
    public class ClaimComparer : IEqualityComparer<Claim>
    {
        public static ClaimComparer DeepComparer
        {
            get { return new ClaimComparer(true); }
        }

        public static ClaimComparer MinimalComparer
        {
            get { return new ClaimComparer(); }
        }

        private bool _deep;

        public ClaimComparer(bool deep = false)
        {
            _deep = deep;
        }
        public bool Equals(Claim x, Claim y)
        {
            if (_deep)
            {
                return DeepEquals(x, y);
            }
            else
            {
                return MinimalEquals(x, y);
            }
        }
        public int GetHashCode(Claim obj)
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
        public bool MinimalEquals(Claim x, Claim y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return x.Type == y.Type;
        }

        public bool DeepEquals(Claim x, Claim y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return x.Type == y.Type &&
                   x.Value == y.Value;
        }
        public int GetDeepHashCode(Claim obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            int hashType = obj.Type == null ? 0 : obj.Type.GetHashCode();
            int hashValue = obj.Value == null ? 0 : obj.Value.GetHashCode();

            return hashType ^ hashValue;
        }

        public int GetMinimalHashCode(Claim obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            int hashType = obj.Type == null ? 0 : obj.Type.GetHashCode();

            return hashType;
        }



    }
}