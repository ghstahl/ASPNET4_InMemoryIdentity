using System;
using System.Collections.Generic;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using Microsoft.Owin.Logging;
using Owin;
using P5.CassandraStore.Settings;
using P5.IdentityServer3.Cassandra.Configuration;
using P5.IdentityServer3.Cassandra.DAO;

namespace P5.IdentityServer3.Cassandra
{
    public static class  AppBuilderExtensions
    {
        public static IAppBuilder UseIdentityServerCassandraStores(this IAppBuilder app,
            Registration<IUserService> userService,
            out IdentityServerServiceFactory identityServerServiceFactory,
            IdentityServerCassandraOptions options = null)
        {
            if (app == null) throw new ArgumentNullException("app");
            var loggerFactory = app.GetLoggerFactory();
            options = options ?? new IdentityServerCassandraOptions()
            {
                ContactPoints = new List<string> {"cassandra"},
                Credentials = new CassandraCredentials() {Password = "", UserName = ""},
                KeySpace = "identityserver3"
            };

            IdentityServer3CassandraDao.CassandraConfig = options.CassandraConfig;
            identityServerServiceFactory = new ServiceFactory(userService);
            return app;
        }
    }
}