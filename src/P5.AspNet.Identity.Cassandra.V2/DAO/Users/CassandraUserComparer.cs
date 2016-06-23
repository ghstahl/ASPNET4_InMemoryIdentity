using System;
using System.Collections.Generic;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class CassandraUserComparer : IEqualityComparer<CassandraUser>
    {
        public static CassandraUserComparer ShallowComparer
        {
            get { return new CassandraUserComparer(); }
        }
       public static CassandraUserComparer DeepComparer
       {
           get { return new CassandraUserComparer(true); }
       }
        private bool _deep;

        public CassandraUserComparer(bool deep = false)
        {
            _deep = deep;
        }

        public bool Equals(CassandraUser x, CassandraUser y)
        {
            if (_deep)
            {
                return DeepEquals(x, y);
            }
            return ShallowEquals(x, y);
        }

        public int GetHashCode(CassandraUser obj)
        {

            if (_deep)
            {
                return GetDeepHashCode(obj);
            }
            return GetShallowHashCode(obj);

        }


        public bool ShallowEquals(CassandraUser x, CassandraUser y)
        {
           
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.AccessFailedCount == y.AccessFailedCount &&
                   x.Email == y.Email &&
                   x.EmailConfirmed == y.EmailConfirmed &&
                   x.Enabled == y.Enabled &&
                   x.LockoutEnabled == y.LockoutEnabled &&
                   x.PasswordHash == y.PasswordHash &&
                   x.PhoneNumber == y.PhoneNumber &&
                   x.PhoneNumberConfirmed == y.PhoneNumberConfirmed &&
                   x.SecurityStamp == y.SecurityStamp &&
                   x.Source == y.Source &&
                   x.SourceId == y.SourceId &&
                   x.TenantId == y.TenantId &&
                   x.TwoFactorEnabled == y.TwoFactorEnabled &&
                   x.Id == y.Id &&
                   x.UserName == y.UserName;
        }
        public bool DeepEquals(CassandraUser x, CassandraUser y)
        {

            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            var modifiedSame = false;
            if (x.Modified == null && y.Modified == null)
            {
                modifiedSame = true;
            }
            if (!modifiedSame && (x.Modified != null && y.Modified != null))
            {
                modifiedSame = DateTimeOffset.Compare(x.Modified.Value, y.Modified.Value) == 0;
            }

            return x.AccessFailedCount == y.AccessFailedCount &&
                   DateTimeOffset.Compare(x.Created, y.Created) == 0 &&
                   x.Email == y.Email &&
                   x.EmailConfirmed == y.EmailConfirmed &&
                   x.Enabled == y.Enabled &&
                   x.LockoutEnabled == y.LockoutEnabled &&
                   DateTimeOffset.Compare(x.LockoutEndDate, y.LockoutEndDate) == 0 &&
                   modifiedSame &&
                   x.PasswordHash == y.PasswordHash &&
                   x.PhoneNumber == y.PhoneNumber &&
                   x.PhoneNumberConfirmed == y.PhoneNumberConfirmed &&
                   x.SecurityStamp == y.SecurityStamp &&
                   x.Source == y.Source &&
                   x.SourceId == y.SourceId &&
                   x.TenantId == y.TenantId &&
                   x.TwoFactorEnabled == y.TwoFactorEnabled &&
                   x.Id == y.Id &&
                   x.UserName == y.UserName;
        }
        
        public int GetShallowHashCode(CassandraUser obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            int a = obj.AccessFailedCount.GetHashCode();
            int b = 0;
            int c = obj.Email == null ? 0 : obj.Email.GetHashCode();
            int d = obj.EmailConfirmed.GetHashCode();
            int e = obj.Enabled.GetHashCode();
            int f = obj.LockoutEnabled.GetHashCode();
            int g = 0;
            int h = 0;
            int i = obj.PasswordHash == null ? 0 : obj.PasswordHash.GetHashCode();
            int j = obj.PhoneNumber == null ? 0 : obj.PhoneNumber.GetHashCode();
            int k = obj.PhoneNumberConfirmed.GetHashCode();
            int l = obj.SecurityStamp == null ? 0 : obj.SecurityStamp.GetHashCode();
            int m = obj.Source == null ? 0 : obj.Source.GetHashCode();
            int n = obj.SourceId == null ? 0 : obj.SourceId.GetHashCode();
            int o = obj.TenantId.GetHashCode();
            int p = obj.TwoFactorEnabled.GetHashCode();
            int q = obj.Id.GetHashCode();
            int r = obj.UserName.GetHashCode();

            return  a ^ b ^ c ^ d ^ e ^ 
                    f ^ g ^ h ^ i ^ j ^ 
                    k ^ l ^ m ^ n ^ o ^ 
                    p ^ q ^ r ;
        }
       public int GetDeepHashCode(CassandraUser obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            int a = obj.AccessFailedCount.GetHashCode();
            int b = obj.Created.GetHashCode();
            int c = obj.Email == null ? 0 : obj.Email.GetHashCode();
            int d = obj.EmailConfirmed.GetHashCode();
            int e = obj.Enabled.GetHashCode();
            int f = obj.LockoutEnabled.GetHashCode();
            int g = obj.LockoutEndDate.GetHashCode();
            int h = obj.Modified == null ? 0 : obj.Modified.GetHashCode();
            int i = obj.PasswordHash == null ? 0 : obj.PasswordHash.GetHashCode();
            int j = obj.PhoneNumber == null ? 0 : obj.PhoneNumber.GetHashCode();
            int k = obj.PhoneNumberConfirmed.GetHashCode();
            int l = obj.SecurityStamp == null ? 0 : obj.SecurityStamp.GetHashCode();
            int m = obj.Source == null ? 0 : obj.Source.GetHashCode();
            int n = obj.SourceId == null ? 0 : obj.SourceId.GetHashCode();
            int o = obj.TenantId.GetHashCode();
            int p = obj.TwoFactorEnabled.GetHashCode();
            int q = obj.Id.GetHashCode();
            int r = obj.UserName.GetHashCode();

            return  a ^ b ^ c ^ d ^ e ^ 
                    f ^ g ^ h ^ i ^ j ^ 
                    k ^ l ^ m ^ n ^ o ^ 
                    p ^ q ^ r ;
        }

    }
}