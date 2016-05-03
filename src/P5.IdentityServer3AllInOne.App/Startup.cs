using System.IdentityModel.Tokens;
using System.Web.Hosting;
using System.Web.Http;
using IdentityServer3.AccessTokenValidation;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using Microsoft.Owin;
using Owin;
using P5.IdentityServer3.BiggyJson;
using P5.IdentityServer3AllInOne.App.Config;
using P5.IdentityServerCore.IdSrv;



[assembly: OwinStartupAttribute(typeof(P5.IdentityServer3AllInOne.App.Startup))]
namespace P5.IdentityServer3AllInOne.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);


            var path = HostingEnvironment.MapPath("~/App_Data");


            var userService = new Registration<IUserService>(new UserServiceBase());

            var inMemoryFactory = new IdentityServerServiceFactory()
                            .UseInMemoryClients(Clients.Get())
                            .UseInMemoryScopes(Scopes.Get())
                            .UseInMemoryUsers(Users.Get());
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
                myScopeStore.CreateAsync(scope);
            }

            // Create the BiggyIdentityService factory
            var factory = new ServiceFactory(userService, settings);

            factory.UseInMemoryUsers(Users.Get());
            factory.CustomGrantValidators.Add(
                new Registration<ICustomGrantValidator>(typeof(CustomGrantValidator)));
            factory.CustomGrantValidators.Add(
                new Registration<ICustomGrantValidator>(typeof(ActAsGrantValidator)));
            factory.ClaimsProvider = new Registration<IClaimsProvider>(typeof(CustomClaimsProvider));

            var options = new IdentityServerOptions
            {
                Factory = factory,
                RequireSsl = false,
      //          SigningCertificate = Certificate.Get(),
                SiteName = "P5 IdentityServer3"
            };


            app.Map("/idsrv3core", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(options);

            });
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = "http://localhost:33854/idsrv3core",
                ValidationMode = ValidationMode.ValidationEndpoint,

                RequiredScopes = new[] { "CustomWebApi1", "api1", "WebApi1", "WebApi2", "read" },
                PreserveAccessToken = true

            });

            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);


        }
    }
}
