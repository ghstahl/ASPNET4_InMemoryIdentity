using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeveloperAuth.Developer;
using Owin;

namespace Owin
{
    /// <summary>
    /// Extension methods for using <see cref="DeveloperAuthenticationMiddleware"/>
    /// </summary>
    public static class DeveloperAuthenticationExtensions
    {
        /// <summary>
        /// Authenticate users using Developer
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="options">Middleware configuration options</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseDeveloperAuthentication(this IAppBuilder app, DeveloperAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            app.Use(typeof(DeveloperAuthenticationMiddleware), app, options);
            return app;
        }

        /// <summary>
        /// Authenticate users using Developer
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="consumerKey">The Developer-issued consumer key</param>
        /// <param name="consumerSecret">The Developer-issued consumer secret</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseDeveloperAuthentication(
            this IAppBuilder app,
            string consumerKey,
            string consumerSecret)
        {
            return UseDeveloperAuthentication(
                app,
                new DeveloperAuthenticationOptions
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                });
        }
    }
}
