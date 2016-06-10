using Cassandra.Mapping;
using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.RefreshToken;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public class MyMappings : Mappings
    {
        public const string WellKnownTokenhandleByKey = "tokenhandle_by_key";
        public const string WellKnownAuthorizationCodeHandleByKey = "authorizationcodehandle_by_key";
        public const string WellKnownRefreshTokenHandleByKey = "refreshtokenhandle_by_key";
        public const string WellKnownConsentByClientId = "consent_by_clientid";
        public const string WellKnownClientsById = "clients_by_id";
        public const string WellKnownUserProfileById = "user_profile_by_id";
        public const string WellKnownUserScopename = "user_scopename";
        public const string WellKnownUserClientId = "user_clientid";

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

            For<IdentityServerUserClientId>()
              .TableName(WellKnownUserClientId); 

            For<IdentityServerUserAllowedScope>()
              .TableName(WellKnownUserScopename); 
            
            For<IdentityServerUserHandle>()
              .TableName(WellKnownUserProfileById); 
            
            For<FlattenedClientHandle>()
               .TableName(WellKnownClientsById);

            For<FlattenedTokenHandle>()
                .TableName(WellKnownTokenhandleByKey);

            For<FlattenedAuthorizationCodeHandle>()
                .TableName(WellKnownAuthorizationCodeHandleByKey);

            For<FlattenedRefreshTokenHandle>()
                .TableName(WellKnownRefreshTokenHandleByKey);

            For<FlattenedConsentHandle>()
                .TableName(WellKnownConsentByClientId);
        }
    }
}