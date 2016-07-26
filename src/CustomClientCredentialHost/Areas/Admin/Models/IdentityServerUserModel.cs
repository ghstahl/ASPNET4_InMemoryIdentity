using System.Collections.Generic;

namespace CustomClientCredentialHost.Areas.Admin.Models
{
    public class IdentityServerUserModel
    {
        public string UserId { get; set; }
        public bool Exists { get; set; }
        public List<string> AllowedScopes { get; set; }
    }
}