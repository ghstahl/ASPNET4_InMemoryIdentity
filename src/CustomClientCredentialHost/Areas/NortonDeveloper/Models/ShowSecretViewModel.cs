using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;

namespace CustomClientCredentialHost.Areas.NortonDeveloper.Models
{
    public class ShowSecretViewModel
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string Hash { get; set; }
        [Required]
        public string PassCode { get; set; }
        [Display(Name = "Client Secret")]
        public string OpenSecret { get; set; }
    }
}