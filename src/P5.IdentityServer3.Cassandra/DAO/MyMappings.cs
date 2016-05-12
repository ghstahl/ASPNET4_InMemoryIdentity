using Cassandra.Mapping;
using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.RefreshToken;

namespace P5.IdentityServer3.Cassandra.DAO
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

            For<FlattenedTokenHandle>()
                .TableName("tokenhandle_by_key");

            For<FlattenedRefreshTokenHandle>()
                .TableName("refreshtokenhandle_by_key");

            For<FlattenedConsentHandle>()
                .TableName("consent_by_clientid");
        }
    }
}