using Cassandra.Mapping;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public class MyMappings : Mappings
    {

        public const string WellKnownClaims = "claims";
        public const string WellKnownUserRoles = "user_roles";

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


            For<ClaimHandle>()
                .TableName(WellKnownClaims);
            For<RoleHandle>()
                .TableName(WellKnownUserRoles);


        }
    }
}