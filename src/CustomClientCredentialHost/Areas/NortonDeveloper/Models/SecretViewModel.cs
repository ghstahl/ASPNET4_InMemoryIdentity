using System.ComponentModel.DataAnnotations;

namespace CustomClientCredentialHost.Areas.NortonDeveloper.Models
{
    public class SecretViewModel
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string OpenClientSecret { get; set; }
        [Required]
        public string PassCode { get; set; }
    }
}