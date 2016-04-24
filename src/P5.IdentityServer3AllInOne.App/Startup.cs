using System.Web.Http;
using IdentityServer3.AccessTokenValidation;
using IdentityServer3.Core.Configuration;
using Microsoft.Owin;
using Owin;
using P5.IdentityServerCore.IdSrv;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;


[assembly: OwinStartupAttribute(typeof(P5.IdentityServer3AllInOne.App.Startup))]
namespace P5.IdentityServer3AllInOne.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var options = new IdentityServerOptions
            {
                Factory = new IdentityServerServiceFactory()
                            .UseInMemoryClients(Clients.Get())
                            .UseInMemoryScopes(Scopes.Get())
                            .UseInMemoryUsers(Users.Get()),

                RequireSsl = false
            };
            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(options);

            });

            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = "http://localhost:41031/identity",
                ValidationMode = ValidationMode.ValidationEndpoint,

                RequiredScopes = new[] { "api1" }
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
