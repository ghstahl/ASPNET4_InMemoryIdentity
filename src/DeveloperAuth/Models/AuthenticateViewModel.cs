using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeveloperAuth.Messages;

namespace DeveloperAuth.Models
{
    public class AuthenticateViewModel
    {
        public RequestToken RequestToken { get; set; }
        public LoginModel LoginModel { get; set; }
    }
}
