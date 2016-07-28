using System.ComponentModel.DataAnnotations;

namespace CustomClientCredentialHost.Areas.NortonDeveloper.Models
{
    public class ShowSecretViewModel
    {
        [Required]
        public string Hash { get; set; }
        [Required]
        public string PassCode { get; set; }
        public string OpenSecret { get; set; }
    }
}