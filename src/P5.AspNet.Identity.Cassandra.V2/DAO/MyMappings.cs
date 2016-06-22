using Cassandra.Mapping;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class MyMappings : Mappings
    {


        private static bool _init { get; set; }

        public static void Init()
        {
            if (!_init)
            {
                MappingConfiguration.Global.Define<MyMappings>();
                _init = true;
            }

        }

        public MyMappings()
        {
            // Define mappings in the constructor of your class
            // that inherits from Mappings

            For<CassandraUserHandle>()
                .TableName("users")
                .TableName("users_by_email")
                .TableName("users_by_username")
                .Column(u => u.AccessFailedCount, cm => cm.WithName("access_failed_count"))
                .Column(u => u.EmailConfirmed, cm => cm.WithName("email_confirmed"))
                .Column(u => u.LockoutEnabled, cm => cm.WithName("lockout_enabled"))
                .Column(u => u.LockoutEndDate, cm => cm.WithName("lockout_end_date"))
                .Column(u => u.PasswordHash, cm => cm.WithName("password_hash"))
                .Column(u => u.PhoneNumber, cm => cm.WithName("phone_number"))
                .Column(u => u.PhoneNumberConfirmed, cm => cm.WithName("phone_number_confirmed"))
                .Column(u => u.SecurityStamp, cm => cm.WithName("security_stamp"))
                .Column(u => u.SourceId, cm => cm.WithName("source_id"))
                .Column(u => u.TwoFactorEnabled, cm => cm.WithName("two_factor_enabled"));

            For<CassandraRole>()
                .TableName("roles_by_name")
                .TableName("roles")
                .Column(u => u.IsGlobal, cm => cm.WithName("is_global"))
                .Column(u => u.IsSystemRole, cm => cm.WithName("is_systemrole"));

            For<ClaimHandle>()
                .TableName("claims");

            For<UserRoleHandle>()
                .TableName("user_roles_by_role")
                .TableName("user_roles");

            For<ProviderLoginHandle>()
                .TableName("logins")
                .TableName("logins_by_provider")
                .Column(u => u.LoginProvider, cm => cm.WithName("login_provider"))
                .Column(u => u.ProviderKey, cm => cm.WithName("provider_key"));
        }
    }
}