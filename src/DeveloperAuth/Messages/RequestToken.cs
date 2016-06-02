using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security;

namespace DeveloperAuth.Messages
{
    public class RequestToken
    {
        public bool CallbackConfirmed { get; set; }

        public AuthenticationProperties Properties { get; set; }

        public string Token { get; set; }

        public string TokenSecret { get; set; }

        /// <summary>
        /// Gets or sets the Developer CallBackUri.
        /// </summary>
        public string CallBackUri { get; set; }
    }
}
