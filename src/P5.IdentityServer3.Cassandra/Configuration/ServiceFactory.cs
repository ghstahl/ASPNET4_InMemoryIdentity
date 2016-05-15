using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Cassandra.AuthorizationCode;
using P5.IdentityServer3.Cassandra.Client;
using P5.IdentityServer3.Cassandra.CommonStore;

namespace P5.IdentityServer3.Cassandra
{
    public class ServiceFactory : IdentityServerServiceFactory
    {
        public ServiceFactory(Registration<IUserService> userService)
        {
            UserService = userService;
            ClientStore = new Registration<IClientStore>(typeof(ClientStore));
            ScopeStore = new Registration<IScopeStore>(typeof(ScopeStore));
            ConsentStore = new Registration<IConsentStore>(typeof(ConsentStore));
            AuthorizationCodeStore = new Registration<IAuthorizationCodeStore>(typeof(AuthorizationCodeStore));
            RefreshTokenStore = new Registration<IRefreshTokenStore>(typeof(RefreshTokenHandleStore));
            TokenHandleStore = new Registration<ITokenHandleStore>(typeof(TokenHandleStore));
        }
    }
}