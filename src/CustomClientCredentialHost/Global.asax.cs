using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using P5.WebApi2.Hub;

namespace CustomClientCredentialHost
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {


            AreaRegistration.RegisterAllAreas();
//            GlobalConfiguration.Configure(WebApiConfig.Register); // NEW way
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            WebApi2Hub.Use(new WebApi2HubOptions() { PluginRootPath = @"C:\MyMefPlugins" });
            
           
        }
        public override void Init()
        {
            this.PostAuthenticateRequest += Application_PostAuthenticateRequest;
            base.Init();
        }

        public static void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }

    }
}
