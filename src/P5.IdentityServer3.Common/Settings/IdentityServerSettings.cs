using System;

namespace P5.IdentityServer3.Common.Settings
{
    public class IdentityServerSettings
    {
      //  string _identity_server_base_route = "/idsrv3core";
        public static string DomainRoot { get; set; }

        public static string IdentityServerBaseRoute { get; set; }

        private static string _identityServerAuthority;
        public static string IdentityServerAuthority
        {
            get
            {
                if (string.IsNullOrEmpty(_identityServerAuthority))
                {
                    Uri baseUri = new Uri(DomainRoot);
                    Uri myUri = new Uri(baseUri, IdentityServerBaseRoute);

                    _identityServerAuthority = myUri.AbsoluteUri;
                }
                return _identityServerAuthority;
            }
        }

        private static string _tokenEndpoint;
        public static string TokenEndpoint
        {
            get
            {
                if (string.IsNullOrEmpty(_tokenEndpoint))
                {
                    Uri baseUri = new Uri(DomainRoot);
                    Uri myUri = new Uri(baseUri, IdentityServerBaseRoute + "/connect/token");
                    _tokenEndpoint = myUri.AbsoluteUri;
                }
                return _tokenEndpoint;
            }
        }
    }
}
