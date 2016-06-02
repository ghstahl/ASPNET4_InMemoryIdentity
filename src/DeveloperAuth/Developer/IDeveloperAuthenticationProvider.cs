using System.Threading.Tasks;
using DeveloperAuth.Developer;

namespace DeveloperAuth.Developer
{
    public interface IDeveloperAuthenticationProvider
    {
        void ApplyRedirect(DeveloperApplyRedirectContext context);
        Task Authenticated(DeveloperAuthenticatedContext context);
        Task ReturnEndpoint(DeveloperReturnEndpointContext context);
    }
}
