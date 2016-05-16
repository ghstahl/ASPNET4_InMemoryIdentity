#define CASSANDRA_STORE
using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Http;
using CustomClientCredentialHost.Config;
using IdentityServer3.AccessTokenValidation;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using Microsoft.Owin;
using Owin;
using P5.CassandraStore.Settings;
using P5.IdentityServer3.BiggyJson;
using P5.IdentityServer3.Cassandra;
using P5.IdentityServer3.Cassandra.Configuration;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
using P5.IdentityServerCore.IdSrv;

[assembly: OwinStartupAttribute(typeof(CustomClientCredentialHost.Startup))]

namespace CustomClientCredentialHost
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var path = HostingEnvironment.MapPath("~/App_Data");

            var userService = new Registration<IUserService>(new UserServiceBase());
            IdentityServerServiceFactory identityServerServiceFactory;

#if CASSANDRA_STORE
            app.UseIdentityServerCassandraStores(userService, out identityServerServiceFactory,
                new IdentityServerCassandraOptions()
                {
                    ContactPoints = new List<string> { "cassandra" },
                    Credentials = new CassandraCredentials() { Password = "", UserName = "" },
                    KeySpace = "identityserver3"
                });
            foreach (var client in Clients.Get())
            {
                IdentityServer3CassandraDao.CreateClientAsync(
                    new FlattenedClientRecord(new FlattenedClientHandle(client)));
            }
            foreach (var scope in Scopes.Get())
            {
                IdentityServer3CassandraDao.CreateScopeAsync(new FlattenedScopeRecord(new FlattenedScopeHandle(scope)));
            }
#endif
#if BIGGY_STORE
            // Create and modify default settings
            StoreSettings settings = StoreSettings.DefaultSettings;
            settings.Folder = path;

            ClientStore myClientStore = new ClientStore(settings);
            foreach (var client in Clients.Get())
            {
                myClientStore.CreateAsync(new ClientHandle(client));
            }
            ScopeStore myScopeStore = new ScopeStore(settings);
            foreach (var scope in Scopes.Get())
            {
                myScopeStore.CreateAsync(new ScopeHandle(scope));
            }

            // Create the BiggyIdentityService factory
            identityServerServiceFactory = new ServiceFactory(userService, settings);
#endif
            identityServerServiceFactory.Register(new Registration<IDictionary<string,IClaimsProvider>>(resolver =>
            {
                var result = new Dictionary<string, IClaimsProvider>
                {
                    {
                        CustomClaimsProviderHub.WellKnown_DefaultProviderName,
                        new DefaultClaimsProvider(resolver.Resolve<IUserService>())
                    },
                    {
                        "openid-provider",new CustomOpenIdClaimsProvider(resolver.Resolve<IUserService>())
                    }

                };
                return result;
            }));

            identityServerServiceFactory.ClaimsProvider = new Registration<IClaimsProvider>(typeof(CustomClaimsProviderHub));

            var options = new IdentityServerOptions
            {
                Factory = identityServerServiceFactory,
                RequireSsl = false,
                SigningCertificate = Certificate.Get(),
                SiteName = "P5 IdentityServer3"
            };


            app.Map("/idsrv3core", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(options);

            });
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = "http://localhost:55970/idsrv3core",
                ValidationMode = ValidationMode.ValidationEndpoint,

                RequiredScopes = new[] {"api1"},
                PreserveAccessToken = true,
                EnableValidationResultCache = true

            });

            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );

            app.UseWebApi(config);
        }
    }
}
