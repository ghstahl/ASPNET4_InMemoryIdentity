using System.Threading.Tasks;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Common
{
    public interface IScopeHandle
    {
        Task<Scope> MakeIdentityServerScopeAsync();
    }
}