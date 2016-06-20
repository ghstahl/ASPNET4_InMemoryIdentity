using System;
using System.Collections.Generic;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class UserRoleHandleComparer : IEqualityComparer<UserRoleHandle>
    {
        public static UserRoleHandleComparer Comparer
        {
            get { return new UserRoleHandleComparer(); }
        }

        public bool Equals(UserRoleHandle x, UserRoleHandle y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return x.Assigned == y.Assigned &&
                   x.RoleName == y.RoleName &&
                   x.UserId == y.UserId;
        }

        public int GetHashCode(UserRoleHandle obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            int hashType =  obj.Assigned.GetHashCode();
            int hashValue = obj.RoleName == null ? 0 : obj.RoleName.GetHashCode();
            int hashUserId = obj.UserId == Guid.Empty ? 0 : obj.UserId.GetHashCode();

            return hashType ^ hashValue ^ hashUserId;
        }
    }
}