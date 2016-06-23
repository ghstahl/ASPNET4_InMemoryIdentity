using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Logging;
using Owin;
using P5.CassandraStore;
using P5.CassandraStore.Settings;

namespace P5.AspNet.Identity.Cassandra
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseAspNetCassandraStores(this IAppBuilder app, CassandraOptions options = null)
        {
            if (app == null) throw new ArgumentNullException("app");
            var loggerFactory = app.GetLoggerFactory();
            options = options ?? new CassandraOptions()
            {
                ContactPoints = new List<string> { "cassandra" },
                Credentials = new CassandraCredentials() { Password = "", UserName = "" },
                KeySpace = "aspnetidentity"
            };

            CassandraAspNetIdentityOptions.CassandraConfig = options.CassandraConfig; 
            return app;
        }
    }
}
