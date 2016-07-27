using System.Web.Mvc;

namespace CustomClientCredentialHost.Areas.NortonDeveloper
{
    public class NortonDeveloperAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "NortonDeveloper";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "NortonDeveloper_default",
                "NortonDeveloper/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "CustomClientCredentialHost.Areas.NortonDeveloper.Controllers" }
            );
        }
    }
}