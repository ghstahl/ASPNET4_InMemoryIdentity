using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AA.Reference.App.Startup))]
namespace AA.Reference.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
