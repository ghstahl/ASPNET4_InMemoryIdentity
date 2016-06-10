using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P5.IdentityServer3.Common
{
    public class IdentityServerUser
    {        
        //
        // Summary:
        //     Unique ID of the UserId
        public string UserId { get; set; }

        //
        // Summary:
        //     User display name (used for logging,etc)
        public string UserName { get; set; }
        //
        // Summary:
        //     Specifies if user is enabled (defaults to true)
        public bool Enabled { get; set; }
        //
        // Summary:
        //     Specifies the scopes that the user is allowed to request. If empty, the
        //     user can't access any scope
        public List<string> AllowedScopes { get; set; }
        //
        // Summary:
        //     Specifies the clients that the user owns. If empty, the
        //     user isn't yet setup to do any IdentityServer stuff.
        public List<string> ClientIds { get; set; }
    }
}
