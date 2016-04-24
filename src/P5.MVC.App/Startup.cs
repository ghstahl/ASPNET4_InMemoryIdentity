using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(P5.MVC.App.Startup))]
namespace P5.MVC.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
