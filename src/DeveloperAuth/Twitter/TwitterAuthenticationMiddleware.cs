using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Security;
using DeveloperAuth.Messages;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace DeveloperAuth.Twitter
{
    public class TwitterAuthenticationMiddleware : AuthenticationMiddleware<TwitterAuthenticationOptions>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public TwitterAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, TwitterAuthenticationOptions options) : base(next, options)
        {
            this._logger = AppBuilderLoggerExtensions.CreateLogger<TwitterAuthenticationMiddleware>(app);
            if (string.IsNullOrWhiteSpace(Options.ConsumerSecret))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Microsoft.Owin.Resources.Exception_OptionMustBeProvided:{0}", new object[] { "ConsumerSecret" }));
            }
            if (string.IsNullOrWhiteSpace(Options.ConsumerKey))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, 
                    "Microsoft.Owin.Resources.Exception_OptionMustBeProvided:{0}", new object[] { "ConsumerKey" }));
            }
            if (Options.Provider == null)
            {
                Options.Provider = new TwitterAuthenticationProvider();
            }
            if (Options.StateDataFormat == null)
            {
                IDataProtector protector = AppBuilderExtensions.CreateDataProtector(app, new string[] { typeof(TwitterAuthenticationMiddleware).FullName, Options.AuthenticationType, "v1" });
                Options.StateDataFormat = new SecureDataFormat<RequestToken>(Serializers.RequestToken, protector, TextEncodings.Base64Url);
            }
            if (string.IsNullOrEmpty(Options.SignInAsAuthenticationType))
            {
                Options.SignInAsAuthenticationType = AppBuilderSecurityExtensions.GetDefaultSignInAsAuthenticationType(app);
            }
            this._httpClient = new HttpClient(ResolveHttpMessageHandler(Options));
            this._httpClient.Timeout = Options.BackchannelTimeout;
            this._httpClient.MaxResponseContentBufferSize = 0xa00000L;
            this._httpClient.DefaultRequestHeaders.Accept.ParseAdd("*/*");
            this._httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft Owin Twitter middleware");
            this._httpClient.DefaultRequestHeaders.ExpectContinue = false;
        }

        protected override AuthenticationHandler<TwitterAuthenticationOptions> CreateHandler()
        {
            return new TwitterAuthenticationHandler(this._httpClient, this._logger);
        }


        private static HttpMessageHandler ResolveHttpMessageHandler(TwitterAuthenticationOptions options)
        {
            HttpMessageHandler handler = options.BackchannelHttpHandler ?? new WebRequestHandler();
            WebRequestHandler handler2 = handler as WebRequestHandler;
            if (handler2 == null)
            {
                if (options.BackchannelCertificateValidator != null)
                {
                    throw new InvalidOperationException("Microsoft.Owin.Resources.Exception_ValidatorHandlerMismatch");
                }
                return handler;
            }
            if (options.BackchannelCertificateValidator != null)
            {
                ICertificateValidator backchannelCertificateValidator = options.BackchannelCertificateValidator;
                handler2.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(backchannelCertificateValidator.Validate);
            }
            return handler;
        }
    }
}