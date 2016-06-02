using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;

namespace DeveloperAuth.Developer
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class DeveloperAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="DeveloperAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="userId">Developer user ID</param>
        /// <param name="screenName">Developer screen name</param>
        /// <param name="accessToken">Developer access token</param>
        /// <param name="accessTokenSecret">Developer access token secret</param>
        public DeveloperAuthenticatedContext(
            IOwinContext context,
            string userId,
            string screenName,
            string accessToken,
            string accessTokenSecret)
            : base(context)
        {
            UserId = userId;
            ScreenName = screenName;
            AccessToken = accessToken;
            AccessTokenSecret = accessTokenSecret;
        }

        /// <summary>
        /// Gets the Developer user ID
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the Developer screen name
        /// </summary>
        public string ScreenName { get; private set; }

        /// <summary>
        /// Gets the Developer access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the Developer access token secret
        /// </summary>
        public string AccessTokenSecret { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }
    }
}