using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.IdentityServer3.Cassandra.Settings
{
    public class IdentityServerSettings
    {
        const string identity_server_base_route = "/idsrv3core";
        public static string DomainRoot { get; set; }

        private static string _identityServerAuthority;
        public static string IdentityServerAuthority
        {
            get
            {
                if (string.IsNullOrEmpty(_identityServerAuthority))
                {
                    Uri baseUri = new Uri(DomainRoot);
                    Uri myUri = new Uri(baseUri, identity_server_base_route);

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
                    Uri myUri = new Uri(baseUri, identity_server_base_route + "/connect/token");
                    _tokenEndpoint = myUri.AbsoluteUri;
                }
                return _tokenEndpoint;
            }
        }
    }
}
