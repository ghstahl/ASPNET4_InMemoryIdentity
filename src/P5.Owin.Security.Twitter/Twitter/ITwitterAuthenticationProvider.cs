using System.Threading.Tasks;


namespace P5.Owin.Security.Twitter.Twitter
{
    public interface ITwitterAuthenticationProvider
    {
        void ApplyRedirect(TwitterApplyRedirectContext context);
        Task Authenticated(TwitterAuthenticatedContext context);
        Task ReturnEndpoint(TwitterReturnEndpointContext context);
    }
}
