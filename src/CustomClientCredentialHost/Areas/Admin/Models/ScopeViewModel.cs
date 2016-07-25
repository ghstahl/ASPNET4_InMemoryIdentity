using IdentityServer3.Core.Models;

namespace CustomClientCredentialHost.Areas.Admin.Models
{
    public class ScopeViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ScopeType ScopeType { get; set; }

    }
}