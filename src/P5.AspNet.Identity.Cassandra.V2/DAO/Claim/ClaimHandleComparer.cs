using System;
using System.Collections.Generic;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class ClaimHandleComparer : IEqualityComparer<ClaimHandle>
    {
        public static ClaimHandleComparer Comparer
        {
            get { return new ClaimHandleComparer(); }
        }

        public bool Equals(ClaimHandle x, ClaimHandle y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return x.Type == y.Type &&
                   x.Value == y.Value &&
                   x.UserId == y.UserId;
        }

        public int GetHashCode(ClaimHandle obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            int hashType = obj.Type == null ? 0 : obj.Type.GetHashCode();
            int hashValue = obj.Value == null ? 0 : obj.Value.GetHashCode();
            int hashUserId = obj.UserId == Guid.Empty ? 0 : obj.UserId.GetHashCode();

            return hashType ^ hashValue ^ hashUserId;
        }
    }
}