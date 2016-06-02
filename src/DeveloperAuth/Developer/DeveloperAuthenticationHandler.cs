using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DeveloperAuth.Messages;

using Microsoft.Owin;
using Microsoft.Owin.Helpers;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace DeveloperAuth.Developer
{
    internal class DeveloperAuthenticationHandler : AuthenticationHandler<DeveloperAuthenticationOptions>
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        internal const string StateCookie = "__DeveloperState";
        public const string RequestTokenCookie = "__RequestToken";

        private const string RequestTokenEndpoint = "https://api.twitter.com/oauth/request_token";
        //private const string AuthenticationEndpoint = "https://twitter.com/oauth/authenticate?oauth_token=";
        private const string AccessTokenEndpoint = "https://api.twitter.com/oauth/access_token";
        private const string AuthenticationEndpoint = "/Developer/Home/Authenticate?accessToken=";

        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public DeveloperAuthenticationHandler(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public override async Task<bool> InvokeAsync()
        {
            if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
            {
                return await InvokeReturnPathAsync();
            }
            return false;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationProperties properties = null;
            try
            {
                IReadableStringCollection query = Request.Query;
                string protectedRequestToken = Request.Cookies[StateCookie];

                RequestToken requestToken = DeveloperAuthenticationOptions.StateDataFormat.Unprotect(protectedRequestToken);

                if (requestToken == null)
                {
                    _logger.WriteWarning("Invalid state");
                    return null;
                }

                properties = requestToken.Properties;

                var formCollection = await Request.ReadFormAsync();

                var accessToken = new AccessToken()
                {
                    CallBackUri = requestToken.CallBackUri,
                    CallbackConfirmed = requestToken.CallbackConfirmed,
                    Properties = requestToken.Properties,
                    ScreenName = formCollection["_email"],
                    UserId = formCollection["_userId"]
                };


                var context = new DeveloperAuthenticatedContext(Context, accessToken.UserId, accessToken.ScreenName, accessToken.Token, accessToken.TokenSecret);

                context.Identity = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, accessToken.UserId, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType),
                        new Claim(ClaimTypes.Name, accessToken.ScreenName, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType),
                    },
                    Options.AuthenticationType,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                context.Properties = requestToken.Properties;

                Response.Cookies.Delete(StateCookie);

                await Options.Provider.Authenticated(context);

                return new AuthenticationTicket(context.Identity, context.Properties);
            }
            catch (Exception ex)
            {
                _logger.WriteError("Authentication failed", ex);
                return new AuthenticationTicket(null, properties);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream.Dispose is idempotent")]
        protected override async Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return;
            }

            AuthenticationResponseChallenge challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

            if (challenge != null)
            {
                string requestPrefix = Request.Scheme + "://" + Request.Host;
                string callBackUrl = requestPrefix + RequestPathBase + Options.CallbackPath;

                AuthenticationProperties extra = challenge.Properties;
                if (string.IsNullOrEmpty(extra.RedirectUri))
                {
                    extra.RedirectUri = requestPrefix + Request.PathBase + Request.Path + Request.QueryString;
                }

                RequestToken requestToken = await ObtainRequestTokenAsync(Options.ConsumerKey, Options.ConsumerSecret, callBackUrl, extra);

                if (requestToken.CallbackConfirmed)
                {
                    string DeveloperAuthenticationEndpoint = AuthenticationEndpoint + requestToken.Token;

                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = Request.IsSecure
                    };

                    Response.Cookies.Append(StateCookie, DeveloperAuthenticationOptions.StateDataFormat.Protect(requestToken), cookieOptions);

                    var redirectContext = new DeveloperApplyRedirectContext(
                        Context, Options,
                        extra, DeveloperAuthenticationEndpoint);
                    Options.Provider.ApplyRedirect(redirectContext);
                }
                else
                {
                    _logger.WriteError("requestToken CallbackConfirmed!=true");
                }
            }
        }

        public async Task<bool> InvokeReturnPathAsync()
        {
            AuthenticationTicket model = await AuthenticateAsync();
            if (model == null)
            {
                _logger.WriteWarning("Invalid return state, unable to redirect.");
                Response.StatusCode = 500;
                return true;
            }

            var context = new DeveloperReturnEndpointContext(Context, model)
            {
                SignInAsAuthenticationType = Options.SignInAsAuthenticationType,
                RedirectUri = model.Properties.RedirectUri
            };
            model.Properties.RedirectUri = null;

            await Options.Provider.ReturnEndpoint(context);

            if (context.SignInAsAuthenticationType != null && context.Identity != null)
            {
                ClaimsIdentity signInIdentity = context.Identity;
                if (!string.Equals(signInIdentity.AuthenticationType, context.SignInAsAuthenticationType, StringComparison.Ordinal))
                {
                    signInIdentity = new ClaimsIdentity(signInIdentity.Claims, context.SignInAsAuthenticationType, signInIdentity.NameClaimType, signInIdentity.RoleClaimType);
                }
                Context.Authentication.SignIn(context.Properties, signInIdentity);
            }

            if (!context.IsRequestCompleted && context.RedirectUri != null)
            {
                if (context.Identity == null)
                {
                    // add a redirect hint that sign-in failed in some way
                    context.RedirectUri = WebUtilities.AddQueryString(context.RedirectUri, "error", "access_denied");
                }
                Response.Redirect(context.RedirectUri);
                context.RequestCompleted();
            }

            return context.IsRequestCompleted;
        }

        private async Task<RequestToken> ObtainRequestTokenAsync(string consumerKey, string consumerSecret, string callBackUri, AuthenticationProperties properties)
        {
            _logger.WriteVerbose("ObtainRequestToken");

            var nonce = Guid.NewGuid().ToString("N");
            var requestToken = new RequestToken()
            {
                CallBackUri = callBackUri,
                Token = nonce,
                Properties = properties,
                TokenSecret = nonce,
                CallbackConfirmed = true
            };
            // this simulates the remote service storing the token.
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsSecure
            };
            var data = DeveloperAuthenticationOptions.StateDataFormat.Protect(requestToken);
            var data2 = DeveloperAuthenticationOptions.StateDataFormat.Unprotect(data);
            // this is basically a database record entry that stores the access token.
            Response.Cookies.Append(RequestTokenCookie, data, cookieOptions);
            return requestToken;
        }

        private async Task<AccessToken> ObtainAccessTokenAsync(string consumerKey, string consumerSecret, RequestToken token, string verifier)
        {
            // https://dev.twitter.com/docs/api/1/post/oauth/access_token

            _logger.WriteVerbose("ObtainAccessToken");

            string nonce = Guid.NewGuid().ToString("N");

            var authorizationParts = new SortedDictionary<string, string>
            {
                { "oauth_consumer_key", consumerKey },
                { "oauth_nonce", nonce },
                { "oauth_signature_method", "HMAC-SHA1" },
                { "oauth_token", token.Token },
                { "oauth_timestamp", GenerateTimeStamp() },
                { "oauth_verifier", verifier },
                { "oauth_version", "1.0" },
            };

            var parameterBuilder = new StringBuilder();
            foreach (var authorizationKey in authorizationParts)
            {
                parameterBuilder.AppendFormat("{0}={1}&", Uri.EscapeDataString(authorizationKey.Key), Uri.EscapeDataString(authorizationKey.Value));
            }
            parameterBuilder.Length--;
            string parameterString = parameterBuilder.ToString();

            var canonicalizedRequestBuilder = new StringBuilder();
            canonicalizedRequestBuilder.Append(HttpMethod.Post.Method);
            canonicalizedRequestBuilder.Append("&");
            canonicalizedRequestBuilder.Append(Uri.EscapeDataString(AccessTokenEndpoint));
            canonicalizedRequestBuilder.Append("&");
            canonicalizedRequestBuilder.Append(Uri.EscapeDataString(parameterString));

            string signature = ComputeSignature(consumerSecret, token.TokenSecret, canonicalizedRequestBuilder.ToString());
            authorizationParts.Add("oauth_signature", signature);
            authorizationParts.Remove("oauth_verifier");

            var authorizationHeaderBuilder = new StringBuilder();
            authorizationHeaderBuilder.Append("OAuth ");
            foreach (var authorizationPart in authorizationParts)
            {
                authorizationHeaderBuilder.AppendFormat(
                    "{0}=\"{1}\", ", authorizationPart.Key, Uri.EscapeDataString(authorizationPart.Value));
            }
            authorizationHeaderBuilder.Length = authorizationHeaderBuilder.Length - 2;

            var request = new HttpRequestMessage(HttpMethod.Post, AccessTokenEndpoint);
            request.Headers.Add("Authorization", authorizationHeaderBuilder.ToString());

            var formPairs = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("oauth_verifier", verifier)
            };

            request.Content = new FormUrlEncodedContent(formPairs);

            HttpResponseMessage response = await _httpClient.SendAsync(request, Request.CallCancelled);

            if (!response.IsSuccessStatusCode)
            {
                _logger.WriteError("AccessToken request failed with a status code of " + response.StatusCode);
                response.EnsureSuccessStatusCode(); // throw
            }

            string responseText = await response.Content.ReadAsStringAsync();

            IFormCollection responseParameters = WebHelpers.ParseForm(responseText);

            return new AccessToken
            {
                Token = Uri.UnescapeDataString(responseParameters["oauth_token"]),
                TokenSecret = Uri.UnescapeDataString(responseParameters["oauth_token_secret"]),
                UserId = Uri.UnescapeDataString(responseParameters["user_id"]),
                ScreenName = Uri.UnescapeDataString(responseParameters["screen_name"]),
                CallBackUri = Uri.UnescapeDataString(responseParameters["oauth_callback"])
            };
        }

        private static string GenerateTimeStamp()
        {
            TimeSpan secondsSinceUnixEpocStart = DateTime.UtcNow - Epoch;
            return Convert.ToInt64(secondsSinceUnixEpocStart.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        private static string ComputeSignature(string consumerSecret, string tokenSecret, string signatureData)
        {
            using (var algorithm = new HMACSHA1())
            {
                algorithm.Key = Encoding.ASCII.GetBytes(
                    string.Format(CultureInfo.InvariantCulture,
                        "{0}&{1}",
                        Uri.EscapeDataString(consumerSecret),
                        string.IsNullOrEmpty(tokenSecret) ? string.Empty : Uri.EscapeDataString(tokenSecret)));
                byte[] hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(signatureData));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
