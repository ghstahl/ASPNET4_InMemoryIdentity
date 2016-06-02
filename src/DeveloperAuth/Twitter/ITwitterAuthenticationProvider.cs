using System.Threading.Tasks;

namespace DeveloperAuth.Twitter
{
    public interface ITwitterAuthenticationProvider
    {
        void ApplyRedirect(TwitterApplyRedirectContext context);
        Task Authenticated(TwitterAuthenticatedContext context);
        Task ReturnEndpoint(TwitterReturnEndpointContext context);
    }
}
