using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CustomClientCredentialHost.Areas.Admin.Models;
using IdentityServer3.Core.Models;

namespace CustomClientCredentialHost.Areas.NortonDeveloper.Models
{
    public class AccessTokenTypeElement
    {
        public AccessTokenType AccessTokenType { get; set; }
        public string Name { get; set; }
    }
    public class FlowsElement
    {
        public Flows Flows { get; set; }
        public string Name { get; set; }
    }
    public class ClientViewModel
    {
        private readonly List<AccessTokenTypeElement> _AccessTokenTypeElements;
        private readonly List<FlowsElement> _FlowsElements;
        public ClientViewModel()
        {
            _AccessTokenTypeElements = new List<AccessTokenTypeElement>
            {
                new AccessTokenTypeElement()
                {
                    Name = "Jwt: Self-Contained and big",
                    AccessTokenType = AccessTokenType.Jwt
                },
                new AccessTokenTypeElement()
                {
                    Name = "Reference: Stored on the backend",
                    AccessTokenType = AccessTokenType.Reference
                }
            };
            _FlowsElements = new List<FlowsElement>
            {
                new FlowsElement()
                {
                    Name = "Client Credentials",
                    Flows = Flows.ClientCredentials
                },
                 new FlowsElement()
                {
                    Name = "Resource Owner",
                    Flows = Flows.ResourceOwner
                }
            };
        }

        [Display(Name = "AccessTokenType")]
        [Required]
        public AccessTokenType AccessTokenType { get; set; }

        public IEnumerable<SelectListItem> AccessTokenTypes
        {
            get { return new SelectList(_AccessTokenTypeElements, "AccessTokenType", "Name"); }
        }
        public IEnumerable<SelectListItem> FlowsTypes
        {
            get { return new SelectList(_FlowsElements, "Flows", "Name"); }
        }

        //
        // Summary:
        //     Specifies the scopes that the client is allowed to request. If empty, the
        //     client can't access any scope
        public List<string> AllowedScopes { get; set; }

        //
        // Summary:
        //     Unique ID of the client
        [Required]
        public string ClientId { get; set; }
        //
        // Summary:
        //     Client display name (used for logging and consent screen)
        [Display(Name = "Friendly Client Name")]
        [Required]
        public string ClientName { get; set; }

        //
        // Summary:
        //     Client secrets - only relevant for flows that require a secret
        public List<Secret> ClientSecrets { get; set; }

        //
        // Summary:
        //     Specifies if client is enabled (defaults to true)
        public bool Enabled { get; set; }

        //
        // Summary:
        //     Specifies allowed flow for client (either AuthorizationCode, Implicit, Hybrid,
        //     ResourceOwner, ClientCredentials or Custom). Defaults to Implicit.
        [Required]
        public Flows Flow { get; set; }
    }
}
