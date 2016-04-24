using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(P5.IdentityServer3AllInOne.App.Startup))]
namespace P5.IdentityServer3AllInOne.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
