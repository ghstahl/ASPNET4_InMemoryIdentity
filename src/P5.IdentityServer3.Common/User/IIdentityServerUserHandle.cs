using System.Collections.Generic;
using System.Threading.Tasks;

namespace P5.IdentityServer3.Common
{
    public interface IIdentityServerUserHandle
    {
        Task<IdentityServerUser> MakeIdentityServerUserAsync();
        string UserId { get; set; }
        
    }
}