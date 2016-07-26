using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomClientCredentialHost.Areas.Admin.Models
{
    public class IdentityServerUserModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        [Range(typeof(bool), "true", "true", ErrorMessage = "You gotta tick the Developer Box!")]
        public bool Developer { get; set; }
        public bool Exists { get; set; }
        public List<string> AllowedScopes { get; set; }
    }
}