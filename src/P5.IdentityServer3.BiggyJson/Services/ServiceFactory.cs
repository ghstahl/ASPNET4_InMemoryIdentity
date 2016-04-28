using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.BiggyJson
{
    public class ServiceFactory : IdentityServerServiceFactory
    {
        public ServiceFactory(Registration<IUserService> userService,
            StoreSettings storeSettings)
        {
            Register(new Registration<StoreSettings>(storeSettings));
            UserService = userService;
            ClientStore = new Registration<IClientStore>(typeof(ClientStore));
            ScopeStore = new Registration<IScopeStore>(typeof(ScopeStore));
            ConsentStore = new Registration<IConsentStore>(typeof(ConsentStore));
            AuthorizationCodeStore = new Registration<IAuthorizationCodeStore>(typeof(AuthorizationCodeStore));
            RefreshTokenStore = new Registration<IRefreshTokenStore>(typeof(RefreshTokenStore));
            TokenHandleStore = new Registration<ITokenHandleStore>(typeof(TokenHandleStore));   
        }      
    }
}
