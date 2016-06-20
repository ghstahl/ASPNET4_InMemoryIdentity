using System;
using System.Collections.Generic;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class ProviderLoginHandleComparer : IEqualityComparer<ProviderLoginHandle>
    {
        public static ProviderLoginHandleComparer Comparer
        {
            get { return new ProviderLoginHandleComparer(); }
        }

        public bool Equals(ProviderLoginHandle x, ProviderLoginHandle y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return x.LoginProvider == y.LoginProvider &&
                   x.ProviderKey == y.ProviderKey &&
                   x.TenantId == y.TenantId &&
                   x.UserId == y.UserId;
        }

        public int GetHashCode(ProviderLoginHandle obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            int hashLoginProvider = obj.LoginProvider == null ? 0 : obj.LoginProvider.GetHashCode();
            int hashProviderKey = obj.ProviderKey == null ? 0 : obj.ProviderKey.GetHashCode();
            int hashTenantId = obj.TenantId == Guid.Empty ? 0 : obj.TenantId.GetHashCode();
            int hashUserId = obj.UserId == Guid.Empty ? 0 : obj.UserId.GetHashCode();

            return hashLoginProvider ^ hashProviderKey ^ hashTenantId ^ hashUserId;
        }
    }
}