using Microsoft.Owin.Security;

namespace P5.Owin.Security.Twitter.Messages
{
    public class RequestToken
    {
        public bool CallbackConfirmed { get; set; }

        public AuthenticationProperties Properties { get; set; }

        public string Token { get; set; }

        public string TokenSecret { get; set; }
    }
}
