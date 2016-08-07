#define CASSANDRA_STORE
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Autofac;
using Autofac.Integration.WebApi;
using CustomClientCredentialHost.Config;
using IdentityServer3.AccessTokenValidation;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using Microsoft.Owin;
using Owin;
using P5.AspNet.Identity.Cassandra;
using P5.CassandraStore;
using P5.CassandraStore.Settings;
using P5.IdentityServer3.Cassandra;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Cassandra.UserService;
using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.Providers;
using P5.IdentityServer3.Common.Settings;
using P5.IdentityServerCore.IdSrv;
using P5.WebApi2.Hub;
using Serilog;

[assembly: OwinStartupAttribute(typeof(CustomClientCredentialHost.Startup))]

namespace CustomClientCredentialHost
{

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            string _domain_root = ConfigurationManager.AppSettings["IdentityServer:Domain"];
            IdentityServerSettings.DomainRoot = _domain_root;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Trace()
                .CreateLogger();

            var builder = new ContainerBuilder();

            ConfigureAuth(app);

            var path = HostingEnvironment.MapPath("~/App_Data");
            var cassandraUserStore = new CassandraUserStore(CassandraAspNetIdentityOptions.CassandraConfig,
                CassandraAspNetApplicationConstants.TenantGuid);
            var userService = new Registration<IUserService>(new AspNetIdentityServerService(cassandraUserStore));
            IdentityServerServiceFactory identityServerServiceFactory;



#if CASSANDRA_STORE

            app.UseAspNetCassandraStores(new CassandraOptions()
            {
                ContactPoints = new List<string> {"cassandra"},
                Credentials = new CassandraCredentials() {Password = "", UserName = ""},
                KeySpace = "aspnetidentity"
            });

            app.UseIdentityServerCassandraStores(userService, out identityServerServiceFactory,
                new CassandraOptions()
                {
                    ContactPoints = new List<string> { "cassandra" },
                    Credentials = new CassandraCredentials() { Password = "", UserName = "" },
                    KeySpace = "identityserver3"
                });
            /*
             * TODO: Cassandra may be down, need a robust app that can reconnect
            foreach (var client in Clients.Get())
            {
                IdentityServer3CassandraDao.UpsertClientAsync(
                    new FlattenedClientRecord(new FlattenedClientHandle(client)));
            }
            foreach (var scope in Scopes.Get())
            {
                IdentityServer3CassandraDao.UpsertScopeAsync(new FlattenedScopeRecord(new FlattenedScopeHandle(scope)));
            }
             * */
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
                    },
                    {
                        "arbritary-provider",new ArbritaryClaimsProvider(resolver.Resolve<IUserService>())
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
  /*          config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );
            */
            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<ApplicationUserManager>();

            // Run other optional steps, like registering filters,
            // per-controller-type services, etc., then set the dependency resolver
            // to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            config.Services.Replace(typeof(IAssembliesResolver), new WebApi2HubAssembliesResolver());
            WebApiConfig.Register(config); // NEW way

            // OWIN WEB API SETUP:

            // Register the Autofac middleware FIRST, then the Autofac Web API middleware,
            // and finally the standard Web API middleware.
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);

        }
    }
}
