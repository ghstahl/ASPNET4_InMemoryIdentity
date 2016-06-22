namespace P5.AspNet.Identity.Cassandra.DAO
{
    public static class CassandraUserExtensions
    {
        public static CassandraUserHandle ToHandle(this CassandraUser user)
        {
            return new CassandraUserHandle
            {
                
                AccessFailedCount = user.AccessFailedCount,
                Created = user.Created,
                Email = user.Email,
                EmailConfirmed = user.IsEmailConfirmed,
                Enabled = user.Enabled,
                LockoutEnabled = user.IsLockoutEnabled,
                LockoutEndDate = user.LockoutEndDate,
                Modified = user.Modified,
                PasswordHash = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.IsPhoneNumberConfirmed,
                SecurityStamp = user.SecurityStamp,
                Source = user.Source,
                SourceId = user.SourceId,
                TenantId = user.TenantId,
                TwoFactorEnabled = user.IsTwoFactorEnabled,
                UserId = user.Id,
                UserName = user.UserName
            };
        }
        public static CassandraUser ToUser(this CassandraUserHandle user)
        {
            return new CassandraUser
            {
                AccessFailedCount = user.AccessFailedCount,
                Created = user.Created,
                Email = user.Email,
                IsEmailConfirmed = user.EmailConfirmed,
                Enabled = user.Enabled,
                IsLockoutEnabled = user.LockoutEnabled,
                LockoutEndDate = user.LockoutEndDate,
                Modified = user.Modified,
                PasswordHash = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                IsPhoneNumberConfirmed = user.PhoneNumberConfirmed,
                SecurityStamp = user.SecurityStamp,
                Source = user.Source,
                SourceId = user.SourceId,
                TenantId = user.TenantId,
                IsTwoFactorEnabled = user.TwoFactorEnabled,
                Id = user.UserId,
                UserName = user.UserName
            };
        }
    }
}