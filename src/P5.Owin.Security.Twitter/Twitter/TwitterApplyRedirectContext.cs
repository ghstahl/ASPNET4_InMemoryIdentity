﻿using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;

namespace P5.Owin.Security.Twitter.Twitter
{
    /// <summary>
    /// Context passed when a Challenge causes a redirect to authorize endpoint in the Twitter middleware
    /// </summary>
    public class TwitterApplyRedirectContext : BaseContext<TwitterAuthenticationOptions>
    {
        /// <summary>
        /// Creates a new context object.
        /// </summary>
        /// <param name="context">The OWIN request context</param>
        /// <param name="options">The Facebook middleware options</param>
        /// <param name="properties">The authenticaiton properties of the challenge</param>
        /// <param name="redirectUri">The initial redirect URI</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#",
            Justification = "Represents header value")]
        public TwitterApplyRedirectContext(IOwinContext context, TwitterAuthenticationOptions options,
            AuthenticationProperties properties, string redirectUri)
            : base(context, options)
        {
            RedirectUri = redirectUri;
            Properties = properties;
        }

        /// <summary>
        /// Gets the URI used for the redirect operation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Represents header value")]
        public string RedirectUri { get; private set; }

        /// <summary>
        /// Gets the authenticaiton properties of the challenge
        /// </summary>
        public AuthenticationProperties Properties { get; private set; }
    }
}