using System.Collections.Generic;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class CassandraRoleComparer : IEqualityComparer<CassandraRole>
    {


        public static CassandraRoleComparer Comparer
        {
            get { return new CassandraRoleComparer(); }
        }

        private bool _deep;

        public CassandraRoleComparer(bool deep = false)
        {
            _deep = deep;
        }

        public bool Equals(CassandraRole x, CassandraRole y)
        {

            return DeepEquals(x, y);

        }

        public int GetHashCode(CassandraRole obj)
        {

            return GetDeepHashCode(obj);

        }


        public bool DeepEquals(CassandraRole x, CassandraRole y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return x.Name == y.Name &&
                   x.IsSystemRole == y.IsSystemRole &&
                   x.IsGlobal == y.IsGlobal &&
                   x.TenantId == y.TenantId &&
                   x.DisplayName == y.DisplayName;
        }

        public int GetDeepHashCode(CassandraRole obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            int hashType = obj.Name == null ? 0 : obj.Name.GetHashCode();
            int hashIsSystemRole = obj.IsSystemRole.GetHashCode();
            int hashIsGlobal = obj.IsGlobal.GetHashCode();
            int hashTenantId = obj.TenantId.GetHashCode();
            int hashDisplayName = obj.DisplayName == null ? 0 : obj.DisplayName.GetHashCode();
            return hashType ^ hashIsSystemRole ^ hashIsGlobal ^ hashTenantId ^ hashDisplayName;
        }

    }
}