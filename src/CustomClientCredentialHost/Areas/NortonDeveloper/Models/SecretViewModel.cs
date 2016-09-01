using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

/*
 *
 *    public static class SecretTypes
        {
            public const string SharedSecret = "SharedSecret";
            public const string X509CertificateBase64 = "X509CertificateBase64";
            public const string X509CertificateName = "X509Name";
            public const string X509CertificateThumbprint = "X509Thumbprint";
        }
 *
 */
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

        public string ThumbPrint { get; set; }

        public string SecretType { get; set; }
        private static List<string> _secretTypes;

        public IEnumerable<SelectListItem> SecretTypes
        {
            get
            {
                var selectedList = new SelectList(_secretTypesProperty);
                return selectedList;
            }
        }

        private IEnumerable<string> _secretTypesProperty
        {
            get
            {
                return _secretTypes ?? (_secretTypes = new List<string>
                {
                    "SharedSecret",
                    "X509CertificateBase64",
                    "X509Name",
                    "X509Thumbprint"
                });
            }
        }
    }
}